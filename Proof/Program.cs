// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");


using Discord;
using Discord.WebSocket;

var clientConfig = new DiscordSocketConfig
{
    //ExclusiveBulkDelete = false,
    AlwaysDownloadUsers = true,
    GatewayIntents =  GatewayIntents.AllUnprivileged
                      | GatewayIntents.GuildMembers
                      | GatewayIntents.GuildMessages
                      | GatewayIntents.DirectMessages
};
clientConfig.GatewayIntents &= Discord.GatewayIntents.GuildMembers;
var client = new DiscordSocketClient(clientConfig);
