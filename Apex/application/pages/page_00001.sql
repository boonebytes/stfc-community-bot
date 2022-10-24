prompt --application/pages/page_00001
begin
--   Manifest
--     PAGE: 00001
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_page.create_page(
 p_id=>1
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Home'
,p_alias=>'HOME'
,p_step_title=>'STFC Home'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_page_component_map=>'13'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20221010193340'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(33200340846258503)
,p_plug_name=>'Menu'
,p_region_template_options=>'#DEFAULT#'
,p_component_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54434419117995718)
,p_plug_display_sequence=>10
,p_include_in_reg_disp_sel_yn=>'Y'
,p_list_id=>wwv_flow_imp.id(54405142762995641)
,p_plug_source_type=>'NATIVE_LIST'
,p_list_template_id=>wwv_flow_imp.id(54522915576995791)
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_plug_footer=>wwv_flow_string.join(wwv_flow_t_varchar2(
'<script type="text/javascript" src="#APP_FILES#jstz.js"></script>',
'<script type="text/javascript">',
'$(function () {',
'	var tz = jstz.determine(); // Determines the time zone of the browser client',
'	//$s("P101_TZ",  tz.name());',
'	apex.server.process(',
'		''SetSessionTimezone'',                             // Process or AJAX Callback name',
'		{x01: tz.name()},  // Parameter "x01"',
'		{',
'			success: function (pData) {             // Success Javascript',
'				return;',
'			},',
'			dataType: "text"                        // Response type (here: plain text)',
'		}',
'	);',
'});',
'</script>'))
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(54564859759996003)
,p_plug_name=>'STFC'
,p_icon_css_classes=>'app-icon'
,p_region_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54452459016995731)
,p_plug_display_sequence=>10
,p_plug_display_point=>'REGION_POSITION_01'
,p_plug_query_num_rows=>15
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_attribute_01=>'N'
,p_attribute_02=>'HTML'
,p_attribute_03=>'Y'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(75001447297727511)
,p_process_sequence=>10
,p_process_point=>'ON_DEMAND'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'SetSessionTimezone'
,p_process_sql_clob=>wwv_flow_string.join(wwv_flow_t_varchar2(
'begin',
'	APEX_UTIL.SET_SESSION_TIME_ZONE(apex_application.g_x01);',
'	htp.p(''Done'');',
'end;'))
,p_process_clob_language=>'PLSQL'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_is_stateful_y_n=>'Y'
);
wwv_flow_imp.component_end;
end;
/
