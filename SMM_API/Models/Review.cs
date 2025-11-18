using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public int? OrderId { get; set; }

    public int? Rating { get; set; }

    public string? Review1 { get; set; }

    public bool? IsOrdered { get; set; }
    [JsonIgnore]
    public virtual Order? Order { get; set; }
    [JsonIgnore]
    public virtual Product Product { get; set; } = null!;
    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}
