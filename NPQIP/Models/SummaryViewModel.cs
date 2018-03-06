using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class SummaryViewModel
    {
        public double[,] ProgressArray { get; set; }
        public int NumCompletedBySeniorReviewer { get; set; }
        //public int NumReviewedBySeniorReviewer { get; set; }
        public int NumCompletedByExternalReviewer { get; set; }
        //public int NumReviewedByExternalReviewer { get; set; }
        //public int NumReviewedInReconciliation { get; set; }
        public int NumCompletedInReconciliation { get; set; }
        public int NumLeft => NumTotal - NumCompletedBySeniorReviewer - NumCompletedInReconciliation - NumCompletedByExternalReviewer
            //- NumReviewedByExternalReviewer            - NumReviewedBySeniorReviewer - NumReviewedInReconciliation
            ;
        public int NumTotal { private get; set; }

        private string NumCompletedBySeniorReviewerStr => (100.0 * NumCompletedBySeniorReviewer / NumTotal).ToString() + "%";

        private string NumCompletedByExternalReviewerStr => (100.0 * NumCompletedByExternalReviewer / NumTotal).ToString() + "%";

        private string NumCompletedInReconciliationStr => (100.0 * NumCompletedInReconciliation / NumTotal).ToString() + "%";

        public string NumCompletedBySeniorReviewerStyleStr => "width:" + NumCompletedBySeniorReviewerStr + ";";

        public string NumCompletedByExternalReviewerStyleStr => "width:" + NumCompletedByExternalReviewerStr + ";";

        public string NumCompletedInReconciliationStyleStr => "width:" + NumCompletedInReconciliationStr + ";";

        //private string NumReviewedBySeniorReviewerStr => (100.0*NumReviewedBySeniorReviewer/NumTotal).ToString() + "%";

        //private string NumReviewedByExternalReviewerStr => (100.0 * NumReviewedByExternalReviewer / NumTotal).ToString() + "%";

        //private string NumReviewedInReconciliationStr => (100.0 * NumReviewedInReconciliation / NumTotal).ToString() + "%";

        private string NumLeftStr => (100.0 * NumLeft / NumTotal).ToString() + "%";

        //public string NumReviewedBySeniorReviewerStyleStr => "width: " + NumReviewedBySeniorReviewerStr + "; min-width: 2em;";

        //public string NumReviewedByExternalReviewerStyleStr => "width: " + NumReviewedByExternalReviewerStr + "; min-width: 2em;";

        //public string NumReviewedInReconciliationStyleStr => "width: " + NumReviewedInReconciliationStr + "; min-width: 2em;";

        public string NumLeftStyleStr => "width: " + NumLeftStr + "";
    }
}