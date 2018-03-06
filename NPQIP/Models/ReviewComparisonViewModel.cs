using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReviewComparisonViewModel
    {
            public ReviewComparisonViewModel(Publication publication, int userid) 
            : this (publication.PublicationID, 
                  publication.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString() ),
                  publication.Reviews.Where(r => r.Status == Enums.Status.Current.ToString() && r.UserID == userid))
        {
        }

        public ReviewComparisonViewModel(int publicationID, IEnumerable<ReviewCompletion> reviewCompletions)
        {
            PublicationID = publicationID;

            ReviewIDs = reviewCompletions
                    .OrderBy(rc => Guid.NewGuid())
                    .Select(rc => rc.ReviewCompletionID)
                    .ToList();

            SeniorReviews = reviewCompletions
                    .OrderBy(rc => rc.ComplesionDate)
                    .FirstOrDefault()
                    .Reviews;

            JuniorReviews = reviewCompletions
                    .OrderBy(rc => rc.ComplesionDate)
                    .Skip(1).FirstOrDefault()?
                    .Reviews;

            LastUpdateDateTime = reviewCompletions.Max(rc => rc.ComplesionDate);
        }

        public ReviewComparisonViewModel(int publicationID, IEnumerable<ReviewCompletion> reviewCompletions,
            IEnumerable<Review> reviews)
        {
            PublicationID = publicationID;

            ReviewIDs = reviewCompletions
                    .OrderBy(rc => Guid.NewGuid())
                    .Select(rc => rc.ReviewCompletionID)
                    .ToList();

            SeniorReviews = reviewCompletions
                    .OrderBy(rc => rc.ComplesionDate)
                    .FirstOrDefault()
                    .Reviews;

            JuniorReviews = reviewCompletions
                    .OrderBy(rc => rc.ComplesionDate)
                    .Skip(1).FirstOrDefault()?
                    .Reviews;

            LastUpdateDateTime = reviewCompletions.Max(rc => rc.ComplesionDate);

            NumLeft = reviews.Any() ? reviews.Count(r => r.OptionOptionID == null) : -1;
        }

        public ReviewComparisonViewModel()
        {
        }

        public int PublicationID { get; set; }

        public IList<int> ReviewIDs { get; set; }

        public bool sameuser => userId1 == userId2;

        public int userId1 => SeniorReviews.FirstOrDefault().UserID;

        public int userId2 => JuniorReviews.FirstOrDefault().UserID;

        private IEnumerable<Review> SeniorReviews { get; set; }

        private IEnumerable<Review> JuniorReviews { get; set; }
        
        public int NumLeft { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double? Agreement {
            get {

                if (JuniorReviews == null) return null;

                var totalpoints = SeniorReviews.Count(r => r.Status == "Current" && r.OptionOptionID != null);

                var point = SeniorReviews.Select(correctRev => JuniorReviews
                .FirstOrDefault(r => r.ChecklistChecklistID == correctRev.ChecklistChecklistID 
                && r.PublicationPublicationID == correctRev.PublicationPublicationID 
                && r.OptionOptionID == correctRev.OptionOptionID))
                .Count(rev => rev != null);

                return (double)point / totalpoints; 

            }
        }

        public string NumLeftString => NumLeft == -1 ? "" : NumLeft.ToString();

        public DateTime LastUpdateDateTime { get; set; }

    }
}