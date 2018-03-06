using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class TrainingScoreViewModel
    {

        public TrainingScoreViewModel(Publication publication, int userid)
    : this(publication.TrainingScores.FirstOrDefault(rc => rc.Status == Enums.Status.Current.ToString() && rc.UserID == userid))
        {
        }

        public TrainingScoreViewModel(TrainingScore trainingScore) 
            : this(trainingScore.PublicationPublicationID, trainingScore.TrainingScoreID, trainingScore.Points, trainingScore.Pass,trainingScore.ScoreTime,trainingScore.TotalPoints)
        {
        }

        public TrainingScoreViewModel(int publicationID, int trainingScoreID, int points, bool pass, DateTime scoreTime, int totalPoints)
        {
            PublicationPublicationID = publicationID;
            TrainingScoreID = trainingScoreID;
            Points = points;
            Pass = pass;
            ScoreTime = scoreTime;
            TotalPoints = totalPoints;
        }

        public TrainingScoreViewModel()
        {
        }

        [DisplayName("Publication ID")]
        public int PublicationPublicationID { get; set; }

        public int TrainingScoreID { get; set; }

        public int Points { get; set; }

        public bool Pass { get; set; }

        public DateTime ScoreTime { get; set; }

        [DisplayName("Total Points")]
        public int TotalPoints { get; set; }

        //public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:P2}")]
        public double Score => (double) Points/(double) TotalPoints;

        public bool IsReviewer { get; set; }

        public bool Promoted { get; set; }

        public Promotion promotion { get; set; }
    }
}