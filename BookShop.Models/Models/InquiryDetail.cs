﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models.Models;

public class InquiryDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int InquiryHeaderId { get; set; }

    [ForeignKey(nameof(InquiryHeaderId))]
    public InquiryHeader InquiryHeader { get; set; }

    [Required]
    public int ProductId { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
}
