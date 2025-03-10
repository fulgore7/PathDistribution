using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {     
        // GET: Admin
        public ActionResult Assignments()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetAssignments());
        }

        [HttpPost]
        public JsonResult UpsertAssignments(Assignments assignment)
        {
            AdminDAL adminDAL = new AdminDAL();
            assignment.pkAssignment = adminDAL.UpsertAssignments(assignment);
            return Json(new { Status = "Success", assignment });
        }

        [HttpPost]
        public JsonResult DeleteAssignment(int pkAssignment)
        {
            AdminDAL adminDAL = new AdminDAL();
            adminDAL.DeleteAssignment(pkAssignment);
            return Json(new { Status = "Success" });
        }
    }
}