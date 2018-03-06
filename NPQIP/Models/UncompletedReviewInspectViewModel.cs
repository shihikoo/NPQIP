using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NPQIP.ViewModel
{
    public class UncompletedReviewInspectViewModel
    {
        public UncompletedReviewInspectViewModel( IEnumerable<Review> reviews) {
            PublicationID = reviews.FirstOrDefault().PublicationPublicationID;
            ReviewedBy = reviews.FirstOrDefault().UserProfile.UserName;
            NumCompleted = reviews.Count(r => r.OptionOptionID != null);
            LastTimeUpdated = reviews.Max(r => r.LastUpdateTime);
            LastActiveTime = reviews.FirstOrDefault().UserProfile.LastHeartbeat;
            userid = reviews.FirstOrDefault().UserID;
        }

        public UncompletedReviewInspectViewModel() { }

        public int userid { get; set; }

        public int PublicationID { get; set; }

        public string ReviewedBy { get; set; }

        [DisplayName("Items Completed")]
        public int   NumCompleted { get; set; }

        public DateTime LastTimeUpdated { get; set; }

        public DateTime LastActiveTime { get; set; }
    }
}
