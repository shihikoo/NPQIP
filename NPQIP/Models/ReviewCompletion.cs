using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class ReviewCompletion
    {
        public ReviewCompletion()
        {
            Status = "Current";
            Reviews = new HashSet<Review>();
        }

        public int ReviewCompletionID { get; set; }

        public DateTime ComplesionDate { get; set; }

        public NPQIP.Enums.userRoles Role { get; set; }

        public int userID { get; set; }

        public int PublicationPublicationID { get; set; }

        public string Status { get; set; }

        //virtual
        public virtual UserProfile user { get; set; }

        public virtual Publication publication { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}