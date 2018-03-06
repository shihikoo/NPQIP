using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ChecklistPublicationReviewViewModel
    {

        public int ChecklistID { get; set; }

        public string SectionNumber { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string ChecklistType { get; set; }

        public string OptionType { get; set; }

        public bool Agreement { get; set; }
    }
}