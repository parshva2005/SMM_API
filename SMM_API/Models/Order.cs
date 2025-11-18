using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public string OrderNumber { get; set; } = null!;

    public string Status { get; set; } = null!;

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public string? Notes { get; set; }

    public string? PaymentMethod { get; set; }

    public int? SalesPersonId { get; set; }
    [JsonIgnore]
    public virtual Customer? Customer { get; set; }
    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    [JsonIgnore]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore]
    public virtual User? SalesPerson { get; set; }
    [JsonIgnore]
    public virtual User? User { get; set; }
}
