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
                .HasColumnName("common_servers")
                .HasMaxLength(4000);

            directMessageConfiguration.Property(dm => dm.ReceivedTimestamp)
                .HasColumnName("received_timestamp");
            directMessageConfiguration.Property(dm => dm.FromUser)
                .HasColumnName("from_user");
        }
    }
}
