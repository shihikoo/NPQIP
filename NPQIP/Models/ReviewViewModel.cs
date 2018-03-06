using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReviewViewModel
    {
        public int ReviewID { get; set; }

        [DisplayName("Publication ID")]
        public int PublicationPublicationID { get; set; }

        //[DisplayName("PMID")]
        //public int PMID { get; set; }

        //[DisplayName("Publication Number")]
        //public string PublicationNumber { get; set; }

        public int ChecklistID { get; set; }

        public string SectionNumber { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string Resource { get; set; }

        public string OptionType { get; set; }

        public int? OptionOptionID { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public DateTime LastUpdateTime { get; set; }

        public string UserName { get; set; }

        public string ChecklistType { get; set; }
    }
}