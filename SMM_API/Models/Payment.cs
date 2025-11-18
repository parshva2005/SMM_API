using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Method { get; set; } = null!;

    public DateTime PaymentDate { get; set; }

    public string? TransactionId { get; set; }

    public string Status { get; set; } = null!;
    [JsonIgnore]
    public virtual Order Order { get; set; } = null!;
}
