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
