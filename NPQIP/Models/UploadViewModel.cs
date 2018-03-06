using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class UploadViewModel
    {
        public int PublicationPublicationID { get; set; }

        public int FileID { get; set; }

        [DisplayName("Publication Number")]
        public string PublicationNumber { get; set; }

        [DisplayName("PMID")]
        public int PMID { get; set; }

        [DisplayName("File Name")]
        [RegularExpression(@"([a-zA-Z 0-9.&'-]+)", ErrorMessage = "File Name should be alphanumeric.")]
        public string FileName { get; set; }

        [DisplayName("File Type")]
        public string FileType { get; set; }

        [DisplayName("File Type")]
        public string FileExtention { get; set; }

        [DisplayName("Comments on publication")]
        [DataType(DataType.MultilineText)]
        public string PubComments { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [DisplayName("File")]
        public HttpPostedFileBase FileUpload { get; set; }

        [DataType(DataType.Url)]
        public string FileUrl { get; set; }

        [DisplayName("Upload User")]
        public string EntryUser { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DisplayName("Update Time")]
        public DateTime LastUpdateTime { get; set; }

        public Boolean DeletePaperDocument { get; set; }


    }
}