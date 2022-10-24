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

/*
This script is a rough sample of how to complete a refresh. It was used
to transfer data after a database upgrade. The upgrade started with
Data Pump, but had a few slight structure changes.
*/


-- select * from dba_triggers where not owner = 'STFC' and table_owner = 'STFC';


ALTER TRIGGER STFC.ALLIANCES_T1 DISABLE;
ALTER TRIGGER STFC.ALLIANCE_DIPLOMACY_T1 DISABLE;
ALTER TRIGGER STFC.ALLIANCE_GROUPS_T1 DISABLE;
ALTER TRIGGER STFC.ALLIANCE_INVENTORY_LINE_T1 DISABLE;
ALTER TRIGGER STFC.ALLIANCE_INVENTORY_T1 DISABLE;
ALTER TRIGGER STFC.ALLIANCE_SERVICES_T1 DISABLE;
ALTER TRIGGER STFC.AUDIT_T1 DISABLE;
ALTER TRIGGER STFC.CT_OFFICERS_T1 DISABLE;
ALTER TRIGGER STFC.CT_OFFICER_FACTION_T1 DISABLE;
ALTER TRIGGER STFC.CT_OFFICER_GROUP_T1 DISABLE;
ALTER TRIGGER STFC.CT_OFFICER_RANK_T1 DISABLE;
ALTER TRIGGER STFC.CT_OFFICER_RARITY_T1 DISABLE;
ALTER TRIGGER STFC.CT_OFFICER_TYPE_T1 DISABLE;
ALTER TRIGGER STFC.MEMBERS_T1 DISABLE;
ALTER TRIGGER STFC.MEMBER_OFFICERS_T1 DISABLE;
ALTER TRIGGER STFC.STARSYSTEMS_T1 DISABLE;
ALTER TRIGGER STFC.USERS_T1 DISABLE;
ALTER TRIGGER STFC.ZONES_T1 DISABLE;
ALTER TRIGGER STFC.ZONE_NEIGHBOURS_T1 DISABLE;
ALTER TRIGGER STFC.ZONE_SERVICES_T1 DISABLE;
ALTER TRIGGER STFC.ZONE_SERVICE_COST_T1 DISABLE;

delete from stfc."AUDIT";
insert into stfc."AUDIT" (select * from stfc."AUDIT"@xepdb1_old);

merge into stfc."ALLIANCE_GROUPS" a
using stfc."ALLIANCE_GROUPS"@xepdb1_old old_a
on (a.ID = old_a.ID)
when matched then
    update set
               a.NAME = old_a.NAME,
               a.MODIFIED_DATE = old_a.modified_date,
               a.MODIFIED_BY = old_a.modified_by
when not matched then
    insert (ID, NAME, MODIFIED_DATE, MODIFIED_BY)
    values (old_a.ID, old_a.NAME, old_a.MODIFIED_DATE, old_a.MODIFIED_BY);

merge into stfc."ALLIANCES" a
using stfc."ALLIANCES"@xepdb1_old old_a
on (a.id = old_a.id)
when matched then
    update set
               a.NAME = old_a.name,
               a.ACRONYM = old_a.acronym,
               a.STATUS = old_a.STATUS,
               a.ALLIANCE_GROUP_ID = old_a.alliance_group_id,
               a.GUILD_ID = old_a.GUILD_ID,
               a.DEFEND_SCHEDULE_POST_CHANNEL = old_a.DEFEND_SCHEDULE_POST_CHANNEL,
               a.DEFEND_SCHEDULE_POST_TIME = old_a.DEFEND_SCHEDULE_POST_TIME,
               a.DEFENDBROADCASTPINGROLE = old_a.DEFENDBROADCASTPINGROLE,
               a.DEFENDBROADCASTPINGFORLOWRISK = old_a.DEFENDBROADCASTPINGFORLOWRISK,
               a.ALLIED_BROADCAST_ROLE = old_a.ALLIED_BROADCAST_ROLE,
               a.DEFEND_BROADCAST_LEAD_TIME = old_a.DEFEND_BROADCAST_LEAD_TIME,
               a.NEXT_SCHEDULED_POST = old_a.NEXT_SCHEDULED_POST,
               a.MODIFIED_DATE = old_a.modified_date,
               a.MODIFIED_BY = old_a.modified_by
