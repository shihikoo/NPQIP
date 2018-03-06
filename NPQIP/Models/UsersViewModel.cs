using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace NPQIP.ViewModel
{
    public class UsersViewModel
    {
        [DisplayName("ID")]
        public int UserId { get; set; }
        [DisplayName("Username")]
        public string Username { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Institution { get; set; }

        public string Details { get; set; }

        [DisplayName("Registration Date")]
        public DateTime? CreateDate { get; set; }

        public string Roles { get; set; }

        [DisplayName("Restriction")]
        public bool suspended { get; set; }

        [DisplayName("Training")]
        public bool TrainingStarted { get; set; }

        public bool TrainingCompleted { get; set; }
    }
}