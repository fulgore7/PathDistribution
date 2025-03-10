using System;
using System.Web.Mvc;
using PathDistribution.Models.DAL;
using PathDistribution.Models;
using WebMarkupMin.AspNet4.Mvc;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Threading.Tasks;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Configuration;

namespace PathDistribution.Controllers
{
    [RoutePrefix("Home")]
    [Route("{action=Index}")]
    public class HomeController : Controller
    {
        // Store and retrieve the settings which contains the dates
        public StartSettings Settings = null;
               
        [Route("Index")]
        [CompressContent]
        [MinifyHtml]
        public ActionResult Index()
        {
            try
            {
                Settings = (StartSettings)TempData["StartSettings"];

                DistDAL dal = new DistDAL();

                // Retrieve our data
                PathData pa = dal.GetPathData(Settings);
                
                TempData["StartSettings"] = Settings;
                
                return View(pa);
            }
            catch
            {
                return RedirectToAction("Index", "Start");
            }
        }

        [HttpPost]
        [ActionName("Index")]
        [Route("Index")]
        public ActionResult Index_Post()
        {
            Settings = (StartSettings)TempData["StartSettings"];

            DistDAL dal = new DistDAL();

            dal.Generate(Settings, User.Identity.Name);

            PathData pa = dal.GetPathData(Settings);
            pa.DownloadPDF = true;

            TempData["StartSettings"] = Settings;

            return View(pa);
         
        }

        [Authorize]
        [HttpPost]
        [Route("MoveCases")]
        public ActionResult MoveCases(CaseMoves cases)
        {
            Settings = (StartSettings)TempData["StartSettings"];

            if (ModelState.IsValid)
            {
                DistDAL dal = new DistDAL();

                foreach (var item in cases)
                {
                    dal.MoveCase(User.Identity.Name, item);
                }
            }

            TempData["StartSettings"] = Settings;

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [Route("AddSlides")]
        public ActionResult AddSlides(List<AddedSlide> AddedSlides)
        {
            Settings = (StartSettings)TempData["StartSettings"];

            if (ModelState.IsValid)
            {
                DistDAL dal = new DistDAL();

                foreach (var item in AddedSlides)
                {
                    dal.AddSlides(User.Identity.Name, item);
                }
            }

            TempData["StartSettings"] = Settings;

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        [Route("UpdateSchedule")]
        public ActionResult UpdateSchedule(PathAssignments pathAssign, PathAssignments offPathAssign)
        {
            Settings = (StartSettings)TempData["StartSettings"];

            if (ModelState.IsValid)
            {
                DistDAL dal = new DistDAL();

                foreach (var item in pathAssign)
                {
                    dal.ChangePathAssign(User.Identity.Name, item);
                }

                foreach (var item in offPathAssign)
                {
                    dal.ChangeOffPathAssign(User.Identity.Name, item);
                }

                dal.RunData(Settings, false);
            }

            TempData["StartSettings"] = Settings;

            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("DownloadPDF")]
        public FileResult DownloadPDF()
        {
            string teststring = string.Empty;

#if DEBUG
            teststring = "_TEST_";
#endif
            Settings = (StartSettings)TempData["StartSettings"];
            string pathName = @"\\spnas2\data\Common\Histology\Daily Distribution\";
            string fileName = string.Format("PathDistribution_{0}{1}*.pdf", teststring, Settings.Distribution.ToString("yyyyMMdd"));
            string fullName = string.Format(@"{0}{1}",pathName, fileName);

            int counter = Directory.GetFiles(pathName, fileName, SearchOption.TopDirectoryOnly).Length + 1;

            fullName = fullName.Replace("*", "_" + counter.ToString());

            byte[] pdf = null;
            byte[] pdf2 = null;
            byte[] pdf3 = null;
            byte[] pdf4 = null;
            byte[] pdf5 = null;
            byte[] pdf6 = null;
            string ssrs_server = ConfigurationManager.AppSettings["SSRS_Server"];

            Parallel.Invoke(() => pdf = RunDistLog(ssrs_server,0),
                            () => pdf2 = RunDistLog(ssrs_server, 1),
                            () => pdf6 = RunDistLog(ssrs_server, 2),
                            () => pdf3 = RunPathSummary(ssrs_server),
                            () => pdf4 = RunCaseSlideSummary(ssrs_server),
                            () => pdf5 = Settings.Priority == Priorities.Saturday ? RunSatMonLog(ssrs_server) : null);

            using (PdfDocument one = PdfReader.Open(new MemoryStream(pdf), PdfDocumentOpenMode.Import))
            using (PdfDocument two = PdfReader.Open(new MemoryStream(pdf2), PdfDocumentOpenMode.Import))
            using (PdfDocument six = PdfReader.Open(new MemoryStream(pdf6), PdfDocumentOpenMode.Import))
            using (PdfDocument three = PdfReader.Open(new MemoryStream(pdf3), PdfDocumentOpenMode.Import))
            using (PdfDocument four = PdfReader.Open(new MemoryStream(pdf4), PdfDocumentOpenMode.Import))
            using (PdfDocument outPdf = new PdfDocument())
            {
                CopyPages(one, outPdf);
                CopyPages(two, outPdf);
                CopyPages(six, outPdf);
                CopyPages(three, outPdf);
                CopyPages(four, outPdf);

                if (Settings.Priority == Priorities.Saturday)
                {
                    using (PdfDocument five = PdfReader.Open(new MemoryStream(pdf5), PdfDocumentOpenMode.Import))
                    {
                        CopyPages(five, outPdf);
                    }
                }

                outPdf.Info.Author = User.Identity.Name;
                outPdf.Options.CompressContentStreams = true;
                outPdf.Save(fullName);
            }

            TempData["StartSettings"] = Settings;

            return File(fullName, "application/pdf", Path.GetFileName(fullName));
        }

        private void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
            {
                to.AddPage(from.Pages[i]);
            }
        }

        private byte[] RunDistLog(string ssrs_server, int IHC)
        {
            byte[] pdf;

            ReportViewer rv = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Remote
            };

#if DEBUG
            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/5 - Histology Distribution Log with Summary";
#else
            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/5 - Histology Distribution Log with Summary Prod";
#endif
            rv.ServerReport.ReportServerUrl = new Uri(ssrs_server);

            List<ReportParameter> parameters = new List<ReportParameter>
            {
                new ReportParameter("from_order_date", Settings.StartAccession.ToString("MM/dd/yyyy")),
                new ReportParameter("to_order_date", Settings.Distribution.ToString("MM/dd/yyyy")),
                new ReportParameter("Day", Convert.ToInt32(Settings.Priority).ToString()),
                new ReportParameter("IHC", IHC.ToString())
            };
            rv.ServerReport.SetParameters(parameters);

            pdf = rv.ServerReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);

