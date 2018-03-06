using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReconciliationViewModel
    {
      
        [DisplayName("Publication ID")]
        public int PublicationPublicationID { get; set; }

        public int ChecklistID { get; set; }

        public string SectionNumber { get; set; }

        public string Section { get; set; }

        public string ItemNumber { get; set; }

        public string Item { get; set; }

        public string OptionType { get; set; }

        public int Review1ID { get; set; }

        public int? Option1ID { get; set; }

        [DisplayName("Answer 1: ")]
        public string Answer1 { get; set; }

        [DisplayName("Comments 1: ")]
        [DataType(DataType.MultilineText)]
        public string Comments1 { get; set; }

        public int Review2ID { get; set; }

        public int? Option2ID { get; set; }

        [DisplayName("Answer 2: ")]
        public string Answer2 { get; set; }

        [DisplayName("Comments 2: ")]
        [DataType(DataType.MultilineText)]
        public string Comments2 { get; set; }

        public int Review3ID { get; set; }

        public int? Option3ID { get; set; }

        [DisplayName("Answer 3: ")]
        public string Answer3 { get; set; }

        [DisplayName("Comments 3: ")]
        [DataType(DataType.MultilineText)]
        public string Comments3 { get; set; }

        public bool Agree { 
            
            get {
                return Option1ID == Option2ID;      
        } 
        }



    }
}