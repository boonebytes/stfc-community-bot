namespace Web.Models;

public class DiscordServer
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public bool? Owner { get; set; }
    public ulong Permissions { get; set; }
    public bool? HasBot { get; set; }

    public DiscordServer(ulong id, string name)
    {
        this.Id = id;
        this.Name = name;
    }

    public bool HasAdmin()
    {
        var adminPermission = (ulong)Discord.GuildPermission.Administrator;
        return ((Permissions & adminPermission) == adminPermission);
    }
}