using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class DiplomacyEntityTypeConfiguration : IEntityTypeConfiguration<Diplomacy>
    {
        public void Configure(EntityTypeBuilder<Diplomacy> diplomacyConfiguration)
        {
            diplomacyConfiguration.ToTable("alliance_diplomacy");

            diplomacyConfiguration.HasKey(a => a.Id);
            diplomacyConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            
            diplomacyConfiguration
                .Property<long>("_ownerId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OwnerId")
                .IsRequired(true);

            diplomacyConfiguration.HasOne<Alliance>(d => d.Owner)
                .WithMany(a => a.AssignedDiplomacy)
                .HasForeignKey("_ownerId")
                .OnDelete(DeleteBehavior.Cascade);

            var ownerNavigation = diplomacyConfiguration.Metadata.FindNavigation(nameof(Diplomacy.Owner));
            ownerNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);


            diplomacyConfiguration
                .Property<long>("_relatedId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RelatedId")
                .IsRequired(true);

            diplomacyConfiguration.HasOne<Alliance>(d => d.Related)
                .WithMany(a => a.ReceivedDiplomacy)
                .HasForeignKey("_relatedId")
                .OnDelete(DeleteBehavior.Cascade);

            var relatedNavigation = diplomacyConfiguration.Metadata.FindNavigation(nameof(Diplomacy.Related));
            relatedNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);


            diplomacyConfiguration
                .Property<int>("_relationshipId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RelationshipId")
                .IsRequired(true);

            diplomacyConfiguration.HasOne<DiplomaticRelation>(d => d.Relationship)
                .WithMany()
                .HasForeignKey("_relationshipId")
                .OnDelete(DeleteBehavior.Restrict);

            var relationshipNavigation = diplomacyConfiguration.Metadata.FindNavigation(nameof(Diplomacy.Relationship));
            relationshipNavigation.SetIsEagerLoaded(true);
            relationshipNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
