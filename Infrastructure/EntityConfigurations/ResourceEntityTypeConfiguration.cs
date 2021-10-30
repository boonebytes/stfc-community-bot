using System;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class ResourceEntityTypeConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> resourceConfiguration)
        {
            resourceConfiguration.ToTable("ct_resources");

            resourceConfiguration.HasKey(r => r.Id);

            resourceConfiguration.Property(r => r.Id)
                .HasDefaultValue(0)
                .ValueGeneratedNever()
                .IsRequired();

            resourceConfiguration.Property(r => r.Name)
                .HasMaxLength(200)
                .IsRequired(true);

            resourceConfiguration.Property(r => r.Label)
                .HasMaxLength(200)
                .IsRequired(true);

            resourceConfiguration.HasData(
                data: Resource.List()
                );
        }
    }
}
