prompt --application/pages/page_00000
begin
--   Manifest
--     PAGE: 00000
--   Manifest End
wwv_flow_api.component_begin (
 p_version_yyyy_mm_dd=>'2021.04.15'
,p_release=>'21.1.0'
,p_default_workspace_id=>18900386187764698
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_api.create_page(
 p_id=>0
,p_user_interface_id=>wwv_flow_api.id(54551875302995858)
,p_name=>'Global Page - Desktop'
,p_step_title=>'Global Page - Desktop'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_protection_level=>'D'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220917214236'
);
wwv_flow_api.create_page_da_event(
 p_id=>wwv_flow_api.id(75004906356727546)
,p_name=>'Auto-Select Number'
,p_event_sequence=>10
,p_triggering_element_type=>'JQUERY_SELECTOR'
,p_triggering_element=>'input.number_field, input.apex-item-text'
,p_bind_type=>'live'
,p_bind_event_type=>'focusin'
);
wwv_flow_api.create_page_da_action(
 p_id=>wwv_flow_api.id(75005011154727547)
,p_event_id=>wwv_flow_api.id(75004906356727546)
,p_event_result=>'TRUE'
,p_action_sequence=>10
,p_execute_on_page_init=>'N'
,p_action=>'NATIVE_JAVASCRIPT_CODE'
,p_attribute_01=>wwv_flow_string.join(wwv_flow_t_varchar2(
unistr('var fieldLength = Number(this.triggeringElement.value.length); //Get the content\2019s length'),
'',
'//select the whole content of the field',
'this.triggeringElement.selectionStart = 0;',
'this.triggeringElement.selectionEnd = fieldLength;'))
);
wwv_flow_api.component_end;
end;
/
