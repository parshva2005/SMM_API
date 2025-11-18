using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class UserLog
{
    public int UserLogId { get; set; }

    public int UserId { get; set; }

    public DateTime LogDate { get; set; }

    public string LogType { get; set; } = null!;

    public string? LogDetails { get; set; }

    public string? IpAddress { get; set; }

    public int RoleId { get; set; }
    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
