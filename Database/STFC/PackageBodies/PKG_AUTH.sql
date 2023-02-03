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

create or replace PACKAGE BODY           STFC.PKG_AUTH AS
    
    PROCEDURE START_AUTH AS
    BEGIN
        apex_util.REDIRECT_URL(PKG_DISCORD_API.GET_AUTHORIZE_URL());
    END START_AUTH;

    PROCEDURE END_AUTH(p_code varchar2) AS
        v_token             VARCHAR2(1000)    := null;
        v_token_type        VARCHAR2(1000)    := null;
        v_token_expires     DATE              := null;
        v_discord_id        VARCHAR2(100);
        v_discord_username  VARCHAR2(1000);
        v_discord_data_raw  CLOB;
        v_discord_data      apex_json.t_values;
        v_user              USERS%ROWTYPE;
        v_new_session_id    NUMBER;
    BEGIN
        -- APEX_DEBUG.ENABLE(9);
        
        pkg_discord_api.get_token(p_code, v_token, v_token_type, v_token_expires);
        v_discord_data_raw := pkg_discord_api.get_discord_data(v_token, v_token_type);
        apex_json.parse(
                p_values => v_discord_data,
                p_source => v_discord_data_raw);

        v_discord_id := apex_json.get_varchar2(p_path=>'discord.id', p_values => v_discord_data);
        v_discord_username := apex_json.get_varchar2(p_path=>'discord.username', p_values => v_discord_data);
        
        apex_debug.info('Checking for Discord ID ' || v_discord_id);
        BEGIN
            SELECT *
            INTO v_user
            FROM USERS
            WHERE discord_id = v_discord_id
            AND USER_STATUS = CT_USER_STATUS_ACTIVE;
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                apex_debug.ERROR('Unable to authenticate user based on Discord info: ' || cast(v_discord_data_raw as varchar2));
                apex_util.set_session_state('LOGIN_MESSAGE','Your Discord account has not been grantde access. Please contact the developer.');
                apex_util.redirect_url(apex_page.get_url(p_page => PAGE_LOGIN_ID));
                return;
        END;

        UPDATE USERS
        SET DISCORD_DATA = v_discord_data_raw
        WHERE id = v_user.id;
        
        apex_authentication.post_login(
            upper(v_user.username),
            null);
        apex_util.clear_page_cache();
        
        apex_util.set_session_state('USER_DISCORD_ID', v_discord_id);
        apex_util.set_session_state('USER_DISCORD_NAME', v_discord_username);
        apex_util.set_session_state('USER_ALLIANCE', v_user.alliance_id);
        apex_util.set_session_state('LOGIN_MESSAGE','');
        
        apex_util.redirect_url(
            apex_page.get_url(
                p_page => 1));
                -- p_session => v_new_session_id));
        return;
    EXCEPTION
        WHEN apex_application.e_stop_apex_engine THEN
            RAISE;
        WHEN OTHERS THEN
            apex_debug.ERROR('Unhandled exception in End Auth: ' || SQLCODE || ' - ' || SQLERRM);
            apex_util.set_session_state('LOGIN_MESSAGE','There was a problem logging you in. Please try again. If the problem persists, please contact the developer.');
            apex_util.redirect_url(apex_page.get_url(p_page => PAGE_LOGIN_ID));
            return;
    END END_AUTH;
        
    FUNCTION AUTH_USER_ALLIANCE(p_AllianceId NUMBER) RETURN BOOLEAN
    AS
        v_UserAllianceId NUMBER := 0;
    BEGIN
        IF apex_acl.has_user_role(p_role_static_id => 'ADMINISTRATOR') THEN
            RETURN TRUE;
        END IF;

        IF NOT apex_acl.has_user_role(p_role_static_id => 'CONTRIBUTOR') THEN
            RETURN FALSE;
        END IF;

        SELECT ALLIANCE_ID INTO v_UserAllianceId
        FROM USERS
        WHERE UPPER(USERNAME) = wwv_flow.g_user;

        IF v_UserAllianceId IS NULL OR v_UserAllianceId = p_AllianceId THEN
            RETURN TRUE;
        ELSE
            RETURN FALSE;
        END IF;
    END AUTH_USER_ALLIANCE;

END PKG_AUTH;
/

