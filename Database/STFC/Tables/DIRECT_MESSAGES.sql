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

create table STFC.DIRECT_MESSAGES
(
    ID                 NUMBER(19)   default "STFC"."ISEQ$$_105366".nextval generated by default on null as identity
        constraint PK_DIRECT_MESSAGES
        primary key,
    RECEIVED_TIMESTAMP TIMESTAMP(7)                                                                             not null,
    FROM_USER          NUMBER(20)                                                                               not null,
    COMMON_SERVERS     NCLOB,
    MESSAGE            NVARCHAR2(2000),
    MODIFIED_BY        NVARCHAR2(500),
    MODIFIED_DATE      TIMESTAMP(7) default TO_TIMESTAMP('0001-01-01 00:00:00.000', 'YYYY-MM-DD HH24:MI:SS.FF') not null
)
/