when not matched then
    insert (ID,
            NAME,
            ACRONYM,
            STATUS,
            ALLIANCE_GROUP_ID,
            GUILD_ID,
            DEFEND_SCHEDULE_POST_CHANNEL,
            DEFEND_SCHEDULE_POST_TIME,
            DEFENDBROADCASTPINGROLE,
            DEFENDBROADCASTPINGFORLOWRISK,
            ALLIED_BROADCAST_ROLE,
            DEFEND_BROADCAST_LEAD_TIME,
            NEXT_SCHEDULED_POST,
            MODIFIED_DATE,
            MODIFIED_BY)
    values (old_a.ID,
            old_a.NAME,
            old_a.ACRONYM,
            old_a.STATUS,
            old_a.ALLIANCE_GROUP_ID,
            old_a.GUILD_ID,
            old_a.DEFEND_SCHEDULE_POST_CHANNEL,
            old_a.DEFEND_SCHEDULE_POST_TIME,
            old_a.DEFENDBROADCASTPINGROLE,
            old_a.DEFENDBROADCASTPINGFORLOWRISK,
            old_a.ALLIED_BROADCAST_ROLE,
            old_a.DEFEND_BROADCAST_LEAD_TIME,
            old_a.NEXT_SCHEDULED_POST,
            old_a.MODIFIED_DATE,
            old_a.MODIFIED_BY);

merge into stfc."ALLIANCE_DIPLOMACY" a
using stfc."ALLIANCE_DIPLOMACY"@xepdb1_old old_a
on (a.ID = old_a.ID)
when matched then
    update set
               a.OWNER_ID = old_a.OWNER_ID,
               a.RELATED_ID = old_a.RELATED_ID,
               a.RELATIONSHIP_ID = old_a.RELATIONSHIP_ID,
               a.MODIFIED_DATE = old_a.modified_date,
               a.MODIFIED_BY = old_a.modified_by
when not matched then
    insert (ID, OWNER_ID, RELATED_ID, RELATIONSHIP_ID, MODIFIED_DATE, MODIFIED_BY)
    values (old_a.ID, old_a.OWNER_ID, old_a.RELATED_ID, old_a.RELATIONSHIP_ID, old_a.MODIFIED_DATE, old_a.MODIFIED_BY);

DELETE FROM stfc.ALLIANCE_INVENTORY_LINE;
DELETE FROM stfc.ALLIANCE_INVENTORY;
INSERT INTO stfc.ALLIANCE_INVENTORY
    (SELECT * FROM stfc.ALLIANCE_INVENTORY@xepdb1_old);
INSERT INTO stfc.ALLIANCE_INVENTORY_LINE
    (SELECT * FROM stfc.ALLIANCE_INVENTORY_LINE@xepdb1_old);

merge into stfc."MEMBERS" a
using stfc."MEMBERS"@xepdb1_old old_a
on (a.id = old_a.id)
when matched then
    update set
               a.NAME = old_a.NAME,
               a.ALLIANCE_ID = old_a.ALLIANCE_ID,
               a.DISCORD = old_a.DISCORD,
               a.THE_LEVEL = old_a.THE_LEVEL,
               a.POWER = old_a.POWER,
               a.POWER_DESTROYED = old_a.POWER_DESTROYED,
               a.PLAYER_SHIPS_DESTROYED = old_a.PLAYER_SHIPS_DESTROYED,
               a.MODIFIED_DATE = old_a.MODIFIED_DATE,
               a.MODIFIED_BY = old_a.MODIFIED_BY
when not matched then
    insert
    (ID, NAME, ALLIANCE_ID, DISCORD, THE_LEVEL, POWER, POWER_DESTROYED, PLAYER_SHIPS_DESTROYED, MODIFIED_DATE, MODIFIED_BY)
    values
        (old_a.ID, old_a.NAME, old_a.ALLIANCE_ID, old_a.DISCORD, old_a.THE_LEVEL, old_a.POWER, old_a.POWER_DESTROYED, old_a.PLAYER_SHIPS_DESTROYED, old_a.MODIFIED_DATE, old_a.MODIFIED_BY);

DELETE FROM stfc.MEMBERS_AUDIT;
INSERT INTO stfc.MEMBERS_AUDIT
    (SELECT * FROM stfc.MEMBERS_AUDIT@xepdb1_old);

merge into stfc."ZONES" a
using stfc."ZONES"@xepdb1_old old_a
on (a.ID = old_a.ID)
when matched then
    update set
               a.NAME = old_a.NAME,
               a.DEFEND_DAY_OF_WEEK = old_a.DEFEND_DAY_OF_WEEK,
               a.DEFEND_EASTERN_DAY = old_a.DEFEND_EASTERN_DAY,
               a.DEFEND_EASTERN_TIME = old_a.DEFEND_EASTERN_TIME,
               a.DEFEND_UTC_TIME = old_a.DEFEND_UTC_TIME,
               a."LEVEL" = old_a."LEVEL",
               a.NOTES = old_a.NOTES,
               a.THREATS = old_a.THREATS,
               a.OWNER_ID = old_a.OWNER_ID,
               a.NEXT_DEFEND = old_a.NEXT_DEFEND,
               a.MODIFIED_DATE = old_a.MODIFIED_DATE,
               a.MODIFIED_BY = old_a.MODIFIED_BY;

