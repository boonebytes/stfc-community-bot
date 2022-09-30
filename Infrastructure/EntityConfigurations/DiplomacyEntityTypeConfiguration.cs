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

using DiscordBot.Domain.Entities.Alliances;
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
                .HasColumnName("owner_id")
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
                .HasColumnName("related_id")
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
                .HasColumnName("relationship_id")
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
