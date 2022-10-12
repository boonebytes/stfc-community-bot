# STFC World Community Bot

This project was created by Boonebytes. Please reach out to him if you
would like to make a derivative of this code or if you find it useful.

## Important KIVs
- ***Never*** provide the bot's token or database credentials to anyone,
  and do not check in those values into the source code.
  - If this happens, reset the credentials in Discord and/or change the
    database password immediately.
## To-Do

### Implementation / Road Map

- Add functionality for alliances to quietly indicate interest in a
zone without disclosing that interest to other alliances.
  - Would this be disclosed to allied/friendly alliances?

## Deployment Keep-In-View

### Discord Server

- The bot identifies the alliance by the Discord server. This means
that if anyone can run the commands on your server (guests included),
the information presented will be based on your alliance's diplomacy
  - If possible, restrict command usage in Server Settings - Integration
  to members and/or leadership only
- Servers, Roles, and Channels are identified by the bot using Discord's
internal identifiers.
  - If a role or channel is deleted and is referenced  by the bot (ie.
  stored as a configuration setting within the bot), then it is deleted
  in Discord, the bot won't pick up on a new item with the same name.
  In these instances, please provide the new IDs to the bot.

#### Discord Permissions

- Server invite settings (from the Discord Developer Portal URL Generator):
  - Scopes:
    - bot
    - applications.commands
  - Bot Permissions:
    - Change Nickname
    - Read Messages / View Channels
    - Send Messages
    - Read Message History
    - Mention Everyone (unless defined explicitly on the channels)
  - Sample URL parameters:
    - permissions=67308544
    - scope=bot%20applications.commands
  
- Territory Schedule channel:
  - Send messages
  - Mention everyone / all roles
  - Manage messages
  - Read message history

- Other channels where the commands may be used:
  - Send messages
  - Read message history


### Command Usage

- The */schedule refresh* and */schedule today* commands are best used
early in the day.
  - Running these commands after a defend has started may cause earlier
  defends to not appear on the "*today* listing.
- The */admin reload* command is resource-intensive and should only be
executed by the bot owner on an as-needed basis.
  - The intent behind this command is to load any data changes from the
  database into the bot's running code. This includes unscheduling and
  rescheduling all defend and assist reminders.
  - This command is executed nightly automatically.
- Do not use */zone set* to assign an alliance to a territory unless
that alliance actually owns the territory.
  - Running this command on an unclaimed territory may show one's hand,
  or be overwritten by a competing alliance.
  - Running this command on territory owned by someone else will break
  any scheduling or defend reminders for that alliance.
  - This command is limited to admin-use only to ensure its use is fair
  and not used to negate reminders and force ownership of territory.

### Self-Hosted

The bare minimum requirements to self-host this bot:
- An Oracle database installation with a dedicated (non-admin) user
account for the bot.
- A Discord Developer account with a generated Bot token.
- Ideally hosted in the cloud on a Linux server with Docker.
- A .NET Core development environment, Oracle database tool, etc.

Steps to deploy:

