/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

BEGIN
    DBMS_NETWORK_ACL_ADMIN.append_host_ace (
            host       => 'discord.com',
            lower_port => 443,
            upper_port => 443,
            ace        => xs$ace_type(privilege_list => xs$name_list('http'),
                                      principal_name => 'APEX_220100',
                                      principal_type => xs_acl.ptype_db));
END;
/