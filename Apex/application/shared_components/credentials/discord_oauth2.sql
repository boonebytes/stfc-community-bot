prompt --application/shared_components/credentials/discord_oauth2
begin
--   Manifest
--     CREDENTIAL: Discord OAuth2
--   Manifest End
wwv_flow_api.component_begin (
 p_version_yyyy_mm_dd=>'2021.04.15'
,p_release=>'21.1.0'
,p_default_workspace_id=>18900386187764698
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_api.create_credential(
 p_id=>wwv_flow_api.id(78200400785586706)
,p_name=>'Discord OAuth2'
,p_static_id=>'Discord_OAuth2'
,p_authentication_type=>'OAUTH2_CLIENT_CREDENTIALS'
,p_scope=>'guilds,identify'
,p_prompt_on_install=>true
);
wwv_flow_api.component_end;
end;
/
