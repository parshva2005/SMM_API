using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int? UserId { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string? Address { get; set; }

    public DateTime RegistrationDate { get; set; }

    public bool IsActive { get; set; }
    [JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    [JsonIgnore]
    public virtual User? User { get; set; }
}
