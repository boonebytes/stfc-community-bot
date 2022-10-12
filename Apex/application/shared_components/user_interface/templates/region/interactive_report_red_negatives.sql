prompt --application/shared_components/user_interface/templates/region/interactive_report_red_negatives
begin
--   Manifest
--     REGION TEMPLATE: INTERACTIVE_REPORT_RED_NEGATIVES
--   Manifest End
wwv_flow_api.component_begin (
 p_version_yyyy_mm_dd=>'2021.04.15'
,p_release=>'21.1.0'
,p_default_workspace_id=>18900386187764698
,p_default_application_id=>109
,p_default_id_offset=>30400279703097675
,p_default_owner=>'STFC'
);
wwv_flow_api.create_plug_template(
 p_id=>wwv_flow_api.id(38800300074900504)
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
);
wwv_flow_api.component_end;
end;
/
