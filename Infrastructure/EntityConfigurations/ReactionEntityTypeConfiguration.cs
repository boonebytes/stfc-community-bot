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
    public class ReactionEntityTypeConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> configuration)
        {
            configuration.ToTable("react_message_reactions");

            configuration.HasKey(e => e.Id);
            configuration.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            configuration.Property(e => e.UserId)
                .HasColumnName("user_id")
                .IsRequired();

            configuration.Property(e => e.Username)
                .HasColumnName("username")
                .HasMaxLength(500)
                .IsRequired();

            configuration.Property(e => e.Nickname)
                .HasColumnName("nickname")
                .HasMaxLength(500)
                .IsRequired(false);

            configuration.Property(e => e.ReactionReceived)
                .HasColumnName("reaction_received")
                .IsRequired();

            

            configuration
                .Property<long>("_messageId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("message_id")
                .IsRequired();

            configuration.HasOne<ReactMessage>(e => e.Message)
                .WithMany(r => r.Reactions)
                .HasForeignKey("_messageId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            var messageNavigation = configuration.Metadata.FindNavigation(nameof(Reaction.Message));
            messageNavigation.SetPropertyAccessMode(PropertyAccessMode.Field);
            messageNavigation.SetIsEagerLoaded(true);
        }
    }
}
