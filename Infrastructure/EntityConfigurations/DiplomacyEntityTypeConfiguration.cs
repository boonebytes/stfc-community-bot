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
                .WithMany()
                .HasForeignKey("_ownerId")
                .OnDelete(DeleteBehavior.Cascade);


            diplomacyConfiguration
                .Property<long>("_relatedId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("RelatedId")
                .IsRequired(true);

            diplomacyConfiguration.HasOne<Alliance>(d => d.Related)
                .WithMany()
                .HasForeignKey("_relatedId")
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
