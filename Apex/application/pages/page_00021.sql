prompt --application/pages/page_00021
begin
--   Manifest
--     PAGE: 00021
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
 p_id=>21
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Territory'
,p_alias=>'TERRITORY'
,p_step_title=>'Territory'
,p_autocomplete_on_off=>'OFF'
,p_javascript_code=>'var htmldb_delete_message=''"DELETE_CONFIRM_MSG"'';'
,p_page_template_options=>'#DEFAULT#'
,p_protection_level=>'C'
,p_page_component_map=>'02'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220917101603'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(75004189654727538)
,p_plug_name=>'Connected Zones'
,p_region_template_options=>'#DEFAULT#'
,p_component_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54460366006995735)
,p_plug_display_sequence=>20
,p_include_in_reg_disp_sel_yn=>'Y'
,p_query_type=>'SQL'
,p_plug_source=>wwv_flow_string.join(wwv_flow_t_varchar2(
'select ZONES.ID, ZONES.NAME as NAME,',
'    ZONES."LEVEL" as "LEVEL",',
'    ZONES.OWNER_ID as OWNER_ID ',
' from ZONE_NEIGHBOURS ZONE_NEIGHBOURS',
' inner join ZONES ZONES on (ZONE_NEIGHBOURS.TO_ZONE_ID=ZONES.ID)',
' where ZONE_NEIGHBOURS.FROM_ZONE_ID = :P21_ID',
'',
'union',
'',
' select ZONES.ID, ZONES.NAME as NAME,',
'    ZONES."LEVEL" as "LEVEL",',
'    ZONES.OWNER_ID as OWNER_ID ',
' from ZONE_NEIGHBOURS ZONE_NEIGHBOURS',
' inner join ZONES ZONES on (ZONE_NEIGHBOURS.FROM_ZONE_ID=ZONES.ID)',
' where ZONE_NEIGHBOURS.TO_ZONE_ID = :P21_ID',
'',
'ORDER BY NAME'))
,p_plug_source_type=>'NATIVE_IR'
,p_ajax_items_to_submit=>'P21_ID'
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_plug_display_condition_type=>'ITEM_IS_NOT_NULL'
,p_plug_display_when_condition=>'P21_ID'
,p_prn_content_disposition=>'ATTACHMENT'
,p_prn_units=>'INCHES'
,p_prn_paper_size=>'LETTER'
,p_prn_width=>11
,p_prn_height=>8.5
,p_prn_orientation=>'HORIZONTAL'
,p_prn_page_header=>'Connected Zones'
,p_prn_page_header_font_color=>'#000000'
,p_prn_page_header_font_family=>'Helvetica'
,p_prn_page_header_font_weight=>'normal'
,p_prn_page_header_font_size=>'12'
,p_prn_page_footer_font_color=>'#000000'
,p_prn_page_footer_font_family=>'Helvetica'
,p_prn_page_footer_font_weight=>'normal'
,p_prn_page_footer_font_size=>'12'
,p_prn_header_bg_color=>'#EEEEEE'
,p_prn_header_font_color=>'#000000'
,p_prn_header_font_family=>'Helvetica'
,p_prn_header_font_weight=>'bold'
,p_prn_header_font_size=>'10'
,p_prn_body_bg_color=>'#FFFFFF'
,p_prn_body_font_color=>'#000000'
,p_prn_body_font_family=>'Helvetica'
,p_prn_body_font_weight=>'normal'
,p_prn_body_font_size=>'10'
,p_prn_border_width=>.5
,p_prn_page_header_alignment=>'CENTER'
,p_prn_page_footer_alignment=>'CENTER'
,p_prn_border_color=>'#666666'
);
wwv_flow_imp_page.create_worksheet(
 p_id=>wwv_flow_imp.id(75004201958727539)
,p_max_row_count=>'1000000'
,p_pagination_type=>'ROWS_X_TO_Y'
,p_pagination_display_pos=>'BOTTOM_RIGHT'
,p_report_list_mode=>'TABS'
,p_lazy_loading=>false
,p_show_detail_link=>'C'
,p_show_notify=>'Y'
,p_download_formats=>'CSV:HTML:XLSX:PDF'
,p_enable_mail_download=>'Y'
,p_detail_link=>'f?p=&APP_ID.:21:&SESSION.::&DEBUG.::P21_ID:#ID#'
,p_detail_link_text=>'<img src="#IMAGE_PREFIX#app_ui/img/icons/apex-edit-pencil.png" class="apex-edit-pencil" alt="">'
,p_owner=>'BOONEBYTES'
,p_internal_uid=>75004201958727539
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(75004616751727543)
,p_db_column_name=>'ID'
,p_display_order=>10
,p_column_identifier=>'D'
,p_column_label=>'Id'
,p_column_type=>'NUMBER'
,p_display_text_as=>'HIDDEN'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(75004394933727540)
,p_db_column_name=>'NAME'
,p_display_order=>20
,p_column_identifier=>'A'
,p_column_label=>'Name'
,p_column_type=>'STRING'
,p_heading_alignment=>'LEFT'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(75004491828727541)
,p_db_column_name=>'LEVEL'
,p_display_order=>30
,p_column_identifier=>'B'
,p_column_label=>'Level'
,p_column_type=>'NUMBER'
,p_heading_alignment=>'LEFT'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(75004502114727542)
,p_db_column_name=>'OWNER_ID'
,p_display_order=>40
,p_column_identifier=>'C'
,p_column_label=>'Owner'
,p_column_type=>'NUMBER'
,p_display_text_as=>'LOV_ESCAPE_SC'
,p_heading_alignment=>'LEFT'
,p_rpt_named_lov=>wwv_flow_imp.id(54700401318017067)
,p_rpt_show_filter_lov=>'1'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_rpt(
 p_id=>wwv_flow_imp.id(84301587042111677)
,p_application_user=>'APXWS_DEFAULT'
,p_report_seq=>10
,p_report_alias=>'843016'
,p_status=>'PUBLIC'
,p_is_default=>'Y'
,p_report_columns=>'NAME:LEVEL:OWNER_ID:ID'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(83600908195815862)
,p_plug_name=>'Breadcrumb'
,p_region_template_options=>'#DEFAULT#:t-BreadcrumbRegion--useBreadcrumbTitle'
,p_component_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54471654815995741)
,p_plug_display_sequence=>10
,p_plug_display_point=>'REGION_POSITION_01'
,p_menu_id=>wwv_flow_imp.id(54404663861995635)
,p_plug_source_type=>'NATIVE_BREADCRUMB'
,p_menu_template_id=>wwv_flow_imp.id(54528727036995806)
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(83601538581815957)
,p_plug_name=>'Territory'
,p_region_template_options=>'#DEFAULT#:t-Region--scrollBody'
,p_plug_template=>wwv_flow_imp.id(54462243687995736)
,p_plug_display_sequence=>10
,p_query_type=>'SQL'
,p_plug_source=>wwv_flow_string.join(wwv_flow_t_varchar2(
'select ID,',
'       NAME,',
'       ZONES."LEVEL",',
'       DEFEND_DAY_OF_WEEK,',
'       DEFEND_UTC_TIME,',
'       DEFEND_EASTERN_DAY,',
'       to_char(to_date(DEFEND_EASTERN_TIME, ''HH24:MI:SS''), ''HH:MI PM'') DEFEND_EASTERN_TIME,',
'       OWNER_ID,',
'       NEXT_DEFEND,',
'    FROM_TZ( CAST( NEXT_DEFEND AS TIMESTAMP ), ''UTC'' ) AT LOCAL AS NEXT_DEFEND_LOCAL',
'  from ZONES',
'  WHERE ID = :P21_ID'))
,p_is_editable=>true
,p_edit_operations=>'i:u:d'
,p_lost_update_check_type=>'VALUES'
,p_plug_source_type=>'NATIVE_FORM'
,p_ajax_items_to_submit=>'P21_ID'
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(83609726216815991)
,p_button_sequence=>30
,p_button_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_button_name=>'SAVE'
,p_button_action=>'SUBMIT'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_is_hot=>'Y'
,p_button_image_alt=>'Apply Changes'
,p_button_position=>'CHANGE'
,p_button_condition=>'P21_ID'
,p_button_condition_type=>'ITEM_IS_NOT_NULL'
,p_security_scheme=>wwv_flow_imp.id(54555485978995935)
,p_database_action=>'UPDATE'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(83608554029815986)
,p_button_sequence=>10
,p_button_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_button_name=>'CANCEL'
,p_button_action=>'REDIRECT_PAGE'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_image_alt=>'Cancel'
,p_button_position=>'CLOSE'
,p_button_redirect_url=>'f?p=&APP_ID.:6:&SESSION.::&DEBUG.:::'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(83610164898815991)
,p_button_sequence=>40
,p_button_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_button_name=>'CREATE'
,p_button_action=>'SUBMIT'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_is_hot=>'Y'
,p_button_image_alt=>'Create'
,p_button_position=>'CREATE'
,p_button_condition=>'P21_ID'
,p_button_condition_type=>'ITEM_IS_NULL'
,p_security_scheme=>wwv_flow_imp.id(54555301792995935)
,p_database_action=>'INSERT'
);
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(83609387311815991)
,p_button_sequence=>20
,p_button_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_button_name=>'DELETE'
,p_button_action=>'REDIRECT_URL'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_image_alt=>'Delete'
,p_button_position=>'DELETE'
,p_button_redirect_url=>'javascript:apex.confirm(htmldb_delete_message,''DELETE'');'
,p_button_execute_validations=>'N'
,p_button_condition=>'P21_ID'
,p_button_condition_type=>'ITEM_IS_NOT_NULL'
,p_security_scheme=>wwv_flow_imp.id(54555301792995935)
,p_database_action=>'DELETE'
);
wwv_flow_imp_page.create_page_branch(
 p_id=>wwv_flow_imp.id(83610456199815991)
,p_branch_action=>'f?p=&APP_ID.:6:&SESSION.::&DEBUG.&success_msg=#SUCCESS_MSG#'
,p_branch_point=>'AFTER_PROCESSING'
,p_branch_type=>'REDIRECT_URL'
,p_branch_sequence=>1
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(75004070043727537)
,p_name=>'P21_NEXT_DEFEND_LOCAL'
,p_source_data_type=>'TIMESTAMP_TZ'
,p_item_sequence=>120
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Next Defend Local'
,p_format_mask=>'MM/DD/RRRR HH:MI PM'
,p_source=>'NEXT_DEFEND_LOCAL'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_DATE_PICKER_JET'
,p_cSize=>30
,p_begin_on_new_line=>'N'
,p_read_only_when_type=>'ALWAYS'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'POPUP'
,p_attribute_03=>'NONE'
,p_attribute_06=>'NONE'
,p_attribute_09=>'N'
,p_attribute_11=>'Y'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83601846075815959)
,p_name=>'P21_ID'
,p_source_data_type=>'NUMBER'
,p_is_primary_key=>true
,p_is_query_only=>true
,p_item_sequence=>10
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_use_cache_before_default=>'NO'
,p_prompt=>'Id'
,p_source=>'ID'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_HIDDEN'
,p_label_alignment=>'RIGHT'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_protection_level=>'S'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83602220805815971)
,p_name=>'P21_NAME'
,p_source_data_type=>'VARCHAR2'
,p_is_required=>true
,p_item_sequence=>20
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Name'
,p_source=>'NAME'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_TEXT_FIELD'
,p_cSize=>60
,p_cMaxlength=>200
,p_field_template=>wwv_flow_imp.id(54526142179995799)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'N'
,p_attribute_02=>'N'
,p_attribute_04=>'TEXT'
,p_attribute_05=>'BOTH'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83602613239815975)
,p_name=>'P21_LEVEL'
,p_source_data_type=>'NUMBER'
,p_is_required=>true
,p_item_sequence=>30
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Level'
,p_source=>'LEVEL'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_NUMBER_FIELD'
,p_cSize=>2
,p_cMaxlength=>1
,p_begin_on_new_line=>'N'
,p_field_template=>wwv_flow_imp.id(54526142179995799)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'1'
,p_attribute_02=>'3'
,p_attribute_03=>'left'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83603057729815975)
,p_name=>'P21_DEFEND_DAY_OF_WEEK'
,p_source_data_type=>'VARCHAR2'
,p_is_required=>true
,p_item_sequence=>70
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Defend UTC Day'
,p_source=>'DEFEND_DAY_OF_WEEK'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_SELECT_LIST'
,p_lov=>'STATIC2:Sunday;Sunday,Monday;Monday,Tuesday;Tuesday,Wednesday;Wednesday,Thursday;Thursday,Friday;Friday,Saturday;Saturday'
,p_lov_display_null=>'YES'
,p_cHeight=>1
,p_field_template=>wwv_flow_imp.id(54526142179995799)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_lov_display_extra=>'YES'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'NONE'
,p_attribute_02=>'N'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83603434948815975)
,p_name=>'P21_DEFEND_UTC_TIME'
,p_source_data_type=>'VARCHAR2'
,p_is_required=>true
,p_item_sequence=>80
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Defend UTC Time'
,p_source=>'DEFEND_UTC_TIME'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_TEXT_FIELD'
,p_cSize=>32
,p_cMaxlength=>10
,p_begin_on_new_line=>'N'
,p_field_template=>wwv_flow_imp.id(54526142179995799)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'N'
,p_attribute_02=>'N'
,p_attribute_04=>'TEXT'
,p_attribute_05=>'NONE'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83603873735815975)
,p_name=>'P21_DEFEND_EASTERN_DAY'
,p_source_data_type=>'NUMBER'
,p_is_query_only=>true
,p_item_sequence=>90
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Defend Eastern Day'
,p_source=>'DEFEND_EASTERN_DAY'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_SELECT_LIST'
,p_named_lov=>'DAY_OF_WEEK'
,p_lov=>'.'||wwv_flow_imp.id(83801096997934991)||'.'
,p_lov_display_null=>'YES'
,p_cHeight=>1
,p_read_only_when_type=>'ALWAYS'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_lov_display_extra=>'YES'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'NONE'
,p_attribute_02=>'N'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83604238165815975)
,p_name=>'P21_DEFEND_EASTERN_TIME'
,p_source_data_type=>'VARCHAR2'
,p_is_query_only=>true
,p_item_sequence=>100
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Defend Eastern Time'
,p_format_mask=>'HH:MI PM'
,p_source=>'DEFEND_EASTERN_TIME'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_TEXT_FIELD'
,p_cSize=>30
,p_cMaxlength=>8
,p_begin_on_new_line=>'N'
,p_read_only_when_type=>'ALWAYS'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'N'
,p_attribute_02=>'N'
,p_attribute_04=>'TEXT'
,p_attribute_05=>'BOTH'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83604641949815976)
,p_name=>'P21_OWNER_ID'
,p_source_data_type=>'NUMBER'
,p_item_sequence=>40
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Owner'
,p_source=>'OWNER_ID'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_SELECT_LIST'
,p_named_lov=>'ALLIANCES'
,p_lov=>wwv_flow_string.join(wwv_flow_t_varchar2(
'SELECT ID, ACRONYM || '' - '' || NAME "DISPLAY"',
'FROM alliances',
'WHERE status = ''A''',
'ORDER BY ACRONYM'))
,p_lov_display_null=>'YES'
,p_cHeight=>1
,p_begin_on_new_line=>'N'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_lov_display_extra=>'YES'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'NONE'
,p_attribute_02=>'N'
);
wwv_flow_imp_page.create_page_item(
 p_id=>wwv_flow_imp.id(83605093639815976)
,p_name=>'P21_NEXT_DEFEND'
,p_source_data_type=>'TIMESTAMP'
,p_is_query_only=>true
,p_item_sequence=>110
,p_item_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_item_source_plug_id=>wwv_flow_imp.id(83601538581815957)
,p_prompt=>'Next Defend UTC'
,p_format_mask=>'MM/DD/RRRR HH:MI PM'
,p_source=>'NEXT_DEFEND'
,p_source_type=>'REGION_SOURCE_COLUMN'
,p_display_as=>'NATIVE_DATE_PICKER_JET'
,p_cSize=>32
,p_cMaxlength=>255
,p_read_only_when_type=>'ALWAYS'
,p_field_template=>wwv_flow_imp.id(54524856483995797)
,p_item_template_options=>'#DEFAULT#'
,p_is_persistent=>'N'
,p_encrypt_session_state_yn=>'N'
,p_attribute_01=>'Y'
,p_attribute_02=>'POPUP'
,p_attribute_03=>'NONE'
,p_attribute_06=>'NONE'
,p_attribute_09=>'N'
,p_attribute_11=>'Y'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(83611323962815997)
,p_process_sequence=>10
,p_process_point=>'AFTER_SUBMIT'
,p_region_id=>wwv_flow_imp.id(83601538581815957)
,p_process_type=>'NATIVE_FORM_DML'
,p_process_name=>'Process form Territory'
,p_attribute_01=>'REGION_SOURCE'
,p_attribute_05=>'Y'
,p_attribute_06=>'Y'
,p_attribute_08=>'Y'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(83610980054815995)
,p_process_sequence=>10
,p_process_point=>'BEFORE_HEADER'
,p_region_id=>wwv_flow_imp.id(83601538581815957)
,p_process_type=>'NATIVE_FORM_INIT'
,p_process_name=>'Initialize form Territory'
);
wwv_flow_imp.component_end;
end;
/