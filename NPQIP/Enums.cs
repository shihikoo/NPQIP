using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP
{
    public static class Enums
    {
        public enum reviewPurpose
        {
            Review,
            Training
        }

        public enum checklistsection
        {
        StudyContent,
        inVivo,
        inVitro,
        human,
        data,
        computerCode
        }


        public enum userRoles
        {
            NA,
            Administrator,
            Reviewer,
            Uploader,
            Trainee,
            SeniorReviewer,
            Suspended,

        }

        public enum OngoingPublicationLimit
        {
            Number = 5
        }

        public enum Status
        {
            Current,
            Archived,
            Deleted
        }

        public enum NumberOfCompletedChecklist
        {
            StudyContent = 4,
            inVitro = 25,
            inVivo = 28,
            human = 6,
            data = 10,
            computerCode = 5, 
        }

        public enum TrainingPublicationIDs
        {
           Min = 36,
           Max = 65
        }
        public enum PassingCriteria
        {
            Percentage = 80,
            Times = 3
        }

        public enum GoldStandard
        {
            UserId = 1
        }

    }
}