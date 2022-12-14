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

create table STFC.ALLIANCE_SERVICES
(
    ID              NUMBER(19)   default "STFC"."ISEQ$$_106913".nextval generated as identity
        constraint ALLIANCE_SERVICES_PK
        primary key,
    ALLIANCE_ID     NUMBER(19)     not null
        constraint ALLIANCE_SERVICES_FK1
            references STFC.ALLIANCES,
    ZONE_SERVICE_ID NUMBER(19)     not null
        constraint ALLIANCE_SERVICES_FK2
            references STFC.ZONE_SERVICES,
    LEVEL_ID        NUMBER(10)     not null
        constraint ALLIANCE_SERVICES_FK3
            references STFC.CT_ALLIANCE_SERVICE_LEVEL,
    MODIFIED_BY     VARCHAR2(1000) not null,
    MODIFIED_DATE   TIMESTAMP(6) default sysdate,
    constraint ALLIANCE_SERVICES_UK1
        unique (ALLIANCE_ID, ZONE_SERVICE_ID)
)
/

