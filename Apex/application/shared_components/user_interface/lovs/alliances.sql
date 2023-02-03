prompt --application/shared_components/user_interface/lovs/alliances
begin
--   Manifest
--     ALLIANCES
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
 p_id=>wwv_flow_imp.id(54700401318017067)
,p_lov_name=>'ALLIANCES'
,p_lov_query=>wwv_flow_string.join(wwv_flow_t_varchar2(
'SELECT ID, ACRONYM || '' - '' || NAME "DISPLAY"',
'FROM alliances',
'WHERE status = ''A''',
'ORDER BY ACRONYM'))
,p_source_type=>'SQL'
,p_location=>'LOCAL'
,p_use_local_sync_table=>false
,p_return_column_name=>'ID'
,p_display_column_name=>'DISPLAY'
,p_group_sort_direction=>'ASC'
,p_default_sort_direction=>'ASC'
);
wwv_flow_imp.component_end;
end;
/
