prompt --application/deployment/definition
begin
--   Manifest
--     INSTALL: 109
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_install(
 p_id=>wwv_flow_imp.id(30601517171476033)
);
wwv_flow_imp.component_end;
end;
/