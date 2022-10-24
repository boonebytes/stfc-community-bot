prompt --application/pages/page_00022
begin
--   Manifest
--     PAGE: 00022
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
 p_id=>22
,p_user_interface_id=>wwv_flow_imp.id(54551875302995858)
,p_name=>'Privacy Policy'
,p_alias=>'PRIVACY-POLICY'
,p_step_title=>'Privacy Policy'
,p_autocomplete_on_off=>'OFF'
,p_page_template_options=>'#DEFAULT#'
,p_page_is_public_y_n=>'Y'
,p_protection_level=>'C'
,p_page_component_map=>'11'
,p_last_updated_by=>'BOONEBYTES'
,p_last_upd_yyyymmddhh24miss=>'20221017212939'
);
wwv_flow_imp_page.create_page_plug(
 p_id=>wwv_flow_imp.id(2314835883727731)
,p_plug_name=>'Privacy Policy'
,p_region_template_options=>'#DEFAULT#:t-Region--scrollBody'
,p_plug_template=>wwv_flow_imp.id(54462243687995736)
,p_plug_display_sequence=>10
,p_include_in_reg_disp_sel_yn=>'Y'
,p_plug_source=>wwv_flow_string.join(wwv_flow_t_varchar2(
'<p><i>Last Updated: October 17, 2022</i></p>',
'',
'<p>By using the STFC World Community Bot or website, you agree that you have',
'    read and agree to this policy.</p>',
'',
'<p>This is our "Privacy Policy" which sets out the policy which governs our',
'    use of information you provide in connection with the STFC World bot and website.',
'    The terms "you", "your", and "end-users" refer to all individuals or entities accessing the bot/website.',
'    The terms "we", "us", and "our" refer to STFC World and "bot" refers to the Discord bot itself.',
'    STFC refers to Star Trek Fleet Command, a mobile game from Scopely.</p>',
'',
'<p>We may update this Privacy Policy from time to time. Changes in our Privacy Policy will be',
'    effective immediately. By using the website and/or bot, you consent to the collection, use,',
'    and transfer of your information in accordance with this Privacy Policy. If you do not agree',
'    to this Privacy Policy, please do not use this website or our bot. You may revoke your consent',
'    at any time by contacting the bot developer.</p>',
'',
'<h2>Privacy Statement</h2>',
'',
'<p>Information we collect about our direct users and why:</p>',
'<ul>',
'    <li>Discord User IDs and associated usernames - This information is used to connect your Discord',
'        account to an application profile.</li>',
'    <li>Associated Discord servers - This information is used to determine associated servers where',
'        you may have sufficient rights to make administrative changes to the bot''s local',
'        configuration.</li>',
'    <li>Non-Personally Identifiable Information - We may automatically collect certain technical',
'        information from your computer such as your Internet service provider, your IP address,',
'        your browser type, your operating system, the pages viewed, the pages viewed immediately',
'        before and after accessing the website, and the search terms entered to get to our site.',
'        This information allows us to improve, customize, and secure our services. We and our',
'        service providers may collect this information using "cookies" or similar technologies.</li>',
'</ul>',
'',
'<p>We do not collect email adresses, nor do we attempt to associate any data provided with true',
'    identities.</p>',
'',
'<p>This privacy policy does not cover information provided by end-users with respect to their alliance',
'    membership, inventory, etc.</p>',
'',
'',
'<h2>The information we collect and how we collect it</h2>',
'',
'<ul>',
'    <li>Discord User IDs, associated usernames, and connected servers are provided from Discord when',
'        you log in. The Discord User ID may be keyed prior to the initial login as an attempt to',
'        initialize / register your account for first use.</li>',
'    <li>Alliance and member information, as well as territory details, are retrieved from the STFC game',
'        using the standard front-end provided to all users of the STFC game. This data is then keyed',
'        into this website by other end-users.</li>',
'    <li>Non-Personally Identifiable Information, such as your IP address, web browser, and operating',
'        system, are gathered automatically by the infrastructure while you access our website. This',
'        information is retained in log files and may be referenced to identify any potential',
'        functionality or security issues. Some of this information is collected by the use of',
'        web browser cookies, especially to associate a web browser session to a server session.</li>',
'</ul>',
'',
'<p>The technologies we use for this automatic data collection may include:</p>',
'<ol>',
'    <li>Cookies (or browser cookies). You may refuse to accept browser cookies by activating the',
'        appropriate setting on your browser. However, if you select this setting you may be unable',
'        to access certain parts of our website. Unless you have adjusted your browser setting so',
'        that it will refuse cookies, our system will issue cookies when you direct your browser',
'        to our website.</li>',
'</ol>',
'',
'<h2>Third-party websites</h2>',
'',
'<p>This website or interactions completed via the bot may contain links to other third-party websites.',
'    These websites are not under our control, and we are not responsible for the privacy practices',
'    or contents of any such linked website or any link contained within a linked website. We recommend',
'    that you familiarize yourself with the privacy policies of any third parties.</p>',
'',
'<p>By nature, the bot utilizes services provided by Discord. For the purposes of this document, Discord',
'    is considered a third-party. Information provided to our bot through the Discord APIs will fall',
'    under this privacy policy, but we cannot be accountable for how the information may be used by',
'    Discord. Please refer to Discord''s website for details on how the information is handled on their',
'    end.</p>',
'',
'',
'<h2>The way we use information</h2>',
'',
'<p>Information provided to the website or the bot may be used for the following purposes:</p>',
'<ul>',
'    <li>To authenticate and authorize access to the website / bot.</li>',
'    <li>To provide relevant information related to your in-game alliance. For instance, the bot may',
'        use the information about your alliance to provide a territory defense schedule in a Discord',
'        channel that you identify, presenting only information that is of interest to your alliance.',
'        Based on the options selected, it may also "ping" the Discord roles for certain events.</li>',
'    <li>To aid in reporting for alliance leadership. These reports are based off of data provided',
'        by the alliance leadership, such as membership and territory inventory.</li>',
'</ul>',
'',
'<p>Under certain circumstances, we may attempt to reach out to you via private messages on Discord.',
'    These messages will be sent from a human using a standard Discord end-user account. Should this',
'    fail, and depending on the urgency of the communication, the bot may be utilized to deliver the',
'    message in a secure manner. Such a delivery mechanism is not yet coded for the bot, and may vary',
'    based on the bot''s permissions on your Discord server, if private messages are enabled, and if',
'    the communication from the bot is determined urgent in nature. Please note: The bot will never',
'    be used to send advertising messages.</p>',
'',
'<p>We may also use or disclose information to resolve disputes, investigate problems or enforce our',
'    Terms of Use. At times, we may review status or activity of multiple users to do so. We may',
'    disclose or access information whenever we believe in good faith that the law so requires or',
'    if we otherwise consider it necessary to do so to maintain service and improve our services.',
'    We use your IP address to help diagnose problems with our server and to manage our website.</p>',
'',
'',
'<h2>Disclosure of information</h2>',
'',
'<p>Information may be disclosed for one of the following reasons:</p>',
'<ul>',
'    <li>To comply with any court order, law or legal process, including to respond to any',
'        government or regulatory request.</li>',
'    <li>To enforce or apply our Terms of Use and other agreements.</li>',
'    <li>If we believe disclosure is necessary or appropriate to protect the rights, property, or',
'        safety of STFC World, our participants, or others. This includes exchanging information with',
'        other organizations for the purposes of fraud protection and security analysis.</li>',
'</ul>',
'',
'',
'<h2>Contact Information</h2>',
'',
'<p>To ask questions or comment about this Privacy Policy and our privacy practices,',
'    please contact us via Discord.</p>'))
,p_plug_query_options=>'DERIVED_REPORT_COLUMNS'
,p_attribute_01=>'N'
,p_attribute_02=>'HTML'
);
wwv_flow_imp.component_end;
end;
/
