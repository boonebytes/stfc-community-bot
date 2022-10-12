BEGIN
    DBMS_NETWORK_ACL_ADMIN.CREATE_ACL (
            acl          => 'apex_access_acl.xml',
            description  => 'Permissions to access resource from APEX',
            principal    => 'APEX_APP',
            is_grant     => TRUE,
            privilege    => 'connect');
    DBMS_NETWORK_ACL_ADMIN.ASSIGN_ACL (
            acl          => 'apex_access_acl.xml',
            host         => '*.discord.com',
            lower_port   => 443,
            upper_port   => 443);
COMMIT;
END;
/