using System;
using System.Threading;
using System.Threading.Tasks;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using DiscordBot.Domain.Seedwork;
using DiscordBot.Infrastructure.EntityConfigurations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBot.Infrastructure
{
    public class BotContext : DbContext, IUnitOfWork
    {
        public DbSet<Alliance> Alliances { get; set; }
        public DbSet<AllianceGroup> AllianceGroups { get; set; }
        public DbSet<Diplomacy> Diplomacies { get; set; }
        public DbSet<DiplomaticRelation> DiplomaticRelaitons { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<StarSystem> StarSystems { get; set; }
        public DbSet<Zone> Zones { get; set; }

        private readonly IMediator _mediator;
        //private IDbContextTransaction _currentTransaction;
        //public bool HasActiveTransaction => _currentTransaction != null;

        //public BotContext() : base(GetBuilderOptions()) {}

        public BotContext(DbContextOptions<BotContext> options) : base(options) { }
        public BotContext(DbContextOptions<BotContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator;
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
            modelBuilder.ApplyConfiguration(new AllianceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AllianceGroupEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DiplomacyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DiplomaticRelationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new StarSystemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ZoneEntityTypeConfiguration());

            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

            var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue ? v.Value.ToUniversalTime() : v,
                v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
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
                }
            }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await _mediator.DispatchDomainEventsAsync(this);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
