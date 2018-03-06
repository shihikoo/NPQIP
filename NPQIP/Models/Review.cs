using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class Review
    {
        public Review(int userid, int publicationid, int checklistid) : this(userid, publicationid, checklistid, null, null)
        {
        }

        public Review(int userid, int publicationid, int checklistid, int? optionid) : this(userid, publicationid, checklistid, optionid, null)
        {
        }

        public Review(int userid, int publicationid, int checklistid, int? optionid, string comments) : this()
        {
            UserID = userid;

            PublicationPublicationID = publicationid;

            ChecklistChecklistID = checklistid;

            OptionOptionID = optionid;

            Comments = comments;

            LastUpdateTime = DateTime.Now;
    }

        public Review()
        {
            Status = "Current";
        }

        [Key]
        public int ReviewID { get; set; }

        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        [ForeignKey("Publication")]
        public int PublicationPublicationID { get; set; }

        [ForeignKey("ReviewCompletion")]
        public int? ReviewCompletionReviewCompletionID { get; set; }

         [ForeignKey("Checklist")]
        public int ChecklistChecklistID { get; set; }

        [ForeignKey("Optoin")]
        public int? OptionOptionID { get; set; }

        [ForeignKey("TrainingScore")]
        public int? TrainingScoreTrainingScoreID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public string Status { get; set; }

        private DateTime? lastUpdateTime { get; set; }
        [Required]
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual Checklist Checklist { get; set; }

        public virtual Publication Publication { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual Option Optoin { get; set; }

        public virtual TrainingScore TrainingScore { get; set; }

        public virtual ReviewCompletion ReviewCompletion { get; set; }


        public void ArchiveReview() {
            Status = Enums.Status.Archived.ToString();
            lastUpdateTime = DateTime.Now;
        }

    }
}