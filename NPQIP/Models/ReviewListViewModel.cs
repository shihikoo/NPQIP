using NPQIP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class ReviewListViewModel
    {

        public ReviewListViewModel(int publicationID, DateTime updateTime)
        {
            PublicationID = publicationID;

            UpdateTime = updateTime;
        }

        public ReviewListViewModel()
        {

        }

        public int PublicationID { get; private  set ; }   

        [DisplayFormat(DataFormatString = "{0:d}")]
        [DisplayName("Updated On")]
        public DateTime UpdateTime { get; private set; }

    }
}