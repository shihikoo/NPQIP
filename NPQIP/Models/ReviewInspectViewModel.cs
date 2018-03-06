using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NPQIP.ViewModel
{
    public class ReviewInspectViewModel
    {
        public int PublicationID { get; set; }

        public string  PublicationNumber { get; set; }

        public int ReviewCompleted { get; set; }

        public int ReviewStarted { get; set; }

        public string ReviewedBy { get; set; }

        public bool PossibleError { get; set; }

    }
}
