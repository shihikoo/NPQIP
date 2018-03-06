using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class FrontPageReview
    {
        public FrontPageReview()
        {
            Status = "Current";
        }

        [Key]
        public int FrontPageReviewID { get; set; }

        [ForeignKey("Publication")]
        public int PublicationID { get; set; }

        [ForeignKey("UserProfile")]
        public int UserID { get; set; }

        [ForeignKey("Checklist")]
        public int ChecklistID { get; set; }

        [ForeignKey("Optoin")]
        public int? OptionID { get; set; }

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
    }
}