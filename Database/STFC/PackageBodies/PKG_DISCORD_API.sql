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

create or replace PACKAGE BODY           STFC.PKG_DISCORD_API AS

    FUNCTION GET_OAUTH_CONFIG(p_name varchar2) RETURN varchar2 AS
        v_value     VARCHAR2(1000)    := null;
    BEGIN
        SELECT value INTO v_value
        FROM OAUTH_CONFIG
        WHERE name = p_name;

        return v_value;
    EXCEPTION
        WHEN OTHERS THEN
            return null;
    END GET_OAUTH_CONFIG;

    FUNCTION GET_AUTHORIZE_URL RETURN VARCHAR2 AS
        v_client_id         VARCHAR2(1000)    := GET_OAUTH_CONFIG(OAUTH_KEY_CLIENT_ID);
        v_scope             VARCHAR2(1000)    := GET_OAUTH_CONFIG(OAUTH_KEY_SCOPE);
        v_redirect_url      VARCHAR2(1000)    := GET_OAUTH_CONFIG(OAUTH_KEY_REDIRECT_URL);
    BEGIN
        RETURN OAUTH_URL_BASE || OAUTH_URL_AUTHORIZE
            || '?client_id=' || v_client_id
            || '&prompt=none'
            || '&response_type=code'
            || '&scope=' || v_scope
            || '&redirect_uri=' || apex_util.url_encode(v_redirect_url);
    END GET_AUTHORIZE_URL;



    PROCEDURE GET_TOKEN(p_code varchar2, p_token OUT varchar2, p_token_type OUT varchar2, p_token_expires OUT date) AS
        v_client_id         VARCHAR2(1000)    := GET_OAUTH_CONFIG(OAUTH_KEY_CLIENT_ID);
        v_client_secret     VARCHAR2(1000)    := GET_OAUTH_CONFIG(OAUTH_KEY_CLIENT_SECRET);
        v_auth_response     apex_json.t_values;
        v_token             VARCHAR2(1000)    := null;
        v_token_type        VARCHAR2(1000)    := null;
        v_token_expires     DATE              := null;
        v_redirect_url      VARCHAR2(1000)    := GET_OAUTH_CONFIG(OAUTH_KEY_REDIRECT_URL);
        v_clob              CLOB;
    BEGIN
        apex_debug.info('Requesting: ' || OAUTH_URL_BASE || OAUTH_URL_TOKEN);

        APEX_WEB_SERVICE.SET_REQUEST_HEADERS(
                'User-Agent', HEADER_USER_AGENT,
                'Content-Type', CONTENT_TYPE_URL_ENCODED
            );
        -- APEX_WEB_SERVICE.g_request_headers(1).name := 'User-Agent';
        -- APEX_WEB_SERVICE.g_request_headers(1).value := 'Oracle-Apex/STFC';
        -- APEX_WEB_SERVICE.g_request_headers(2).name := 'Content-Type';
        -- APEX_WEB_SERVICE.g_request_headers(2).value := 'application/x-www-form-urlencoded';
        
        v_clob := apex_web_service.make_rest_request(
            p_url => OAUTH_URL_BASE || OAUTH_URL_TOKEN,
            p_http_method => 'POST',
            p_body => 'client_id=' || apex_util.url_encode(v_client_id)
                || '&client_secret=' || apex_util.url_encode(v_client_secret)
                || '&grant_type=authorization_code'
                || '&code=' || apex_util.url_encode(p_code)
                || '&redirect_uri=' || apex_util.url_encode(v_redirect_url));
        apex_json.parse(
                p_values => v_auth_response,
                p_source => v_clob);
        v_token := apex_json.get_varchar2(p_path=>'access_token', p_values=> v_auth_response);
        v_token_type := apex_json.get_varchar2(p_path=>'token_type', p_values=> v_auth_response);
        v_token_expires := SYSDATE + (
                    apex_json.get_number(p_path=>'expires_in', p_values=> v_auth_response)
                        / (24 * 60 * 60)
                );
        DBMS_SESSION.SLEEP(0.5);
        
        p_token := v_token;
        p_token_type := v_token_type;
        p_token_expires := v_token_expires;
        RETURN;
    END GET_TOKEN;
    
    FUNCTION GET_DISCORD_DATA(p_token varchar2, p_token_type varchar2) RETURN clob AS
        v_clob              CLOB;
        v_identity          apex_json.t_values;
        v_discord_id        VARCHAR2(100);
        v_discord_username  VARCHAR2(1000);
        v_guilds            apex_json.t_values;
        v_guild_count       NUMBER;
        v_discord_data      CLOB;
    BEGIN
        APEX_WEB_SERVICE.SET_REQUEST_HEADERS(
                'User-Agent', HEADER_USER_AGENT,
                'Authorization', p_token_type || ' ' || p_token
            );

        v_clob := apex_web_service.make_rest_request(
                p_url => OAUTH_URL_BASE || OAUTH_URL_IDENTIFY,
                p_http_method => 'GET');
        apex_json.parse(
                p_values => v_identity,
                p_source => v_clob);

        v_discord_id := apex_json.get_varchar2(p_path=>'id', p_values=> v_identity);
        v_discord_username := apex_json.get_varchar2(p_path=>'username', p_values=> v_identity);
        
        v_clob := apex_web_service.make_rest_request(
                p_url => OAUTH_URL_BASE || OAUTH_URL_GUILDS,
                p_http_method => 'GET');
        apex_json.parse(
                p_values => v_guilds,
                p_source => v_clob);
        apex_json.initialize_clob_output( p_preserve => true );
        apex_json.open_object();
        apex_json.open_object('discord');
        apex_json.write(p_name =>'id', p_value => v_discord_id);
        apex_json.write(p_name =>'username', p_value => v_discord_username);
        apex_json.open_array('guilds');
        v_guild_count := apex_json.get_count(p_path => '.', p_values => v_guilds);
        for i in 1..v_guild_count loop
            apex_json.open_object();
            apex_json.write(p_name =>'id', p_value => apex_json.get_varchar2(p_path=>'[' || i || '].id', p_values=> v_guilds));
            apex_json.write(p_name =>'name', p_value => apex_json.get_varchar2(p_path=>'[' || i || '].name', p_values=> v_guilds));
            apex_json.write(p_name =>'permissions', p_value => apex_json.get_varchar2(p_path=>'[' || i || '].permissions', p_values=> v_guilds));
            apex_json.close_object();
        end loop;
        apex_json.close_array();
        apex_json.close_object();
        apex_json.close_object();
        v_discord_data := apex_json.get_clob_output();
        apex_json.free_output();
        
        return v_discord_data;
    END GET_DISCORD_DATA;
        
END PKG_DISCORD_API;
/

