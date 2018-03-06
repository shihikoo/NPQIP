using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPQIP.Models
{
    public class TrainingCoverViewModel
    {

        public TrainingCoverViewModel(int totalNum, int ongoingNum, int completedNum)
        {
            TotalNum = totalNum;
            OngoingNum = ongoingNum;
            CompletedNum = completedNum;

        }

        public TrainingCoverViewModel()
        {
        }

        public int TotalNum { get; private set; }

        public int AvaliableNum => TotalNum - CompletedNum - OngoingNum;

        public int OngoingNum { get; private set; }

        public int CompletedNum { get; private set; }


        public string OngoingTrainingStyle => OngoingNum == 0 ? "disabled" : "";

        public string NewPublicationStyle =>  AvaliableNum == 0 ? "disabled" : "";

        public string ViewPreviouseRecordStyle => CompletedNum == 0 ? "disabled" : "";

        public string ArchivePublicationStyle => (CompletedNum > 0 && AvaliableNum <= 2) ? "" : "disabled";

        public string NewTrainingText => AvaliableNum == 0 ? "No More New Training" : "New Training &nbsp  <i class='fa fa-angle-double-right'>&nbsp </i>";



    }
}
