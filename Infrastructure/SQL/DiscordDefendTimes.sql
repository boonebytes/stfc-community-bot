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

select concat(
    `alliances`.`Acronym`,
    '/',
    `zones`.`Name`,
    '(',
    `zones`.`Level`,
    '^): ',
    ' <t:', truncate(unix_timestamp(NextDefend), 0), ':t> local / ',
    date_format(
        convert_tz(
            `zones`.`NextDefend`,
            'UTC',
            'EST'),'%a %l:%i %p'),
    ' EST [',
    coalesce(`zones`.`Threats`,'None'),
    ']'
) AS `Message` from (
    `alliances`
    join `zones` on(`zones`.`OwnerId` = `alliances`.`Id`)
)
where `alliances`.`Acronym` not in ('BBPG','KBAB')
order by `zones`.`NextDefend`;