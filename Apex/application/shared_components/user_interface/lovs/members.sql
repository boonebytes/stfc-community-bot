prompt --application/shared_components/user_interface/lovs/members
begin
--   Manifest
--     MEMBERS
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
 p_id=>wwv_flow_api.id(54200105134345747)
,p_lov_name=>'MEMBERS'
,p_lov_query=>wwv_flow_string.join(wwv_flow_t_varchar2(
'SELECT MEMBERS.ID, MEMBERS.NAME, ALLIANCES.NAME ALLIANCE, THE_LEVEL',
'FROM MEMBERS',
'LEFT OUTER JOIN ALLIANCES ON (MEMBERS.ALLIANCE_ID = ALLIANCES.ID)',
'ORDER BY NAME'))
,p_source_type=>'SQL'
,p_location=>'LOCAL'
,p_use_local_sync_table=>false
,p_query_table=>'MEMBERS'
,p_return_column_name=>'ID'
,p_display_column_name=>'NAME'
,p_group_sort_direction=>'ASC'
,p_default_sort_direction=>'ASC'
);
wwv_flow_api.create_list_of_values_cols(
 p_id=>wwv_flow_api.id(54200519852360580)
,p_query_column_name=>'ID'
,p_display_sequence=>10
,p_data_type=>'NUMBER'
,p_is_visible=>'N'
,p_is_searchable=>'N'
);
wwv_flow_api.create_list_of_values_cols(
 p_id=>wwv_flow_api.id(54200953348360581)
,p_query_column_name=>'NAME'
,p_heading=>'Name'
,p_display_sequence=>20
,p_data_type=>'VARCHAR2'
);
wwv_flow_api.create_list_of_values_cols(
 p_id=>wwv_flow_api.id(54201353979360581)
,p_query_column_name=>'ALLIANCE'
,p_heading=>'Alliance'
,p_display_sequence=>30
,p_data_type=>'VARCHAR2'
);
wwv_flow_api.create_list_of_values_cols(
 p_id=>wwv_flow_api.id(54201723645360581)
,p_query_column_name=>'THE_LEVEL'
,p_heading=>'Level'
,p_display_sequence=>40
,p_data_type=>'NUMBER'
);
wwv_flow_api.component_end;
end;
/
