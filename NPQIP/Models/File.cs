using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

namespace NPQIP.Models
{
    public class File
    {
        [Key]
        public int FileID { get; set; }

        public int? PublicationPublicationID { get; set; }

        public string PublicationPublicationNumber { get; set; }

        public string EntryUser { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public string FileExtention { get; set; }

        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        public Boolean DeleteFile { get; set; }

        [DataType(DataType.Url)]
        public string FileUrl { get; set; }

        private DateTime? lastUpdateTime;
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime LastUpdateTime
        {
            get { return lastUpdateTime ?? DateTime.Now; }
            set { lastUpdateTime = value; }
        }

        public virtual Publication Publications { get; set; }
    }
    }