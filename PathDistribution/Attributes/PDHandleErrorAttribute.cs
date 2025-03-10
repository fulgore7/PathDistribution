using SPFramework.Email;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PathDistribution.Attributes
{
    public class PDHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled) return;

#if !DEBUG
            EmailServices.SendEmail(new List<string>() { "programming@sarapath.com" }, 
                                    "Path Distribution Error", 
                                    "The following error has occurred while executing Path Distribution \r\n" + filterContext.Exception.ToString());
#endif       
            base.OnException(filterContext);
        }
    }
}