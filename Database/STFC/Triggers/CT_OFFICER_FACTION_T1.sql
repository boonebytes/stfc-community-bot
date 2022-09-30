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

create or replace trigger STFC.CT_OFFICER_FACTION_T1
    before insert or update or delete
    on STFC.CT_OFFICER_FACTION
    for each row
declare
    v_audit_user varchar(1000) := coalesce(sys_context('APEX$SESSION','APP_USER'),user);
begin
    IF NOT DELETING THEN
        :new.modified_by := v_audit_user;
        :new.modified_date := sysdate;
    END IF;
end;
/

