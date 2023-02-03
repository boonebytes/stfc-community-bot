prompt --application/user_interfaces
begin
--   Manifest
--     USER INTERFACES: 109
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_user_interface(
 p_id=>wwv_flow_imp.id(54551875302995858)
,p_ui_type_name=>'DESKTOP'
,p_display_name=>'Desktop'
,p_display_seq=>10
,p_use_auto_detect=>false
,p_is_default=>true
,p_theme_id=>42
,p_home_url=>'f?p=&APP_ID.:1:&SESSION.'
,p_login_url=>'f?p=&APP_ID.:9998:&SESSION.'
,p_theme_style_by_user_pref=>false
,p_built_with_love=>false
,p_global_page_id=>0
,p_navigation_list_id=>wwv_flow_imp.id(54405142762995641)
,p_navigation_list_position=>'SIDE'
,p_navigation_list_template_id=>wwv_flow_imp.id(54512083043995782)
,p_nav_list_template_options=>'#DEFAULT#:js-defaultCollapsed:js-navCollapsed--hidden:t-TreeNav--styleA'
,p_css_file_urls=>'#APP_IMAGES#app-icon.css?version=#APP_VERSION#'
,p_javascript_file_urls=>'#IMAGE_PREFIX#libraries/jquery/3.6.0/jquery-3.6.0.min.js'
,p_nav_bar_type=>'LIST'
,p_nav_bar_list_id=>wwv_flow_imp.id(54551536994995857)
,p_nav_bar_list_template_id=>wwv_flow_imp.id(54514976322995785)
,p_nav_bar_template_options=>'#DEFAULT#'
);
wwv_flow_imp.component_end;
end;
/
