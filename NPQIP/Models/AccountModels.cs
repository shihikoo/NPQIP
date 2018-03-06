using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NPQIP.Models
{
    [Table("UserProfile")]
    public class UserProfile
    {
        public UserProfile()
        {
            Reviews = new HashSet<Review>();

            FrontPageReviews = new HashSet<FrontPageReview>();

            TrainingScores = new HashSet<TrainingScore>();

            Promotions = new HashSet<Promotion>();

            ReviewCompletions = new HashSet<ReviewCompletion>();
        }
        
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }

        public string ForeName { get; set; }

        public string SurName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string Institution { get; set; }

        public string Details { get; set; }

        public DateTime LastHeartbeat { get; set; }

        public bool CurrentlyLogged { get; set; }

        [DisplayName("Registration Date")]
        public DateTime? CreateDate { get; set; }

        public bool deleted { get; set; }
        public void SetDefautvalue()
        {
            deleted = false;
        }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<TrainingScore> TrainingScores { get; set; }

        public virtual ICollection<Promotion> Promotions { get; set; }

        public virtual ICollection<ReviewCompletion> ReviewCompletions { get; set; }

        public virtual ICollection<FrontPageReview> FrontPageReviews { get; set; }

    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage="Username is required")]
        [Display(Name = "Username *")]
        [MinLength(1, ErrorMessage = "Username must be longer than 1 digit")]
        [RegularExpression(@"([a-zA-Z0-9.&'-]+)", ErrorMessage = "Username should be alphanumeric with no space.")]
        //[Remote("ValidateUserName", "Record", ErrorMessage = "This Username is already in the record. ")]       
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password *")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Forename")]
        public string ForeName { get; set; }

        [DisplayName("Surname")]
        public string SurName { get; set; }

        [Required]
        //[DataType(DataType.EmailAddress,ErrorMessage = "Valid Message is required")]
        [EmailAddress]
        [Display(Name = "Email *")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Confirm Email *")]
        [System.ComponentModel.DataAnnotations.Compare("Email", ErrorMessage = "The emails you've entered do not match.")]
        public string ConfirmEmail { get; set; }

        public string Institution { get; set; }

        [DisplayName("Notes")]
        public string Details { get; set; }

        [DisplayName("Registration Date")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }

    public class LostPasswordModel
    {
        [Required(ErrorMessage = "We need your email to send you a reset link!")]
        [Display(Name = "Your account email")]
        [EmailAddress(ErrorMessage = "Not a valid email--what are you trying to do here?")]
        [DataType(DataType.Password)]
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "New password and confirmation does not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ReturnToken { get; set; }
    }
}
