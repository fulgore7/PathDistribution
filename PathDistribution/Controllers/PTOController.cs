using PathDistribution.Models.DAL;
using SPFramework.Email;
using System;
using System.DirectoryServices.AccountManagement;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        [HttpPost]
        public ActionResult CheckPTO()
        {
            AdminDAL checkPTO = new AdminDAL();
            return Json(new { Status = checkPTO.CheckPTO() });
        }

        [HttpPost]
        public ActionResult UpsertPTO(int pkPTO, string chrPath, DateTime dteStart, DateTime dteEnd, int intStatus, string chrAbbr, string chrStatusChangedBy, DateTime dteStatusChanged, string chrComments)
        {
            if (pkPTO < 0) pkPTO = 0;

            AdminDAL admin = new AdminDAL();

            admin.UpsertPTO(pkPTO, chrPath, dteStart, dteEnd, intStatus, chrAbbr, chrStatusChangedBy.Replace("SARAPATH",""), dteStatusChanged, chrComments);

            EmailBuilder<EmailGenerator> email = new EmailBuilder<EmailGenerator>();
            string user = UserPrincipal.Current.EmailAddress;
            string body = body = $"Requested by: {chrPath}\nStart: {dteStart.ToShortDateString()}\nEnd: {dteEnd.ToShortDateString()}\nOff Type: {chrAbbr}\nReason: {chrComments}";
            string subject = string.Empty;
#if DEBUG
            email.AddTo(user);
#else
            email.AddTo(user);
            email.AddTo("ExecManagement@sarapath.com");
#endif
            switch (intStatus)
            {
                case 0:
                    subject = "A New PTO Request has been Submitted through Path Schedule";
                    break;
                case 1:
                    subject = "A PTO Request has been Approved through Path Schedule";
                    break;
                case 2:
                    subject = "A New PTO Request has been Rejected/Cancelled through Path Schedule";
                    break;
            }

            email.Send(subject, body, 0);

            return Json(new { Status = "Success" });
        }
    }
}