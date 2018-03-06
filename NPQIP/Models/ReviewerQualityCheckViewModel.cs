using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReviewerQualityCheckViewModel
    {
        
        public ReviewerQualityCheckViewModel(IEnumerable<Publication> publications , int userid ,string username)
        {
            ReviewComparisonVMs = new List<ReviewComparisonViewModel>();

            foreach (var publication in publications.Where(p => !p.Training
            //&& p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 2
                && p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.userID == userid) == 1))
            {
                ReviewComparisonVMs.Add(new ReviewComparisonViewModel(publication,userid));
            }

            ReviewComparisonVMs = ReviewComparisonVMs.OrderByDescending(rc => rc.LastUpdateDateTime).ToList();

            TrainingScoreVMs = new List<TrainingScoreViewModel>();

            var trainingPublications =
                publications.Where(
                    p =>
                        p.Training &&
                        p.TrainingScores.Any(rc => rc.Status == Enums.Status.Current.ToString() && rc.UserID == userid));

            foreach (var publication in trainingPublications)
            {
                TrainingScoreVMs.Add(new TrainingScoreViewModel(publication, userid));
            }

            TrainingScoreVMs = TrainingScoreVMs.OrderByDescending(ts => ts.ScoreTime).ToList();

            ReviewerName = username;
        }
      
        public ReviewerQualityCheckViewModel()
        {
        }

        public string ReviewerName { get; set; }

        public ICollection<ReviewComparisonViewModel> ReviewComparisonVMs { get; set; }

        public ICollection<TrainingScoreViewModel> TrainingScoreVMs { get; set; }
    }
}