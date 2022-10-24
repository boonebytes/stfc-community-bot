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

create or replace PACKAGE STFC.PKG_OFFICER AS

    PROCEDURE IMPORT_OFFICERS_FROM_TOOL(p_member_id MEMBERS.ID%type, p_data in out nocopy clob);

    FUNCTION IMPORT_OFFICER_FROM_TOOL (p_member_id MEMBERS.ID%type, p_line varchar2)
        RETURN MEMBER_OFFICERS%rowtype;

    FUNCTION FIND_OFFICER_ID(p_officer_name CT_OFFICERS.NAME%type, p_rarity varchar2)
        RETURN CT_OFFICERS.ID%type;

END PKG_OFFICER;
/
