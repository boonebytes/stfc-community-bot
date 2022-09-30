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

create table STFC.MEMBERS_AUDIT
(
    ID                     NUMBER default to_number(sys_guid(), 'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX') not null,
    ACTION                 VARCHAR2(6),
    EVENT_TIMESTAMP        TIMESTAMP(6),
    MEMBER_ID              NUMBER(19),
    ALLIANCE_ID            NUMBER(19),
    NAME                   VARCHAR2(2000),
    DISCORD                VARCHAR2(2000),
    THE_LEVEL              NUMBER,
    POWER                  NUMBER,
    POWER_DESTROYED        NUMBER,
    PLAYER_SHIPS_DESTROYED NUMBER,
    MODIFIED_BY            VARCHAR2(1000),
    MODIFIED_DATE          DATE,
    AUDIT_USER             VARCHAR2(1000)
)
/

