using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class FileViewModel
    {
        public int FileID { get; set; }

        public int? PublicationID { get; set; }

        public string PublicationNumber { get; set; }

        public string FileName { get; set; }

        public string FileUrl { get; set; }
    }
}