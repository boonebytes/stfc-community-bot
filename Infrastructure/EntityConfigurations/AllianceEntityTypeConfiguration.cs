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
                .HasMaxLength(200);

            allianceConfiguration.Property(a => a.Acronym)
                .HasMaxLength(5);

            allianceConfiguration.Property(a => a.DefendSchedulePostTime)
                .HasMaxLength(10);


            allianceConfiguration
                .Property<long?>("_allianceGroupId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("AllianceGroupId")
                .IsRequired(false);

            allianceConfiguration.HasOne<AllianceGroup>(a => a.Group)
                .WithMany(ag => ag.Alliances)
                .HasForeignKey("_allianceGroupId")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            var groupNavigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.Group));
            groupNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);


            
            allianceConfiguration.HasMany<Diplomacy>(a => a.AssignedDiplomacy)
                .WithOne(d => d.Owner)
                .OnDelete(DeleteBehavior.Cascade);

            var assignedDiplomacyNavigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.AssignedDiplomacy));
            assignedDiplomacyNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            allianceConfiguration.HasMany<Diplomacy>(a => a.ReceivedDiplomacy)
                .WithOne(d => d.Related)
                .OnDelete(DeleteBehavior.Cascade);

            var receivedDiplomacyNavigation = allianceConfiguration.Metadata.FindNavigation(nameof(Alliance.ReceivedDiplomacy));
            receivedDiplomacyNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
