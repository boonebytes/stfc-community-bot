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

create or replace PACKAGE      STFC.PKG_DISCORD_API AS 

    HEADER_USER_AGENT        CONSTANT VARCHAR2(100) := 'Oracle-Apex/STFC';
    
    CONTENT_TYPE_URL_ENCODED CONSTANT VARCHAR2(100) := 'application/x-www-form-urlencoded';
    
    OAUTH_URL_BASE           CONSTANT VARCHAR2(100) := 'https://discord.com/api/v10';
    OAUTH_URL_AUTHORIZE      CONSTANT VARCHAR2(100) := '/oauth2/authorize';
    OAUTH_URL_TOKEN          CONSTANT VARCHAR2(100) := '/oauth2/token';
    OAUTH_URL_IDENTIFY       CONSTANT VARCHAR2(100) := '/users/@me';
    OAUTH_URL_GUILDS         CONSTANT VARCHAR2(100) := '/users/@me/guilds';

    OAUTH_KEY_CLIENT_ID      CONSTANT VARCHAR2(100) := 'CLIENT_ID';
    OAUTH_KEY_CLIENT_SECRET  CONSTANT VARCHAR2(100) := 'CLIENT_SECRET';
    OAUTH_KEY_SCOPE          CONSTANT VARCHAR2(100) := 'SCOPE';
    OAUTH_KEY_CRED_STATIC    CONSTANT VARCHAR2(100) := 'CRED_STATIC';
    OAUTH_KEY_REDIRECT_URL   CONSTANT VARCHAR2(100) := 'REDIRECT_URL';

    FUNCTION GET_AUTHORIZE_URL RETURN VARCHAR2;

    PROCEDURE GET_TOKEN(p_code varchar2, p_token OUT varchar2, p_token_type OUT varchar2, p_token_expires OUT date);

    FUNCTION GET_DISCORD_DATA(p_token varchar2, p_token_type varchar2) RETURN clob;

END PKG_DISCORD_API;
/
