using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ChecklistReviewViewModel
    {

        public int ChecklistID { get; set; }

        public string SectionNumber { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string ChecklistType { get; set; }

        public string OptionType { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public double Rate { get; set; }
    }
}