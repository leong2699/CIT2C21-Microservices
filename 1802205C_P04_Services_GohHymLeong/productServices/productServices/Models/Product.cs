using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace productServices.Models
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        [Required]
        public int MallID { get; set; }
        public string Brand { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string Price { get; set; }
        public byte[] ProductImage { get; set; }
        public int Sold { get; set; }
    }
}