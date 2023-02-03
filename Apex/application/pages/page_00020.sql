prompt --application/pages/page_00020
begin
--   Manifest
--     PAGE: 00020
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
 p_id=>20
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Player - Upload Officers'
,p_alias=>'PLAYER-UPLOAD-OFFICERS'
,p_page_mode=>'MODAL'
,p_step_title=>'Player - Upload Officers'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_page_component_map=>'16'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220619221915'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(56625808339916616)
,p_plug_name=>'Upload Officer Data'
,p_region_template_options=>'#DEFAULT#:t-Region--scrollBody'
,p_plug_template=>wwv_flow_imp.id(54462243687995736)
,p_plug_display_sequence=>10
,p_include_in_reg_disp_sel_yn=>'Y'
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_attribute_01=>'N'
,p_attribute_02=>'HTML'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(56626257629916620)
,p_button_sequence=>30
,p_button_plug_id=>wwv_flow_imp.id(56625808339916616)
,p_button_name=>'Upload'
,p_button_action=>'SUBMIT'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_image_alt=>'Upload'
,p_grid_new_row=>'Y'
,p_security_scheme=>wwv_flow_imp.id(54555485978995935)
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(56626093472916618)
,p_name=>'P20_MEMBER_ID'
,p_item_sequence=>10
,p_item_plug_id=>wwv_flow_imp.id(56625808339916616)
,p_display_as=>'NATIVE_HIDDEN'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(56626171570916619)
,p_name=>'P20_OFFICER_DATA'
,p_is_required=>true
,p_item_sequence=>20
,p_item_plug_id=>wwv_flow_imp.id(56625808339916616)
,p_prompt=>'Officer Data'
,p_display_as=>'NATIVE_TEXTAREA'
,p_cSize=>70
,p_cHeight=>20
,p_field_template=>wwv_flow_imp.id(54524887331995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'N'
,p_attribute_02=>'N'
,p_attribute_03=>'N'
,p_attribute_04=>'BOTH'
);
wwv_flow_imp_page.create_page_da_event(
 p_id=>wwv_flow_imp.id(56626773708916625)
,p_name=>'Dialog Opened'
,p_event_sequence=>10
,p_bind_type=>'bind'
,p_bind_event_type=>'ready'
,p_display_when_type=>'NEVER'
);
wwv_flow_imp_page.create_page_da_action(
 p_id=>wwv_flow_imp.id(56626810777916626)
,p_event_id=>wwv_flow_imp.id(56626773708916625)
,p_event_result=>'TRUE'
,p_action_sequence=>10
,p_execute_on_page_init=>'N'
,p_action=>'NATIVE_SET_VALUE'
,p_affected_elements_type=>'ITEM'
,p_affected_elements=>'P20_MEMBER_ID'
,p_attribute_01=>'DIALOG_RETURN_ITEM'
,p_attribute_09=>'N'
,p_attribute_10=>'P20_MEMBER_ID'
,p_wait_for_result=>'Y'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(56626579164916623)
,p_process_sequence=>10
,p_process_point=>'AFTER_SUBMIT'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'Accept Uload'
,p_process_sql_clob=>wwv_flow_string.join(wwv_flow_t_varchar2(
'BEGIN  STFC.PKG_OFFICER.IMPORT_OFFICERS_FROM_TOOL(',
'    P_MEMBER_ID => :P20_MEMBER_ID,',
'    P_DATA => :P20_OFFICER_DATA',
'  );',
'END;'))
,p_process_clob_language=>'PLSQL'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_success_message=>'Data received successfully'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(56625979826916617)
,p_process_sequence=>10
,p_process_point=>'BEFORE_HEADER'
,p_region_id=>wwv_flow_imp.id(56625808339916616)
,p_process_type=>'NATIVE_FORM_INIT'
,p_process_name=>'Initialize form Player - Upload Officers'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_when_type=>'NEVER'
);
wwv_flow_imp.component_end;
end;
/
