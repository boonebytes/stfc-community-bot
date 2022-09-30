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

create table STFC.ALLIANCE_INVENTORY
(
    ID             NUMBER default to_number(sys_guid(), 'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX') not null
        constraint ALLIANCE_INVENTORY_PK
            primary key,
    EFFECTIVE_DATE DATE                                                                     not null,
    ALLIANCE_ID    NUMBER(19)                                                               not null
        constraint ALLIANCE_INVENTORY_ALLIANCE_ID_FK
            references STFC.ALLIANCES,
    MODIFIED_BY    VARCHAR2(1000)                                                           not null,
    MODIFIED_DATE  DATE                                                                     not null
)
/

create index STFC.ALLIANCE_INVENTORY_I1
    on STFC.ALLIANCE_INVENTORY (ALLIANCE_ID)
/

create index STFC.ALLIANCE_INVENTORY_I3
    on STFC.ALLIANCE_INVENTORY (EFFECTIVE_DATE)
/

