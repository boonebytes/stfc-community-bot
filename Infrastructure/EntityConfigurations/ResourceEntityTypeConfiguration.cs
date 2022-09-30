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
