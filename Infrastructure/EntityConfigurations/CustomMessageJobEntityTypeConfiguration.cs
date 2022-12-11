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

using DiscordBot.Infrastructure;
using DiscordBot.Domain.Entities.Admin;
using DiscordBot.Domain.Entities.Alliances;
using DiscordBot.Domain.Entities.Zones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations;

public class CustomMessageJobEntityTypeConfiguration : IEntityTypeConfiguration<CustomMessageJob>
{
    public void Configure(EntityTypeBuilder<CustomMessageJob> jobEntityConfiguration)
    {
        jobEntityConfiguration.ToTable("custom_message_jobs");

        jobEntityConfiguration.HasKey(j => j.Id);

        jobEntityConfiguration.Property(j => j.Id)
            .ValueGeneratedOnAdd();

        jobEntityConfiguration.Property(j => j.ScheduledTimestamp)
            .HasColumnName("scheduled_timestamp")
            .IsRequired();
        
        jobEntityConfiguration.Property(j => j.FromUser)
            .HasColumnName("from_user")
            .IsRequired();
        
        jobEntityConfiguration.Property(j => j.FromUsername)
            .HasColumnName("from_username")
            .HasMaxLength(200)
            .IsRequired();
        
        jobEntityConfiguration.Property(j => j.FromUserNickname)
            .HasColumnName("from_user_nickname")
            .HasMaxLength(200)
            .IsRequired(false);
        
        jobEntityConfiguration.Property(j => j.ChannelId)
            .HasColumnName("channel_id")
            .IsRequired();
        
        
        
        var allianceNav = jobEntityConfiguration.Metadata.FindNavigation(nameof(CustomMessageJob.Alliance));
        allianceNav.SetPropertyAccessMode(PropertyAccessMode.Field);
        allianceNav.SetIsEagerLoaded(true);

        jobEntityConfiguration
            .Property<long?>("_allianceId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("alliance_id")
            .IsRequired(false);

        jobEntityConfiguration.HasOne<Alliance>(j => j.Alliance)
            .WithMany()
            .HasForeignKey("_allianceId")
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);
        
        
        jobEntityConfiguration.Property(j => j.Message)
            .HasMaxLength(500)
            .IsRequired();
        
        jobEntityConfiguration
            .HasOwnEnumeration(
                j => j.Status,
                nameof(CustomMessageJob.Status),
                "job_status_id",
                true);
        
    }
}
