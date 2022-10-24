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

create or replace PACKAGE      STFC.PKG_AUTH AS 
    
    PAGE_LOGIN_ID           CONSTANT NUMBER        := 9998;
    CT_USER_STATUS_ACTIVE   CONSTANT NUMBER        := 1;
    CT_USER_STATUS_INACTIVE CONSTANT NUMBER        := 2;
    
    PROCEDURE START_AUTH;

    PROCEDURE END_AUTH(p_code varchar2);

    FUNCTION AUTH_USER_ALLIANCE(p_AllianceId NUMBER) RETURN BOOLEAN;

END PKG_AUTH;
/
