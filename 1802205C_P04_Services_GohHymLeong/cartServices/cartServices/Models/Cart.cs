using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cartServices.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }
        public string UserID { get; set; }
        public string ProductID { get; set; }
        public string Quantity { get; set; }
        public string Size { get; set; }
    }
}