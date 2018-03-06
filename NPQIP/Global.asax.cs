using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WebMatrix.WebData;

namespace NPQIP
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            if (!WebSecurity.Initialized)
            {
                WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: false);
            }

            //if (!Roles.RoleExists("Administrator"))
            //{
            //    Roles.CreateRole("Administrator");
            //}
            //if (!Roles.RoleExists("Reviewer"))
            //{
            //    Roles.CreateRole("Reviewer");
            //}
            //if (!Roles.RoleExists("Uploader"))
            //{
            //    Roles.CreateRole("Uploader");
            //}
            //if (!WebSecurity.UserExists("administrator"))
            //{
            //    WebSecurity.CreateUserAndAccount("admin", "admin");            
            //}
            //if (!Roles.GetRolesForUser("administrator").Contains("Administrator"))
            //{
            //    Roles.AddUsersToRoles(new[] { "administrator" }, new[] { "Administrator" });
            //}
            
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
    }
}