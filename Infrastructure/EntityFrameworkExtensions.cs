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
using System.Linq.Expressions;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscordBot.Infrastructure;

internal static class EntityFrameworkExtensions
{
    internal static void HasOwnEnumeration<TEntity, TEnum>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, TEnum>> property,
        string propertyName,
        string columnName,
        bool isRequired)
        where TEntity : class
        where TEnum : Enumeration
    {
        builder.Property(property)
            .HasColumnName(columnName)
            .HasConversion(x => x.Id, x => Enumeration.FromValue<TEnum>(x))
            .IsRequired(isRequired);
        
        builder.HasIndex(propertyName);
        
    }
        
}