prompt --application/shared_components/user_interface/lovs/p11_members_alliance_id
begin
--   Manifest
--     P11_MEMBERS_ALLIANCE_ID
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
 p_id=>wwv_flow_api.id(36802022205204047)
,p_lov_name=>'P11_MEMBERS_ALLIANCE_ID'
,p_lov_query=>wwv_flow_string.join(wwv_flow_t_varchar2(
'select ALLIANCES.ID as ID,',
'    ALLIANCES.ACRONYM as ACRONYM ',
' from ALLIANCES ALLIANCES',
'order by acronym'))
,p_source_type=>'SQL'
,p_location=>'LOCAL'
,p_return_column_name=>'ACRONYM'
,p_display_column_name=>'ID'
);
wwv_flow_api.component_end;
end;
/
