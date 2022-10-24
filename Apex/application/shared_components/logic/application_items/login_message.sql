prompt --application/shared_components/logic/application_items/login_message
begin
--   Manifest
--     APPLICATION ITEM: LOGIN_MESSAGE
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_flow_item(
 p_id=>wwv_flow_imp.id(2622725671370341)
,p_name=>'LOGIN_MESSAGE'
,p_protection_level=>'I'
);
wwv_flow_imp.component_end;
end;
/