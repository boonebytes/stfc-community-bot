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

using DiscordBot.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure.EntityConfigurations
{
    public class AuditEntityTypeConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> auditConfiguration)
        {
            auditConfiguration.ToTable("audit");

            auditConfiguration.HasKey(a => a.Id);
            auditConfiguration.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            auditConfiguration.Property(a => a.Type)
                .HasMaxLength(50);
            auditConfiguration.Property(a => a.TableName)
                .HasColumnName("table_name")
                .HasMaxLength(200);
            auditConfiguration.Property(a => a.DateTime)
                .HasColumnName("date_time");
            auditConfiguration.Property(a => a.OldValues)
                .HasColumnName("old_values");
            auditConfiguration.Property(a => a.NewValues)
                .HasColumnName("new_values");
            auditConfiguration.Property(a => a.AffectedColumns)
                .HasColumnName("affected_columns");
            auditConfiguration.Property(a => a.PrimaryKey)
                .HasColumnName("primary_key");
            auditConfiguration.Property(a => a.UserId)
                .HasColumnName("user_id")
                .HasMaxLength(200);
        }
    }
}
