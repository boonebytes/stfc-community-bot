prompt --application/pages/page_groups
begin
--   Manifest
--     PAGE GROUPS: 109
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_page.create_page_group(
 p_id=>wwv_flow_imp.id(54557183068995950)
,p_group_name=>'Administration'
);
wwv_flow_imp_page.create_page_group(
 p_id=>wwv_flow_imp.id(53300339989123476)
,p_group_name=>'Code Tables'
);
wwv_flow_imp.component_end;
end;
/