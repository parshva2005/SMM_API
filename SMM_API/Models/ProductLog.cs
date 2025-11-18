using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class ProductLog
{
    public int ProductLogId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public DateTime LogDate { get; set; }

    public string LogType { get; set; } = null!;

    public string? LogDetails { get; set; }

    public int QuantityChange { get; set; }

    public string? Reference { get; set; }

    public string? Notes { get; set; }
    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
