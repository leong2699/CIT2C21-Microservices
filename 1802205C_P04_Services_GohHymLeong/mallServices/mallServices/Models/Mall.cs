using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace mallServices.Models
{
    public class Mall
    {
        [Key]
        public int MallID { get; set; }
        [Required]
        public string MallName { get; set; }
        public byte[] MallImage { get; set; }
        public string Description { get; set; }
        public string CarparkCode { get; set; }
        public string Location { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string OpeningHours { get; set; }
        public string OpeningDate { get; set; }

    }
}