using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin
        public ActionResult VacationSchedule(bool _calendar)
        {
            AdminDAL adminDAL = new AdminDAL();
            if (_calendar)
            {
                return PartialView("VacationSchedule2", adminDAL.GetVacationSchedulesCal(new DateTime?(), new DateTime?()));
            }
            else
            {
                return PartialView("VacationSchedule", adminDAL.GetVacationSchedules(new DateTime?(), new DateTime?()).PathScheduleData);
            }   
        }

        // GET: Admin
        public ActionResult RefreshVacationSchedule(DateTime dteStart, DateTime dteEnd)
        {
            AdminDAL adminDAL = new AdminDAL();

            return PartialView("VacationScheduleCal",adminDAL.GetVacationSchedules(dteStart, dteEnd).PathScheduleData);
        }
        public JsonResult RefreshVacationScheduleCal(DateTime dteStart, DateTime dteEnd)
        {
            AdminDAL adminDAL = new AdminDAL();

           return Json(new { error = true, message = PartialView("VacationScheduleCal2", adminDAL.GetVacationSchedulesCal(dteStart, dteEnd))});
        }

    }
}