using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPQIP.Models;
using System.Web.Security;
using NPQIP.ViewModel;
using NPQIP.Authorization;
using static System.Web.Security.Membership;


namespace NPQIP.Controllers
{
    [Authorize]
    [Restrict("Suspended")]
    public class ReviewController : Controller
    {
        private readonly NPQIPContext _db = new NPQIP.Models.NPQIPContext();

        private int Userid { get; } = (int)GetUser().ProviderUserKey;

        public ActionResult Cover()
        {
            var points = _db.ReviewCompletions.Count(r => r.Status == Enums.Status.Current.ToString() && r.userID == Userid);

            if (points >= 50)
            {
                ViewBag.Message = "You currently have reviewed " + points.ToString() +
                            " publication.<br /> You are doing an amazing job! Hurry up to do more!</p>";
            }
            else if (points >= 10)
            {
                ViewBag.Message = "You currently have reviewed " + points.ToString() +
                                 " publications.<br /> Good job! Hurry up to do more!</p>";
            }
            else
            {
                ViewBag.Message = "You currently have reviewed " + points.ToString() +
                                      " publications. <br />Hurry up to do more!</p>";
            }

            return View();
        }

        public ActionResult ReviewResource()
        {
            const string fileurl = "Content/NatureReviewResource.pdf";
            const string filetype = "application/pdf";

            return File(fileurl, filetype);
        }

        [Authorize(Roles = "Trainee")]
        public ActionResult TrainingCover()
        {
            var totalNum = _db.Publications.Count(p => p.DeletePublication != true && p.PublicationID >= 36 && p.PublicationID <= 65);

            var completedNumber =
                _db.TrainingScores.Count(ts => ts.Status == Enums.Status.Current.ToString() && ts.UserID == Userid);

            var uncompletedNum = _db.Publications.Count(p => p.DeletePublication != true
                                                             && p.Training
                                                              && p.Reviews.Any(
                                                                 r =>
                                                                     r.Status == Enums.Status.Current.ToString() &&
                                                                     r.UserID == Userid));

            var trainingCoverVM = new TrainingCoverViewModel(totalNum, uncompletedNum, completedNumber);

            return View(trainingCoverVM);
        }

        [Authorize(Roles = "Trainee")]
        public ActionResult PreviousRecordsIndex()
        {
            //var userid = (int)Membership.GetUser().ProviderUserKey;

            var records = (from r in _db.TrainingScores.Where(ts => ts.Status == "Current" && ts.UserID == Userid && !ts.Publication.DeletePublication)
                           select r).AsEnumerable().Select(r => new TrainingScoreViewModel
                           {
                               PublicationPublicationID = r.PublicationPublicationID,
                               TrainingScoreID = r.TrainingScoreID,
                               Points = r.Points,
                               TotalPoints = r.TotalPoints,
                               Pass = r.Pass,
                               ScoreTime = r.ScoreTime,
                           });

            return View(records.ToList());
        }

        [Authorize(Roles = "Trainee")]
        public ActionResult ViewRecord(int scoreid, int userid = 0)
        {
            var ts = _db.TrainingScores.Find(scoreid);

            if (!User.IsInRole("Administrator") && ts.UserID != userid && userid != 0) return View("Error");

            ViewBag.trainingscore = ts;

            if (ts == null) throw new HttpException(404, "File not Found.");

            if (ts.Status != "Current") return View("Error");

            var transcript = (
                from gr in _db.Reviews.Where(gr => gr.PublicationPublicationID == ts.PublicationPublicationID && gr.UserID == 1 && gr.Status == "Current")
                from rev in _db.Reviews.Where(r => r.TrainingScoreTrainingScoreID == scoreid && r.ChecklistChecklistID == gr.ChecklistChecklistID && r.Status == "Current").DefaultIfEmpty()
                select new MarkedReviewViewModel
                {
                    PublicationPublicationID = ts.PublicationPublicationID,
                    ChecklistID = gr.ChecklistChecklistID,
                    ReviewID = gr.ReviewID,
                    OptionOptionID = gr.OptionOptionID,
                    OptionType = gr.Checklist.OptionType,
                    Comments = gr.Comments,
                    SectionNumber = gr.Checklist.SectionNumber,
                    Section = gr.Checklist.Section,
                    ItemNumber = gr.Checklist.ItemNumber,
                    Item = gr.Checklist.Item,
                    CorrectAnswer = gr.Optoin.OptionName,
                    YourAnswer = rev.Optoin.OptionName,
                    YourComments = rev.Comments,
                    Correct = gr.OptionOptionID == rev.OptionOptionID,

                }
                        );
            return View(transcript.ToList());
        }

        [Authorize(Roles = "Trainee")]
        public ActionResult ArchiveRecord()
        {
            var reviews = _db.Reviews.Where(r => r.UserID == Userid && (r.Status == Enums.Status.Current.ToString() || r.Status == Enums.Status.Deleted.ToString()));

            foreach (var rev in reviews)
            {
                rev.Status = rev.Status == "Current" ? "Archived" : "ArchivedDeleted";
            }

            var ts = _db.TrainingScores.Where(r => r.UserID == Userid && r.Status == "Current");

            foreach (var score in ts)
            {
                score.Status = "Archived";
            }

            //if (ModelState.IsValid) { _db.SaveChanges(); }
            _db.SaveChanges();

            return RedirectToAction("TrainingCover");
        }

