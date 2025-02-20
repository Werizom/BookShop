﻿using BookShop.Models.Models;

namespace BookShop.Models.ViewModels;

public class InquiryViewModel
{
    public InquiryHeader InquiryHeader {  get; set; }
    public IEnumerable<InquiryDetail> InquiryDetail { get; set; }
}
