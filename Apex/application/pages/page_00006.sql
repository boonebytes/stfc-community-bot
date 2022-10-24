prompt --application/pages/page_00006
begin
--   Manifest
--     PAGE: 00006
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
 p_id=>6
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Territories'
,p_alias=>'TERRITORIES'
,p_step_title=>'Territories'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_page_component_map=>'21'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220917094532'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(55301449981180841)
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
 p_id=>wwv_flow_imp.id(55302078716180848)
,p_plug_name=>'Territories'
,p_region_template_options=>'#DEFAULT#'
,p_component_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54460366006995735)
,p_plug_display_sequence=>20
,p_query_type=>'SQL'
,p_plug_source=>wwv_flow_string.join(wwv_flow_t_varchar2(
'select ZONES.ID as ID,',
'    ZONES.NAME as NAME,',
'    ZONES."LEVEL" as "LEVEL",',
'    ZONES.DEFEND_DAY_OF_WEEK as DEFEND_DAY_OF_WEEK,',
'    ZONES.DEFEND_UTC_TIME as DEFEND_UTC_TIME,',
'    ZONES.DEFEND_EASTERN_DAY as DEFEND_EASTERN_DAY,',
'    ZONES.DEFEND_EASTERN_TIME as DEFEND_EASTERN_TIME,',
'    ZONES.NOTES as NOTES,',
'    ZONES.OWNER_ID as OWNER_ID,',
'    ZONES.NEXT_DEFEND as NEXT_DEFEND,',
'    FROM_TZ( CAST( NEXT_DEFEND AS TIMESTAMP ), ''UTC'' ) AT LOCAL AS NEXT_DEFEND_LOCAL,',
'    ZONES.MODIFIED_BY as MODIFIED_BY,',
'    ZONES.MODIFIED_DATE as MODIFIED_DATE ',
' from ZONES ZONES'))
,p_plug_source_type=>'NATIVE_IG'
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_prn_page_header=>'Territories'
,p_plug_header=>wwv_flow_string.join(wwv_flow_t_varchar2(
'<i>When updating territory owners in this website, run the RELOAD command',
'    on the Discord bot. This will repopulate the Threats column and',
'    schedule any defend reminder broadcasts. The RELOAD command is executed',
'    automatically at 6:00 AM UTC daily. This is not required if territory',
'    owners are updated via the bot.</i>'))
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(52805965880951492)
,p_name=>'NEXT_DEFEND_LOCAL'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'NEXT_DEFEND_LOCAL'
,p_data_type=>'TIMESTAMP_TZ'
,p_is_query_only=>true
,p_item_type=>'NATIVE_DISPLAY_ONLY'
,p_heading=>'Local Defend Time'
,p_heading_alignment=>'LEFT'
,p_display_sequence=>130
,p_value_alignment=>'LEFT'
,p_attribute_02=>'VALUE'
,p_attribute_05=>'PLAIN'
,p_format_mask=>'DD-MON-YYYY HH:MIPM'
,p_enable_filter=>true
,p_filter_is_required=>false
,p_filter_date_ranges=>'ALL'
,p_filter_lov_type=>'DISTINCT'
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_hide=>true
,p_is_primary_key=>false
,p_include_in_export=>true
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55303357235180861)
,p_name=>'APEX$ROW_SELECTOR'
,p_item_type=>'NATIVE_ROW_SELECTOR'
,p_display_sequence=>10
,p_attribute_01=>'Y'
,p_attribute_02=>'Y'
,p_attribute_03=>'N'
,p_enable_hide=>true
,p_is_primary_key=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55303865148180863)
,p_name=>'APEX$ROW_ACTION'
,p_item_type=>'NATIVE_ROW_ACTION'
,p_label=>'Actions'
,p_heading_alignment=>'CENTER'
,p_display_sequence=>20
,p_value_alignment=>'CENTER'
,p_enable_hide=>true
,p_is_primary_key=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55305805290180932)
,p_name=>'ID'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'ID'
,p_data_type=>'NUMBER'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>40
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>true
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55306745434180933)
,p_name=>'NAME'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'NAME'
,p_data_type=>'VARCHAR2'
,p_is_query_only=>false
,p_item_type=>'NATIVE_DISPLAY_ONLY'
,p_heading=>'Name'
,p_heading_alignment=>'LEFT'
,p_display_sequence=>50
,p_value_alignment=>'LEFT'
,p_attribute_02=>'VALUE'
,p_attribute_05=>'PLAIN'
,p_enable_filter=>true
,p_filter_operators=>'C:S:CASE_INSENSITIVE:REGEXP'
,p_filter_is_required=>false
,p_filter_text_case=>'MIXED'
,p_filter_lov_type=>'NONE'
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_hide=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>true
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55307766272180934)
,p_name=>'LEVEL'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'LEVEL'
,p_data_type=>'NUMBER'
,p_is_query_only=>false
,p_item_type=>'NATIVE_DISPLAY_ONLY'
,p_heading=>'Level'
,p_heading_alignment=>'LEFT'
,p_display_sequence=>70
,p_value_alignment=>'LEFT'
,p_attribute_02=>'VALUE'
,p_attribute_05=>'PLAIN'
,p_enable_filter=>true
,p_filter_is_required=>false
,p_filter_lov_type=>'NONE'
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_hide=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>true
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55309725204180936)
,p_name=>'DEFEND_DAY_OF_WEEK'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'DEFEND_DAY_OF_WEEK'
,p_data_type=>'VARCHAR2'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>80
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55310710915180936)
,p_name=>'DEFEND_UTC_TIME'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'DEFEND_UTC_TIME'
,p_data_type=>'VARCHAR2'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>90
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55311726666180936)
,p_name=>'DEFEND_EASTERN_DAY'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'DEFEND_EASTERN_DAY'
,p_data_type=>'NUMBER'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>100
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55312752309180937)
,p_name=>'DEFEND_EASTERN_TIME'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'DEFEND_EASTERN_TIME'
,p_data_type=>'VARCHAR2'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>110
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55313710134180937)
,p_name=>'NOTES'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'NOTES'
,p_data_type=>'VARCHAR2'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>140
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>false
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55314759288180938)
,p_name=>'OWNER_ID'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'OWNER_ID'
,p_data_type=>'NUMBER'
,p_is_query_only=>false
,p_item_type=>'NATIVE_SELECT_LIST'
,p_heading=>'Owner'
,p_heading_alignment=>'LEFT'
,p_display_sequence=>60
,p_value_alignment=>'LEFT'
,p_is_required=>false
,p_lov_type=>'SHARED'
,p_lov_id=>wwv_flow_imp.id(54700401318017067)
,p_lov_display_extra=>true
,p_lov_display_null=>true
,p_enable_filter=>true
,p_filter_operators=>'C:S:CASE_INSENSITIVE:REGEXP'
,p_filter_is_required=>false
,p_filter_text_case=>'MIXED'
,p_filter_exact_match=>true
,p_filter_lov_type=>'LOV'
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_hide=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>true
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55315689313180938)
,p_name=>'NEXT_DEFEND'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'NEXT_DEFEND'
,p_data_type=>'TIMESTAMP'
,p_is_query_only=>true
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>120
,p_attribute_01=>'Y'
,p_format_mask=>'DD-MON-YYYY HH:MIPM'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55316733786180939)
,p_name=>'MODIFIED_BY'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'MODIFIED_BY'
,p_data_type=>'VARCHAR2'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>150
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>false
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(55317719123180939)
,p_name=>'MODIFIED_DATE'
,p_source_type=>'DB_COLUMN'
,p_source_expression=>'MODIFIED_DATE'
,p_data_type=>'TIMESTAMP'
,p_is_query_only=>false
,p_item_type=>'NATIVE_HIDDEN'
,p_display_sequence=>160
,p_attribute_01=>'Y'
,p_filter_is_required=>false
,p_use_as_row_header=>false
,p_enable_sort_group=>true
,p_enable_control_break=>true
,p_enable_pivot=>false
,p_is_primary_key=>false
,p_duplicate_value=>true
,p_include_in_export=>false
);
wwv_flow_imp_page.create_region_column(
 p_id=>wwv_flow_imp.id(75003743056727534)
,p_name=>'View'
,p_source_type=>'NONE'
,p_item_type=>'NATIVE_LINK'
,p_heading_alignment=>'LEFT'
,p_display_sequence=>30
,p_value_alignment=>'LEFT'
,p_link_target=>'f?p=&APP_ID.:21:&SESSION.::&DEBUG.::P21_ID:&ID.'
,p_link_text=>'<img src="#IMAGE_PREFIX#app_ui/img/icons/apex-edit-view.png" class="apex-edit-view" alt="">'
,p_use_as_row_header=>false
,p_enable_hide=>true
,p_escape_on_http_output=>true
);
wwv_flow_imp_page.create_interactive_grid(
 p_id=>wwv_flow_imp.id(55302485364180850)
,p_internal_uid=>24902205661083175
,p_is_editable=>true
,p_edit_operations=>'i:u:d'
,p_add_authorization_scheme=>wwv_flow_imp.id(54555301792995935)
,p_update_authorization_scheme=>wwv_flow_imp.id(54555485978995935)
,p_delete_authorization_scheme=>wwv_flow_imp.id(54555301792995935)
,p_lost_update_check_type=>'VALUES'
,p_add_row_if_empty=>true
,p_submit_checked_rows=>false
,p_lazy_loading=>false
,p_requires_filter=>false
,p_select_first_row=>true
,p_fixed_row_height=>true
,p_pagination_type=>'SCROLL'
,p_show_total_row_count=>true
,p_show_toolbar=>true
,p_enable_save_public_report=>false
,p_enable_subscriptions=>true
,p_enable_flashback=>true
,p_define_chart_view=>true
,p_enable_download=>true
,p_enable_mail_download=>true
,p_fixed_header=>'PAGE'
,p_show_icon_view=>false
,p_show_detail_view=>false
);
wwv_flow_imp_page.create_ig_report(
 p_id=>wwv_flow_imp.id(55302953425180854)
,p_interactive_grid_id=>wwv_flow_imp.id(55302485364180850)
,p_static_id=>'249027'
,p_type=>'PRIMARY'
,p_default_view=>'GRID'
,p_show_row_number=>false
,p_settings_area_expanded=>true
);
wwv_flow_imp_page.create_ig_report_view(
 p_id=>wwv_flow_imp.id(55303165058180856)
,p_report_id=>wwv_flow_imp.id(55302953425180854)
,p_view_type=>'GRID'
,p_stretch_columns=>true
,p_srv_exclude_null_values=>false
,p_srv_only_display_columns=>true
,p_edit_mode=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55304224895180865)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>0
,p_column_id=>wwv_flow_imp.id(55303865148180863)
,p_is_visible=>true
,p_is_frozen=>true
,p_width=>75
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55306164685180933)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>2
,p_column_id=>wwv_flow_imp.id(55305805290180932)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55307112887180934)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>3
,p_column_id=>wwv_flow_imp.id(55306745434180933)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55308119500180935)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>5
,p_column_id=>wwv_flow_imp.id(55307766272180934)
,p_is_visible=>true
,p_is_frozen=>false
,p_width=>100
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55310131471180936)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>6
,p_column_id=>wwv_flow_imp.id(55309725204180936)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55311083812180936)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>7
,p_column_id=>wwv_flow_imp.id(55310710915180936)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55312103603180937)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>8
,p_column_id=>wwv_flow_imp.id(55311726666180936)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55313132110180937)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>9
,p_column_id=>wwv_flow_imp.id(55312752309180937)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55314129655180938)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>10
,p_column_id=>wwv_flow_imp.id(55313710134180937)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55315108086180938)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>4
,p_column_id=>wwv_flow_imp.id(55314759288180938)
,p_is_visible=>true
,p_is_frozen=>false
,p_width=>125
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55316147204180938)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>12
,p_column_id=>wwv_flow_imp.id(55315689313180938)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55317136325180939)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>13
,p_column_id=>wwv_flow_imp.id(55316733786180939)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55318088584180939)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>14
,p_column_id=>wwv_flow_imp.id(55317719123180939)
,p_is_visible=>true
,p_is_frozen=>false
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(55404453869337270)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>14
,p_column_id=>wwv_flow_imp.id(52805965880951492)
,p_is_visible=>true
,p_is_frozen=>false
,p_sort_order=>1
,p_sort_direction=>'ASC'
,p_sort_nulls=>'LAST'
);
wwv_flow_imp_page.create_ig_report_column(
 p_id=>wwv_flow_imp.id(83703960234870283)
,p_view_id=>wwv_flow_imp.id(55303165058180856)
,p_display_seq=>2
,p_column_id=>wwv_flow_imp.id(75003743056727534)
,p_is_visible=>true
,p_is_frozen=>false
,p_width=>46
);
wwv_flow_imp_page.create_page_da_event(
 p_id=>wwv_flow_imp.id(52806837678951501)
,p_name=>'Set Local Defend Time'
,p_event_sequence=>10
,p_triggering_element_type=>'ITEM'
,p_triggering_element=>'NEXT_DEFEND'
,p_bind_type=>'bind'
,p_bind_event_type=>'change'
);
wwv_flow_imp_page.create_page_da_action(
 p_id=>wwv_flow_imp.id(52806909572951502)
,p_event_id=>wwv_flow_imp.id(52806837678951501)
,p_event_result=>'TRUE'
,p_action_sequence=>10
,p_execute_on_page_init=>'Y'
,p_action=>'NATIVE_SET_VALUE'
,p_affected_elements_type=>'COLUMN'
,p_affected_elements=>'NEXT_DEFEND_LOCAL'
,p_attribute_01=>'STATIC_ASSIGNMENT'
,p_attribute_02=>'zzFROM_TZ( CAST( :NEXT_DEFEND AS TIMESTAMP ), ''UTC'' ) AT LOCAL'
,p_attribute_09=>'Y'
,p_wait_for_result=>'Y'
);
wwv_flow_imp_page.create_page_process(
 p_id=>wwv_flow_imp.id(55318686905180942)
,p_process_sequence=>10
,p_process_point=>'AFTER_SUBMIT'
,p_region_id=>wwv_flow_imp.id(55302078716180848)
,p_process_type=>'NATIVE_IG_DML'
,p_process_name=>'Territories - Save Interactive Grid Data'
,p_attribute_01=>'REGION_SOURCE'
,p_attribute_05=>'Y'
,p_attribute_06=>'Y'
,p_attribute_08=>'Y'
,p_error_display_location=>'INLINE_IN_NOTIFICATION'
);
wwv_flow_imp.component_end;
end;
/