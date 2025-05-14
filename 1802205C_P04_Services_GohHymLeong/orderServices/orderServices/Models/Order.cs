using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace orderServices.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
        public string TotalPrice { get; set; }
        public string Payer { get; set; }
        public string UserID { get; set; }
    }
}