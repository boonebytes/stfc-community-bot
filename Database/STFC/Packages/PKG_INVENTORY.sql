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

create or replace PACKAGE      STFC.PKG_INVENTORY AS 
    RES_RISO1               CONSTANT NUMBER := 51;
    RES_RISO2               CONSTANT NUMBER := 52;
    RES_RISO3               CONSTANT NUMBER := 53;
    RES_EMITTERS            CONSTANT NUMBER := 61;
    RES_DIODES              CONSTANT NUMBER := 62;
    RES_CORES               CONSTANT NUMBER := 63;
    RES_REACTORS            CONSTANT NUMBER := 64;
    RES_COLLISIONAL_PLASMA  CONSTANT NUMBER := 71;
    RES_MAGNETIC_PLASMA     CONSTANT NUMBER := 72;
    RES_SUPERCONDUCTORS     CONSTANT NUMBER := 73;
    RES_RESERVES            CONSTANT NUMBER := 74;
    
    PROCEDURE INSERT_ALLIANCE_INVENTORY(in_Alliance_ID number,
                                        in_Effective_Date date,
                                        in_iso1 number,
                                        in_iso2 number,
                                        in_iso3 number,
                                        in_cores number,
                                        in_diodes number,
                                        in_emitters number,
                                        in_reactors number,
                                        in_collisional_plasma number,
                                        in_magnetic_plasma number,
                                        in_superconductors number,
                                        in_reserves number);

END PKG_INVENTORY;
/

create or replace PACKAGE BODY           STFC.PKG_INVENTORY AS
    PROCEDURE INSERT_ALLIANCE_INVENTORY(in_Alliance_ID number,
                                        in_Effective_Date date,
                                        in_iso1 number,
                                        in_iso2 number,
                                        in_iso3 number,
                                        in_cores number,
                                        in_diodes number,
                                        in_emitters number,
                                        in_reactors number,
                                        in_collisional_plasma number,
                                        in_magnetic_plasma number,
                                        in_superconductors number,
                                        in_reserves number)
    AS
        v_Line_Id    NUMBER;
    BEGIN
        INSERT INTO ALLIANCE_INVENTORY
                (EFFECTIVE_DATE, ALLIANCE_ID)
            VALUES
                (in_Effective_Date, in_Alliance_ID)
            RETURNING ID INTO v_Line_Id;
        
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_RISO1, in_iso1);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_RISO2, in_iso2);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_RISO3, in_iso3);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_CORES, in_cores);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_DIODES, in_diodes);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_EMITTERS, in_emitters);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_REACTORS, in_reactors);

        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_COLLISIONAL_PLASMA, in_collisional_plasma);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_MAGNETIC_PLASMA, in_magnetic_plasma);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_SUPERCONDUCTORS, in_superconductors);
        INSERT INTO ALLIANCE_INVENTORY_LINE
                (ALLIANCE_INVENTORY_ID, RESOURCE_ID, AMOUNT)
            VALUES
                (v_Line_Id, RES_RESERVES, in_reserves);
    END;

END PKG_INVENTORY;
/

