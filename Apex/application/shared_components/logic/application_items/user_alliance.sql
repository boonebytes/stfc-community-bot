prompt --application/shared_components/logic/application_items/user_alliance
begin
--   Manifest
--     APPLICATION ITEM: USER_ALLIANCE
--   Manifest End
wwv_flow_api.component_begin (
 p_version_yyyy_mm_dd=>'2021.04.15'
,p_release=>'21.1.0'
,p_default_workspace_id=>18900386187764698
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_api.create_flow_item(
 p_id=>wwv_flow_api.id(37300390841446874)
,p_name=>'USER_ALLIANCE'
,p_protection_level=>'I'
);
wwv_flow_api.component_end;
end;
/
