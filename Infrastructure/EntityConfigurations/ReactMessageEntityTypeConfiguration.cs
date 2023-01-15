/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class ReactMessageEntityTypeConfiguration : IEntityTypeConfiguration<ReactMessage>
    {
        public void Configure(EntityTypeBuilder<ReactMessage> configuration)
        {
            configuration.ToTable("react_messages");

            configuration.HasKey(e => e.Id);
            configuration.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            configuration.Property(e => e.FromUserId)
                .HasColumnName("from_user_id")
                .IsRequired();

            configuration.Property(e => e.FromUsername)
                .HasColumnName("from_username")
                .HasMaxLength(500)
                .IsRequired();

            configuration.Property(e => e.Posted)
                .IsRequired(false);

            configuration.Property(e => e.GuildId)
                .HasColumnName("guild_id")
                .IsRequired();
            
            configuration.Property(e => e.ChannelId)
                .HasColumnName("channel_id")
                .IsRequired();
            
            configuration.Property(e => e.MessageId)
                .HasColumnName("discord_message_id")
                .IsRequired(false);
            
            configuration.Property(e => e.Message)
                .HasMaxLength(4000)
                .IsRequired();
            
            configuration.Property(e => e.ResponseText)
                .HasMaxLength(500)
                .IsRequired();

            


            configuration
                .Property<long>("_allianceId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("alliance_id")
                .IsRequired();

            configuration.HasOne<Alliance>(e => e.Alliance)
                .WithMany()
                .HasForeignKey("_allianceId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            var allianceNavigation = configuration.Metadata.FindNavigation(nameof(ReactMessage.Alliance));
            allianceNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            allianceNavigation.SetIsEagerLoaded(true);
            
            configuration.HasMany<Reaction>(e => e.Reactions)
                .WithOne(re => re.Message)
                .OnDelete(DeleteBehavior.Cascade);

            var reactionsNavigation = configuration.Metadata.FindNavigation(nameof(ReactMessage.Reactions));
            reactionsNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            reactionsNavigation.SetIsEagerLoaded(true);
        }
    }
}
