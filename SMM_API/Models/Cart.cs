using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int ProductQuantity { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public bool IsCheckedOut { get; set; }

    public DateTime? CheckoutDate { get; set; }

    public string? SessionId { get; set; }
    [JsonIgnore]
    public virtual Product? Product { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
}
