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

create table STFC.MEMBER_OFFICERS
(
    ID              NUMBER default to_number(sys_guid(), 'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX') not null,
    MEMBER_ID       NUMBER(19)                                                               not null
        constraint MEMBER_OFFICERS_MEMBER_FK
            references STFC.MEMBERS,
    OFFICER_ID      NUMBER(18)                                                               not null
        constraint MEMBER_OFFICERS_OFFICER_FK
            references STFC.CT_OFFICERS,
    OFFICER_RANK_ID NUMBER(5)
        constraint MEMBER_OFFICERS_RANK_ID_FK
            references STFC.CT_OFFICER_RANK,
    MODIFIED_BY     VARCHAR2(1000),
    MODIFIED_DATE   DATE,
    TRAIT1_LEVEL    NUMBER(10),
    TRAIT2_LEVEL    NUMBER(10),
    TRAIT3_LEVEL    NUMBER(10),
    THE_LEVEL       NUMBER default 10
)
/

create unique index STFC.CT_MEMBER_OFFICERS_PK1
    on STFC.MEMBER_OFFICERS (ID)
/

alter table STFC.MEMBER_OFFICERS
    add constraint CT_MEMBER_OFFICERS_PK
        primary key (ID)
/

