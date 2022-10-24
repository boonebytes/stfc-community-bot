prompt --application/pages/page_09998
begin
--   Manifest
--     PAGE: 09998
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
 p_id=>9998
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Discord Login'
,p_alias=>'DISCORD-LOGIN'
,p_step_title=>'Discord Login'
,p_autocomplete_on_off=>'OFF'
,p_step_template=>wwv_flow_imp.id(54410073767995681)
,p_page_template_options=>'#DEFAULT#'
,p_page_is_public_y_n=>'Y'
,p_page_component_map=>'12'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20221017202016'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(2311809892727701)
,p_plug_name=>'Login'
,p_region_template_options=>'#DEFAULT#:t-Region--scrollBody'
,p_plug_template=>wwv_flow_imp.id(54462243687995736)
,p_plug_display_sequence=>20
,p_include_in_reg_disp_sel_yn=>'Y'
,p_plug_source=>wwv_flow_string.join(wwv_flow_t_varchar2(
'<p>This site uses Discord to authenticate.</p>',
'<ol>',
'    <li>Click on the buton below to be redircted to Discord.</li>',
'    <li>You may be prompted for your Discord credentials. You can verify',
'        that the credentials are going to Discord by looking at the URL.',
'        Your Discord credentials are never received by this application.</li>',
'    <li>If this is your first time accessing this website, Discord will show',
'        a list of permissions being requested.',
'        <ul>',
'            <li>Identify - This is required for core functionality, to',
'                associate you to an account in this application. Your email',
'                address will not be included in the data received from Discord.</li>',
'            <li>List of Servers - This permission is intended for future development;',
'                specifically to provide a web interface to manage server-specific',
'                settings on the STFC Community Bot.</li>',
'        </ul></li>',
'    <li>If you accept the authorization prompt from Discord, you will be returned to this',
'        website automatically. If there''s an account for you here, you will be logged in.</li>',
'</ol>',
'<p>If you encounter any problems or require further information, please contact Boonebytes#0570</p>'))
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_attribute_01=>'N'
,p_attribute_02=>'HTML'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(2311938661727702)
,p_button_sequence=>10
,p_button_plug_id=>wwv_flow_imp.id(2311809892727701)
,p_button_name=>'LoginWithDiscord'
,p_button_action=>'SUBMIT'
,p_button_template_options=>'#DEFAULT#:t-Button--large:t-Button--primary:t-Button--iconLeft'
,p_button_template_id=>wwv_flow_imp.id(54527438844995804)
,p_button_image_alt=>'Login with Discord'
,p_button_position=>'CHANGE'
,p_icon_css_classes=>'fa-key'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(2312236914727705)
,p_name=>'CODE'
,p_item_sequence=>40
,p_display_as=>'NATIVE_HIDDEN'
,p_attribute_01=>'N'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(2313778603727720)
,p_name=>'P9998_LOGIN_MESSAGE'
,p_item_sequence=>10
,p_prompt=>'Login Message'
,p_display_as=>'NATIVE_DISPLAY_ONLY'
,p_display_when=>'P9998_LOGIN_MESSAGE'
,p_display_when_type=>'ITEM_IS_NOT_NULL'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_attribute_01=>'N'
,p_attribute_02=>'VALUE'
,p_attribute_04=>'Y'
,p_attribute_05=>'PLAIN'
);
wwv_flow_imp_page.create_page_computation(
 p_id=>wwv_flow_imp.id(2314591549727728)
,p_computation_sequence=>10
,p_computation_item=>'P9998_LOGIN_MESSAGE'
,p_computation_point=>'AFTER_HEADER'
,p_computation_type=>'ITEM_VALUE'
,p_computation=>'LOGIN_MESSAGE'
,p_compute_when=>'LOGIN_MESSAGE'
,p_compute_when_type=>'ITEM_IS_NOT_NULL'
);
wwv_flow_imp_page.create_page_computation(
 p_id=>wwv_flow_imp.id(2314759214727730)
,p_computation_sequence=>20
,p_computation_item=>'LOGIN_MESSAGE'
,p_computation_point=>'AFTER_HEADER'
,p_computation_type=>'STATIC_ASSIGNMENT'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(2312135100727704)
,p_process_sequence=>10
,p_process_point=>'AFTER_SUBMIT'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'StartDiscordLogin'
,p_process_sql_clob=>wwv_flow_string.join(wwv_flow_t_varchar2(
'PKG_AUTH.START_AUTH();',
''))
,p_process_clob_language=>'PLSQL'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
,p_process_when_button_id=>wwv_flow_imp.id(2311938661727702)
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(2314645320727729)
,p_process_sequence=>20
,p_process_point=>'BEFORE_HEADER'
,p_process_type=>'NATIVE_PLSQL'
,p_process_name=>'FinishDiscordLogin'
,p_process_sql_clob=>'PKG_AUTH.END_AUTH(:CODE);'
,p_process_clob_language=>'PLSQL'
,p_process_when=>'CODE'
,p_process_when_type=>'ITEM_IS_NOT_NULL'
);
wwv_flow_imp.component_end;
end;
/