            return pdf;
        }

        private byte[] RunPathSummary(string ssrs_server)
        {
            byte[] pdf;

            ReportViewer rv = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Remote
            };

#if DEBUG
            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/6 - Pathologist Distribution Summary";
#else
            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/6 - Pathologist Distribution Summary Prod";;
#endif

            rv.ServerReport.ReportServerUrl = new Uri(ssrs_server);

            List<ReportParameter> parameters = new List<ReportParameter>
            {
                new ReportParameter("Begin", Settings.Distribution.ToString("MM/dd/yyyy")),
                new ReportParameter("End", Settings.Distribution.AddDays(1).ToString("MM/dd/yyyy"))
            };
            rv.ServerReport.SetParameters(parameters);

            pdf = rv.ServerReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);

            return pdf;
        }

        private byte[] RunCaseSlideSummary(string ssrs_server)
        {
            byte[] pdf;

            ReportViewer rv = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Remote
            };

#if DEBUG
            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/Case Slide Distribution";
#else
            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/Case Slide Distribution Prod";
#endif
            rv.ServerReport.ReportServerUrl = new Uri(ssrs_server);

            List<ReportParameter> parameters = new List<ReportParameter>
            {
                new ReportParameter("Assigned", Settings.Distribution.ToString("MM/dd/yyyy")),
                new ReportParameter("ShowConsultants", bool.TrueString)
            };
            rv.ServerReport.SetParameters(parameters);

            pdf = rv.ServerReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);

            return pdf;
        }

        private byte[] RunSatMonLog(string ssrs_server)
        {
            byte[] pdf;

            ReportViewer rv = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Remote
            };

            rv.ServerReport.ReportPath = "/Histology/Distribution/Application Reports/9 - Saturday Monday Cases for Bone Marrows and Out Patient NCB Cases";
            rv.ServerReport.ReportServerUrl = new Uri(ssrs_server);

            List<ReportParameter> parameters = new List<ReportParameter>
            {
                new ReportParameter("from_order_date", Settings.StartAccession.ToString("MM/dd/yyyy")),
                new ReportParameter("to_order_date", Settings.EndAccession.AddDays(2).AddSeconds(-1).ToString("MM/dd/yyyy"))
            };
            rv.ServerReport.SetParameters(parameters);

            pdf = rv.ServerReport.Render("PDF", null, out string mimeType, out string encoding, out string extension, out string[] streamIds, out Warning[] warnings);

            return pdf;
        }
    }

}