        [Authorize(Roles = "Trainee")]
        public ActionResult Score(int id)
        {
            const double passingGrade = (double)Enums.PassingCriteria.Percentage / 100.0;
            const int passingNumbers = (int)Enums.PassingCriteria.Times;

            var pub = _db.Publications.Find(id);

            if (pub == null || pub.DeletePublication)
            {
                ViewBag.ErrorMessage = "No publication found";
                return View("Error");
            }

            var existingScore = _db.TrainingScores.Where(t => t.PublicationPublicationID == id && t.Status == Enums.Status.Current.ToString() && t.UserID == Userid);

            if (existingScore.Count() > 1) return View("Error");

            if (existingScore.Count() == 1)
            {
                var estm = new TrainingScoreViewModel
                {
                    TrainingScoreID = existingScore.FirstOrDefault().TrainingScoreID,
                    PublicationPublicationID = existingScore.FirstOrDefault().PublicationPublicationID,
                    Points = existingScore.FirstOrDefault().Points,
                    Pass = existingScore.FirstOrDefault().Pass,
                    ScoreTime = existingScore.FirstOrDefault().ScoreTime,
                    TotalPoints = existingScore.FirstOrDefault().TotalPoints,
                    Promoted = false,
                };

                return View(estm);
            }

            // if there is no existing score, then creates the new score
            // all reviews that are done by the current reviewer, for current publiction and not yet been scored. && r.TrainingScoreTrainingScoreID == null
            var traineeReviews = pub.Reviews.Where(r => r.UserID == Userid && r.Status == Enums.Status.Current.ToString()).ToList();

            if (!traineeReviews.Any()) return View("Error");

            var goldenReviews = pub.Reviews.Where(r => r.UserID == (int)Enums.GoldStandard.UserId && r.Status == Enums.Status.Current.ToString()).ToList();

            if (!goldenReviews.Any()) return View("Error");

            //grading the traning review according to the goldenreview
            var points = 0;
            var pass = true;

            foreach (var answer in goldenReviews)
            {
                if (traineeReviews.Any(t => t.ChecklistChecklistID == answer.ChecklistChecklistID && t.OptionOptionID == answer.OptionOptionID))
                {
                    points = points + answer.Checklist.Points;
                }
                else if (answer.Checklist.Critical)
                {
                    pass = false;
                }
            }

            var totalpoints = traineeReviews.Select(gr => gr.Checklist.Points).Sum();

            var score = 1.0 * points / totalpoints;

            //create traning record scoring and add to the database
            var ts = new TrainingScore()
            {
                UserID = Userid,
                PublicationPublicationID = id,
                Points = points,
                Pass = score >= passingGrade,
                ScoreTime = DateTime.Now,
                TotalPoints = totalpoints,
            };

            _db.TrainingScores.Add(ts);

            foreach (var answer in traineeReviews)
            {
                answer.TrainingScore = ts;
            }

            _db.SaveChanges();

            //create traning recored view model to send it to view
            var tsVM = new TrainingScoreViewModel
            {
                TrainingScoreID = ts.TrainingScoreID,
                PublicationPublicationID = ts.PublicationPublicationID,
                Points = ts.Points,
                Pass = ts.Pass,
                ScoreTime = ts.ScoreTime,
                TotalPoints = ts.TotalPoints,
                IsReviewer = Roles.IsUserInRole(NPQIP.Enums.userRoles.Reviewer.ToString()),
            };

            // to see whether it should be a promotion or not
            if (tsVM.IsReviewer) return View(tsVM);

            var latestScore = _db.TrainingScores.Where(t => t.Status == "Current" && t.UserID == Userid).OrderByDescending(t => t.ScoreTime).Take(passingNumbers);

            if (latestScore.Count(s => s.Pass) != passingNumbers) return View(tsVM);

            Roles.AddUserToRole(User.Identity.Name, NPQIP.Enums.userRoles.Reviewer.ToString());

            var userprofile = _db.UserProfiles.Find(Userid);
            var promotion = new Promotion()
            {
                Name = userprofile.ForeName + " " + userprofile.SurName,
                oldRole = NPQIP.Enums.userRoles.Trainee,
                newRole = NPQIP.Enums.userRoles.Reviewer,
                promotiondate = ts.ScoreTime,
                UserID = Userid
            };

            _db.Promotions.Add(promotion);

            _db.SaveChanges();

            tsVM.Promoted = true;

            tsVM.promotion = promotion;

            return View(tsVM);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult TrainingProgress(int publicationid)
        {
            var transcript = (
                from gr in _db.Reviews.Where(gr => gr.PublicationPublicationID == publicationid && gr.UserID == 1 && gr.Status == "Current")
                join allrev in _db.Reviews.Where(r => r.TrainingScoreTrainingScoreID != null && r.UserID != 1
                    && r.PublicationPublicationID == publicationid && r.Status == "Current").DefaultIfEmpty() on gr.ChecklistChecklistID equals allrev.ChecklistChecklistID into newRev
                from subset in newRev
                select new TrainingProgressViewModel
                {
                    PublicationPublicationID = publicationid,
                    ReviewID = gr.ReviewID,
                    ChecklistID = gr.ChecklistChecklistID,
                    OptionOptionID = gr.OptionOptionID,
                    OptionType = gr.Checklist.OptionType,
                    Comments = gr.Comments,
                    SectionNumber = gr.Checklist.SectionNumber,
                    Section = gr.Checklist.Section,
                    ItemNumber = gr.Checklist.ItemNumber,
                    Item = gr.Checklist.Item,
                    CorrectAnswer = gr.Optoin.OptionName,
                    AllReviewCorrectRate = newRev.Count(r => r.OptionOptionID == gr.OptionOptionID) * 100.0 / newRev.Count(),
                    NumberOfReviewers = newRev.Count()
                }
                        );


            return View(transcript.ToList());
        }

        [Authorize(Roles = "Trainee")]
        public ActionResult UncompletedTrainingList()
        {
            var reviewListVM = _db.Publications.Where(
                p =>
                    p.DeletePublication == false 
                    && p.Training
                    && p.Reviews.Any(
                        r =>
                            r.UserID == Userid && r.Status == Enums.Status.Current.ToString() &&
                            r.TrainingScoreTrainingScoreID == null)).AsEnumerable()
                .Select(p => new ReviewListViewModel(p.PublicationID, p.Reviews.Where(r => r.Status == Enums.Status.Current.ToString() && r.UserID == Userid).Max(r => r.LastUpdateTime))).ToList();

            return View(reviewListVM);
        }

        //--------------------------------------------------------------------------------------//
        public ActionResult ReviewerRecord()
        {
            var npub = _db.ReviewCompletions.Count(rc => rc.Status == "Current" && rc.userID == Userid);

            var ntpub = _db.TrainingScores.Count(ts => ts.Status == "Current" && ts.UserID == Userid);

            if (User.IsInRole("SuperReviewer")) ViewBag.level = "Super Reviewer";
            else if (User.IsInRole("SeniorReviewer")) ViewBag.level = "Senior Reviewer";
            else if (User.IsInRole("Reviewer")) ViewBag.level = "Reviewer";
            else ViewBag.level = "Trainee";

            ViewBag.npub = npub;

            ViewBag.ntpub = ntpub;

            return View();
        }

        public ActionResult ReviewIndex(int id)
        {
            var pub = _db.Publications.Find(id);

            if (pub == null || pub.DeletePublication)
            {
                ViewBag.ErrorMessage = "No publication found.";
                return View("Error");
            }

            var purpose = pub.Training ? Enums.reviewPurpose.Training : Enums.reviewPurpose.Review;

            var pubReview = new ReviewOverviewViewModel(pub, Userid);

            //LoadUserUncompletedReviewedPublicationsToViewForPurpose(Userid, purpose).AsEnumerable();

            //var result = PubReview.FirstOrDefault(p => p.PublicationVM.PublicationID == id);

            if (pubReview.Reviews.All(r => r.TrainingScoreTrainingScoreID != null))
            {
                ViewBag.ErrorMessage = "Error! You cannot review this publication. You may have already completed reviewing this publication. Or you do not have correct rights to review this publication. ";
                return View("Error");
            }

            //pubReview.ReviewPurpose = purpose;

            ViewBag.Title = purpose + " Section";

            return View(pubReview);
        }

        public ActionResult ReviewIndexNew(NPQIP.Enums.reviewPurpose purpose)
        {
            ViewBag.Title = purpose + " Section";

            var lastreview = FindMyLatestReview(_db.Reviews, Userid, purpose);

            var publications = GetAvaliablePublications(Userid, purpose).ToList();

            if (!publications.Any()) return RedirectToAction("Cover");

            var newPublications = publications.Where(p => p.PublicationNumber.Substring(0, 3) != lastreview);

            var randomPublication = RandomPublication(newPublications.Any() ? newPublications : publications);

            if (randomPublication == null) return RedirectToAction("Cover");

            var result = new ReviewOverviewViewModel(randomPublication,Userid);

            return View("ReviewIndex", result);
        }

        public ActionResult Review(int id, string reviewType = "")
        {
            ViewBag.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer;

            var pub = _db.Publications.Find(id);
            if (pub == null || pub.DeletePublication)
            {
                ViewBag.ErrorMessage = "Error! Publication not found";
                return View("Error");
            }

            var checklists = _db.Checklists.Where(c => c.SectionNumber == "-999");

            switch (reviewType)
            {
                case "Question16":
                    checklists = _db.Checklists.Where(c => c.ChecklistType == "in vitro");
                    break;
                case "Question17":
                    checklists = _db.Checklists.Where(c => c.ChecklistType == "in vivo");
                    break;
                default:
                    checklists = _db.Checklists.Where(c => c.SectionNumber == reviewType.Substring(8));
                    break;
            }

            var reviewViewmodel = (from c in checklists.AsEnumerable()
                                   join r in pub.Reviews.Where(r => r.UserID == Userid && r.Status == "Current")
                                   on c.ChecklistID equals r.ChecklistChecklistID into fullreviews
                                   from rv in fullreviews.DefaultIfEmpty()
                                   select new ReviewViewModel
                                   {
                                       PublicationPublicationID = id,
                                       ChecklistID = c.ChecklistID,
                                       ReviewID = rv?.ReviewID ?? 0,
                                       OptionOptionID = (rv == null) ? 0 : rv.OptionOptionID,
                                       OptionType = c.OptionType,
                                       Comments = (rv == null) ? string.Empty : rv.Comments,
                                       SectionNumber = c.SectionNumber,
                                       Section = c.Section,
                                       ItemNumber = c.ItemNumber,
                                       Item = c.Item,
                                       UserName = User.Identity.Name,
                                       LastUpdateTime = rv?.LastUpdateTime ?? DateTime.Now,
                                       Resource = c.Resource,
                                       ChecklistType = c.ChecklistType,
                                   }).ToList();

            if (!reviewViewmodel.Any()) return RedirectToAction("Cover");

            TempData["1"] = new SelectList(_db.Options.Where(o => o.OptionType == "1"), "OptionID", "OptionName");
            TempData["2"] = new SelectList(_db.Options.Where(o => o.OptionType == "2"), "OptionID", "OptionName");
            TempData["3"] = new SelectList(_db.Options.Where(o => o.OptionType == "3"), "OptionID", "OptionName");
            TempData["4"] = new SelectList(_db.Options.Where(o => o.OptionType == "4"), "OptionID", "OptionName");
            TempData["5"] = new SelectList(_db.Options.Where(o => o.OptionType == "5"), "OptionID", "OptionName");
            TempData["6"] = new SelectList(_db.Options.Where(o => o.OptionType == "6"), "OptionID", "OptionName");
            TempData["7"] = new SelectList(_db.Options.Where(o => o.OptionType == "7"), "OptionID", "OptionName");
            TempData["8"] = new SelectList(_db.Options.Where(o => o.OptionType == "8"), "OptionID", "OptionName");
            TempData["9"] = new SelectList(_db.Options.Where(o => o.OptionType == "9"), "OptionID", "OptionName");
            TempData["10"] = new SelectList(_db.Options.Where(o => o.OptionType == "10"), "OptionID", "OptionName");
            TempData["11"] = new SelectList(_db.Options.Where(o => o.OptionType == "11"), "OptionID", "OptionName");
            TempData["12"] = new SelectList(_db.Options.Where(o => o.OptionType == "12"), "OptionID", "OptionName");
            TempData["13"] = new SelectList(_db.Options.Where(o => o.OptionType == "13"), "OptionID", "OptionName");
            TempData["14"] = new SelectList(_db.Options.Where(o => o.OptionType == "14"), "OptionID", "OptionName");
            TempData["15"] = new SelectList(_db.Options.Where(o => o.OptionType == "15"), "OptionID", "OptionName");
            TempData["16"] = new SelectList(_db.Options.Where(o => o.OptionType == "16"), "OptionID", "OptionName");
            TempData["17"] = new SelectList(_db.Options.Where(o => o.OptionType == "17"), "OptionID", "OptionName");

            ViewBag.PublicationPublicationID = id;

            ViewBag.Title = "Publication " + id;

            return View(reviewViewmodel.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Review(int id, IList<ReviewViewModel> reviewRecord, string reviewType = "", string previousURL = "")
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values.SelectMany(v => v.Errors).ToString();

                ViewBag.ErrorMessage = errorMessage;

                return View("Error");
            }

            foreach (var eachReview in reviewRecord)
            {
                var exsitingReview = _db.Reviews.Where(r => r.PublicationPublicationID == eachReview.PublicationPublicationID && r.UserID == Userid && r.ChecklistChecklistID == eachReview.ChecklistID && r.Status == "Current");

                var exsitingReviewNumber = exsitingReview.Count();

                if (exsitingReviewNumber > 1)
                {
                    ViewBag.ErrorMessage = "Duplicate record found. ";
                    return View("Error");
                }

                if (exsitingReviewNumber == 1)
                {
                    eachReview.ReviewID = exsitingReview.FirstOrDefault().ReviewID;
                }

                if (eachReview.ReviewID > 0)
                {
                    var review = _db.Reviews.Find(eachReview.ReviewID);
                    review.OptionOptionID = eachReview.OptionOptionID;
                    review.Comments = eachReview.Comments;
                    review.LastUpdateTime = DateTime.Now;
                    review.UserID = Userid;
                    review.Status = "Current";
                }
                else
                {
                    var review = new Review
                    {
                        PublicationPublicationID = eachReview.PublicationPublicationID,
                        ChecklistChecklistID = eachReview.ChecklistID,
                        OptionOptionID = eachReview.OptionOptionID,
                        Comments = eachReview.Comments,
                        UserID = Userid,
                    };
                    _db.Reviews.Add(review);
                }
                var revs = _db.Reviews.Where(r => (r.Status == "Current" || r.Status == "Deleted")
                                                      && r.PublicationPublicationID == id
                                                      && r.Publication.DeletePublication != true
                                                      && r.UserID == Userid).AsEnumerable();
                if (eachReview.ItemNumber == "0.1")
                {
                    switch (eachReview.OptionOptionID)
                    {
                        case 31:
                            {
                                var revs1 = revs.Where(r => r.Checklist.ChecklistType == "in vivo");
                                if (revs1.Any())
                                {
                                    foreach (var rev in revs1)
                                    {
                                        rev.Status = "Current";
                                    }
                                }

                                var revs2 = revs.Where(r => r.Checklist.ChecklistType == "in vitro");
                                if (revs2.Any())
                                {
                                    foreach (var rev in revs2)
                                    {
                                        rev.Status = "Deleted";
                                    }
                                }
                            }
                            break;
                        case 32:
                            {
                                var revs1 = revs.Where(r => r.Checklist.ChecklistType == "in vitro");
                                if (revs1.Any())
                                {
                                    foreach (var rev in revs1)
                                    {
                                        rev.Status = "Current";
                                    }
                                }

                                var revs2 = revs.Where(r => r.Checklist.ChecklistType == "in vivo");
                                if (!revs2.Any()) continue;
                                foreach (var rev in revs2)
                                {
                                    rev.Status = "Deleted";
                                }
                            }
                            break;
                        case 33:
                            revs = revs.Where(r => r.Checklist.ChecklistType == "in vitro" || r.Checklist.ChecklistType == "in vivo");
                            if (!revs.Any()) continue;
                            foreach (var rev in revs)
                            {
                                rev.Status = "Current";
                            }
                            break;
                        case 34:
                            revs = revs.Where(r => r.Checklist.ChecklistType == "in vitro" || r.Checklist.ChecklistType == "in vivo");
                            if (!revs.Any()) continue;
                            foreach (var rev in revs)
                            {
                                rev.Status = "Deleted";
                            }
                            break;
                    }
                }
                else if (eachReview.ItemNumber == "0.2")
                {
                    switch (eachReview.OptionOptionID)
                    {
                        case 23:
                            revs = revs.Where(r => r.Checklist.SectionNumber == "8");
                            if (!revs.Any()) continue;
                            foreach (var rev in revs)
                            {
                                rev.Status = "Deleted";
                            }
                            break;
                        case 22:
                            revs = revs.Where(r => r.Checklist.SectionNumber == "8");
                            if (!revs.Any()) continue;
                            foreach (var rev in revs)
                            {
                                rev.Status = "Current";
                            }
                            break;
                    }
                }
                else if (eachReview.ItemNumber == "0.3")
                {
                    switch (eachReview.OptionOptionID)
                    {
                        case 23:
                            revs = revs.Where(r => r.Checklist.SectionNumber == "10");
                            if (!revs.Any()) continue;
                            foreach (var rev in revs)
                            {
                                rev.Status = "Deleted";
                            }
                            break;
                        case 22:
                            revs = revs.Where(r => r.Checklist.SectionNumber == "10");
                            if (!revs.Any()) continue;
                            foreach (var rev in revs)
                            {
                                rev.Status = "Current";
                            }
                            break;
                    }
                }
            }

            _db.SaveChanges();

            if (previousURL.Contains("ReviewIndexNew"))
            {
                return RedirectToAction("ReviewIndex", new { id = id });
            }
            else
            {
                return Redirect(previousURL.TrimEnd());
            }
        }

        public ActionResult CertificateList()
        {
            //var userid = (int)Membership.GetUser().ProviderUserKey;

            var promotionCertificateVM = _db.Promotions.Where(p => p.status == "Current" && p.UserID == Userid).AsEnumerable()
                .Select(p => new CertificateViewModel
                {
                    promotionid = p.PromotionID,
                    name = p.Name,
                    Year = p.promotiondate.Year.ToString(),
                    Month = p.promotiondate.Month.ToString(),
                    Date = p.promotiondate.Day.ToString(),
                    role = p.newRole.ToString(),
                });

            return View(promotionCertificateVM);
        }

        public ActionResult Read(int id)
        {
            ViewBag.N = _db.Files.Count(m => m.DeleteFile != true && m.PublicationPublicationID == id);

            var pub = _db.Publications.Find(id);

            TempData["PublicationID"] = pub.PublicationID;
            ViewBag.PMID = pub.PMID;
            if (ViewBag.N > 0)
            {
                var uploadviewmodel = from f in _db.Files
                                      where (f.PublicationPublicationID == id && f.DeleteFile != true)
                                      select new UploadViewModel
                                      {
                                          PublicationPublicationID = id,
                                          FileID = f.FileID,
                                          FileName = f.FileName,
                                          FileExtention = f.FileExtention.Substring(1),
                                      };

                return View(uploadviewmodel.ToList());
            }
            else
            {
                throw new HttpException(404, "File not Found.");
            }
        }

        public ActionResult Certificate(int promotionid = 0)
        {
            Promotion promotion;

            if (promotionid > 0)
            {
                //var userid = (int)Membership.GetUser().ProviderUserKey;

                var promotions = _db.Promotions.Where(p => p.UserID == Userid);

                var a = promotions.Count();

                if (promotions.Any()) RedirectToAction("UserProfile", "Account");

                promotion = promotions.OrderByDescending(p => p.promotiondate).FirstOrDefault();
            }
            else if (User.IsInRole("Administrator"))
            {
                promotion = _db.Promotions.Find(promotionid);
            }
            else return View("Error");

            if (promotion == null)
            {
                return View("Error");
            }

            var certificateVM = new CertificateViewModel()
            {
                name = promotion.Name,
                Year = promotion.promotiondate.Year.ToString(),
                Month = promotion.promotiondate.Month.ToString(),
                Date = promotion.promotiondate.Day.ToString(),
                role = promotion.newRole.ToString(),
            };

            return View(certificateVM);
        }
        //--------------------------------------------------------------------------------------//
        [Authorize(Roles = "Administrator, Reviewer, SeniorReviewer")]
        public ActionResult MyReviewProgress()
        {
            var reviewprogressVM = new ReviewProgressViewModel()
            {
                ReviewInProgress = GetOngoingPublications(Userid, NPQIP.Enums.reviewPurpose.Review).Select(p => new ReviewListViewModel(p.PublicationID,
                p.Reviews.Where(r => r.Status == Enums.Status.Current.ToString() && r.UserID == Userid).Max(r => r.LastUpdateTime)))
                .OrderByDescending(p => p.UpdateTime).ToList(),

                NumberOfCompletedReviews = GetNumberOfCompletedReviews(Userid),

                NumberOfToReviews = GetAvaliablePublications(Userid, NPQIP.Enums.reviewPurpose.Review).Count(),
            };

            return View(reviewprogressVM);
        }

        private int GetNumberOfCompletedReviews(int userid)
        {
            return _db.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString() && rc.ComplesionDate != null && rc.userID == userid);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Reviewer, SeniorReviewer")]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteReview(int PublicationID)
        {
            if (!ModelState.IsValid) { ViewBag.ErrorMessage = "Model invalid."; return View("Error"); }

            if (PublicationID == 0) return View("Error");

            var userid = (int)GetUser().ProviderUserKey;

            var ExistingRecord = _db.ReviewCompletions.Any(r => r.userID == userid && r.PublicationPublicationID == PublicationID && r.Status == "Current");

            if (ExistingRecord)
            {
                ViewBag.ErrorMessage = "Review has been submitted.";
                return RedirectToAction("MyReviewProgress");
            }

            IEnumerable<Review> revs = _db.Reviews.Where(r => r.PublicationPublicationID == PublicationID && r.Status == "Current" && r.UserID == userid);
            if (revs.Count() > 88) return View("Error");

            ReviewCompletion rc = new ReviewCompletion()
            {
                ComplesionDate = DateTime.UtcNow,
                Role = User.IsInRole("SeniorReviewer") ? Enums.userRoles.SeniorReviewer : Enums.userRoles.Reviewer,
                userID = userid,
                PublicationPublicationID = PublicationID,
            };

            _db.ReviewCompletions.Add(rc);

            foreach (var rev in revs)
            {
                rev.ReviewCompletion = rc;
            }

            _db.SaveChanges();

            return RedirectToAction("MyReviewProgress");
        }

        //--------------------------------------------------------------------------------------//
        [Authorize(Roles = "Administrator")]
        public ActionResult GoldStandardList()
        {
            if (!CanEditGoldStandard(Userid))
            {
                ViewBag.ErrorMessage = "You don't have rights to edit gold standards.";
                return View("Error");
            }

            ViewBag.numberofchecklist0 = _db.Checklists.Count(c => c.SectionNumber == "0");
            ViewBag.numberofchecklist16 = _db.Checklists.Count(c => c.ChecklistType == "in vitro");
            ViewBag.numberofchecklist17 = _db.Checklists.Count(c => c.ChecklistType == "in vivo");
            ViewBag.numberofchecklist8 = _db.Checklists.Count(c => c.SectionNumber == "8");
            ViewBag.numberofchecklist9 = _db.Checklists.Count(c => c.SectionNumber == "9");
            ViewBag.numberofchecklist10 = _db.Checklists.Count(c => c.SectionNumber == "10");

            var pubReview = _db.Publications
                .Where(p => p.DeletePublication != true && p.Training && p.PMID > 0).OrderBy(p => p.PublicationID).ToList().Select( p => new ReviewOverviewViewModel(p, Userid));

            ViewBag.totalPubReview = pubReview.Count();

            return View(pubReview.ToList());
        }

        //[Authorize(Roles = "Administrator")]
        //public ActionResult ReviewSameReviewer()
        //{
        //    var publicationlist = _db.Publications.Include(p => p.Reviews)
        //        .Where(p =>
        //            p.DeletePublication == false
        //            && p.ReviewCompletions.Count(rc => rc.Status == "Current") >= 2
        //            && p.Reviews.Where(r => r.Status == "Current").Select(r => r.UserID).Distinct().Count() == 2
        //        ).AsEnumerable().Select(p => new ReviewComparisonViewModel(p, Userid));

        //    ViewBag.sameReviewer = publicationlist.Any(p => p.sameuser);

        //    return View(publicationlist.OrderByDescending(p => p.NumLeft).ThenByDescending(p => p.Agreement));
        //}

        [Authorize(Roles = "Reconciler")]
        public ActionResult ReviewComparison()
        {
            var publicationlist = _db.Publications.Include(p => p.Reviews)
                .Where(p =>
                    p.DeletePublication == false
                    && p.ReviewCompletions.Count(rc => rc.Status == "Current") == 2
                    && !p.ReviewCompletions.Where(rc => rc.Status == "Current").Select(rc => rc.userID).Contains(Userid)
                    && (p.Reviews.Where(r => r.Status == "Current").Select(r => r.UserID).Distinct().Count() == 2 
                    || p.Reviews.Where(r => r.Status == "Current").Select(r => r.UserID).Distinct().Contains(Userid))
                ).AsEnumerable().Select( p => new ReviewComparisonViewModel(p, Userid));

            ViewBag.sameReviewer = publicationlist.Any(p => p.sameuser);

            return View(publicationlist.OrderByDescending(p => p.NumLeft).ThenByDescending(p => new Guid()));
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ReviewerQualityCheck(int userid)
        {
            var username = _db.UserProfiles.Find(userid).UserName;

            if(username == null)
            {
                ViewBag.ErrorMessage = "user not valid.";
                return View("Error");
            }

            var publications = _db.Publications.Where(p => p.DeletePublication == false).AsEnumerable();

            var publicationlist = new ReviewerQualityCheckViewModel(publications, userid, username);

            return View(publicationlist);
        }

        [Authorize(Roles = "Reconciler")]
        public ActionResult Reconciliation(int rc1, int rc2)
        {
            var reviews1 = _db.ReviewCompletions.Find(rc1).Reviews.Where(r => r.Status == "Current");

            var reviews2 = _db.ReviewCompletions.Find(rc2).Reviews.Where(r => r.Status == "Current");

            if(rc1 == Userid || rc2 == Userid)
            {
                ViewBag.ErrorMessage = "You have already reviewed this publication.";
                return View("Error");
            }

            if (reviews1.FirstOrDefault() == null || reviews2.FirstOrDefault() == null || reviews1.FirstOrDefault().PublicationPublicationID != reviews2.FirstOrDefault().PublicationPublicationID)
            {
                ViewBag.ErrorMessage = "Url not valid.";
                return View("Error");
            }

            var pubid = reviews1.FirstOrDefault().PublicationPublicationID;

            var checklists = reviews1.Select(r => r.Checklist).Union(reviews2.Select(r => r.Checklist));

            var reviews3 = _db.Reviews.Where(r =>
                 r.Status == Enums.Status.Current.ToString()
               && r.ReviewCompletionReviewCompletionID != rc1
               && r.ReviewCompletionReviewCompletionID != rc2
               && r.UserID == Userid
               && r.PublicationPublicationID == pubid
               ).ToList();

            if (!reviews3.Any())
            {
                reviews3 = (from checklist in checklists
                           from newreview1 in reviews1.Where(r => r.ChecklistChecklistID == checklist.ChecklistID).DefaultIfEmpty()
                           from newreview2 in reviews2.Where(r => r.ChecklistChecklistID == checklist.ChecklistID).DefaultIfEmpty()
                           select new Review(Userid, pubid, checklist.ChecklistID
                           , (newreview1 == null || newreview2 == null) ? null : (newreview1.OptionOptionID == newreview2.OptionOptionID ? newreview1.OptionOptionID : null))
                           ).ToList();
                //checklists.Select(c => new Review(Userid, pubid, c.ChecklistID, optionId1, optionId2)).ToList()
                _db.Reviews.AddRange(reviews3);
                _db.SaveChanges();
            }

            var comparisons = from checklist in checklists
                              from newreview1 in reviews1.Where(r => r.ChecklistChecklistID == checklist.ChecklistID).DefaultIfEmpty()
                              from newreview2 in reviews2.Where(r => r.ChecklistChecklistID == checklist.ChecklistID).DefaultIfEmpty()
                              from newreview3 in reviews3.Where(r => r.ChecklistChecklistID == checklist.ChecklistID).DefaultIfEmpty()
                              select new ReconciliationViewModel
                              {
                                  PublicationPublicationID = pubid,
                                  ChecklistID = checklist.ChecklistID,
                                  OptionType = checklist.OptionType,
                                  SectionNumber = checklist.SectionNumber,
                                  Section = checklist.Section,
                                  ItemNumber = checklist.ItemNumber,
                                  Item = checklist.Item,

                                  Review1ID = newreview1?.ReviewID ?? 0,
                                  Option1ID = newreview1 == null ? 0 : newreview1.OptionOptionID,
                                  Answer1 = newreview1 == null ? "" : (newreview1.Optoin == null ? "" : newreview1.Optoin.OptionName),
                                  Comments1 = newreview1 == null ? "" : newreview1.Comments,

                                  Review2ID = newreview2?.ReviewID ?? 0,
                                  Option2ID = newreview2 == null ? 0 : newreview2.OptionOptionID,
                                  Answer2 = newreview2 == null ? "" : (newreview2.Optoin == null ? "" : newreview2.Optoin.OptionName),
                                  Comments2 = newreview2 == null ? "" : newreview2.Comments,

                                  Review3ID = newreview3?.ReviewID ?? 0,
                                  Option3ID = newreview3 == null ? 0 : newreview3.OptionOptionID,

                                  //newreview3 == null ? 
                                  //(newreview1 == null ? 0 : (newreview2 == null ? 0 : (newreview1.OptionOptionID == newreview2.OptionOptionID ? newreview1.OptionOptionID : 0)))
                                  //: newreview3.OptionOptionID,

                                  Comments3 = newreview3?.Comments,
                              };

            TempData["1"] = new SelectList(_db.Options.Where(o => o.OptionType == "1"), "OptionID", "OptionName");
            TempData["2"] = new SelectList(_db.Options.Where(o => o.OptionType == "2"), "OptionID", "OptionName");
            TempData["3"] = new SelectList(_db.Options.Where(o => o.OptionType == "3"), "OptionID", "OptionName");
            TempData["4"] = new SelectList(_db.Options.Where(o => o.OptionType == "4"), "OptionID", "OptionName");
            TempData["5"] = new SelectList(_db.Options.Where(o => o.OptionType == "5"), "OptionID", "OptionName");
            TempData["6"] = new SelectList(_db.Options.Where(o => o.OptionType == "6"), "OptionID", "OptionName");
            TempData["7"] = new SelectList(_db.Options.Where(o => o.OptionType == "7"), "OptionID", "OptionName");
            TempData["8"] = new SelectList(_db.Options.Where(o => o.OptionType == "8"), "OptionID", "OptionName");
            TempData["9"] = new SelectList(_db.Options.Where(o => o.OptionType == "9"), "OptionID", "OptionName");
            TempData["10"] = new SelectList(_db.Options.Where(o => o.OptionType == "10"), "OptionID", "OptionName");
            TempData["11"] = new SelectList(_db.Options.Where(o => o.OptionType == "11"), "OptionID", "OptionName");
            TempData["12"] = new SelectList(_db.Options.Where(o => o.OptionType == "12"), "OptionID", "OptionName");
            TempData["13"] = new SelectList(_db.Options.Where(o => o.OptionType == "13"), "OptionID", "OptionName");
            TempData["14"] = new SelectList(_db.Options.Where(o => o.OptionType == "14"), "OptionID", "OptionName");
            TempData["15"] = new SelectList(_db.Options.Where(o => o.OptionType == "15"), "OptionID", "OptionName");
            TempData["16"] = new SelectList(_db.Options.Where(o => o.OptionType == "16"), "OptionID", "OptionName");
            TempData["17"] = new SelectList(_db.Options.Where(o => o.OptionType == "17"), "OptionID", "OptionName");

            return View(comparisons.ToList());
        }

        [Authorize(Roles = "Reconciler")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Reconciliation(IEnumerable<ReconciliationViewModel> reviews)
        {
            if (!ModelState.IsValid) return View("Error");

            if (!reviews.All(r => r.Review3ID > 0) && !reviews.All(r => r.Review3ID == 0))
            {

                ViewBag.ErrorMessage = "Reconsilation Reviews Contanminated. Maybe reviews have been changed after the creation of reconciliation. Please contact admin";
                return View("Error");
            }

            if (reviews.Any(r => r.Review3ID == 0))
            {
                var review3 = from r in reviews
                              select new Review
                              {
                                  PublicationPublicationID = r.PublicationPublicationID,
                                  ChecklistChecklistID = r.ChecklistID,
                                  UserID = Userid,
                                  OptionOptionID = r.Option3ID > 0 ? r.Option3ID : (r.Option1ID == r.Option2ID ? r.Option1ID : 0),
                                  Comments = r.Comments3,
                                  ReviewID = r.Review3ID,
                              };

                _db.Reviews.AddRange(review3);
            }
            else
            {
                foreach (var rev in reviews)
                {
                    var ExistingReview = _db.Reviews.Find(rev.Review3ID);
                    if (ExistingReview == null) continue;
                    ExistingReview.OptionOptionID = rev.Option3ID;
                    ExistingReview.Comments = rev.Comments3;
                }
            }
            _db.SaveChanges();

            return RedirectToAction("ReviewComparison");
        }

        [Authorize(Roles = "Reconciler")]
        public ActionResult ReconciliationComplete(int pubid)
        {
            var reviews = _db.Reviews.Where(r => r.PublicationPublicationID == pubid && r.ReviewCompletionReviewCompletionID == null && r.Status == "Current" && r.UserID == Userid).ToList();

            if (reviews.FirstOrDefault() == null) { ViewBag.ErrorMessage = "Model invalid."; return View("Error"); }

            var rc = new ReviewCompletion()
            {
                ComplesionDate = DateTime.UtcNow,
                Role = User.IsInRole("SeniorReviewer") ? NPQIP.Enums.userRoles.SeniorReviewer : NPQIP.Enums.userRoles.Reviewer,
                userID = Userid,
                PublicationPublicationID = reviews.FirstOrDefault().PublicationPublicationID
            };

            _db.ReviewCompletions.Add(rc);

            foreach (var rev in reviews)
            {
                rev.ReviewCompletion = rc;
            }

            _db.SaveChanges();

            return RedirectToAction("ReviewComparison");
        }

        public ActionResult ScoringBoard()
        {
            var score = new ScoringBoardViewModel(_db.UserProfiles.Where(up => !up.deleted).AsEnumerable());
            //&& !Roles.IsUserInRole(up.UserName, Enums.userRoles.Suspended.ToString()
            return View(score);
        }

        public ActionResult ChecklistReview()
        {
            var Reviews = (from r in _db.Publications.Where(p => p.DeletePublication != true && p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) >= 2).SelectMany(p => p.Reviews)
                           group r by new { r.ChecklistChecklistID, r.PublicationPublicationID } into pairedReviews
                           select new ChecklistPublicationReviewViewModel()
                           {
                               ChecklistID = pairedReviews.FirstOrDefault().ChecklistChecklistID,
                               SectionNumber = pairedReviews.FirstOrDefault().Checklist.SectionNumber,
                               Section = pairedReviews.FirstOrDefault().Checklist.Section,
                               ItemNumber = pairedReviews.FirstOrDefault().Checklist.ItemNumber,
                               ChecklistType = pairedReviews.FirstOrDefault().Checklist.ChecklistType,
                               OptionType = pairedReviews.FirstOrDefault().Checklist.OptionType,
                               Item = pairedReviews.FirstOrDefault().Checklist.Item,
                               Agreement = pairedReviews.Count() > 1 && (pairedReviews.Select(pr => pr.OptionOptionID).Min() == pairedReviews.Select(pr => pr.OptionOptionID).Max()),
                           }).ToList();

            var AgreementRateVM = from r in Reviews
                                  group r by r.ChecklistID into groupedReviews
                                  select new ChecklistReviewViewModel
                                  {
                                      ChecklistID = groupedReviews.FirstOrDefault().ChecklistID,
                                      SectionNumber = groupedReviews.FirstOrDefault().SectionNumber,
                                      Section = groupedReviews.FirstOrDefault().Section,
                                      ItemNumber = groupedReviews.FirstOrDefault().ItemNumber,
                                      ChecklistType = groupedReviews.FirstOrDefault().ChecklistType,
                                      OptionType = groupedReviews.FirstOrDefault().OptionType,
                                      Rate = (double)groupedReviews.Count(gr => gr.Agreement) / groupedReviews.Count(),
                                      Item = groupedReviews.FirstOrDefault().Item
                                  };

            return View(AgreementRateVM.OrderBy(ar => ar.ChecklistID).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        #region helper
              private IEnumerable<Publication> GetOngoingPublications(int userid, NPQIP.Enums.reviewPurpose purpose)
        {
            // publicatin that has been reviewed (started to review by more than two reviewers, at least one of them is a senior reviewer)
            return _db.Publications.Where(p => p.DeletePublication == false
            && (p.Training ^ purpose == Enums.reviewPurpose.Review)
            && p.Reviews.Any(r => r.Status == Enums.Status.Current.ToString() && r.UserID == userid 
            && (r.TrainingScoreTrainingScoreID == null && r.ReviewCompletionReviewCompletionID == null )));
        }

        private IEnumerable<Publication> GetAvaliablePublications(int userid, NPQIP.Enums.reviewPurpose purpose)
        {
            // publicatin that has been reviewed (started to review by more than two reviewers, at least one of them is a senior reviewer)
            if (purpose == NPQIP.Enums.reviewPurpose.Training)
                return _db.Publications.Where(p => p.DeletePublication == false && (p.Training ^ purpose == Enums.reviewPurpose.Review) &&
                    !p.Reviews.Any(r => r.Status == Enums.Status.Current.ToString() && r.UserID == userid));
            
            var username = _db.UserProfiles.Find(userid).UserName;

            //senior reviewer review publications that haven't been done
            if (Roles.IsUserInRole(username, "SeniorReviewer"))  
                {
                return _db.Publications.Where(p => p.DeletePublication == false && (p.Training ^ purpose == Enums.reviewPurpose.Review)
                && p.Reviews.All(r => r.Status != Enums.Status.Current.ToString()));
                }

            return _db.Publications.Where(p =>
                p.DeletePublication == false && (p.Training ^ purpose == Enums.reviewPurpose.Review)
                && !p.Reviews.Any(r => r.UserID == userid)
                && p.ReviewCompletions.Count(rc => rc.Status == Enums.Status.Current.ToString()) == 1
                &&
                p.Reviews.Where(r => r.Status == Enums.Status.Current.ToString())
                    .Select(r => r.UserID)
                    .Distinct()
                    .Count() == 1
                );
        }

        private IEnumerable<Publication> GetPublications(Enums.reviewPurpose purpose)
        {
            return _db.Publications.Where( p =>p.DeletePublication != true && (p.Training ^ purpose == Enums.reviewPurpose.Review));
        }
        
        private string FindMyLatestReview(IEnumerable<Review> reviews, int userid, NPQIP.Enums.reviewPurpose purpose)
        {
            var latestReview = new Review();

            if (purpose == Enums.reviewPurpose.Training)
            {
                latestReview = reviews
                    .Where(r => r.Status == Enums.Status.Current.ToString()
                                && r.UserID == userid
                                && r.Publication.Training                                
                    )
                    .OrderByDescending(r => r.LastUpdateTime).FirstOrDefault();
            }
            else
            {
                latestReview = reviews
                    .Where(r => r.Status == Enums.Status.Current.ToString()
                                && r.UserID == userid
                                && r.Publication.Training
                    )
                    .OrderByDescending(r => r.LastUpdateTime).FirstOrDefault();

            }
            return latestReview?.Publication.PublicationNumber.Substring(0, 3) ?? "";
        }

        public bool CanEditGoldStandard(int userid)
        {
            return userid == 1;
        }

        private static ReviewOverviewViewModel RandomReview(IEnumerable<ReviewOverviewViewModel> allReviews)
        {
            var reviewOverviewViewModels = allReviews as ReviewOverviewViewModel[] ?? allReviews.ToArray();
            return !reviewOverviewViewModels.Any() ? null : reviewOverviewViewModels.OrderBy(a => Guid.NewGuid()).FirstOrDefault();
        }

           private static Publication RandomPublication(IEnumerable<Publication> allPublications)
        {
            var reviewOverviewViewModels = allPublications as Publication[] ?? allPublications.ToArray();
            return !reviewOverviewViewModels.Any() ? null : reviewOverviewViewModels.OrderBy(a => Guid.NewGuid()).FirstOrDefault();
}
        #endregion
    }
}