using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AllianceEntityTypeConfiguration : IEntityTypeConfiguration<Alliance>
    {
        public void Configure(EntityTypeBuilder<Alliance> allianceConfiguration)
        {
            allianceConfiguration.ToTable("alliances");

            allianceConfiguration.HasKey(a => a.Id);
            allianceConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            allianceConfiguration.Property(a => a.Name)
                .HasMaxLength(2000);

            allianceConfiguration.Property(a => a.Acronym)
                .HasMaxLength(10);

            allianceConfiguration.Property(a => a.DefendSchedulePostTime)
                .HasColumnName("defend_schedule_post_time")
                .HasMaxLength(10);

            allianceConfiguration.Property(a => a.DefendBroadcastLeadTime)
                .HasColumnName("defend_broadcast_lead_time")
                .IsRequired(false);
            
            allianceConfiguration.Property(a => a.AlliedBroadcastRole)
                .HasColumnName("allied_broadcast_role")
                .IsRequired(false);

            allianceConfiguration
                .Property<long?>("_allianceGroupId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("alliance_group_id")
                .IsRequired(false);

            allianceConfiguration.HasOne<AllianceGroup>(a => a.Group)
                .WithMany(ag => ag.Alliances)
                .HasForeignKey("_allianceGroupId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            var groupNavigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.Group));
            groupNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            groupNavigation.SetIsEagerLoaded(true);

            allianceConfiguration.Property(a => a.GuildId)
                .HasColumnName("guild_id");
            allianceConfiguration.Property(a => a.DefendSchedulePostChannel)
                .HasColumnName("defend_schedule_post_channel");
            allianceConfiguration.Property(a => a.NextScheduledPost)
                .HasColumnName("next_scheduled_post");

            
            allianceConfiguration.HasMany<Diplomacy>(a => a.AssignedDiplomacy)
                .WithOne(d => d.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            var assignedDiplomacyNavigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.AssignedDiplomacy));
            assignedDiplomacyNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            assignedDiplomacyNavigation.SetIsEagerLoaded(true);

            allianceConfiguration.HasMany<Diplomacy>(a => a.ReceivedDiplomacy)
                .WithOne(d => d.Related)
                .OnDelete(DeleteBehavior.Cascade);

            var receivedDiplomacyNavigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.ReceivedDiplomacy));
            receivedDiplomacyNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
