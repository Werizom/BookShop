﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models.Models;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int OrderHeaderId { get; set; }

    [ForeignKey("OrderHeaderId")]
    public OrderHeader OrderHeader { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    public int Count { get; set; }
    public double PricePerCount { get; set; }
}
