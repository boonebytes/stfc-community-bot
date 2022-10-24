prompt --application/shared_components/user_interface/templates/region/interactive_report_red_negatives
begin
--   Manifest
--     REGION TEMPLATE: INTERACTIVE_REPORT_RED_NEGATIVES
--   Manifest End
wwv_flow_imp.component_begin (
 p_version_yyyy_mm_dd=>'2022.04.12'
,p_release=>'22.1.0'
,p_default_workspace_id=>2100437598979157
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_imp_shared.create_plug_template(
 p_id=>wwv_flow_imp.id(38800300074900504)
,p_layout=>'TABLE'
,p_template=>wwv_flow_string.join(wwv_flow_t_varchar2(
'<div role="region" aria-label="#TITLE!ATTR#" id="#REGION_STATIC_ID#" #REGION_ATTRIBUTES# class="bb-red-neg t-IRR-region #REGION_CSS_CLASSES#">',
'  <h2 class="u-VisuallyHidden" id="#REGION_STATIC_ID#_heading" data-apex-heading>#TITLE#</h2>',
'#PREVIOUS##BODY##SUB_REGIONS##NEXT#',
'</div>'))
,p_page_plug_template_name=>'Interactive Report - Red Negatives'
,p_internal_name=>'INTERACTIVE_REPORT_RED_NEGATIVES'
,p_javascript_code_onload=>wwv_flow_string.join(wwv_flow_t_varchar2(
'$("div.bb-red-neg").bind("apexafterrefresh", function(){',
'    $("div.bb-red-neg table.a-IRR-table td").each(function(i){',
'        var lThis=$(this)',
'            , lVal=lThis.text().replace(/,/g, '''');',
'        if(lVal<0){',
'            lThis.css({"color":"red"});',
'        }else{',
'            //lThis.css({"color":"black"});',
'        }',
'    });',
'});'))
,p_theme_id=>42
,p_theme_class_id=>9
,p_default_label_alignment=>'RIGHT'
,p_default_field_alignment=>'LEFT'
,p_translate_this_template=>'N'
);
wwv_flow_imp_shared.create_plug_tmpl_display_point(
 p_id=>wwv_flow_imp.id(2115557923011893)
,p_plug_template_id=>wwv_flow_imp.id(38800300074900504)
,p_name=>'Region Body'
,p_placeholder=>'BODY'
,p_has_grid_support=>true
,p_has_region_support=>true
,p_has_item_support=>true
,p_has_button_support=>true
,p_glv_new_row=>true
);
wwv_flow_imp_shared.create_plug_tmpl_display_point(
 p_id=>wwv_flow_imp.id(2115629856011893)
,p_plug_template_id=>wwv_flow_imp.id(38800300074900504)
,p_name=>'Sub Regions'
,p_placeholder=>'SUB_REGIONS'
,p_has_grid_support=>true
,p_has_region_support=>true
,p_has_item_support=>false
,p_has_button_support=>false
,p_glv_new_row=>true
);
wwv_flow_imp_shared.create_plug_tmpl_display_point(
 p_id=>wwv_flow_imp.id(2115729200011893)
,p_plug_template_id=>wwv_flow_imp.id(38800300074900504)
,p_name=>'Next'
,p_placeholder=>'NEXT'
,p_has_grid_support=>false
,p_has_region_support=>false
,p_has_item_support=>false
,p_has_button_support=>true
,p_glv_new_row=>true
);
wwv_flow_imp_shared.create_plug_tmpl_display_point(
 p_id=>wwv_flow_imp.id(2115813048011893)
,p_plug_template_id=>wwv_flow_imp.id(38800300074900504)
,p_name=>'Previous'
,p_placeholder=>'PREVIOUS'
,p_has_grid_support=>false
,p_has_region_support=>false
,p_has_item_support=>false
,p_has_button_support=>true
,p_glv_new_row=>true
);
wwv_flow_imp.component_end;
end;
/
