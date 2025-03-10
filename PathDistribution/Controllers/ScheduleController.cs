using PathDistribution.Helpers;
using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        public ActionResult Schedule()
        {
            Calendar Calendar = CultureInfo.InvariantCulture.Calendar;
            int weekNumber = Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            string weekyear = $"{weekNumber}-{DateTime.Now.Year}";
            weekyear = weekyear.PadLeft(7, '0');

            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetSchedules(weekyear));
        }

        [HttpPost]
        public JsonResult RefreshSchedulePartial(string weekyear)
        {
            AdminDAL adminDAL = new AdminDAL();

            ScheduleData data = adminDAL.GetSchedules(weekyear);
            string sw = PartialHTML.RenderViewToString(ControllerContext, "ScheduleWeek", data);
            string so = PartialHTML.RenderViewToString(ControllerContext, "ScheduleOff", data.OffSchedule);
            string sc = PartialHTML.RenderViewToString(ControllerContext, "ScheduleComments", data.Comments);
            string si = PartialHTML.RenderViewToString(ControllerContext, "ScheduleIssues", data.Issues);

            return Json(new { sw = sw, so = so, sc = sc, si = si});
        }

        [HttpPost]
        public JsonResult GetSchedWeekPathList(string chrAbbr, DateTime dteAssigned)
        {
            AdminDAL adminDAL = new AdminDAL();

            List<PathSchedWeek> data = adminDAL.GetSchedWeekPathList(chrAbbr, dteAssigned);
            return Json(new { Status = "Success", data });
        }

        [HttpPost]
        public JsonResult GetSchedWeekPathOffList(string chrPath, DateTime dteAssigned)
        {
            AdminDAL adminDAL = new AdminDAL();

            List<PathOffScheduleWeek> data = adminDAL.GetSchedWeekPathOffList(chrPath, dteAssigned);
            return Json(new { Status = "Success", data });
        }

        [HttpPost]
        public JsonResult GetCannedComments()
        {
            AdminDAL adminDAL = new AdminDAL();

            List<CannedComment> data = adminDAL.GetCannedComments();
            return Json(new { Status = "Success", data });
        }

        [HttpPost]
        public ActionResult UpsertScheduleComments(ScheduleComments comment)
        {
            AdminDAL adminDAL = new AdminDAL();

            List<ScheduleComments> comments = adminDAL.UpsertSchedComment(comment);
            return PartialView("ScheduleComments", comments);
        }

        [HttpPost]
        public JsonResult UpsertScheduledPath(SchedulePath item)
        {
            AdminDAL adminDAL = new AdminDAL();
            adminDAL.UpsertSchedPath(item);
            return Json(new { Status = "Success" });
        }

        [HttpPost]
        public JsonResult UpsertScheduledOffPath(SchedulePath item)
        {
            AdminDAL adminDAL = new AdminDAL();
            adminDAL.UpsertSchedOffPath(item);
            return Json(new { Status = "Success" });
        }
    }
}