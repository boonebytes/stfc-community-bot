prompt --application/pages/page_00004
begin
--   Manifest
--     PAGE: 00004
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
 p_id=>4
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Alliances'
,p_alias=>'ALLIANCES'
,p_step_title=>'Alliances'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_protection_level=>'C'
,p_page_component_map=>'18'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220905210943'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(54810535471062504)
,p_plug_name=>'Alliances'
,p_region_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54460366006995735)
,p_plug_display_sequence=>10
,p_query_type=>'TABLE'
,p_query_table=>'ALLIANCES'
,p_query_where=>'nvl(v(''USER_ALLIANCE''), ID) = ID'
,p_include_rowid_column=>false
,p_plug_source_type=>'NATIVE_IR'
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_prn_page_header=>'Alliances'
);
wwv_flow_imp_page.create_worksheet(
 p_id=>wwv_flow_imp.id(54810947810062505)
,p_name=>'Alliances'
,p_max_row_count_message=>'The maximum row count for this report is #MAX_ROW_COUNT# rows.  Please apply a filter to reduce the number of records in your query.'
,p_no_data_found_message=>'No data found.'
,p_pagination_type=>'ROWS_X_TO_Y'
,p_pagination_display_pos=>'BOTTOM_RIGHT'
,p_show_display_row_count=>'Y'
,p_report_list_mode=>'TABS'
,p_lazy_loading=>false
,p_show_detail_link=>'C'
,p_show_rows_per_page=>'N'
,p_download_formats=>'CSV:HTML:XLSX:PDF'
,p_enable_mail_download=>'Y'
,p_detail_link=>'f?p=&APP_ID.:5:&SESSION.::&DEBUG.:RP:P5_ID:\#ID#\'
,p_detail_link_text=>'<img src="#IMAGE_PREFIX#app_ui/img/icons/apex-edit-pencil.png" class="apex-edit-pencil" alt="Edit">'
,p_owner=>'BOONEBYTES'
,p_internal_uid=>24410668106964830
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(54811002594062507)
,p_db_column_name=>'ID'
,p_display_order=>1
,p_column_identifier=>'A'
,p_column_label=>'Id'
,p_column_type=>'NUMBER'
,p_display_text_as=>'HIDDEN'
,p_tz_dependent=>'N'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(54811447937062516)
,p_db_column_name=>'NAME'
,p_display_order=>2
,p_column_identifier=>'B'
,p_column_label=>'Name'
,p_column_type=>'STRING'
,p_heading_alignment=>'LEFT'
,p_tz_dependent=>'N'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(54811794801062516)
,p_db_column_name=>'ACRONYM'
,p_display_order=>3
,p_column_identifier=>'C'
,p_column_label=>'Acronym'
,p_column_type=>'STRING'
,p_heading_alignment=>'LEFT'
,p_tz_dependent=>'N'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(54812226683062517)
,p_db_column_name=>'ALLIANCE_GROUP_ID'
,p_display_order=>4
,p_column_identifier=>'D'
,p_column_label=>'Alliance Group'
,p_column_type=>'NUMBER'
,p_display_text_as=>'LOV_ESCAPE_SC'
,p_column_alignment=>'CENTER'
,p_rpt_named_lov=>wwv_flow_imp.id(54824985257083259)
,p_rpt_show_filter_lov=>'1'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22407669166853837)
,p_db_column_name=>'GUILD_ID'
,p_display_order=>14
,p_column_identifier=>'E'
,p_column_label=>'Guild Id'
,p_column_type=>'NUMBER'
,p_display_text_as=>'HIDDEN'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22407753921853838)
,p_db_column_name=>'DEFEND_SCHEDULE_POST_CHANNEL'
,p_display_order=>24
,p_column_identifier=>'F'
,p_column_label=>'Defend Schedule Post Channel'
,p_column_type=>'NUMBER'
,p_display_text_as=>'HIDDEN'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22407841604853839)
,p_db_column_name=>'DEFEND_SCHEDULE_POST_TIME'
,p_display_order=>34
,p_column_identifier=>'G'
,p_column_label=>'Defend Schedule Post Time'
,p_column_type=>'STRING'
,p_display_text_as=>'HIDDEN'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22407974328853840)
,p_db_column_name=>'DEFEND_BROADCAST_LEAD_TIME'
,p_display_order=>44
,p_column_identifier=>'H'
,p_column_label=>'Defend Broadcast Lead Time'
,p_column_type=>'NUMBER'
,p_display_text_as=>'HIDDEN'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22408087243853841)
,p_db_column_name=>'NEXT_SCHEDULED_POST'
,p_display_order=>54
,p_column_identifier=>'I'
,p_column_label=>'Next Scheduled Post'
,p_column_type=>'DATE'
,p_display_text_as=>'HIDDEN'
,p_tz_dependent=>'N'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22408151076853842)
,p_db_column_name=>'MODIFIED_BY'
,p_display_order=>64
,p_column_identifier=>'J'
,p_column_label=>'Modified By'
,p_column_type=>'STRING'
,p_display_text_as=>'HIDDEN'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22408235499853843)
,p_db_column_name=>'MODIFIED_DATE'
,p_display_order=>74
,p_column_identifier=>'K'
,p_column_label=>'Modified Date'
,p_column_type=>'DATE'
,p_display_text_as=>'HIDDEN'
,p_tz_dependent=>'N'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(22408306555853844)
,p_db_column_name=>'STATUS'
,p_display_order=>84
,p_column_identifier=>'L'
,p_column_label=>'Status'
,p_column_type=>'STRING'
,p_display_text_as=>'LOV_ESCAPE_SC'
,p_rpt_named_lov=>wwv_flow_imp.id(33005320941237571)
,p_rpt_show_filter_lov=>'1'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_rpt(
 p_id=>wwv_flow_imp.id(55000382090147577)
,p_application_user=>'APXWS_DEFAULT'
,p_report_seq=>10
,p_report_alias=>'246002'
,p_status=>'PUBLIC'
,p_is_default=>'Y'
,p_report_columns=>'ID:NAME:ACRONYM:ALLIANCE_GROUP_ID:GUILD_ID:DEFEND_SCHEDULE_POST_CHANNEL:DEFEND_SCHEDULE_POST_TIME:DEFEND_BROADCAST_LEAD_TIME:NEXT_SCHEDULED_POST:MODIFIED_BY:MODIFIED_DATE:STATUS'
,p_sort_column_1=>'ACRONYM'
,p_sort_direction_1=>'ASC'
);
wwv_flow_imp_page.create_worksheet_condition(
 p_id=>wwv_flow_imp.id(33102691719251351)
,p_report_id=>wwv_flow_imp.id(55000382090147577)
,p_condition_type=>'FILTER'
,p_allow_delete=>'Y'
,p_column_name=>'STATUS'
,p_operator=>'='
,p_expr=>'Active'
,p_condition_sql=>'"STATUS" = #APXWS_EXPR#'
,p_condition_display=>'#APXWS_COL_NAME# = ''Active''  '
,p_enabled=>'Y'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(54813554047062524)
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
wwv_flow_imp_page.create_page_button(
 p_id=>wwv_flow_imp.id(54814703583062526)
,p_button_sequence=>30
,p_button_plug_id=>wwv_flow_imp.id(54810535471062504)
,p_button_name=>'CREATE'
,p_button_action=>'REDIRECT_PAGE'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_is_hot=>'Y'
,p_button_image_alt=>'Create'
,p_button_position=>'RIGHT_OF_IR_SEARCH_BAR'
,p_button_redirect_url=>'f?p=&APP_ID.:5:&SESSION.::&DEBUG.:5'
,p_security_scheme=>wwv_flow_imp.id(54555301792995935)
);
wwv_flow_imp.component_end;
end;
/
