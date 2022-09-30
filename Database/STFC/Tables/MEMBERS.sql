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

create table STFC.MEMBERS
(
    ID                     NUMBER(19) generated by default on null as identity,
    ALLIANCE_ID            NUMBER(19)
        constraint MEMBERS_ALLIANCE_ID_FK
            references STFC.ALLIANCES,
    NAME                   VARCHAR2(2000),
    DISCORD                VARCHAR2(2000),
    THE_LEVEL              NUMBER,
    POWER                  NUMBER,
    POWER_DESTROYED        NUMBER,
    PLAYER_SHIPS_DESTROYED NUMBER,
    MODIFIED_BY            VARCHAR2(1000),
    MODIFIED_DATE          DATE
)
/

create index STFC.MEMBERS_I1
    on STFC.MEMBERS (ALLIANCE_ID)
/

create unique index STFC.MEMBERS_PK1
    on STFC.MEMBERS (ID)
/

alter table STFC.MEMBERS
    add constraint MEMBERS_PK
        primary key (ID)
/

