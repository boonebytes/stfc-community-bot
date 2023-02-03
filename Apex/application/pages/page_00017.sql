prompt --application/pages/page_00017
begin
--   Manifest
--     PAGE: 00017
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
 p_id=>17
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Officer Factions'
,p_alias=>'OFFICER-FACTIONS'
,p_step_title=>'Officer Factions'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_protection_level=>'C'
,p_page_component_map=>'18'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20220905210943'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(55808841144546214)
,p_plug_name=>'Officer Factions'
,p_region_template_options=>'#DEFAULT#'
,p_plug_template=>wwv_flow_imp.id(54460366006995735)
,p_plug_display_sequence=>10
,p_query_type=>'TABLE'
,p_query_table=>'CT_OFFICER_FACTION'
,p_include_rowid_column=>false
,p_plug_source_type=>'NATIVE_IR'
,p_prn_page_header=>'Officer Factions'
);
wwv_flow_imp_page.create_worksheet(
 p_id=>wwv_flow_imp.id(55809200165546214)
,p_name=>'Officer Factions'
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
,p_detail_link=>'f?p=&APP_ID.:18:&SESSION.::&DEBUG.:RP:P18_ID:\#ID#\'
,p_detail_link_text=>'<img src="#IMAGE_PREFIX#app_ui/img/icons/apex-edit-pencil.png" class="apex-edit-pencil" alt="Edit">'
,p_owner=>'BOONEBYTES'
,p_internal_uid=>55809200165546214
);
wwv_flow_imp_page.create_worksheet_column(
 p_id=>wwv_flow_imp.id(55809361910546217)
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
 p_id=>wwv_flow_imp.id(55809711372546229)
,p_db_column_name=>'NAME'
,p_display_order=>2
,p_column_identifier=>'B'
,p_column_label=>'Name'
,p_column_type=>'STRING'
,p_heading_alignment=>'LEFT'
,p_tz_dependent=>'N'
,p_use_as_row_header=>'N'
);
wwv_flow_imp_page.create_worksheet_rpt(
 p_id=>wwv_flow_imp.id(55842780232589386)
,p_application_user=>'APXWS_DEFAULT'
,p_report_seq=>10
,p_report_alias=>'558428'
,p_status=>'PUBLIC'
,p_is_default=>'Y'
,p_report_columns=>'ID:NAME'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(55811337447546239)
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
 p_id=>wwv_flow_imp.id(55812560150546241)
,p_button_sequence=>30
,p_button_plug_id=>wwv_flow_imp.id(55808841144546214)
,p_button_name=>'CREATE'
,p_button_action=>'REDIRECT_PAGE'
,p_button_template_options=>'#DEFAULT#'
,p_button_template_id=>wwv_flow_imp.id(54527310383995804)
,p_button_is_hot=>'Y'
,p_button_image_alt=>'Create'
,p_button_position=>'RIGHT_OF_IR_SEARCH_BAR'
,p_button_redirect_url=>'f?p=&APP_ID.:18:&SESSION.::&DEBUG.:18'
,p_security_scheme=>wwv_flow_imp.id(54555485978995935)
);
wwv_flow_imp.component_end;
end;
/
