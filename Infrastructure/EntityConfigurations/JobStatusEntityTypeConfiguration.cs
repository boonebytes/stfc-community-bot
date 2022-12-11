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
    public class JobStatusEntityTypeConfiguration : IEntityTypeConfiguration<JobStatus>
    {
        public void Configure(EntityTypeBuilder<JobStatus> jobStatusConfiguration)
        {
            jobStatusConfiguration.ToTable("ct_job_status");

            jobStatusConfiguration.HasKey(e => e.Id);

            jobStatusConfiguration.Property(e => e.Id)
                .HasDefaultValue(0)
                .ValueGeneratedNever()
                .IsRequired();

            jobStatusConfiguration.Property(e => e.Name)
                .HasMaxLength(200)
                .IsRequired(true);

            jobStatusConfiguration.Property(e => e.Label)
                .HasMaxLength(200)
                .IsRequired(true);

            jobStatusConfiguration.HasData(
                data: JobStatus.List()
                );
        }
    }
}
