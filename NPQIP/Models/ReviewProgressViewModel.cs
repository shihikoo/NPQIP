using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using NPQIP.Models;

namespace NPQIP.ViewModel
{
    public class ReviewProgressViewModel
    {
        public ReviewProgressViewModel()
        {
        }

        public int NumberOfCompletedReviews { get; set; }

        public int NumberOfActiveReviews => ReviewInProgress.Count();

        public int NumberOfToReviews { get; set; }

        public int TotalNumber => NumberOfCompletedReviews + NumberOfActiveReviews + NumberOfToReviews;

        public string StyleOfCompletedReviews => "width: " + (100.0 * NumberOfCompletedReviews / TotalNumber) + "%; min-width: 2em;";

        public string StyleOfActiveReviews => "width: " + (100.0 * NumberOfActiveReviews / TotalNumber) + "%; min-width: 2em;";

        public string StyleOfToReviews => "width: " + (100.0 * NumberOfToReviews / TotalNumber) + "%";


        public string newPublicationAvaliabilityStyle
            => (NumberOfToReviews > 0 && NumberOfActiveReviews < (int)Enums.OngoingPublicationLimit.Number) ? "" : "disabled";


        public ICollection<ReviewListViewModel> ReviewInProgress { get; set; }



    }
}