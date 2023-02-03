prompt --application/shared_components/security/authentications/custom_discord_auth
begin
--   Manifest
--     AUTHENTICATION: Custom Discord Auth
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_authentication(
 p_id=>wwv_flow_imp.id(2626811672771794)
,p_name=>'Custom Discord Auth'
,p_scheme_type=>'NATIVE_CUSTOM'
,p_attribute_05=>'N'
,p_use_secure_cookie_yn=>'N'
,p_ras_mode=>0
);
wwv_flow_imp.component_end;
end;
/
