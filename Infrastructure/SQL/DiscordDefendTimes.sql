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