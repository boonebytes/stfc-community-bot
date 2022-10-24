prompt --application/pages/page_10024
begin
--   Manifest
--     PAGE: 10024
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
 p_id=>10024
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Feedback'
,p_alias=>'FEEDBACK1'
,p_page_mode=>'MODAL'
,p_step_title=>'Feedback'
,p_autocomplete_on_off=>'OFF'
,p_group_id=>wwv_flow_imp.id(54557183068995950)
,p_javascript_code=>'var htmldb_delete_message=''"DELETE_CONFIRM_MSG"'';'
,p_inline_css=>wwv_flow_string.join(wwv_flow_t_varchar2(
'/* Color feedback rating icons */',
'.feedback-positive { color: #4caf50; }',
'.feedback-neutral { color: #eb9f41; }',
'.feedback-negative { color: #f44336; }'))
,p_required_role=>wwv_flow_imp.id(54555301792995935)
,p_required_patch=>wwv_flow_imp.id(54553386857995923)
,p_dialog_chained=>'N'
,p_protection_level=>'C'
,p_page_component_map=>'16'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220322205940'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(54600279238996129)
,p_plug_name=>'Form Items Region'
,p_region_template_options=>'#DEFAULT#:t-Form--stretchInputs'
,p_escape_on_http_output=>'Y'
,p_plug_template=>wwv_flow_imp.id(54431268247995715)
,p_plug_display_sequence=>10
,p_plug_query_num_rows=>15
,p_attribute_01=>'N'
,p_attribute_02=>'HTML'
,p_attribute_03=>'Y'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(54600284833996129)
,p_plug_name=>'Buttons Region'
,p_region_template_options=>'#DEFAULT#'
,p_escape_on_http_output=>'Y'
,p_plug_template=>wwv_flow_imp.id(54432201299995716)
,p_plug_display_sequence=>10
,p_plug_display_point=>'REGION_POSITION_03'
,p_plug_query_num_rows=>15
,p_attribute_01=>'N'
,p_attribute_02=>'HTML'
,p_attribute_03=>'Y'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(54600408228996129)
,p_button_sequence=>10
,p_button_plug_id=>wwv_flow_imp.id(54600284833996129)
,p_button_name=>'SAVE'
,p_button_action=>'SUBMIT'
,p_button_template_options=>'#DEFAULT#:t-Button--gapLeft'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_is_hot=>'Y'
,p_button_image_alt=>'Apply Changes'
,p_button_position=>'CREATE'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(54600502277996129)
,p_button_sequence=>20
,p_button_plug_id=>wwv_flow_imp.id(54600284833996129)
,p_button_name=>'CANCEL'
,p_button_action=>'DEFINED_BY_DA'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_image_alt=>'Cancel'
,p_button_position=>'EDIT'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(54600636410996129)
,p_button_sequence=>30
,p_button_plug_id=>wwv_flow_imp.id(54600284833996129)
,p_button_name=>'DELETE'
,p_button_action=>'REDIRECT_URL'
,p_button_template_options=>'#DEFAULT#:t-Button--danger:t-Button--simple'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_image_alt=>'Delete'
,p_button_position=>'EDIT'
,p_button_redirect_url=>'javascript:apex.confirm(htmldb_delete_message,''DELETE'');'
,p_button_condition=>'P10024_ID'
,p_button_condition_type=>'ITEM_IS_NOT_NULL'
,p_database_action=>'DELETE'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54611501140996144)
,p_name=>'P10024_ID'
,p_item_sequence=>10
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_display_as=>'NATIVE_HIDDEN'
,p_lov_display_extra=>'NO'
,p_protection_level=>'S'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54611906610996144)
,p_name=>'P10024_PAGE_ID'
,p_item_sequence=>20
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'Page'
,p_display_as=>'NATIVE_DISPLAY_ONLY'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'VALUE'
,p_attribute_04=>'Y'
,p_attribute_05=>'PLAIN'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54612328399996145)
,p_name=>'P10024_FILED'
,p_item_sequence=>30
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'Filed'
,p_display_as=>'NATIVE_DISPLAY_ONLY'
,p_begin_on_new_line=>'N'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'VALUE'
,p_attribute_04=>'Y'
,p_attribute_05=>'PLAIN'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54612699757996145)
,p_name=>'P10024_RATING_ICON'
,p_item_sequence=>40
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'Rating'
,p_display_as=>'NATIVE_DISPLAY_ONLY'
,p_begin_on_new_line=>'N'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_escape_on_http_output=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'VALUE'
,p_attribute_04=>'Y'
,p_attribute_05=>'PLAIN'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54613159476996147)
,p_name=>'P10024_FEEDBACK'
,p_item_sequence=>50
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'Feedback'
,p_display_as=>'NATIVE_DISPLAY_ONLY'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'VALUE'
,p_attribute_04=>'Y'
,p_attribute_05=>'PLAIN'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54613507715996147)
,p_name=>'P10024_RESPONSE'
,p_item_sequence=>60
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'Response'
,p_display_as=>'NATIVE_TEXTAREA'
,p_cSize=>64
,p_cMaxlength=>4000
,p_cHeight=>4
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'N'
,p_attribute_03=>'N'
,p_attribute_04=>'BOTH'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54613933729996148)
,p_name=>'P10024_FEEDBACK_STATUS'
,p_item_sequence=>70
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'Status'
,p_display_as=>'NATIVE_SELECT_LIST'
,p_named_lov=>'FEEDBACK_STATUS'
,p_lov=>'.'||wwv_flow_imp.id(54614072353996148)||'.'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'NONE'
,p_attribute_02=>'N'
,p_attribute_03=>'Y'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(54616206664996149)
,p_name=>'P10024_USER_AGENT'
,p_item_sequence=>100
,p_item_plug_id=>wwv_flow_imp.id(54600279238996129)
,p_prompt=>'User Agent'
,p_display_as=>'NATIVE_DISPLAY_ONLY'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_lov_display_extra=>'NO'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'VALUE'
,p_attribute_04=>'Y'
,p_attribute_05=>'PLAIN'
);
wwv_flow_imp_page.create_page_da_event(
 p_id=>wwv_flow_imp.id(54600730581996129)
,p_name=>'Cancel Dialog'
,p_event_sequence=>10
,p_triggering_element_type=>'BUTTON'
,p_triggering_button_id=>wwv_flow_imp.id(54600502277996129)
,p_bind_type=>'bind'
,p_bind_event_type=>'click'
);
wwv_flow_imp_page.create_page_da_action(
 p_id=>wwv_flow_imp.id(54617054814996149)
,p_event_id=>wwv_flow_imp.id(54600730581996129)
,p_event_result=>'TRUE'
,p_action_sequence=>10
,p_execute_on_page_init=>'N'
,p_action=>'NATIVE_DIALOG_CANCEL'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(54617528174996150)
,p_process_sequence=>10
,p_process_point=>'AFTER_HEADER'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'Load Data'
,p_process_sql_clob=>wwv_flow_string.join(wwv_flow_t_varchar2(
'for c1 in (',
'   select page_id,',
'          page_name,',
'          case feedback_rating',
'                when 1 then ''<span class="fa fa-frown-o feedback-negative" aria-hidden="true" title="Negative"></span>'' ',
'                when 2 then ''<span class="fa fa-emoji-neutral feedback-neutral" aria-hidden="true" title="Neutral"></span>''  ',
'                when 3 then ''<span class="fa fa-smile-o feedback-positive" aria-hidden="true" title="Positive"></span>'' ',
'                end rating_icon,',
'          lower(created_by) || '' - '' || apex_util.get_since(created_on) filed,',
'          feedback,',
'          public_response,',
'          feedback_status,',
'          http_user_agent',
'     from apex_team_feedback',
'    where feedback_id = :P10024_ID',
') loop',
'   :P10024_PAGE_ID := c1.page_id||''. ''||c1.page_name;',
'   :P10024_FILED := c1.filed;',
'   :P10024_RATING_ICON := c1.rating_icon;',
'   :P10024_FEEDBACK := c1.feedback;',
'   :P10024_RESPONSE := c1.public_response;',
'   :P10024_FEEDBACK_STATUS := c1.feedback_status;',
'   :P10024_USER_AGENT := c1.http_user_agent;',
'end loop;'))
,p_process_clob_language=>'PLSQL'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(54617886730996150)
,p_process_sequence=>10
,p_process_point=>'AFTER_SUBMIT'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'Update'
,p_process_sql_clob=>wwv_flow_string.join(wwv_flow_t_varchar2(
'apex_util.reply_to_feedback (',
'    p_feedback_id        => :P10024_ID,',
'    p_status             => :P10024_FEEDBACK_STATUS,',
'    p_public_response    => :P10024_RESPONSE );'))
,p_process_clob_language=>'PLSQL'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_when_button_id=>wwv_flow_imp.id(54600408228996129)
,p_process_success_message=>'Action Processed.'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(54618316956996150)
,p_process_sequence=>20
,p_process_point=>'AFTER_SUBMIT'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'Delete'
,p_process_sql_clob=>wwv_flow_string.join(wwv_flow_t_varchar2(
'apex_util.delete_feedback (',
'    p_feedback_id        => :P10024_ID );'))
,p_process_clob_language=>'PLSQL'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_when_button_id=>wwv_flow_imp.id(54600636410996129)
,p_process_success_message=>'Action Processed.'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(54618709522996151)
,p_process_sequence=>40
,p_process_point=>'AFTER_SUBMIT'
,p_process_type=>'NATIVE_SESSION_STATE'
,p_process_name=>'E.PROPERTY.BUTTON_ACTION.LOV.RESET.D'
,p_attribute_01=>'CLEAR_CACHE_CURRENT_PAGE'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_when_button_id=>wwv_flow_imp.id(54600636410996129)
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(54619090790996151)
,p_process_sequence=>50
,p_process_point=>'AFTER_SUBMIT'
,p_process_type=>'NATIVE_CLOSE_WINDOW'
,p_process_name=>'Close Dialog'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
);
wwv_flow_imp.component_end;
end;
/
