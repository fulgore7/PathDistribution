using PathDistribution.Models.DAL;
using System;
using System.Linq;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin
        public ActionResult VacationSchedule()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetVacationSchedules(new DateTime?(), new DateTime?()));
        }

        // GET: Admin
        public ActionResult RefreshVacationScheduleCal(DateTime dteStart, DateTime dteEnd)
        {
            AdminDAL adminDAL = new AdminDAL();

            return PartialView("VacationScheduleCal2",adminDAL.GetVacationSchedules(dteStart, dteEnd).PathScheduleData);
        }
    }
}