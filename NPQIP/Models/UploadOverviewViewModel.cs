using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class UploadOverviewViewModel 
    {
        [DisplayName("Nature Publication")]
        public string PublicationNumber { get; set; }

        public int PublicationID { get; set; }
              
        [DisplayName("Publication Updated On")]
        public DateTime LastUpdateTime { get; set; }

        [DisplayName("Publication Details")]
        public int PMID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DisplayName("Uploaded Files")]
        public int NumberOfFiles { get; set; }

        [DisplayName("Experiment Type")]
        public string ExperimentType { get; set; }

        public string Country { get; set; }

        [DisplayName("Publication Date")]
        public DateTime PublicationDate { get; set; }

        public string Species { get; set; }

        public string Keywords { get; set; }

    }
}