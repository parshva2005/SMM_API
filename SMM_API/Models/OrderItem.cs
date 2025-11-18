using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int UserId { get; set; }

    public decimal QuantityWisePrice { get; set; }

    public int ProductQuantity { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ModifyDate { get; set; }
    [JsonIgnore]
    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
}
