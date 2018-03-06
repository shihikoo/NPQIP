using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class Option
    {
        public Option()
        {
            Reviews = new HashSet<Review>();
            FrontPageReviews = new HashSet<FrontPageReview>();
        }

        public int OptionID { get; set; }
        public string OptionName { get; set; }
        public string OptionType { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<FrontPageReview> FrontPageReviews { get; set; }

    }
}