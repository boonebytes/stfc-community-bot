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
        v_client_id         VARCHAR2    := GET_OAUTH_CONFIG(OAUTH_KEY_CLIENT_ID);
        v_scope             VARCHAR2    := GET_OAUTH_CONFIG(OAUTH_KEY_SCOPE);
    BEGIN
        apex_util.REDIRECT_URL(OAUTH_URL_BASE || OAUTH_URL_AUTHORIZE
                                   || '?client_id=' || v_client_id
                                   || '&prompt=none'
                                   || '&response_type=code'
                                   || '&scope=' || v_scope
                                   || '&redirect_uri=' || apex_util.url_encode(apex_page.get_url));
    END START_AUTH;

    PROCEDURE END_AUTH(p_code varchar2) AS
        v_client_id         VARCHAR2    := GET_OAUTH_CONFIG(OAUTH_KEY_CLIENT_ID);
        v_client_secret     VARCHAR2    := GET_OAUTH_CONFIG(OAUTH_KEY_CLIENT_SECRET);
        v_cred_static       VARCHAR2    := GET_OAUTH_CONFIG(CRED_STATIC);
        v_token             VARCHAR2    := null;
        v_clob              CLOB;
        v_identity          apex_json.t_values;
        v_guilds            apex_json.t_values;
    BEGIN
        /*
            client_id - your application's client id
            client_secret - your application's client secret
            grant_type - must be set to authorization_code
            code - the code from the querystring
            redirect_uri - the redirect_uri associated with this authorization, usually from your authorization URL
         */
        -- Post to oauth2/token
        -- Send as 'Content-Type': 'application/x-www-form-urlencoded'
        /*
            {
              "access_token": "6qrZcUqja7812RVdnEKjpzOL4CvHBFG",
              "token_type": "Bearer",
              "expires_in": 604800,
              "refresh_token": "D43f5y0ahjqew82jZ4NViEr2YafMKhue",
              "scope": "identify"
            }
         */
        apex_web_service.OAUTH_AUTHENTICATE_CREDENTIAL(
                p_token_url  => OAUTH_URL_BASE || OAUTH_URL_TOKEN,
                p_credential_static_id  => v_cred_static);

        -- v_token := apex_web_service.OAUTH_GET_LAST_TOKEN();

        v_clob := apex_web_service.make_rest_request(
                p_url => OAUTH_URL_BASE || OAUTH_URL_IDENTIFY,
                p_http_method => 'GET');
        apex_json.parse(
                p_values => v_identity,
                p_source => v_clob);

        -- id snowflake
        -- username varchar2
        -- discriminator varchar2



        v_clob := apex_web_service.make_rest_request(
                p_url => OAUTH_URL_BASE || OAUTH_URL_GUILDS,
                p_http_method => 'GET');
        apex_json.parse(
                p_values => v_guilds,
                p_source => v_clob);
        -- id snowflake
        -- name varchar2

    END END_AUTH;

    FUNCTION GET_OAUTH_CONFIG(p_name varchar2) RETURN varchar2 AS
        v_value     VARCHAR2    := null;
    BEGIN
        SELECT value INTO v_value
        FROM OAUTH_CONFIG
        WHERE name = p_name;

        return v_value;
    EXCEPTION
        WHEN OTHERS THEN
            return null;
    END GET_OAUTH_CONFIG;

    PROCEDURE POST_AUTH AS
        v_UserAlliance USERS.ALLIANCE_ID%TYPE := NULL;
    BEGIN
        SELECT alliance_id INTO v_UserAlliance
        FROM USERS
        WHERE username = UPPER(v('APP_USER'));
        apex_util.set_session_state('USER_ALLIANCE', v_UserAlliance);

        --apex_util.set_session_state('USER_ALLIANCE', 'APP_USER');
    END POST_AUTH;

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

