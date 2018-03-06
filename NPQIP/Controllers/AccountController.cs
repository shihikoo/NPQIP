using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using NPQIP.Models;
using System.Net.Mail;
using System.Web.UI;
using System.Data.Entity;
using NPQIP.ViewModel;
using NPQIP.Authorization;

namespace NPQIP.Controllers
{
    [Authorize]
    [Restrict("Suspended")]
    public class AccountController : Controller
    {
        private readonly NPQIPContext _db = new NPQIPContext();

        //
        // GET: /Account/Users
        [Authorize(Roles = "Administrator")]
        public ActionResult Users(string sortOrder = "CreateDate desc")
        {
            //var users = db.UserProfiles;
            var result = _db.UserProfiles.Where(r => r.deleted != true).AsEnumerable()
                .Select(u => new UsersViewModel { 
                    Username = u.UserName,
                    Institution = u.Institution,
                    Name = u.ForeName + " " + u.SurName,
                    CreateDate = u.CreateDate,
                    UserId = u.UserId,
                    Roles = string.Join(", ", Roles.GetRolesForUser(u.UserName)),
                    suspended = Roles.GetRolesForUser(u.UserName).Contains("Suspended"),
                    Email = u.Email,
                    TrainingStarted = u.Reviews.Any(r => r.Status == Enums.Status.Current.ToString()),
                    TrainingCompleted = u.Promotions.Any(p => p.status == Enums.Status.Current.ToString())

                });
    
              switch (sortOrder)
              {
                  case "UserName":
                      result = result.OrderBy(s => s.Username);
                      break;
                  case "UserName desc":
                      result = result.OrderByDescending(s => s.Username);
                      break;
                  case "Name":
                      result = result.OrderBy(s => s.Name);
                      break;
                  case "Name desc":
                      result = result.OrderByDescending(s => s.Name);
                      break;
                  case "Institution":
                      result = result.OrderBy(s => s.Institution);
                      break;
                  case "Institution desc":
                      result = result.OrderByDescending(s => s.Institution);
                      break;
                case "Email":
                    result = result.OrderBy(s => s.Email);
                    break;
                case "Email desc":
                    result = result.OrderByDescending(s => s.Email);
                    break;
                case "Roles":
                      result = result.OrderBy(s => s.Roles);
                      break;
                  case "Roles desc":
                      result = result.OrderByDescending(s => s.Roles);
                      break;
                  case "suspended":
                      result = result.OrderBy(s => s.suspended);
                      break;
                  case "suspended desc":
                      result = result.OrderByDescending(s => s.suspended);
                      break;
                  case "CreateDate":
                      result = result.OrderBy(s => s.CreateDate);
                      break;
                  case "CreateDate desc":
                      result = result.OrderByDescending(s => s.CreateDate);
                      break;
                case "Training":
                    result = result.OrderBy(s => s.TrainingCompleted).ThenBy(s => s.TrainingStarted);
                    break;
                case "Training desc":
                    result = result.OrderByDescending(s => s.TrainingCompleted).ThenByDescending(s => s.TrainingStarted);
                    break;
                default:
                      result = result.OrderBy(s => s.CreateDate);
                      break;
              }
              ViewBag.sortOrder = sortOrder;

            return View(result.ToList());
        }

        public ActionResult UserProfile()
        {
            int userid = (int)Membership.GetUser().ProviderUserKey;
            
            UserProfile user = _db.UserProfiles.Find(userid);

            return View(user);
        }

        //
        //GET: /Account/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!User.IsInRole("Administrator") || id == 0)
            {
                id = (int)Membership.GetUser().ProviderUserKey;
            }

            UserProfile userprofile = _db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }

            if (userprofile.deleted == true) {
                return HttpNotFound();
            }

