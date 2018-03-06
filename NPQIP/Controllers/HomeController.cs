using NPQIP.Models;
using NPQIP.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;


namespace NPQIP.Controllers
{
    public class HomeController : Controller
    {
        private readonly NPQIP.Models.NPQIPContext _db = new NPQIP.Models.NPQIPContext();
        
        public ActionResult FinalRankingTable()
        {
            var scoreReview = new FinalRankingTableViewModel(_db.ReviewCompletions.Where(rc => rc.Status == "Current" & !rc.publication.DeletePublication));

            return View(scoreReview);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Background()
        {
            return View();
        }

        public ActionResult Reward()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Checklist()
        {
            var options = _db.Options.AsEnumerable();
            var checklist = _db.Checklists.AsEnumerable().Select(c => new ChecklistViewModel
            {
                ChecklistID = c.ChecklistID,
                ChecklistType = c.ChecklistType,
                OptionType = c.OptionType,
                Item = c.Item,
                ItemNumber = c.ItemNumber,
                Points = c.Points,
                Section = c.Section,
                SectionNumber = c.SectionNumber,
                Options = String.Join(",", options.Where(o => o.OptionType == c.OptionType).Select(o => o.OptionName)),
            }).OrderBy(c => Convert.ToInt32(c.SectionNumber));
            return View(checklist);

        }

        public ActionResult Summary()
        {
            var ReviewPubs = _db.Publications.Include("ReviewCompletions").Where(p => !p.Training && p.DeletePublication == false);                       

            var summaryVM =  new SummaryViewModel
                            {
                                NumCompletedBySeniorReviewer = ReviewPubs.Count(p => p.ReviewCompletions.Count(rc => rc.Status == "Current") >= 1 && p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 1),
                                //NumReviewedBySeniorReviewer =  ReviewPubs.Count(p => p.ReviewCompletions.Count(rc => rc.Status == "Current") == 1 && p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 0),
                                NumCompletedByExternalReviewer = ReviewPubs.Count(p => p.ReviewCompletions.Count(rc => rc.Status == "Current") >= 2 && p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 2),
                                //NumReviewedByExternalReviewer = ReviewPubs.Count(p => p.ReviewCompletions.Count(rc => rc.Status == "Current") == 2 && p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 1),
                                NumCompletedInReconciliation = ReviewPubs.Count(p => p.ReviewCompletions.Count(rc => rc.Status == "Current") >= 3 && p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) >= 3),
                                //NumReviewedInReconciliation = ReviewPubs.Count(p => p.ReviewCompletions.Count(rc => rc.Status == "Current") == 3 && p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.ComplesionDate != null) == 2),
                                NumTotal = ReviewPubs.Count(),
                    //            UserNamesPassedTrainingProgram = db.Promotions.Where(p => p.status == "Current").Distinct().Select(p => p.user.UserName),
                                ProgressArray = GetProgressArray(_db.ReviewCompletions.Where(rc => rc.Status == "Current").AsEnumerable())
                            };

            return View(summaryVM);          
        }

        public ActionResult Output()
        {
            var pubRev = _db.Publications.Where(p => !p.Training && p.DeletePublication == false
            && p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 3).ToList();
            var Reviews = pubRev.SelectMany(pub => pub.ReviewCompletions.LastOrDefault(rc => rc.Status == Enums.Status.Current.ToString()).Reviews.Where(r => r.Status == Enums.Status.Current.ToString())).ToList();

            var ReviewOutput = Reviews.Select(r => new OutputViewModel(r));

            ViewBag.Complete = pubRev.Count() == 882;

            return View(ReviewOutput);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult Report()
        {
            var reportVM = new ReportViewModel(_db.UserProfiles.ToList(), _db.Publications.Where(p => !p.Training && p.DeletePublication == false 
                && p.ReviewCompletions.Any(rc => rc.Status == Enums.Status.Current.ToString())).ToList());
            
            return View(reportVM);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult UncompletedReviewInspect()
        {
            var ReviewPubs =
               _db.Reviews.Where(r => r.Status == Enums.Status.Current.ToString() && (r.ReviewCompletionReviewCompletionID == null  || (r.ReviewCompletion.Status != Enums.Status.Current.ToString()))
               && r.Publication.Training == false)
               .GroupBy( r => new { userid = r.UserID, recordid = r.PublicationPublicationID}).ToList()
               .Select(r => new UncompletedReviewInspectViewModel(r));

            return View(ReviewPubs.OrderBy(r => r.LastActiveTime).ThenBy(r => r.LastTimeUpdated).ToList());
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ArchiveReview(int userId, int publicationId)
        {
            var reviews =
               _db.Reviews.Where(r => r.Status == Enums.Status.Current.ToString() && r.ReviewCompletionReviewCompletionID == null
               && r.Publication.Training == false && r.UserID == userId && r.PublicationPublicationID == publicationId 
               //&& r.Publication.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) < 2
               );

            foreach (var review in reviews) review.ArchiveReview();

            _db.SaveChanges();

            return RedirectToAction("UncompletedReviewInspect");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ReviewInspect()
        {
            var ReviewPubs =
                _db.Publications.Where(p => p.DeletePublication == false).AsEnumerable().Select(
                    p => new ReviewInspectViewModel
                    {
                        PublicationID = p.PublicationID,
                        PublicationNumber = p.PublicationNumber,
                        ReviewCompleted = p.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.userID != 1) + p.TrainingScores.Count(ts => ts.Status == "Current"),
                        ReviewStarted = p.Reviews.Where(r => r.Status == "Current" && r.UserID != 1).Select(r => r.UserID).Distinct().Count(),
                        ReviewedBy = string.Join(", ", p.Reviews.Where(r => r.Status == "Current" && r.UserID != 1).Select(r => r.UserProfile).Distinct().Select(u => u.UserName)),
                        PossibleError = p.Reviews.Count(r => r.Status =="Current") > 0 && p.Files.Where(f => f.DeleteFile == false).Max(f => f.LastUpdateTime) > p.Reviews.Where(r => r.Status == "Current").Min(r =>  r.LastUpdateTime)
                    });

            return View(ReviewPubs.OrderBy(r => r.PossibleError));

        }

        private static double[,] GetProgressArray(IEnumerable<ReviewCompletion> reviewCompletions)
        {
            const long nCut = 10;

            var progressArray = new double[nCut, 2];

            var startDate = new DateTime(2016, 1, 1); // reviewCompletions.Select(rc => rc.ComplesionDate).Min();

            var endDate = DateTime.Now;  // reviewCompletions.Select(rc => rc.ComplesionDate).Max();

            var timeGrid = (endDate-startDate).TotalSeconds/nCut;

            for (var ii = 0; ii < nCut; ii++)
            {

                progressArray[ii,0] = startDate.AddSeconds(timeGrid*(ii+1)).ToOADate();
                progressArray[ii, 1] = reviewCompletions.Count(rc => rc.ComplesionDate >= startDate && rc.ComplesionDate <= startDate.AddSeconds(timeGrid * (ii + 1)));           
            }

            return progressArray;
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

    }
}
