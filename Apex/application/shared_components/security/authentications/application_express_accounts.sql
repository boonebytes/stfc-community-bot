prompt --application/shared_components/security/authentications/application_express_accounts
begin
--   Manifest
--     AUTHENTICATION: Application Express Accounts
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
 p_id=>wwv_flow_api.id(54404368615995633)
,p_name=>'Application Express Accounts'
,p_scheme_type=>'NATIVE_APEX_ACCOUNTS'
,p_invalid_session_type=>'LOGIN'
,p_post_auth_process=>'PKG_AUTH.POST_AUTH'
,p_use_secure_cookie_yn=>'N'
,p_ras_mode=>0
);
wwv_flow_api.component_end;
end;
/
