using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class TrainingScore
    {
        public TrainingScore()
        {
            Reviews = new HashSet<Review>();
            Status = "Current";
        }

        public int TrainingScoreID { get; set; }

        public int UserID { get; set; }

        public int PublicationPublicationID { get; set; }

        public int Points { get; set; }

        public int TotalPoints { get; set; }

        public bool Pass { get; set; }

        public DateTime ScoreTime { get; set; }

        public string Status { get; set; }

        // virtual
        public virtual UserProfile UserProfile { get; set; }

        public virtual Publication Publication { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

    }
}