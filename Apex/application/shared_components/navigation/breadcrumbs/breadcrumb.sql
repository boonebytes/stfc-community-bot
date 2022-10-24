prompt --application/shared_components/navigation/breadcrumbs/breadcrumb
begin
--   Manifest
--     MENU: Breadcrumb
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_menu(
 p_id=>wwv_flow_imp.id(54404663861995635)
,p_name=>'Breadcrumb'
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(35009768669221976)
,p_parent_id=>wwv_flow_imp.id(54404782778995639)
,p_short_name=>'Services'
,p_link=>'f?p=&APP_ID.:7:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>7
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(35010976373221982)
,p_parent_id=>wwv_flow_imp.id(35009768669221976)
,p_short_name=>'Service'
,p_link=>'f?p=&APP_ID.:8:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>8
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(36820039966204106)
,p_parent_id=>wwv_flow_imp.id(54404782778995639)
,p_short_name=>'Players'
,p_link=>'f?p=&APP_ID.:10:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>10
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(36821252731204115)
,p_parent_id=>wwv_flow_imp.id(36820039966204106)
,p_short_name=>'Edit Player'
,p_link=>'f?p=&APP_ID.:11:&SESSION.::&DEBUG.:::'
,p_page_id=>11
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(53410869213137861)
,p_parent_id=>wwv_flow_imp.id(54404782778995639)
,p_short_name=>'Officers'
,p_link=>'f?p=&APP_ID.:12:&SESSION.::&DEBUG.:::'
,p_page_id=>12
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(53412087250137866)
,p_parent_id=>wwv_flow_imp.id(53410869213137861)
,p_short_name=>'Officer'
,p_link=>'f?p=&APP_ID.:13:&SESSION.::&DEBUG.:::'
,p_page_id=>13
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(53502103834152576)
,p_short_name=>'Code Tables'
,p_link=>'f?p=&APP_ID.:14:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>14
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(54404782778995639)
,p_short_name=>'Home'
,p_link=>'f?p=&APP_ID.:1:&APP_SESSION.::&DEBUG.'
,p_page_id=>1
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(54634908814996175)
,p_short_name=>'Administration'
,p_link=>'f?p=&APP_ID.:10000:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>10000
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(54813369509062521)
,p_parent_id=>wwv_flow_imp.id(54404782778995639)
,p_short_name=>'Alliances'
,p_link=>'f?p=&APP_ID.:4:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>4
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(54814506612062526)
,p_parent_id=>wwv_flow_imp.id(54813369509062521)
,p_short_name=>'Edit Alliance'
,p_link=>'f?p=&APP_ID.:5:&SESSION.::&DEBUG.:::'
,p_page_id=>5
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55011193537112310)
,p_parent_id=>wwv_flow_imp.id(53502103834152576)
,p_short_name=>'Officer Groups'
,p_link=>'f?p=&APP_ID.:2:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>2
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55012360054112316)
,p_parent_id=>wwv_flow_imp.id(55011193537112310)
,p_short_name=>'Officer Group'
,p_link=>'f?p=&APP_ID.:3:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>3
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55211231442198260)
,p_parent_id=>wwv_flow_imp.id(53502103834152576)
,p_short_name=>'Officer Rarities'
,p_link=>'f?p=&APP_ID.:15:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>15
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55212403962198265)
,p_parent_id=>wwv_flow_imp.id(55211231442198260)
,p_short_name=>'Officer Rarity'
,p_link=>'f?p=&APP_ID.:16:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>16
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55301785917180845)
,p_parent_id=>wwv_flow_imp.id(54404782778995639)
,p_short_name=>'Territories'
,p_link=>'f?p=&APP_ID.:6:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>6
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55811172555546236)
,p_parent_id=>wwv_flow_imp.id(53502103834152576)
,p_short_name=>'Officer Factions'
,p_link=>'f?p=&APP_ID.:17:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>17
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(55812368632546241)
,p_parent_id=>wwv_flow_imp.id(55811172555546236)
,p_short_name=>'Officer Faction'
,p_link=>'f?p=&APP_ID.:18:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>18
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(81001481579844713)
,p_parent_id=>wwv_flow_imp.id(54813369509062521)
,p_short_name=>'Alliance Inventory'
,p_link=>'f?p=&APP_ID.:19:&SESSION.::&DEBUG.:::'
,p_page_id=>19
,p_security_scheme=>wwv_flow_imp.id(54555301792995935)
);
wwv_flow_imp_shared.create_menu_option(
 p_id=>wwv_flow_imp.id(83601373387815866)
,p_parent_id=>wwv_flow_imp.id(55301785917180845)
,p_short_name=>'Territory'
,p_link=>'f?p=&APP_ID.:21:&APP_SESSION.::&DEBUG.:::'
,p_page_id=>21
);
wwv_flow_imp.component_end;
end;
/
