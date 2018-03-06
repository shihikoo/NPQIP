using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReviewOverviewViewModel
    {
        public ReviewOverviewViewModel(Publication publication, int userid)
            : this(
                publication,
                publication.Reviews.Where(r => r.Status == Enums.Status.Current.ToString() && r.UserID == userid)
                )
        {
        }

        public ReviewOverviewViewModel(Publication publication, IEnumerable<Review> reviews)
        {
            PublicationVM = new PublicationViewModel(publication);

            Reviews = reviews;
        }

        //publication details
        public PublicationViewModel PublicationVM { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

        public int NumberOfCompletedChecklist => Reviews.Count(r => r.OptionOptionID != null);

        [DisplayName("Study Content")]
        public int NumberOfCompletedChecklist0 => Reviews.Count(r => r.OptionOptionID != null && r.Checklist.SectionNumber == "0");

        [DisplayName("1-6 (in vitro)")]
        public int NumberOfCompletedChecklist16 => Reviews.Count(r => r.OptionOptionID != null && r.Checklist.ChecklistType == "in vitro");

        [DisplayName("1-7 (in vivo)")]
        public int NumberOfCompletedChecklist17 => Reviews.Count(r => r.OptionOptionID != null && r.Checklist.ChecklistType == "in vivo");

        [DisplayName("8")]
        public int NumberOfCompletedChecklist8 => Reviews.Count(r => r.OptionOptionID != null && r.Checklist.SectionNumber == "8");

        [DisplayName("9")]
        public int NumberOfCompletedChecklist9 => Reviews.Count(r => r.OptionOptionID != null && r.Checklist.SectionNumber == "9");

        [DisplayName("10")]
        public int NumberOfCompletedChecklist10 => Reviews.Count(r => r.OptionOptionID != null && r.Checklist.SectionNumber == "10");

        public int NumberOfChecklist => Reviews.Count();

        public int NumberOfChecklist0 => (int) Enums.NumberOfCompletedChecklist.StudyContent;

        public int NumberOfChecklist16 => (int)Enums.NumberOfCompletedChecklist.inVitro;

        public int NumberOfChecklist17 => (int)Enums.NumberOfCompletedChecklist.inVivo;

        public int NumberOfChecklist8 => (int) Enums.NumberOfCompletedChecklist.human;

        public int NumberOfChecklist9 => (int)Enums.NumberOfCompletedChecklist.data;

        public int NumberOfChecklist10 => (int)Enums.NumberOfCompletedChecklist.computerCode;

        [DisplayName("Study Type")]
        public string ExperimentTarget => !Reviews.Any() ? "": Reviews.LastOrDefault(r => r.Checklist.ItemNumber == "0.1" && r.OptionOptionID != null)?.Optoin.OptionName.ToLower().Trim();

        [DisplayName("Study Content")]
        public string ExperimentTarget8 => !Reviews.Any() ? "" : Reviews.LastOrDefault(r => r.Checklist.ItemNumber == "0.2" && r.OptionOptionID != null)?.Optoin.OptionName.ToLower().Trim();

        public string ExperimentTarget10 => !Reviews.Any() ? "" : Reviews.LastOrDefault(r => r.Checklist.ItemNumber == "0.3" && r.OptionOptionID != null)?.Optoin.OptionName.ToLower().Trim();

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Updated On")]
        public DateTime CompleteTime
            => Reviews.OrderBy(r => r.LastUpdateTime).Select(r => r.LastUpdateTime).LastOrDefault();

    }
}