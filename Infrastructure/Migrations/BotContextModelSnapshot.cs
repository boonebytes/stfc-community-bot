﻿// <auto-generated />
using System;
using DiscordBot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DiscordBot.Infrastructure.Migrations
{
    [DbContext(typeof(BotContext))]
    partial class BotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Alliance", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Acronym")
                        .HasColumnType("varchar(5) CHARACTER SET utf8mb4")
                        .HasMaxLength(5);

                    b.Property<ulong?>("DefendSchedulePostChannel")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("DefendSchedulePostTime")
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.Property<ulong?>("GuildId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("alliances");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Diplomacy", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("_ownerId")
                        .HasColumnName("OwnerId")
                        .HasColumnType("bigint");

                    b.Property<long>("_relatedId")
                        .HasColumnName("RelatedId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("_ownerId");

                    b.HasIndex("_relatedId");

                    b.ToTable("alliance_diplomacy");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.DiplomaticRelation", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("ct_diplomatic_relation");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Unspecified"
                        },
                        new
                        {
                            Id = -1,
                            Name = "Enemy"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Neutral"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Friendly"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Allied"
                        });
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.Resource", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("ct_resources");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Label = "Unspecified",
                            Name = "Unspecified"
                        },
                        new
                        {
                            Id = 1,
                            Label = "Parasteel",
                            Name = "Parasteel"
                        },
                        new
                        {
                            Id = 2,
                            Label = "Tritanium",
                            Name = "Tritanium"
                        },
                        new
                        {
                            Id = 3,
                            Label = "Dilithium",
                            Name = "Dilithium"
                        },
                        new
                        {
                            Id = 11,
                            Label = "Isogen Tier 1",
                            Name = "IsogenTier1"
                        },
                        new
                        {
                            Id = 12,
                            Label = "Isogen Tier 2",
                            Name = "IsogenTier2"
                        },
                        new
                        {
                            Id = 13,
                            Label = "Isogen Tier 3",
                            Name = "IsogenTier3"
                        },
                        new
                        {
                            Id = 23,
                            Label = "Gas Tier 3",
                            Name = "GasTier3"
                        },
                        new
                        {
                            Id = 24,
                            Label = "Gas Tier 4",
                            Name = "GasTier4"
                        },
                        new
                        {
                            Id = 33,
                            Label = "Crystal Tier 3",
                            Name = "CrystalTier3"
                        },
                        new
                        {
                            Id = 34,
                            Label = "Crystal Tier 4",
                            Name = "CrystalTier4"
                        },
                        new
                        {
                            Id = 43,
                            Label = "Ore Tier 3",
                            Name = "OreTier3"
                        },
                        new
                        {
                            Id = 44,
                            Label = "Ore Tier 4",
                            Name = "OreTier4"
                        });
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.StarSystem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<int?>("_resourceId")
                        .HasColumnName("ResourceId")
                        .HasColumnType("int");

                    b.Property<long?>("_zoneId")
                        .IsRequired()
                        .HasColumnName("ZoneId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("_resourceId");

                    b.HasIndex("_zoneId");

                    b.ToTable("starsystems");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.Zone", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("DefendUtcDayOfWeek")
                        .IsRequired()
                        .HasColumnType("varchar(15) CHARACTER SET utf8mb4")
                        .HasMaxLength(15);

                    b.Property<string>("DefendUtcTime")
                        .IsRequired()
                        .HasColumnType("varchar(10) CHARACTER SET utf8mb4")
                        .HasMaxLength(10);

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(200) CHARACTER SET utf8mb4")
                        .HasMaxLength(200);

                    b.Property<string>("Notes")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Threats")
                        .HasColumnType("varchar(2000) CHARACTER SET utf8mb4")
                        .HasMaxLength(2000);

                    b.Property<long?>("_ownerId")
                        .HasColumnName("OwnerId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("_ownerId");

                    b.ToTable("zones");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Diplomacy", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Alliances.Alliance", "Owner")
                        .WithMany("Diplomacy")
                        .HasForeignKey("_ownerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Alliances.Alliance", "Related")
                        .WithMany()
                        .HasForeignKey("_relatedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.StarSystem", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Zones.Resource", null)
                        .WithMany()
                        .HasForeignKey("_resourceId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DiscordBot.Domain.Entities.Zones.Zone", "Zone")
                        .WithMany("StarSystems")
                        .HasForeignKey("_zoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.Zone", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Alliances.Alliance", "Owner")
                        .WithMany()
                        .HasForeignKey("_ownerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });
#pragma warning restore 612, 618
        }
    }
}
