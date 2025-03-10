using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Eligibility()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetEligibilities());
        }

        [HttpPost]
        public JsonResult UpdateEligibility(Eligibility item)
        {
            AdminDAL adminDAL = new AdminDAL();

            int performed = adminDAL.UpdateEligibility(item);
            return Json(new { Status = "Success", performed });
        }
    }
}