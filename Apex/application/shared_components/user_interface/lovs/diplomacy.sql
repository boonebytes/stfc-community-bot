prompt --application/shared_components/user_interface/lovs/diplomacy
begin
--   Manifest
--     DIPLOMACY
--   Manifest End
wwv_flow_api.component_begin (
 p_version_yyyy_mm_dd=>'2021.04.15'
,p_release=>'21.1.0'
,p_default_workspace_id=>18900386187764698
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_api.create_list_of_values(
 p_id=>wwv_flow_api.id(85100240116319140)
,p_lov_name=>'DIPLOMACY'
,p_source_type=>'TABLE'
,p_location=>'LOCAL'
,p_use_local_sync_table=>false
,p_query_table=>'CT_DIPLOMATIC_RELATION'
,p_return_column_name=>'ID'
,p_display_column_name=>'NAME'
,p_group_sort_direction=>'ASC'
,p_default_sort_column_name=>'ID'
,p_default_sort_direction=>'ASC'
);
wwv_flow_api.component_end;
end;
/
