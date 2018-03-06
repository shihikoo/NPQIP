using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class Checklist
    {
        public Checklist()
        {
            Reviews = new HashSet<Review>();
            FrontPageReviews = new HashSet<FrontPageReview>();

        }

        [Key]
        public int ChecklistID { get; set; }
        public string SectionNumber { get; set; }
        public string Section { get; set; }
        public string ItemNumber { get; set; }
        public string Item { get; set; }
        public string ChecklistType { get; set; }

        public string OptionType { get; set; }

        public int Points { get; set; }

        public bool Critical { get; set; }

        public string Resource { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<FrontPageReview> FrontPageReviews { get; set; }

    }
}