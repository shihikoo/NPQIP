using System;
using System.Web;
using System.Web.Mvc;

namespace NPQIP.Authorization
{
    public class Restrict : AuthorizeAttribute
    {

         private readonly string _role;

         public Restrict(string role)
        {
            _role = role;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            return !httpContext.User.IsInRole(_role);
        }

    }
}