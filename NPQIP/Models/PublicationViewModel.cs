using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using NPQIP.Models;

namespace NPQIP.ViewModel
{
    public class PublicationViewModel
    {

        public PublicationViewModel(Publication publication)
        {
            PublicationID = publication.PublicationID;
            PublicationNumber = publication.PublicationNumber;
            PMID = publication.PMID;
            ExperimentType = publication.ExperimentType;
            Country = publication.Country;
            PublicationDate = publication.PublicationDate;
            Species = publication.Species;
            Comments = publication.Comments;
            Training = publication.Training;
        }

        public PublicationViewModel()
        {
        }

        [DisplayName("ID")]
        public int PublicationID { get; set; }

        [DisplayName("Publication Number")]
        public string PublicationNumber { get; set; }

        public int PMID { get; set; }

        [DisplayName("in vivo / in vitro / both")]
        public string ExperimentType { get; set; }

        public string Country { get; set; }

        [DisplayName("Publication Date")]
        public DateTime PublicationDate { get; set; }

        public string Species { get; set; }

        //public string Keywords { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public bool Training { get; set; }
    }
}