merge into stfc."ZONE_SERVICES" a
using stfc."ZONE_SERVICES"@xepdb1_old old_a
on (a.id = old_a.id)
when matched then
    update set
               a.NAME = old_a.NAME,
               a.DESCRIPTION = old_a.DESCRIPTION,
               a.ZONE_ID = old_a.ZONE_ID,
               a.MODIFIED_DATE = old_a.MODIFIED_DATE,
               a.MODIFIED_BY = old_a.MODIFIED_BY
when not matched then
    insert
    (ID, NAME, DESCRIPTION, ZONE_ID, MODIFIED_DATE, MODIFIED_BY)
    values
        (old_a.ID, old_a.NAME, old_a.DESCRIPTION, old_a.ZONE_ID, old_a.MODIFIED_DATE, old_a.MODIFIED_BY);

merge into stfc."ZONE_SERVICE_COST" a
using stfc."ZONE_SERVICE_COST"@xepdb1_old old_a
on (a.id = old_a.id)
when matched then
    update set
               a.COST = old_a.COST,
               a.RESOURCE_ID = old_a.RESOURCE_ID,
               a.SERVICE_ID = old_a.SERVICE_ID,
               a.MODIFIED_DATE = old_a.MODIFIED_DATE,
               a.MODIFIED_BY = old_a.MODIFIED_BY
when not matched then
    insert (ID, COST, RESOURCE_ID, SERVICE_ID, MODIFIED_DATE, MODIFIED_BY)
    values (old_a.ID, old_a.COST, old_a.RESOURCE_ID, old_a.SERVICE_ID, old_a.MODIFIED_DATE, old_a.MODIFIED_BY);

merge into stfc."ALLIANCE_SERVICES" a
using stfc."ALLIANCE_SERVICES"@xepdb1_old old_a
on (a.id = old_a.id)
when matched then
    update set
               a.ZONE_SERVICE_ID = old_a.ZONE_SERVICE_ID,
               a.ALLIANCE_ID = old_a.ALLIANCE_ID,
               a.LEVEL_ID = old_a.LEVEL_ID,
               a.MODIFIED_DATE = old_a.MODIFIED_DATE,
               a.MODIFIED_BY = old_a.MODIFIED_BY
when not matched then
    insert  (ID, ZONE_SERVICE_ID, ALLIANCE_ID, LEVEL_ID, MODIFIED_DATE, MODIFIED_BY)
    values (old_a.ID, old_a.ZONE_SERVICE_ID, old_a.ALLIANCE_ID, old_a.LEVEL_ID, old_a.MODIFIED_DATE, old_a.MODIFIED_BY);

DELETE FROM stfc.DIRECT_MESSAGES;
INSERT INTO stfc.DIRECT_MESSAGES
    (SELECT * FROM stfc.DIRECT_MESSAGES@xepdb1_old);


ALTER TRIGGER STFC.ALLIANCES_T1 ENABLE;
ALTER TRIGGER STFC.ALLIANCE_DIPLOMACY_T1 ENABLE;
ALTER TRIGGER STFC.ALLIANCE_GROUPS_T1 ENABLE;
ALTER TRIGGER STFC.ALLIANCE_INVENTORY_LINE_T1 ENABLE;
ALTER TRIGGER STFC.ALLIANCE_INVENTORY_T1 ENABLE;
ALTER TRIGGER STFC.ALLIANCE_SERVICES_T1 ENABLE;
ALTER TRIGGER STFC.AUDIT_T1 ENABLE;
ALTER TRIGGER STFC.CT_OFFICERS_T1 ENABLE;
ALTER TRIGGER STFC.CT_OFFICER_FACTION_T1 ENABLE;
ALTER TRIGGER STFC.CT_OFFICER_GROUP_T1 ENABLE;
ALTER TRIGGER STFC.CT_OFFICER_RANK_T1 ENABLE;
ALTER TRIGGER STFC.CT_OFFICER_RARITY_T1 ENABLE;
ALTER TRIGGER STFC.CT_OFFICER_TYPE_T1 ENABLE;
ALTER TRIGGER STFC.MEMBERS_T1 ENABLE;
ALTER TRIGGER STFC.MEMBER_OFFICERS_T1 ENABLE;
ALTER TRIGGER STFC.STARSYSTEMS_T1 ENABLE;
ALTER TRIGGER STFC.USERS_T1 ENABLE;
ALTER TRIGGER STFC.ZONES_T1 ENABLE;
ALTER TRIGGER STFC.ZONE_NEIGHBOURS_T1 ENABLE;
ALTER TRIGGER STFC.ZONE_SERVICES_T1 ENABLE;
ALTER TRIGGER STFC.ZONE_SERVICE_COST_T1 ENABLE;
