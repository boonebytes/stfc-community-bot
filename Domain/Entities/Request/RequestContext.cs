namespace DiscordBot.Domain.Entities.Request;

public class RequestContext
{
    private long? _allianceId;

    public void Init(long? allianceId)
    {
        _allianceId = allianceId;
    }

    public long? GetAllianceId()
    {
        return _allianceId;
    }
}