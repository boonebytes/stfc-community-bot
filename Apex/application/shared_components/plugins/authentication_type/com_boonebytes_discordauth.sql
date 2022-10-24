prompt --application/shared_components/plugins/authentication_type/com_boonebytes_discordauth
begin
--   Manifest
--     PLUGIN: COM.BOONEBYTES.DISCORDAUTH
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_plugin(
 p_id=>wwv_flow_imp.id(2200693170108998)
,p_plugin_type=>'AUTHENTICATION TYPE'
,p_name=>'COM.BOONEBYTES.DISCORDAUTH'
,p_display_name=>'Discord Authentication'
,p_category=>'COMPONENT'
,p_supported_ui_types=>'DESKTOP:JQM_SMARTPHONE'
,p_image_prefix => nvl(wwv_flow_application_install.get_static_plugin_file_prefix('AUTHENTICATION TYPE','COM.BOONEBYTES.DISCORDAUTH'),'')
,p_plsql_code=>wwv_flow_string.join(wwv_flow_t_varchar2(
'FUNCTION discord_session_sentry (',
'    p_authentication in apex_plugin.t_authentication,',
'    p_plugin         in apex_plugin.t_plugin,',
'    p_is_public_page in boolean )',
'    return apex_plugin.t_authentication_sentry_result',
'AS',
'BEGIN',
'    NULL;',
'END discord_session_sentry;',
'',
'FUNCTION discord_session_invalid (',
'    p_authentication in apex_plugin.t_authentication,',
'    p_plugin         in apex_plugin.t_plugin )',
'    return apex_plugin.t_authentication_inval_result',
'AS',
'BEGIN',
'    NULL;',
'END discord_session_invalid;',
'',
'FUNCTION discord_auth (',
'    p_authentication in apex_plugin.t_authentication,',
'    p_plugin         in apex_plugin.t_plugin,',
'    p_password       in varchar2 )',
'    return apex_plugin.t_authentication_auth_result',
'AS',
'BEGIN',
'    NULL;',
'END discord_auth;',
'',
'FUNCTION discord_logout (',
'    p_authentication in apex_plugin.t_authentication,',
'    p_plugin         in apex_plugin.t_plugin )',
'    return apex_plugin.t_authentication_logout_resul',
'AS',
'BEGIN',
'    NULL;',
'END discord_logout;'))
,p_api_version=>2
,p_session_sentry_function=>'discord_session_sentry'
,p_invalid_session_function=>'discord_session_invalid'
,p_authentication_function=>'discord_auth'
,p_post_logout_function=>'discord_logout'
,p_substitute_attributes=>true
,p_subscribe_plugin_settings=>true
,p_version_identifier=>'1.0'
);
wwv_flow_imp.component_end;
end;
/
