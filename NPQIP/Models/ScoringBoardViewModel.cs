using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Security;
using NPQIP.Models;

namespace NPQIP.ViewModel
{

    public class FinalRankingTableViewModel
    {
        public FinalRankingTableViewModel(IEnumerable<ReviewCompletion> reviewCompletions)
        {
            RankedReviewerAllTimeVMs = reviewCompletions.OrderByDescending(rc => rc.ComplesionDate).GroupBy(rc => rc.PublicationPublicationID).SelectMany(rc => rc.Skip(1))
                .GroupBy(rc => rc.userID, (key, g) => new RankedRviewerViewModel(g.FirstOrDefault().user, g.ToList())).OrderByDescending(u => u.nReviewCompleted).ToList();

            ReconcileRankedReviewerAllTimeVMs = reviewCompletions.OrderByDescending(rc => rc.ComplesionDate).GroupBy(rc => rc.PublicationPublicationID).Select(rc => rc.FirstOrDefault())
                .GroupBy(rc => rc.userID, (key, g) => new RankedRviewerViewModel(g.FirstOrDefault().user, g.ToList())).OrderByDescending(u => u.nReviewCompleted).ToList();
            }

        public List<RankedRviewerViewModel> RankedReviewerAllTimeVMs { get; set; }

        public List<RankedRviewerViewModel> ReconcileRankedReviewerAllTimeVMs { get; set; }
    }

    public class ScoringBoardViewModel
    {
        public ScoringBoardViewModel(IEnumerable<UserProfile> users)
        {
            RankedReviewerAllTimeVMs = new List<RankedRviewerViewModel>();
            foreach (var user in users.Where(u => u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) > 0).ToList())
            {
                RankedReviewerAllTimeVMs.Add(new RankedRviewerViewModel(user, user.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString())));
            }
            RankedReviewerAllTimeVMs = RankedReviewerAllTimeVMs.OrderByDescending(s => s.nReviewCompleted).ThenBy(s => s.CompletedTrainingProgram).ToList();

            RankedExternalReviewerMonthVMs = new List<RankedRviewerViewModel>();
            foreach (var user in users.Where(u => u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() 
            && rc.ComplesionDate >=  new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1,0,0,0)
            && rc.ComplesionDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(1)) > 0
            && !Roles.IsUserInRole(u.UserName, Enums.userRoles.Administrator.ToString())
            ).ToList())
            {
                RankedExternalReviewerMonthVMs.Add(new RankedRviewerViewModel(user, user.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()
            && rc.ComplesionDate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0)
            && rc.ComplesionDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(1))));
            }
            RankedExternalReviewerMonthVMs = RankedExternalReviewerMonthVMs.OrderByDescending(s => s.nReviewCompleted).ThenBy(s => s.CompletedTrainingProgram).ToList();

            RankedFestiveChanneledgeVMs = new List<RankedRviewerViewModel>();
            foreach (var user in users.Where(u => u.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()
            && rc.ComplesionDate >= new DateTime(2016, 11, 1, 0, 0, 0)
            && rc.ComplesionDate < new DateTime(2017, 1, 1, 0, 0, 0)) > 0
            && !Roles.IsUserInRole(u.UserName, Enums.userRoles.Administrator.ToString())
            ).ToList())
            {
                RankedFestiveChanneledgeVMs.Add(new RankedRviewerViewModel(user, user.ReviewCompletions.Where(rc => rc.Status == Enums.Status.Current.ToString()
            && rc.ComplesionDate >= new DateTime(2016, 11, 1, 0, 0, 0)
            && rc.ComplesionDate < new DateTime(2017, 1, 1, 0, 0, 0))));
            }
            RankedFestiveChanneledgeVMs = RankedFestiveChanneledgeVMs.OrderByDescending(s => s.nReviewCompleted).ThenBy(s => s.CompletedTrainingProgram).ThenBy(s => s.LastActiveTime).ToList();

            TrainingCompletedNames =
                users.Where(
                    u =>
                        u.Promotions.Any(
                            p =>
                                p.status == Enums.Status.Current.ToString()
                                && p.promotiondate >= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0)
                                && p.promotiondate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0).AddMonths(1))).Select(u => u.ForeName + " " + u.SurName).ToArray();
        } 

        public ICollection<RankedRviewerViewModel> RankedReviewerAllTimeVMs { get; set; }

        //public ICollection<RankedRviewerViewModel> ExternalRankedReviewerAllTimeVMs { get; set; }

        //public ICollection<RankedRviewerViewModel> ReconcileRankedReviewerAllTimeVMs { get; set; }
         
        public ICollection<RankedRviewerViewModel> RankedExternalReviewerMonthVMs { get; set; }

        public ICollection<RankedRviewerViewModel> RankedFestiveChanneledgeVMs { get; set; }

        public string[] TrainingCompletedNames { get; set; }
    }

    public class RankedRviewerViewModel
    {
        public RankedRviewerViewModel(UserProfile user, IEnumerable<ReviewCompletion> reviewCompletions)
        {
            username = user.ForeName + " " + user.SurName;
            nReviewCompleted = reviewCompletions.Count();
            LastActiveTime = user.LastHeartbeat;
            CompletedTrainingProgram = user.Promotions.Any(p => p.status == Enums.Status.Current.ToString());
            email = user.Email;
        }

        public int rank { get; set; }
        [DisplayName("User")]
        public string username { get; set; }

        public string email { get; set; }

        //[DisplayName("Points")]
        //public int points { get; set; }

        //[DisplayName("Number of Training Reivewed")]
        //public int nTraingStarted { get; set; }

        //[DisplayName("Number of Training Completed")]
        //public int nTraingCompleted { get; set; }

        //[DisplayName("Number of Review Reivewed")]
        //public int nReviewStarted { get; set; }

        [DisplayName("Number of Review Completed")]    
        public int nReviewCompleted { get; set; }

        [DisplayName("Last Active Time")]
        public DateTime LastActiveTime { get; set; }

        public bool CompletedTrainingProgram { get; set; }
    }
}