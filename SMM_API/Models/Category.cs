using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public int? UserId { get; set; }

    public string CategoryName { get; set; } = null!;

    public DateTime? CreationDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public bool? IsRemoved { get; set; }
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    [JsonIgnore]
    public virtual User? User { get; set; }
}
