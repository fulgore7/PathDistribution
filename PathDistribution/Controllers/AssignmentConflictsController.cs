using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin
        public ActionResult AssignmentConflicts()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetAssignmentConflicts());
        }

        [HttpPost]
        public JsonResult MoveAssignmentConflict(MoveAssignmentConflict moveAssignmentConflict)
        {
            AdminDAL dal = new AdminDAL();
            dal.MoveAssignmentConflict(moveAssignmentConflict);

            return Json(new { Status = "Success", moveAssignmentConflict });
        }

    }
}