            return View(userprofile);
        }

        //
        // POST: /Review/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile userprofile)
        {
            int userid = (int)Membership.GetUser().ProviderUserKey;

            if (userid != userprofile.UserId && !User.IsInRole("Administrator")) return View("Error");

            if (ModelState.IsValid)
            {
                _db.Entry(userprofile).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index","Home");
            }

            return View(userprofile);
        }

        //
        //POST: /Account/SuspendUser/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult SuspendUser(int id)
        {
            var userprofile = _db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            if (userprofile.deleted == true)
            {
                return HttpNotFound();
            }

            Roles.AddUserToRole(userprofile.UserName, "Suspended");

            return RedirectToAction("Users");
        }

        //
        //POST: /Account/UnSuspendUser/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult UnSuspendUser(int id)
        {
            var userprofile = _db.UserProfiles.Find(id);

            if (userprofile == null)
            {
                return HttpNotFound();
            }
            if (userprofile.deleted == true)
            {
                return HttpNotFound();
            }

            Roles.RemoveUserFromRole(userprofile.UserName, "Suspended");

            return RedirectToAction("Users");
        }

        //
        //GET: /Account/DELETE
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0)
        {
            UserProfile userprofile = _db.UserProfiles.Find(id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            if (userprofile.deleted == true) 
            {
             return HttpNotFound();
            }
            return View(userprofile);
        }

        //
        //POST: /Account/DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteUser(UserProfile userprofile)
        {

            if (userprofile == null)
            {
                return HttpNotFound();
            }
            if (userprofile.deleted == true)
            {
                return HttpNotFound();
            }
            foreach (var role in Roles.GetRolesForUser(userprofile.UserName)) Roles.RemoveUserFromRole(userprofile.UserName, role);
            System.Web.Security.Membership.DeleteUser(userprofile.UserName);

            return RedirectToAction("Users");
        }

        //
        // GET: /Account/Roles/5

        [Authorize(Roles = "Administrator")]
        public ActionResult ManageRoles(int id=0)
        {
            UserProfile userprofile = _db.UserProfiles.Find(id);

            if (userprofile == null | userprofile.deleted == true)
            {
                return HttpNotFound();
            }

            ViewBag.UserName = userprofile.UserName;
            ViewBag.UserId = userprofile.UserId;
            if (TempData["ResultMessage"] != null) ViewBag.ResultMessage = TempData["ResultMessage"];

            string[] RolesForThisUser = Roles.GetRolesForUser(userprofile.UserName);
            ViewBag.RolesForThisUser = RolesForThisUser;

            SelectList allroles = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = allroles;

            return View();
        }

        //
        // GET: /Account/Roles/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult AddRoles(string RoleName, string UserName, string UserId)
        {
            if (RoleName != "")
            {
                if (Roles.IsUserInRole(UserName, RoleName))
                {
                    TempData["ResultMessage"] = "This user is already in this role.";
                }
                else
                {
                    Roles.AddUserToRole(UserName, RoleName);
                    TempData["ResultMessage"] = "Role added for this user successfully!";
                }
            }
            //return View("ManageRoles");
            return RedirectToAction("ManageRoles", new { id = Convert.ToInt32(UserId) });     
        }

        //
        // POST: /Review/Delete/5

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteRole(string UserId, string UserName, string RoleName)
        {
            if (Roles.IsUserInRole(UserName, RoleName))
            {
                Roles.RemoveUserFromRole(UserName, RoleName);
                TempData["ResultMessage"] = "Role removed from this user successfully!";
            }
            else
            {
                TempData["ResultMessage"] = "This user doesn't belong to selected role.";
            }

            ViewBag.RolesForThisUser = Roles.GetRolesForUser(UserName);
            SelectList list = new SelectList(Roles.GetAllRoles());
            TempData["AllRoles"] = list;
            ViewBag.Roles = list;

            //return View("ManageRoles"); 

            return RedirectToAction("ManageRoles", new { id = Convert.ToInt32(UserId) });
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewBag.ip = logUserAccess(model.UserName);

            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }
        
        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/LostPassword

        [AllowAnonymous]
        public ActionResult LostPassword()
        {
            ViewBag.sent = "no";
            return View();
        }

        //
        // POST: Account/LostPassword

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LostPassword(LostPasswordModel model)
        {
            if (ModelState.IsValid)
            { 
                MembershipUser user;
                using (var context = new NPQIPContext())
                {
                var foundUserName = (from u in context.UserProfiles
                                         where u.Email == model.Email
                                         select u.UserName).FirstOrDefault();

                    if(foundUserName != null)
                    {
                        user = Membership.GetUser(foundUserName.ToString());
                    }
                    else{
                        user = null;
                    }
                }
                    if(user !=null)
                    {
                    // Generae password token that will be used in the email link to authenticate user
                        var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                    // Generate the html link sent via email
                        string resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { rt = token }, "http") + "'>Reset Password Link</a><br/>";

                        MailMessage mail = new MailMessage();

                        mail.To.Add(new MailAddress(model.Email));
                        mail.From = new MailAddress("npqip.team@ed.ac.uk");
                        mail.Bcc.Add(new MailAddress("multipart.camarades@gmail.com"));
                        mail.Subject = "Reset your password for NPQIP website";
                        mail.Body = "Someone has requrest to reset password for this account on NPQIP website. Please ignore this email if the request was not sent by you. <br/> If you want to reset your password, please click on the link: " + resetLink +"<br>,<br/> Thank you, <br/>NPQIP Group";

                        mail.IsBodyHtml = true;

                        SmtpClient smtp = new SmtpClient();

                         // Attempt to send the email
                         try
                         {
                             smtp.Send(mail);
                         }
                         catch (Exception e)
                         {
                             ModelState.AddModelError("", "Issue sending email: " + e.Message + "<br/>");
                         }
                    }            
                }
            ViewBag.sent = "yes";
             return View(model);
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string rt)
        {
            ViewBag.Message = "";
            ResetPasswordModel model = new ResetPasswordModel();
            model.ReturnToken = rt;
            return View(model);
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                bool resetResponse = WebSecurity.ResetPassword(model.ReturnToken, model.Password);
                if (resetResponse)
                {
                    ViewBag.Message = "Successfully Changed";
                }
                else
                {
                    ViewBag.Message = "Something went wrong!";
                }
            }
            return View(model);
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {   
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                 //Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password, 
                        new { Email = model.Email
                        , ForeName = model.ForeName
                    ,SurName=model.SurName
                    ,Institution = model.Institution
                    ,Details = model.Details
                    ,CreateDate = DateTime.Now
                    });

                    Roles.AddUserToRole(model.UserName, "Trainee");

                    WebSecurity.Login(model.UserName, model.Password, true);

                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                    //model.RoleName = RoleName;
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }
    
        //
        // GET: /Account/Manage
        //[Authorize(Roles = "Administrator")]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrator")]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public int Heartbeat(string userName)
        {
                var user = (from u in _db.UserProfiles where userName == u.UserName select u)
                            .FirstOrDefault();
                if (user != null)
                {
                    user.LastHeartbeat = DateTime.Now;
                    user.CurrentlyLogged = true;
                    var count = _db.SaveChanges();
                    return count;
                }
            return 0;
        }

        public ActionResult ActiveUsers()
        {
            return View();
        }

        public string LoadOnlineUsers()
        {
            var onlineusers = LoadActiveUsers();

            string table = "<table class='table table-bordered table-striped '> <thead> <tr> <th>User </th> <th>Latest Active Time</th></tr></thead><tbody id='user'>";
                
            foreach( var user in onlineusers)
            {
                 table += "<tr><td>" + user.UserName + "</td>";
                 table += "<td>" + user.LastHeartbeat + "</td></tr>";
            }

            table += "</tbody></table>";

            return table;

        }

        public int NumberOfActiveUsers()
        {
            return LoadActiveUsers().Count();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers

        public IEnumerable<UserProfile> LoadActiveUsers()
        {
            Int32 expirationTime = 60*60;
            var allusers = _db.UserProfiles;
            foreach (var user in allusers)
            {
                if ((DateTime.Now - user.LastHeartbeat).TotalSeconds > expirationTime) user.CurrentlyLogged = false;

            }
            _db.SaveChanges();
            var onlineusers = allusers.Where(a => a.CurrentlyLogged == true);
            return onlineusers;
        }

        [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public JsonResult ValidateUserName(string UserName)
        {
            return Json(!_db.UserProfiles.Any(m => m.UserName == UserName), JsonRequestBehavior.AllowGet);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        private static bool IsPrivateIpAddress(string ipAddress)
        {
            // http://en.wikipedia.org/wiki/Private_network
            // Private IP Addresses are: 
            //  24-bit block: 10.0.0.0 through 10.255.255.255
            //  20-bit block: 172.16.0.0 through 172.31.255.255
            //  16-bit block: 192.168.0.0 through 192.168.255.255
            //  Link-local addresses: 169.254.0.0 through 169.254.255.255 (http://en.wikipedia.org/wiki/Link-local_address)

            var ip = System.Net.IPAddress.Parse(ipAddress);
            var octets = ip.GetAddressBytes();

            var is24BitBlock = octets[0] == 10;
            if (is24BitBlock) return true; // Return to prevent further processing

            var is20BitBlock = octets[0] == 172 && octets[1] >= 16 && octets[1] <= 31;
            if (is20BitBlock) return true; // Return to prevent further processing

            var is16BitBlock = octets[0] == 192 && octets[1] == 168;
            if (is16BitBlock) return true; // Return to prevent further processing

            var isLinkLocalAddress = octets[0] == 169 && octets[1] == 254;
            return isLinkLocalAddress;
        }

        private string getipaddress()
        {
            var szRemoteAddr = Request.UserHostAddress;
            var szXForwardedFor = Request.ServerVariables["X_FORWARDED_FOR"];
            var szIP = "";

            if (szXForwardedFor == null)
            {
                szIP = szRemoteAddr;
            }
            else
            {
                szIP = szXForwardedFor;
                if (szIP.IndexOf(",") > 0)
                {
                    string[] arIPs = szIP.Split(',');

                    foreach (string item in arIPs)
                    {
                        if (!IsPrivateIpAddress(item))
                        {
                            return item;
                        }
                    }
                }
            }
            return szIP;
        }

        private string logUserAccess(string username) 
        {
            var ip = getipaddress();
            var filename = AppDomain.CurrentDomain.BaseDirectory + "Content\\" + "logs\\" + "log.txt";
            var sw = new System.IO.StreamWriter(filename, true);

            try
            {
                sw.WriteLine(DateTime.Now.ToString() + "               " + ip + "               " + username);
            }
            catch (Exception e)
            {
                throw new Exception("writing to logs error: " + username + " : Error : " + e);
            }
            finally
            {
                sw.Close();
            }
            return ip;
        }

        #endregion
    }
}