1. Create a schema to hold the data tables. This should not be
the same as the bot's database user account.
2. Create a database account to be used by the bot. It should
have select/insert/update/delete grants on all the tables identified
or referenced in Entity Framework. The Entity Framework database
generation script could be used as a reference point.
3. Configure a development environment with a connection to an
Oracle database. This is required for the Entity Framework commands 
to work.
4. Set the database connection info in BotContextFactory. This should
***not*** be checked into the database; it's only required for the
Entity Framework commands.
5. Run the Entity Framework commands to generate a SQL script to
create the latest version of the database. (For upgrades, only use
the Entity Framework migration scripts)
6. Apply the migration scripts to the data schema.
7. Load the database with a list of zones, neighbours, alliance info,
diplomacy details, zone ownership, etc.
8. Generate a Discord Invite URL for the bot with the required
permissions. (TODO: Identify these permissions)
9. Invite the bot to the Discord server. Note, this requires the
Manage Server permission.
10. Enable Discord's Developer Mode.
11. Copy the Discord server's ID into the Alliance table's Guild ID.
12. If desired, create a channel for the defense schedule. Copy this
channel's ID into the relevant field in the Alliance table as well.
Then set the time to update the schedule daily.
13. In the DiscordBot project, update appsettings.json with the
generated bot token, database connection information, etc. Again,
***never commit the bot token, database credentials, etc. into the
source code management solution*** (ie. git). If this happens, change
reset the bot's token in Discord and/or update the database user
account in Oracle.
14. Run the DiscordBot project. Review the output / log files to ensure
it's able to connect. After a brief delay, it should auto-populate each
zone's next defend time and each alliance's next schedule-post time.
15. Try running some of the commands. Ensure contender data seems
correct based on the alliance associated to the Discord server.

Once the bot's token and database have been initialized and tested
in a  non-production environment, copy

16. Remove the database credentials from the BotContextFactory.
17. Copy the bot's code to a production server.
18. Ensure appsettings.json has the correct bot token and database
credentials.
19. Run ```docker-compose build```. Note: This command must be executed
whenever the source code is updated.
20. Run ```docker-compose up -d``` to start the bot instance.
21. Review the docker logs for any issues.

## History

- Started as a way to post a weekly territory defend schedule in LEGN 
on Server 61 using local timestamps. Nearby threats were manually
specified in the database records.
- First versions read data from CSV files that had to be updated by-hand;
next generation used the MySQL database engine. This was all eventually
migrated to Oracle.
- Eventually added the connections between zones to the database and
instructed the bot to identify and list nearby threats.
- Added code to identify diplomatic statuses and groups of territory
defenses (ie. TDL)
- Added the bot to the TDL Discord.
- Added defend reminders for the local alliance.
- Later added assist defend reminders for a specific Discord role.
- Added the territory service cost summary command.
- Restructured slash commands to have differing root commands. This
gives more flexibility to the Discord server admins around who can
execute commands.
- Moved the threats / contenders out of the database record and
populated it dynamically based on the alliance that's asking.
  - Example: If A, B, and C are allied and D is not, and if B's
  territory border's C, then...
    - A asking for B's territory will not list C as a contender.
    - D asking for B's territory will list C as a contender.

## External Sources / APIs

- Discord Developer APIs
- Optional: Oracle APEX (for a GUI front-end, both for some bot data and
for access to data not exposed via the bot commands)

## Referenced Components / Technologies

The below are listed for internal reference purposes only. For further
information or a copy of the respective licensing terms, please refer
to the third-party library's website.

These components are referenced via Nuget packages. As the actual files
are not included in this repository, it is understood that they should
not be referenced by this project's licensing material.

| Component                            | License                  |
|:-------------------------------------|:-------------------------|
| [Discord.NET]                        | The MIT License (MIT)    |
| [Docker]                             | Apache License v2.0      |
| [MediatR] v10                        | Apache License v2.0      |
| Microsoft C# .NET Core v6            | The MIT License (MIT)    |
| Microsoft Entity Framework Core v3.1 | The MIT License (MIT)    | 
| [Newtonsoft.Json] v13                | The MIT License (MIT)    |
| Oracle Managed Data Access Core      | Oracle Free Use          |
| Oracle Entity Framework Core v3.21   | Oracle Free Use          |
| [Quartz.NET]                         | Apache License v2.0      |
| [Entity Framework Plus]              | The MIT License (MIT)    | 

[Discord.NET]:https://discordnet.dev/
[Docker]:https://www.docker.com/
[MediatR]:https://github.com/jbogard/MediatR
[Newtonsoft.Json]:https://www.newtonsoft.com/json
[Quartz.NET]:https://www.quartz-scheduler.net/
[Entity Framework Plus]:https://entityframework-plus.net/
