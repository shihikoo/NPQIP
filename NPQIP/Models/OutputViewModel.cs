using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class OutputViewModel
    {
        public OutputViewModel(Review Review) : this() {
            ReviewID = Review.ReviewID;
            UserID = Review.UserID;
            PublicationPublicationID = Review.PublicationPublicationID;
            ReviewCompletionReviewCompletionID = Review.ReviewCompletionReviewCompletionID;
            ChecklistChecklistID = Review.ChecklistChecklistID;
            Comments = Review.Comments;
            Optoin = Review.Optoin == null ? "NULL": Review.Optoin.OptionName;
            PubDate = Review.Publication.PublicationDate;
            Country = Review.Publication.Country;
            PMID = Review.Publication.PMID;
            Category = Review.Publication.PublicationNumber;
        }

        public OutputViewModel()
        {
        }

        [DisplayName("Id")]
        public int ReviewID { get; set; }

        [DisplayName("UserId")]
        public int UserID { get; set; } 

        [DisplayName("PublicationId")]
        public int PublicationPublicationID { get; set; }

        [DisplayName("ReviewGroupID")]
        public int? ReviewCompletionReviewCompletionID { get; set; }
        [DisplayName("ChecklistID")]
        public int ChecklistChecklistID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public string Optoin { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PubDate { get; set; }

        public string Country { get; set; }

        public int PMID { get; set; }

        public string Category { get; set; }

    }
}