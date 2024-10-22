﻿using Mailo.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mailo.Models
{
    [NotMapped]
    public class Product
    {
        public int ID { get; set; }
		[MaxLength(30)]
		public string Name { get; set; }
		public string? Description { get; set; }

        public Product_Categories Category { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime AdditionDate { get; set; } = DateTime.Now;
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than 0.")]

        public decimal? Discount { get; set; } = 0;
        [Range(0, int.MaxValue, ErrorMessage = "The value must be greater than 0.")]
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        [Range(0, float.MaxValue, ErrorMessage = "The value must be greater than 0.")]
        public decimal Price { get; set; }
        public ICollection<OrderProduct>? OrderProducts { get; set; }
        public ICollection<Wishlist>? wishlists { get; set; }
       
        [NotMapped]
        public IFormFile? clientFile { get; set; }
        public byte[]? dbImage { get; set; }
        [NotMapped]
        public string? imageSrc
        {
            get
            {
                if (dbImage != null)
                {
                    string base64String = Convert.ToBase64String(dbImage, 0, dbImage.Length);
                    return "data:image/jpg;base64," + base64String;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }  
}
