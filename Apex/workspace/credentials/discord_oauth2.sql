prompt --workspace/credentials/discord_oauth2
begin
--   Manifest
--     CREDENTIAL: Discord OAuth2
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_imp_workspace.create_credential(
 p_id=>wwv_flow_imp.id(78200400785586706)
,p_name=>'Discord OAuth2'
,p_static_id=>'Discord_OAuth2'
,p_authentication_type=>'BASIC'
,p_scope=>'guilds,identify'
,p_valid_for_urls=>wwv_flow_string.join(wwv_flow_t_varchar2(
'https://discord.com/oauth2',
'https://discord.com/api',
'https://discord.com/api/oauth2/token'))
,p_prompt_on_install=>true
);
wwv_flow_imp.component_end;
end;
/
