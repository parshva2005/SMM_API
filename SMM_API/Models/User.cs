using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SMM_API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string UserName { get; set; } = null!;
    
    public string UserAddress { get; set; } = null!;

    public string UserMobileNumber { get; set; } = null!;

    public string UserEmailAddress { get; set; } = null!;

    public int RoleId { get; set; }

    public DateTime? CreationDate { get; set; }

    public DateTime? ModifyDate { get; set; }

    public bool? IsActive { get; set; }

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;

    public DateTime? LastLoginDate { get; set; }
    [BindNever]
    public string? FilePath { get; set; }
    [NotMapped]
    [JsonIgnore]
    public IFormFile? File { get; set; }
    [JsonIgnore]
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    [JsonIgnore]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    [JsonIgnore]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    [JsonIgnore]
    public virtual ICollection<Order> OrderSalesPeople { get; set; } = new List<Order>();
    [JsonIgnore]
    public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();
    [JsonIgnore]
    public virtual ICollection<ProductLog> ProductLogs { get; set; } = new List<ProductLog>();
    [JsonIgnore]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore]
    public virtual Role? Role { get; set; }
    [JsonIgnore]
    public virtual ICollection<UserLog> UserLogs { get; set; } = new List<UserLog>();
}
