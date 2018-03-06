using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using NPQIP.Models;
namespace NPQIP.ViewModel
{
    public class FrontPageViewModel
    {

        public FrontPageViewModel()
        {

        }

        [DisplayName("ID")]
        public int PublicationID { get; set; }

        public string PublicationNumber { get; set; }

        [DisplayName("PMID")]
        public int PMID { get; set; }

        public ICollection<FrontPageReview> FrontPageReviews {get;set;}

        public bool Training { get; set; }
               
    }
}