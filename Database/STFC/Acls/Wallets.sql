BEGIN
    dbms_network_acl_admin.append_wallet_ace(
            wallet_path => 'file:/opt/oracle/ext/owm/wallets/oracle/apex-wallet/',
            ace         =>  xs$ace_type(privilege_list => xs$name_list('use_client_certificates', 'use_passwords'),
                                        principal_name => 'APEX_APP',
                                        principal_type => xs_acl.ptype_db));
COMMIT;
end;
/