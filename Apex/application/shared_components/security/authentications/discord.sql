prompt --application/shared_components/security/authentications/discord
begin
--   Manifest
--     AUTHENTICATION: Discord
--   Manifest End
wwv_flow_api.component_begin (
 p_version_yyyy_mm_dd=>'2021.04.15'
,p_release=>'21.1.0'
,p_default_workspace_id=>18900386187764698
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_api.create_authentication(
 p_id=>wwv_flow_api.id(78200610038588567)
,p_name=>'Discord'
,p_scheme_type=>'NATIVE_SOCIAL'
,p_attribute_01=>wwv_flow_api.id(78200400785586706)
,p_attribute_02=>'OAUTH2'
,p_attribute_04=>'https://discord.com/api/oauth2/authorize'
,p_attribute_05=>'https://discord.com/api/oauth2/token'
,p_attribute_06=>'https://discord.com/api/users/@me'
,p_attribute_07=>'guilds,identify'
,p_attribute_09=>'username'
,p_attribute_10=>'id'
,p_attribute_11=>'Y'
,p_attribute_12=>'BASIC_AND_CLID'
,p_attribute_13=>'Y'
,p_attribute_14=>'G_USERID'
,p_invalid_session_type=>'LOGIN'
,p_post_auth_process=>'PKG_AUTH.POST_AUTH'
,p_use_secure_cookie_yn=>'N'
,p_ras_mode=>0
);
wwv_flow_api.component_end;
end;
/
