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

create or replace package body           STFC.PKG_OFFICER as
    PROCEDURE IMPORT_OFFICERS_FROM_TOOL(p_member_id MEMBERS.ID%type, p_data in out nocopy clob)
    IS
        TYPE t_officer_table IS TABLE OF MEMBER_OFFICERS%rowtype; -- INDEX BY BINARY_INTEGER;
        v_officer_row    MEMBER_OFFICERS%rowtype;
        v_officer_table  t_officer_table            := t_officer_table();
        v_line           varchar2(4000);
        v_data_offset    number                     := 1;
        v_data_amount    number                     := 32767;
        v_data_length    number                     := dbms_lob.getlength(p_data);
    BEGIN
        if dbms_lob.ISOPEN(p_data) != 1 then
            dbms_lob.open(p_data, 0);
        end if;

        
        -- Find the first line break
        v_data_amount := dbms_lob.instr(p_data, chr(10), v_data_offset);

        while (v_data_offset < v_data_length)
        loop
            -- Read up to the line break
            dbms_lob.read(p_data, v_data_amount, v_data_offset, v_line);

            v_line := REPLACE(v_line, chr(13) || chr(10), chr(10));
            v_line := TRIM(chr(10) FROM v_line);
            v_line := TRIM(chr(13) FROM v_line);
            
            v_officer_row := IMPORT_OFFICER_FROM_TOOL(p_member_id, v_line);
            if v_officer_row.OFFICER_ID <> 0 and v_officer_row.THE_LEVEL > 0 then
                -- v_officer_table.insert(v_officer_row);
                v_officer_table.extend();
                v_officer_table(v_officer_table.count) := v_officer_row;
            end if;

            -- Prep for reading the next line
            v_data_offset := v_data_offset + v_data_amount;
            v_data_amount := dbms_lob.instr(p_data, chr(10), v_data_offset + 1);
            if v_data_amount = 0 then
                v_data_amount := v_data_length - v_data_offset + 1;
            else
                v_data_amount := v_data_amount - v_data_offset;
            end if;
        end loop;

        if dbms_lob.ISOPEN(p_data) = 1 then
            dbms_lob.close(p_data);
        end if;

        -- If we've gotten this far without an exception,
        -- start applying the changes to the tables.
        delete from MEMBER_OFFICERS where member_id = p_member_id;

        for i in v_officer_table.first..v_officer_table.last
        loop
            v_officer_row := v_officer_table(i);
            INSERT INTO MEMBER_OFFICERS
                (member_id, officer_id, officer_rank_id, trait1_level, trait2_level, trait3_level, the_level)
            VALUES
                (p_member_id, v_officer_row.OFFICER_ID, v_officer_row.OFFICER_RANK_ID, v_officer_row.TRAIT1_LEVEL, v_officer_row.TRAIT2_LEVEL, v_officer_row.TRAIT3_LEVEL, v_officer_row.THE_LEVEL);
        end loop;
        commit;
    END IMPORT_OFFICERS_FROM_TOOL;

    FUNCTION IMPORT_OFFICER_FROM_TOOL (p_member_id MEMBERS.ID%type, p_line varchar2)
        RETURN MEMBER_OFFICERS%rowtype
    IS
        -- v_offset    number := 1;
        -- v_end       number := 1;
        v_rarity    VARCHAR2(5);
        v_name      VARCHAR2(500);
        v_level     VARCHAR2(5);
        v_rank      VARCHAR2(5);
        v_trait1    VARCHAR2(5);
        v_trait2    VARCHAR2(5);
        v_trait3    VARCHAR2(5);

        -- v_officer_id        MEMBER_OFFICERS.OFFICER_ID%type;
        -- v_level_num         MEMBER_OFFICERS.THE_LEVEL%type;
        -- v_rank_id           MEMBER_OFFICERS.OFFICER_RANK_ID%type;
        -- v_trait1            MEMBER_OFFICERS.TRAIT1_LEVEL%type;
        -- v_trait2            MEMBER_OFFICERS.TRAIT2_LEVEL%type;
        -- v_trait3            MEMBER_OFFICERS.TRAIT3_LEVEL%type;
        
        v_result MEMBER_OFFICERS%rowtype;
    BEGIN
        v_rarity := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 1, null, 1);
        v_name   := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 2, null, 1);
        v_level  := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 3, null, 1);
        v_rank   := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 4, null, 1);
        v_trait1 := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 5, null, 1);
        v_trait2 := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 6, null, 1);
        v_trait3 := REGEXP_SUBSTR(p_line, '(.*?)(' || CHR(9) || '|$)', 1, 7, null, 1);

        v_result.MEMBER_ID          := p_member_id;
        v_result.OFFICER_ID         := FIND_OFFICER_ID(v_name, v_rarity);
        v_result.THE_LEVEL          := TO_NUMBER(v_level);
        v_result.OFFICER_RANK_ID    := TO_NUMBER(v_rank);
        v_result.TRAIT1_LEVEL       := TO_NUMBER(v_trait1);
        v_result.TRAIT2_LEVEL       := TO_NUMBER(v_trait2);
        v_result.TRAIT3_LEVEL       := TO_NUMBER(v_trait3);
        return v_result;
    END IMPORT_OFFICER_FROM_TOOL;

    FUNCTION FIND_OFFICER_ID(p_officer_name CT_OFFICERS.NAME%type, p_rarity varchar2)
        RETURN CT_OFFICERS.ID%type
    IS
        v_return CT_OFFICERS.ID%type;
    BEGIN
        SELECT o.id INTO v_return
        FROM CT_OFFICERS o
        INNER JOIN CT_OFFICER_RARITY r ON (o.officer_rarity_id = r.id)
        WHERE UPPER(o.name) = UPPER(p_officer_name)
        AND UPPER(SUBSTR(r.name, 1, 1)) = UPPER(p_rarity);

        return v_return;
    END FIND_OFFICER_ID;
end PKG_OFFICER;
/

