using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace storeServices.Models
{
    public class Store
    {
        [Key]
        public int StoreID { get; set; }
        [Required]
        public int MallID { get; set; }
        public string StoreName { get; set; }
        public byte[] CoverImage { get; set; }
        public string OperatingHours { get; set; }
        public string ContactNumber { get; set; }
        public string StoreDescription { get; set; }
        public string Level { get; set; }


    }
}