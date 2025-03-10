using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Pathologists()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetPathologists());
        }

        [HttpPost]
        public JsonResult UpsertPathologist(Pathologists path)
        {
            AdminDAL adminDAL = new AdminDAL();
            adminDAL.UpsertPatholgist(path);
            return Json(new { Status = "Success" });
        }

        [HttpPost]
        public JsonResult DeletePathologist(string pkPath)
        {
            AdminDAL adminDAL = new AdminDAL();
            adminDAL.DeletePatholgist(pkPath);
            return Json(new { Status = "Success" });
        }

        [HttpPost]
        public JsonResult GetCopathPathologists(string filter)
        {
            AdminDAL adminDAL = new AdminDAL();

            List<Pathologists> data = adminDAL.GetCopathPathologists(filter);
            return Json(new { Status = "Success", data });
        }
    }
}