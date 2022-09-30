﻿// <auto-generated />
using System;
using DiscordBot.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

namespace DiscordBot.Infrastructure.Migrations
{
    [DbContext(typeof(BotContext))]
    partial class BotContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "3.1.21")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            modelBuilder.Entity("DiscordBot.Domain.Entities.Admin.DirectMessage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CommonServers")
                        .HasColumnName("COMMON_SERVERS")
                        .HasColumnType("NCLOB")
                        .HasMaxLength(4000);

                    b.Property<decimal>("FromUser")
                        .HasColumnName("FROM_USER")
                        .HasColumnType("NUMBER(20)");

                    b.Property<string>("Message")
                        .HasColumnName("MESSAGE")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<DateTime>("ReceivedTimestamp")
                        .HasColumnName("RECEIVED_TIMESTAMP")
                        .HasColumnType("TIMESTAMP(7)");

                    b.HasKey("Id");

                    b.ToTable("DIRECT_MESSAGES");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Alliance", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Acronym")
                        .HasColumnName("ACRONYM")
                        .HasColumnType("NVARCHAR2(10)")
                        .HasMaxLength(10);

                    b.Property<decimal?>("AlliedBroadcastRole")
                        .HasColumnName("ALLIED_BROADCAST_ROLE")
                        .HasColumnType("NUMBER(20)");

                    b.Property<int?>("DefendBroadcastLeadTime")
                        .HasColumnName("DEFEND_BROADCAST_LEAD_TIME")
                        .HasColumnType("NUMBER(10)");

                    b.Property<bool?>("DefendBroadcastPingForLowRisk")
                        .HasColumnName("DEFENDBROADCASTPINGFORLOWRISK")
                        .HasColumnType("NUMBER(1)");

                    b.Property<decimal?>("DefendBroadcastPingRole")
                        .HasColumnName("DEFENDBROADCASTPINGROLE")
                        .HasColumnType("NUMBER(20)");

                    b.Property<decimal?>("DefendSchedulePostChannel")
                        .HasColumnName("DEFEND_SCHEDULE_POST_CHANNEL")
                        .HasColumnType("NUMBER(20)");

                    b.Property<string>("DefendSchedulePostTime")
                        .HasColumnName("DEFEND_SCHEDULE_POST_TIME")
                        .HasColumnType("NVARCHAR2(10)")
                        .HasMaxLength(10);

                    b.Property<decimal?>("GuildId")
                        .HasColumnName("GUILD_ID")
                        .HasColumnType("NUMBER(20)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<string>("Name")
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(2000)")
                        .HasMaxLength(2000);

                    b.Property<DateTime?>("NextScheduledPost")
                        .HasColumnName("NEXT_SCHEDULED_POST")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<long?>("_allianceGroupId")
                        .HasColumnName("ALLIANCE_GROUP_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_allianceGroupId");

                    b.ToTable("ALLIANCES");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.AllianceGroup", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<string>("Name")
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("ALLIANCE_GROUPS");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Diplomacy", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<long>("_ownerId")
                        .HasColumnName("OWNER_ID")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("_relatedId")
                        .HasColumnName("RELATED_ID")
                        .HasColumnType("NUMBER(19)");

                    b.Property<int>("_relationshipId")
                        .HasColumnName("RELATIONSHIP_ID")
                        .HasColumnType("NUMBER(10)");

                    b.HasKey("Id");

                    b.HasIndex("_ownerId");

                    b.HasIndex("_relatedId");

                    b.HasIndex("_relationshipId");

                    b.ToTable("ALLIANCE_DIPLOMACY");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.DiplomaticRelation", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(10)")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("CT_DIPLOMATIC_RELATION");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Unspecified"
                        },
                        new
                        {
                            Id = -99,
                            Name = "Enemy"
                        },
                        new
                        {
                            Id = -1,
                            Name = "Untrusted"
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

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.AllianceService", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<long?>("_allianceId")
                        .IsRequired()
                        .HasColumnName("ALLIANCE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.Property<int?>("_allianceServiceLevelId")
                        .IsRequired()
                        .HasColumnName("LEVEL_ID")
                        .HasColumnType("NUMBER(10)");

                    b.Property<long?>("_serviceId")
                        .IsRequired()
                        .HasColumnName("ZONE_SERVICE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_allianceId");

                    b.HasIndex("_allianceServiceLevelId");

                    b.HasIndex("_serviceId");

                    b.ToTable("ALLIANCE_SERVICES");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.AllianceServiceLevel", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(10)")
                        .HasDefaultValue(0);

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnName("LABEL")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("CT_ALLIANCE_SERVICE_LEVEL");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Label = "Undefined",
                            Name = "Undefined"
                        },
                        new
                        {
                            Id = 1,
                            Label = "Disabled",
                            Name = "Disabled"
                        },
                        new
                        {
                            Id = 2,
                            Label = "Redundant",
                            Name = "Redundant"
                        },
                        new
                        {
                            Id = 3,
                            Label = "Desired",
                            Name = "Desired"
                        },
                        new
                        {
                            Id = 4,
                            Label = "Preferred",
                            Name = "Preferred"
                        },
                        new
                        {
                            Id = 5,
                            Label = "Basic",
                            Name = "Basic"
                        });
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.Service", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnName("DESCRIPTION")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.Property<long?>("_zoneId")
                        .IsRequired()
                        .HasColumnName("ZONE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_zoneId");

                    b.ToTable("ZONE_SERVICES");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.ServiceCost", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("Cost")
                        .HasColumnName("COST")
                        .HasColumnType("NUMBER(19)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<int?>("_resourceId")
                        .IsRequired()
                        .HasColumnName("RESOURCE_ID")
                        .HasColumnType("NUMBER(10)");

                    b.Property<long?>("_serviceId")
                        .IsRequired()
                        .HasColumnName("SERVICE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_resourceId");

                    b.HasIndex("_serviceId");

                    b.ToTable("ZONE_SERVICE_COST");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.Resource", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(10)")
                        .HasDefaultValue(0);

                    b.Property<string>("Label")
                        .IsRequired()
                        .HasColumnName("LABEL")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("CT_RESOURCES");

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
                        },
                        new
                        {
                            Id = 51,
                            Label = "Refined Isogen Tier 1",
                            Name = "RefinedIsogenTier1"
                        },
                        new
                        {
                            Id = 52,
                            Label = "Refined Isogen Tier 2",
                            Name = "RefinedIsogenTier2"
                        },
                        new
                        {
                            Id = 53,
                            Label = "Refined Isogen Tier 3",
                            Name = "RefinedIsogenTier3"
                        },
                        new
                        {
                            Id = 61,
                            Label = "Progenitor Emitters",
                            Name = "ProgenitorEmitters"
                        },
                        new
                        {
                            Id = 62,
                            Label = "Progenitor Diodes",
                            Name = "ProgenitorDiodes"
                        },
                        new
                        {
                            Id = 63,
                            Label = "Progenitor Cores",
                            Name = "ProgenitorCores"
                        },
                        new
                        {
                            Id = 64,
                            Label = "Progenitor Reactors",
                            Name = "ProgenitorReactors"
                        },
                        new
                        {
                            Id = 71,
                            Label = "Collisional Plasma",
                            Name = "CollisionalPlasma"
                        },
                        new
                        {
                            Id = 72,
                            Label = "Magnetic Plasma",
                            Name = "MagneticPlasma"
                        },
                        new
                        {
                            Id = 73,
                            Label = "Subspace Superconductors",
                            Name = "SubspaceSuperconductor"
                        },
                        new
                        {
                            Id = 74,
                            Label = "Alliance Reserves",
                            Name = "AllianceReserves"
                        });
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.StarSystem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.Property<int?>("_resourceId")
                        .HasColumnName("RESOURCE_ID")
                        .HasColumnType("NUMBER(10)");

                    b.Property<long?>("_zoneId")
                        .IsRequired()
                        .HasColumnName("ZONE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_resourceId");

                    b.HasIndex("_zoneId");

                    b.ToTable("STARSYSTEMS");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.Zone", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("DefendEasternDay")
                        .HasColumnName("DEFEND_EASTERN_DAY")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("DefendEasternTime")
                        .HasColumnName("DEFEND_EASTERN_TIME")
                        .HasColumnType("NVARCHAR2(50)")
                        .HasMaxLength(50);

                    b.Property<string>("DefendUtcDayOfWeek")
                        .IsRequired()
                        .HasColumnName("DEFEND_DAY_OF_WEEK")
                        .HasColumnType("NVARCHAR2(15)")
                        .HasMaxLength(15);

                    b.Property<string>("DefendUtcTime")
                        .IsRequired()
                        .HasColumnName("DEFEND_UTC_TIME")
                        .HasColumnType("NVARCHAR2(10)")
                        .HasMaxLength(10);

                    b.Property<int>("Level")
                        .HasColumnName("LEVEL")
                        .HasColumnType("NUMBER(10)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnName("NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("NextDefend")
                        .HasColumnName("NEXT_DEFEND")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("Notes")
                        .HasColumnName("NOTES")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<long?>("_ownerId")
                        .HasColumnName("OWNER_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_ownerId");

                    b.ToTable("ZONES");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.ZoneNeighbour", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<long>("_fromZoneId")
                        .HasColumnName("FROM_ZONE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.Property<long>("_toZoneId")
                        .HasColumnName("TO_ZONE_ID")
                        .HasColumnType("NUMBER(19)");

                    b.HasKey("Id");

                    b.HasIndex("_fromZoneId");

                    b.HasIndex("_toZoneId");

                    b.ToTable("ZONE_NEIGHBOURS");
                });

            modelBuilder.Entity("DiscordBot.Infrastructure.Entities.Audit", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ID")
                        .HasColumnType("NUMBER(19)")
                        .HasAnnotation("Oracle:ValueGenerationStrategy", OracleValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AffectedColumns")
                        .HasColumnName("AFFECTED_COLUMNS")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnName("DATE_TIME")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("ModifiedBy")
                        .HasColumnName("MODIFIED_BY")
                        .HasColumnType("NVARCHAR2(500)")
                        .HasMaxLength(500);

                    b.Property<DateTime>("ModifiedDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("MODIFIED_DATE")
                        .HasColumnType("TIMESTAMP(7)")
                        .HasDefaultValueSql("SYSDATE");

                    b.Property<string>("NewValues")
                        .HasColumnName("NEW_VALUES")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("OldValues")
                        .HasColumnName("OLD_VALUES")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PrimaryKey")
                        .HasColumnName("PRIMARY_KEY")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("TableName")
                        .HasColumnName("TABLE_NAME")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.Property<string>("Type")
                        .HasColumnName("TYPE")
                        .HasColumnType("NVARCHAR2(50)")
                        .HasMaxLength(50);

                    b.Property<string>("UserId")
                        .HasColumnName("USER_ID")
                        .HasColumnType("NVARCHAR2(200)")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("AUDIT");
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Alliance", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Alliances.AllianceGroup", "Group")
                        .WithMany("Alliances")
                        .HasForeignKey("_allianceGroupId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Alliances.Diplomacy", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Alliances.Alliance", "Owner")
                        .WithMany("AssignedDiplomacy")
                        .HasForeignKey("_ownerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Alliances.Alliance", "Related")
                        .WithMany("ReceivedDiplomacy")
                        .HasForeignKey("_relatedId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Alliances.DiplomaticRelation", "Relationship")
                        .WithMany()
                        .HasForeignKey("_relationshipId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.AllianceService", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Alliances.Alliance", "Alliance")
                        .WithMany("AllianceServices")
                        .HasForeignKey("_allianceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Services.AllianceServiceLevel", "AllianceServiceLevel")
                        .WithMany()
                        .HasForeignKey("_allianceServiceLevelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Services.Service", "Service")
                        .WithMany("AllianceServices")
                        .HasForeignKey("_serviceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.Service", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Zones.Zone", "Zone")
                        .WithMany("Services")
                        .HasForeignKey("_zoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Services.ServiceCost", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Zones.Resource", "Resource")
                        .WithMany()
                        .HasForeignKey("_resourceId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Services.Service", "Service")
                        .WithMany("Costs")
                        .HasForeignKey("_serviceId")
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
                        .WithMany("Zones")
                        .HasForeignKey("_ownerId")
                        .OnDelete(DeleteBehavior.SetNull);
                });

            modelBuilder.Entity("DiscordBot.Domain.Entities.Zones.ZoneNeighbour", b =>
                {
                    b.HasOne("DiscordBot.Domain.Entities.Zones.Zone", "FromZone")
                        .WithMany("ZoneNeighbours")
                        .HasForeignKey("_fromZoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("DiscordBot.Domain.Entities.Zones.Zone", "ToZone")
                        .WithMany()
                        .HasForeignKey("_toZoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
