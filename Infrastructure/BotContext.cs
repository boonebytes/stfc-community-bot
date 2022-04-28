using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Services;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Events;
using DiscordBot.Domain.Seedwork;
using DiscordBot.Infrastructure.DTOs;
using DiscordBot.Infrastructure.Entities;
using DiscordBot.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace DiscordBot.Infrastructure
{
    public class BotContext : DbContext, IUnitOfWork
    {
        public DbSet<Audit> AuditLogs { get; set; }
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<AllianceGroup> AllianceGroups { get; set; }
        public DbSet<Diplomacy> Diplomacies { get; set; }
        public DbSet<DiplomaticRelation> DiplomaticRelaitons { get; set; }
        public DbSet<DirectMessage> DirectMessages { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StarSystem> StarSystems { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<ZoneNeighbour> ZoneNeighbours { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceCost> ServiceCosts { get; set; }
        public DbSet<AllianceService> AllianceServices { get; set; }

        private readonly IMediator _mediator;
        private readonly ILogger<BotContext> _logger;
        //private IDbContextTransaction _currentTransaction;
        //public bool HasActiveTransaction => _currentTransaction != null;

        //public BotContext() : base(GetBuilderOptions()) {}

        public BotContext(DbContextOptions<BotContext> options, ILogger<BotContext> logger) : base(options)
        {
            _logger = logger;
        }
        
        public BotContext(DbContextOptions<BotContext> options, IMediator mediator, ILogger<BotContext> logger) : base(options)
        {
            _mediator = mediator;
            _logger = logger;
        }
        /*
        public static DbContextOptions<BotContext> GetBuilderOptions()
        {
            DbContextOptionsBuilder<BotContext> options = new();
            options.UseMySql("server=localhost; port=3306; database=DiscordBot; user=dbot; Persist Security Info=False; Connect Timeout=300");
            return options.Options;
        }
        */
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<DomainEvent>();
            modelBuilder.ApplyConfiguration(new AuditEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AllianceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AllianceGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AllianceServiceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AllianceServiceLevelEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DiplomacyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DiplomaticRelationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DirectMessageTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceCostEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StarSystemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ZoneEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ZoneNeighbourEntityTypeConfiguration());

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);
            
            var timespanConverter = new ValueConverter<TimeSpan, string>(
                v => v.ToString(),
                v => TimeSpan.Parse(v));

            var nullableTimespanConverter = new ValueConverter<TimeSpan?, string>(
                v => v.HasValue ? v.ToString() : string.Empty,
                v => string.IsNullOrEmpty(v) ? new TimeSpan?() : TimeSpan.Parse(v));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.SetTableName(entityType.GetTableName().ToUpper());
                foreach(var property in entityType.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToUpper());
                }
                
                if (entityType.IsKeyless)
                {
                    continue;
                }

                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                    else if (property.ClrType == typeof(TimeSpan))
                    {
                        property.SetValueConverter(timespanConverter);
                        property.SetMaxLength(50);
                    }
                    else if (property.ClrType == typeof(TimeSpan?))
                    {
                        property.SetValueConverter(nullableTimespanConverter);
                        property.SetMaxLength(50);
                    }

                    switch (property.Name.ToUpper())
                    {
                        case "MODIFIEDBY":
                            property.SetColumnName("MODIFIED_BY");
                            property.SetMaxLength(500);
                            break;
                        case "MODIFIEDDATE":
                            property.SetColumnName("MODIFIED_DATE");
                            property.SetDefaultValueSql("SYSDATE");
                            break;
                    }
                }
            }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                // Dispatch Domain Events collection. 
                // Choices:
                // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
                // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
                // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
                // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
                await _mediator.DispatchDomainEventsAsync(this, _logger, DomainEventType.PreCommit);

                // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
                // performed through the DbContext will be committed
                PrepAuditLogs();
                var result = await base.SaveChangesAsync(cancellationToken);

                await _mediator.DispatchDomainEventsAsync(this, _logger, DomainEventType.PostCommit);

                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Exception thrown when saving entities");
                return false;
            }
        }

        protected void PrepAuditLogs()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                var auditEntry = new AuditEntry(entry);
                auditEntry.TableName = entry.Entity.GetType().Name;
                //auditEntry.UserId = userId;
                auditEntries.Add(auditEntry);
                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }
                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = AuditType.Create;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            break;
                        case EntityState.Deleted:
                            auditEntry.AuditType = AuditType.Delete;
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            break;
                        case EntityState.Modified:
                            if (property.IsModified && property.OriginalValue != property.CurrentValue)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);
                                auditEntry.AuditType = AuditType.Update;
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries)
            {
                AuditLogs.Add(auditEntry.ToAudit());
            }
        }
    }
}
