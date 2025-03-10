using Microsoft.SharePoint.Client;
using PathDistribution.Models;
using SPFramework.Email;
using SPFramework.Security;
using SPFramework.SSRS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;

namespace PathDistribution.Controllers
{
    public partial class AdminController : Controller
    {
        public ActionResult Reports(ReportTypes id)
        {
            Report report = new Report();
            report.ReportType = (ReportTypes)id;
            report.ReportUser = HttpContext.User;
            return View("Reports",report);
        }

        [HttpGet]
        public FileStreamResult GetReport(ReportTypes id)
        {
            Report report = new Report();
            report.ReportType = (ReportTypes)id;
            report.ReportUser = HttpContext.User;

            Response.AppendHeader("content-disposition", $"inline;filename={report.ReportName}.pdf");

            ReportBuilder<ReportGenerator> rb = new ReportBuilder<ReportGenerator>(new ReportGenerator(report.ReportPath));

            foreach (RptParameter param in report.ReportParameters.Where(x => !string.IsNullOrEmpty(x.Default)))
            {
                if (param.MultiValue)
                {
                    foreach (string p in param.Default.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        rb = rb.AddParameter(param.Name, p);
                    }
                }
                else
                {
                    rb = rb.AddParameter(param.Name, param.Default);
                }
            }

            Stream s = rb.Render(ReportFormats.Pdf);

            if (s != null) s.Seek(0, SeekOrigin.Begin);
                        
            return new FileStreamResult(s, "application/pdf");
        }

        [ActionName("GetReport")]
        [HttpPost]
        public ContentResult GetReport_Post(ReportTypes id, List<RptParameter> parameters)
        {
            Report report = new Report();
            report.ReportType = (ReportTypes)id;
            report.ReportUser = HttpContext.User;

            ReportBuilder<ReportGenerator> rb = new ReportBuilder<ReportGenerator>(new ReportGenerator(report.ReportPath));

            foreach (RptParameter param in parameters.Where(x => !string.IsNullOrEmpty(x.Default)))
            {
                if (param.MultiValue)
                {
                    foreach (string p in param.Default.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        rb = rb.AddParameter(param.Name, p);
                    }
                }
                else
                {
                    rb = rb.AddParameter(param.Name, param.Default);
                }
            }

            Stream s = rb.Render(ReportFormats.Pdf);

            if (s != null) s.Seek(0, SeekOrigin.Begin);
            var mem = new MemoryStream();
            s.CopyTo(mem);
            byte[] bytes = mem.ToArray();

            string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
            return Content(base64);
        }

        [HttpPost]
        public JsonResult PostSP(ReportTypes id, List<RptParameter> parameters)
        {
            Report report = new Report();
            report.ReportType = (ReportTypes)id;
            report.ReportUser = HttpContext.User;

            ReportBuilder<ReportGenerator> rb = new ReportBuilder<ReportGenerator>(new ReportGenerator(report.ReportPath));

            foreach (RptParameter param in parameters.Where(x => !string.IsNullOrEmpty(x.Default)))
            {
                if (param.MultiValue)
                {
                    foreach (string p in param.Default.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        rb = rb.AddParameter(param.Name, p);
                    }
                }
                else
                {
                    rb = rb.AddParameter(param.Name, param.Default);
                }
            }

            Stream s = rb.Render(ReportFormats.Pdf);

            string url = "https://sarapath.sharepoint.com/";

            using (var ctx = new ClientContext(url))
            {
                SecureString pwd = new SecureString();
                foreach (char c in SecurityLoader.GetConnectionString("dbAdmin").ToCharArray()) pwd.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials("dbadmin@sarapath.com", pwd);

                List list = ctx.Web.Lists.GetByTitle("General Logs");
                ctx.ExecuteQuery();

                Folder folder = ctx.Web.GetFolderByServerRelativeUrl($"/General Logs/Pathologist Schedule");
                ctx.Load(folder);
                ctx.ExecuteQuery();

                // Each sliced upload requires a unique ID.
                Guid uploadId = Guid.NewGuid();

                // Get the name of the file.
                string uniqueFileName = Path.GetFileName(Enum.GetName(typeof(ReportTypes),id));

                // File object.
                Microsoft.SharePoint.Client.File uploadFile = null;

                // Calculate block size in bytes.
                int blockSize = 2 * 1024 * 1024;

                // Get the size of the file.
                long fileSize = s.Length;

                // Use large file upload approach.
                ClientResult<long> bytesUploaded = null;

                using (BinaryReader br = new BinaryReader(s))
                {
                    byte[] buffer = new byte[blockSize];
                    Byte[] lastBuffer = null;
                    long fileoffset = 0;
                    long totalBytesRead = 0;
                    int bytesRead;
                    bool first = true;
                    bool last = false;

                    // Read data from file system in blocks.
                    while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytesRead = totalBytesRead + bytesRead;

                        // You've reached the end of the file.
                        if (totalBytesRead == fileSize)
                        {
                            last = true;
                            // Copy to a new buffer that has the correct size.
                            lastBuffer = new byte[bytesRead];
                            Array.Copy(buffer, 0, lastBuffer, 0, bytesRead);
                        }

                        if (first)
                        {
                            using (MemoryStream contentStream = new MemoryStream())
                            {
                                // Add an empty file.
                                FileCreationInformation fileInfo = new FileCreationInformation();
                                fileInfo.ContentStream = contentStream;
                                fileInfo.Url = uniqueFileName;
                                fileInfo.Overwrite = true;
                                uploadFile = folder.Files.Add(fileInfo);

                                // Start upload by uploading the first slice.
                                using (MemoryStream ms = new MemoryStream(buffer))
                                {
                                    // Call the start upload method on the first slice.
                                    bytesUploaded = uploadFile.StartUpload(uploadId, ms);
                                    ctx.ExecuteQuery();
                                    // fileoffset is the pointer where the next slice will be added.
                                    fileoffset = bytesUploaded.Value;
                                }

                                // You can only start the upload once.
                                first = false;
                            }
                        }
                        else
                        {
                            if (last)
                            {
                                // Is this the last slice of data?
                                using (MemoryStream ms = new MemoryStream(lastBuffer))
                                {
                                    // End sliced upload by calling FinishUpload.
                                    uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, ms);
                                    ctx.ExecuteQuery();
                                }
                            }
                            else
                            {
                                using (MemoryStream ms = new MemoryStream(buffer))
                                {
                                    // Continue sliced upload.
                                    bytesUploaded = uploadFile.ContinueUpload(uploadId, fileoffset, ms);
                                    ctx.ExecuteQuery();
                                    // Update fileoffset for the next slice.
                                    fileoffset = bytesUploaded.Value;
                                }
                            }
                        }
                    }
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EmailCouriers()
        {
            EmailBuilder<EmailGenerator> builder = new EmailBuilder<EmailGenerator>();
            builder.AddTo("CouriersSupervisorsDistribution@sarapath.com");
            builder.AddCC("cwarner@sarapath.com");
            builder.AddCC("jtedesco@sarapath.com");
            builder.Send("Pathologist Schedule Updated", "The pathologist schedule has been updated.  Please redistribute as soon as possible.", 0);
            
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}