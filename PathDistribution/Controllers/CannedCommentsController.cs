using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {   
        // GET: Admin
        public ActionResult CannedComments()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetCannedComments());
        }

        [HttpPost]
        public JsonResult UpsertCannedComments(CannedComment comment)
        {
            AdminDAL dal = new AdminDAL();

            comment.pkComment = dal.UpsertCannedComments(comment,false);

            return Json(new { Status = "Success", comment });
        }

        [HttpPost]
        public JsonResult DeleteCannedComments(int pkComment)
        {
            AdminDAL dal = new AdminDAL();

            dal.UpsertCannedComments(new CannedComment() {  pkComment = pkComment, chrComment = string.Empty}, true);

            return Json(new { Status = "Success", pkComment });
        }
    }
}