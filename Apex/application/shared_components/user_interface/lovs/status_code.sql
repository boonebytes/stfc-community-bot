prompt --application/shared_components/user_interface/lovs/status_code
begin
--   Manifest
--     STATUS CODE
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
 p_id=>wwv_flow_api.id(33005320941237571)
,p_lov_name=>'STATUS CODE'
,p_lov_query=>'.'||wwv_flow_api.id(33005320941237571)||'.'
,p_location=>'STATIC'
);
wwv_flow_api.create_static_lov_data(
 p_id=>wwv_flow_api.id(33005616752237615)
,p_lov_disp_sequence=>1
,p_lov_disp_value=>'Active'
,p_lov_return_value=>'A'
);
wwv_flow_api.create_static_lov_data(
 p_id=>wwv_flow_api.id(33006004120237620)
,p_lov_disp_sequence=>2
,p_lov_disp_value=>'Inactive'
,p_lov_return_value=>'I'
);
wwv_flow_api.component_end;
end;
/
