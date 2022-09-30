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

create or replace trigger STFC.MEMBERS_T1
    before insert or update or delete
    on STFC.MEMBERS
    for each row
declare
    v_audit_user varchar(1000) := coalesce(sys_context('APEX$SESSION','APP_USER'),user);
    v_new_level MEMBERS.THE_LEVEL%type;
    v_new_power MEMBERS.POWER%type;
    v_new_power_destroyed MEMBERS.POWER_DESTROYED%type;
    v_new_player_ships_destroyed MEMBERS.PLAYER_SHIPS_DESTROYED%type;
begin
    IF NOT DELETING THEN
        :new.modified_by := v_audit_user;
        :new.modified_date := sysdate;
    END IF;
    IF INSERTING THEN
        INSERT INTO MEMBERS_AUDIT
        (ACTION, EVENT_TIMESTAMP, AUDIT_USER, MEMBER_ID, ALLIANCE_ID, NAME, DISCORD, THE_LEVEL, POWER, POWER_DESTROYED, PLAYER_SHIPS_DESTROYED, MODIFIED_BY, MODIFIED_DATE)
        VALUES
            ('INSERT', CURRENT_TIMESTAMP, v_audit_user, :NEW.ID, :NEW.ALLIANCE_ID, :NEW.NAME, :NEW.DISCORD, :NEW.THE_LEVEL, :NEW.POWER, :NEW.POWER_DESTROYED, :NEW.PLAYER_SHIPS_DESTROYED, :NEW.MODIFIED_BY, :NEW.MODIFIED_DATE);
    END IF;
    IF UPDATING THEN
        IF :OLD.THE_LEVEL = :NEW.THE_LEVEL THEN
            v_new_level := null;
        ELSE
            v_new_level := :NEW.THE_LEVEL;
        END IF;
        
        IF :OLD.POWER = :NEW.POWER THEN
            v_new_power := null;
        ELSE
            v_new_power := :NEW.POWER;
        END IF;
        
        IF :OLD.POWER_DESTROYED = :NEW.POWER_DESTROYED THEN
            v_new_power_destroyed := null;
        ELSE
            v_new_power_destroyed := :NEW.POWER_DESTROYED;
        END IF;
        
        IF :OLD.PLAYER_SHIPS_DESTROYED = :NEW.PLAYER_SHIPS_DESTROYED THEN
            v_new_player_ships_destroyed := null;
        ELSE
            v_new_player_ships_destroyed := :NEW.PLAYER_SHIPS_DESTROYED;
        END IF;
        
        INSERT INTO MEMBERS_AUDIT
        (ACTION, EVENT_TIMESTAMP, AUDIT_USER, MEMBER_ID, ALLIANCE_ID, NAME, DISCORD, THE_LEVEL, POWER, POWER_DESTROYED, PLAYER_SHIPS_DESTROYED, MODIFIED_BY, MODIFIED_DATE)
        VALUES
            ('UPDATE', CURRENT_TIMESTAMP, v_audit_user, :NEW.ID, :NEW.ALLIANCE_ID, :NEW.NAME, :NEW.DISCORD, v_new_level, v_new_power, v_new_power_destroyed, v_new_player_ships_destroyed, :NEW.MODIFIED_BY, :NEW.MODIFIED_DATE);
    END IF;
    IF DELETING THEN
        INSERT INTO MEMBERS_AUDIT
        (ACTION, EVENT_TIMESTAMP, AUDIT_USER, MEMBER_ID, ALLIANCE_ID, NAME, DISCORD, THE_LEVEL, POWER, POWER_DESTROYED, PLAYER_SHIPS_DESTROYED, MODIFIED_BY, MODIFIED_DATE)
        VALUES
            ('DELETE', CURRENT_TIMESTAMP, v_audit_user, :OLD.ID, :OLD.ALLIANCE_ID, :OLD.NAME, :OLD.DISCORD, :OLD.THE_LEVEL, :OLD.POWER, :OLD.POWER_DESTROYED, :OLD.PLAYER_SHIPS_DESTROYED, :OLD.MODIFIED_BY, :OLD.MODIFIED_DATE);
    END IF;
end;
/

