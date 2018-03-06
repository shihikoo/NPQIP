using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NPQIP.Models
{
    public class Publication
    {
        public Publication()
        {
            Reviews = new HashSet<Review>();
            FrontPageReviews = new HashSet<FrontPageReview>();
            Files = new HashSet<File>();
            TrainingScores = new HashSet<TrainingScore>();
            DeletePublication = false;
            ReviewCompletions = new HashSet<ReviewCompletion>();
        }

        public int PublicationID { get; set; }

        [DisplayName("Publication Number")]
        public string PublicationNumber { get; set; }

        public int PMID { get; set; }

        public string ExperimentType { get; set; }

        [ForeignKey("Option")]
        public int? ExperimentTypeByReviewer { get; set; }

        public string Country { get; set; }

        public DateTime PublicationDate { get; set; }

        public string Species { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public bool DeletePublication { get; set; }

        private DateTime? lastUpdateTime;
        [Required]
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public bool Training { get; set; }

        // virtual
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<TrainingScore> TrainingScores { get; set; }

        public virtual ICollection<File> Files { get; set; }

        public virtual ICollection<ReviewCompletion> ReviewCompletions { get; set; }

        public virtual Option Option { get; set; }

        public virtual ICollection<FrontPageReview> FrontPageReviews { get; set; }



        #region Public Methods

        //public bool ForTraining(userId)
        //{
        //    return (PublicationID >= 36 && PublicationID <= 65);
        //}

        #endregion

    }
}