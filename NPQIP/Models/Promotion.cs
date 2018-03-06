using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class Promotion
    {
        public Promotion()
        {
            status = "Current";
        }

        [Key]
        public int PromotionID { get; set; }

        public int UserID { get; set; }

        public string Name { get; set; }

        public NPQIP.Enums.userRoles oldRole { get; set; }

        public NPQIP.Enums.userRoles newRole { get; set; } 

        public string url { get; set; }

        public string status {get;set;}

        public DateTime promotiondate { get; set; }

        public virtual UserProfile user { get; set; }

    }
}