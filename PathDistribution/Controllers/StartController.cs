using PathDistribution.Models;
using PathDistribution.Models.DAL;
using System;
using System.Web.Mvc;
using WebMarkupMin.AspNet4.Mvc;

namespace PathDistribution.Controllers
{
    public class StartController : Controller
    {
        [ActionName("Index")]
        [CompressContent]
        [MinifyHtml]
        public ActionResult Index()
        {
            StartSettings ss = (new DistDAL()).GetDates();
            
            return View(ss);
        }

        [HttpPost]
        [ActionName("Index")]
        [CompressContent]
        [MinifyHtml]
        public ActionResult Index_Post(StartSettings ss)
        {
            DistDAL dal = new DistDAL();

            // What state for the specified day are we in
            ProcessTypes pt = dal.IsProcessing(ss.StartAccession,ss.EndAccession,ss.Priority);

            switch (pt)
            {
                // If we have not yet processed this day then download the data from Copath and massage it.
                case ProcessTypes.NotProcessed:
                    dal.DownloadCopath(ss.StartAccession, ss.EndAccession);
                    dal.RunData(ss, true);
                    break;
                // We have already generated before so we need to clone it before we continue
                case ProcessTypes.Generated:
                    dal.CloneData(ss.EndAccession.AddDays(1));
                    break;
                // Already processed start date = "BAD DATE" error
                case ProcessTypes.BadStartDate:
                    break;
                // Future end date =  "BAD DATE" error
                case ProcessTypes.BadEndDate:
                    break;
            }
            TempData["StartSettings"] = ss;

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        [ActionName("Reset")]
        [CompressContent]
        [MinifyHtml]
        public ActionResult Reset_Post(StartSettings ss)
        {
            DistDAL dal = new DistDAL();

            bool bSuccess = dal.ResetDay(ss);

            TempData["StartSettings"] = ss;
            TempData["ResetDay"] = bSuccess;

            return RedirectToAction("Index", "Start");
        }

        // We check what has happened on the provided date to ask the user if we should continue
        public ActionResult IsProcessing(DateTime dte1, DateTime dte, Priorities Priority)
        {
            DistDAL dal = new DistDAL();

            return Json(new { result = dal.IsProcessing(dte1, dte, Priority) }, JsonRequestBehavior.AllowGet);
        }
    }
}