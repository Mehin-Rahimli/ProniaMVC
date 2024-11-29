﻿using ProniaMVC.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace ProniaMVC.Areas.Admin.ViewModels
{
    public class CreateProductVM
    {
        public string Name { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public string SKU { get; set; }

        [Required]
        public int? CategoryId { get; set; }
        public List<int>? TagIds {get; set; } 

        public List<Category>? Categories { get; set; }
        public List<Tag>? Tags { get; set; }

    }
}
