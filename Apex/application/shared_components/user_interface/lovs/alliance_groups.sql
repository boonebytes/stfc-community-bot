prompt --application/shared_components/user_interface/lovs/alliance_groups
begin
--   Manifest
--     ALLIANCE GROUPS
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_list_of_values(
 p_id=>wwv_flow_imp.id(54824985257083259)
,p_lov_name=>'ALLIANCE GROUPS'
,p_source_type=>'TABLE'
,p_location=>'LOCAL'
,p_query_table=>'ALLIANCE_GROUPS'
,p_return_column_name=>'ID'
,p_display_column_name=>'NAME'
,p_default_sort_column_name=>'NAME'
,p_default_sort_direction=>'ASC'
);
wwv_flow_imp.component_end;
end;
/