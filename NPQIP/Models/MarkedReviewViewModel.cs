using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class MarkedReviewViewModel
    {

        public int ReviewID { get; set; }

        [DisplayName("Publication ID")]
        public int PublicationPublicationID { get; set; }

        public int ChecklistID { get; set; }

        public string SectionNumber { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string OptionType { get; set; }

        public int? OptionOptionID { get; set; }
        [DisplayName("Explanation: ")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DisplayName("Correct Answer: ")]

        public string CorrectAnswer { get; set; }
        [DisplayName("Your Answer: ")]

        public string YourAnswer { get; set; }
        [DisplayName("Your Comments: ")]
        public string YourComments { get; set; }

        public bool Correct { get; set; }
    }
}