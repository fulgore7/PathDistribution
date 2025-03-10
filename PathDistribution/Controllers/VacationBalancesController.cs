using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        // GET: Admin
        public ActionResult VacationBalances()
        {
            AdminDAL adminDAL = new AdminDAL();

            return View(adminDAL.GetVacationBalances());
        }

        [HttpPost]
        public JsonResult UpsertBalances(VacationBalance balance)
        {
            AdminDAL adminDAL = new AdminDAL();
            balance.pkCredit = adminDAL.UpsertBalances(balance);

            return Json(new { Status = "Success", balance });
        }
    }
}