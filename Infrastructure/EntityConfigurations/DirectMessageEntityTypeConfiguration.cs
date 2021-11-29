using System;
using DiscordBot.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class DirectMessageTypeConfiguration : IEntityTypeConfiguration<DirectMessage>
    {
        public void Configure(EntityTypeBuilder<DirectMessage> directMessageConfiguration)
        {
            directMessageConfiguration.ToTable("direct_messages");

            directMessageConfiguration.HasKey(dm => dm.Id);
            directMessageConfiguration.Property(dm => dm.Id)
                .ValueGeneratedOnAdd();

            directMessageConfiguration.Property(dm => dm.CommonServers)
                .HasMaxLength(4000);
        }
    }
}
