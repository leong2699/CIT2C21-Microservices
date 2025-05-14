using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace reviewServices.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        [Required]
        public int ProductID { get; set; }
        public int UserID { get; set; }
        public string Comment { get; set; }
        public string CommentDate { get; set; }
    }
}