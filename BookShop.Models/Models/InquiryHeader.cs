﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShop.Models.Models;

public class InquiryHeader
{
    [Key]
    public int Id { get; set; }

    public string ApplicationUserId { get; set; }

    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser User { get; set; }

    public DateTime InquiryDate { get; set; }

    [Required]
    public string FullName { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Email { get; set; }

}
