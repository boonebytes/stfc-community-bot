using System;
using System.Collections.Generic;
using DiscordBot.Infrastructure.Entities;
using DiscordBot.Domain.Seedwork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace DiscordBot.Infrastructure.DTOs;

public class AuditEntry
{
    public EntityEntry Entry { get; }
    public string UserId { get; set; }
    public string TableName { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new List<string>();
    
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }
    
    public Audit ToAudit()
    {
        var audit = new Audit();
        audit.UserId = UserId;
        audit.Type = AuditType.ToString();
        audit.TableName = TableName;
        audit.DateTime = DateTime.Now.ToUniversalTime();
        audit.PrimaryKey = JsonConvert.SerializeObject(KeyValues);
        audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
        audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
        audit.AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns);
        return audit;
    }
}