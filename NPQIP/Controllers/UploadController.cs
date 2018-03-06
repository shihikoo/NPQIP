using NPQIP.Authorization;
using NPQIP.Models;
using NPQIP.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NPQIP.Controllers
{
    [Authorize(Roles = "Administrator, Uploader")]
    [Restrict("Suspended")]
    public class UploadController : Controller
    {
        private readonly NPQIP.Models.NPQIPContext _db = new NPQIP.Models.NPQIPContext();

        //
        // GET:/Upload/
        [Authorize(Roles = "Uploader")]
        public ActionResult Index(string searchTerm, string sortOrder, string onlyUncompleted)
        {
            var pubs = from p in _db.Publications
                       where p.DeletePublication != true && p.PublicationNumber.Substring(0,3) == "NSS"
                           orderby p.Files.Count() ascending, p.LastUpdateTime ascending
                           select new UploadOverviewViewModel
                           {
                               PublicationNumber = p.PublicationNumber.Substring(3, 3),
                               //PublicationNumberNSS = "NSS" + p.PublicationNumber.Substring(3,3),
                               PublicationID = p.PublicationID,
                               Comments = p.Comments,
                               NumberOfFiles = p.Files.Count(f => f.DeleteFile != true),
                               PMID = p.PMID,
                               ExperimentType = p.ExperimentType,
                               Country = p.Country,
                               Species = p.Species,
                               PublicationDate = p.PublicationDate,
                               LastUpdateTime = p.LastUpdateTime
                           };
            ViewBag.TotalRecord = pubs.Count();

            ViewBag.ActionNeededRecord = pubs.Count(pd => pd.NumberOfFiles == 0 || (pd.PMID == 0));
            ViewBag.CompletedRecord = ViewBag.TotalRecord - ViewBag.ActionNeededRecord;
            ViewBag.searchTerm = searchTerm;
            ViewBag.onlyUncompleted = onlyUncompleted;
            var result = pubs.Where(pd => (pd.PublicationNumber.Contains(searchTerm) || searchTerm == null) && (onlyUncompleted != "true" || (pd.NumberOfFiles == 0 || pd.PMID == 0)));
            if (sortOrder == null) sortOrder = "PublicationNumber";
            switch (sortOrder)
            {
                case "LastUpdateTime":
                    result = result.OrderBy(s => s.LastUpdateTime);
                    break;
                case "LastUpdateTime desc":
                    result = result.OrderByDescending(s => s.LastUpdateTime);
                    break;
                case "NumberOfFiles":
                    result = result.OrderBy(s => s.NumberOfFiles);
                    break;
                case "NumberOfFiles desc":
                    result = result.OrderByDescending(s => s.NumberOfFiles);
                    break;
                case "PublicationNumber":
                    result = result.OrderBy(s => s.PublicationNumber);
                    break;
                case "PublicationNumber desc":
                    result = result.OrderByDescending(s => s.PublicationNumber);
                    break;
                default:
                    result = result.OrderBy(s => s.PublicationNumber);
                    break;
            }
            ViewBag.sortOrder = sortOrder;

            return View(result.ToList());
        }

        //
        //   GET: /Upload/Upload
         [Authorize(Roles = "Uploader")]
        public ActionResult Upload(int id = 0)
        {
            ViewBag.N = _db.Files.Count(m => m.DeleteFile != true && m.PublicationPublicationID == id);

            var pub = _db.Publications.Find(id);

            TempData["PublicationID"] = pub.PublicationID;
            TempData["PublicationNumber"] = pub.PublicationNumber;

            ViewBag.PublicationNumber = pub.PublicationNumber;

            if (ViewBag.N > 0)
            {
                var uploadviewmodel = from f in _db.Files
                                      where (f.PublicationPublicationID == id && f.DeleteFile != true)
                                      select new UploadViewModel
                                      {
                                          PublicationPublicationID = id,
                                          PublicationNumber = f.Publications.PublicationNumber,
                                          FileID = f.FileID,
                                          FileName = f.FileName,
                                          FileType = f.FileType,
                                          EntryUser = f.EntryUser,
                                          Comments = f.Comments,
                                          FileUrl = f.FileUrl,
                                          LastUpdateTime = f.LastUpdateTime,
                                          PMID = f.Publications.PMID
                                      };

                return View(uploadviewmodel.ToList());
            }
            else
            {
                var uploadviewmodel = from r in _db.Publications
                                      where (r.PublicationID == id && r.DeletePublication != true)
                                      select new UploadViewModel
                                      {
                                          PublicationPublicationID = id,
                                          PublicationNumber = r.PublicationNumber,
                                          PMID = r.PMID
                                      };

                return View(uploadviewmodel.ToList());
            }
        }

        //
        // POST: /Upload/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Uploader")]
        public ActionResult Upload(FormCollection formCollection)
        {

            HttpPostedFileBase uploadfile = Request.Files["uploadfile"];

            var validFileTypes = new string[]
                {   "application/pdf",
                    "application/msword",
                    "application/postscript",
                    "application/zip",
                    "application/x-compressed",
                    "application/x-zip-compressed",
                    "application/octet-stream",
                    "multipart/x-zip",
                    "multipart/x-gzip",
                    "application/x-gzip",
                    "application/vnd.ms-excel",
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    "image/bmp",
                    "image/x-windows-bmp",
                    "image/gif",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png",
                    "image/tiff",
                    "image/x-tiff"                 
                };

            if (uploadfile == null || uploadfile.ContentLength == 0)
            {
                ModelState.AddModelError("FileUpload", "This field is required");
                ViewBag.Error = "This field is required";
            }
            else if (!validFileTypes.Contains(uploadfile.ContentType))
            {
                ModelState.AddModelError("FileUpload", "Please choose either a pdf, doc or docx file, or a EPS, BMP, GIF, JPG, PNG or tif image, or zip file.");
                ViewBag.Error = "Please choose either a pdf, doc or docx file, or a EPS, BMP, GIF, JPG, PNG or tif image, or zip file.";
            }
            else if (uploadfile.ContentLength > 104857600)
            {
                ModelState.AddModelError("FileUpload", "The maximum size is 100MB.");
                ViewBag.Error = "The maximum size is 100MB.";
            }

            if (ModelState.IsValid)
            {
                var file = new NPQIP.Models.File
                {
                    PublicationPublicationID = Convert.ToInt32(formCollection["publicationid"]),
                     PublicationPublicationNumber = formCollection["publicationnumber"],
                    FileName = uploadfile.FileName,
                    FileType = uploadfile.ContentType,
                    DeleteFile = false
                };

                var path = "~/Content/upload";
                if (!Directory.Exists(Server.MapPath(path)))
                {
                    Directory.CreateDirectory(Server.MapPath(path));
                }
                //DirectoryInfo di = new DirectoryInfo(path);

                //var folder = Server.MapPath(file.RecordRecordID.ToString());
                //DirectoryInfo d2 = di.CreateSubdirectory(folder);

                var uploadDir = String.Format("{0}/{1}", path, formCollection["publicationnumber"]);

                if (!Directory.Exists(Server.MapPath(uploadDir)))
                {
                    Directory.CreateDirectory(Server.MapPath(uploadDir));
                }

                file.Comments = formCollection["inputcomments"];

                if (formCollection["inputfilename"] != null && formCollection["inputfilename"].Length > 0)
                { file.FileName = formCollection["inputfilename"]; }
                else { file.FileName = Path.GetFileNameWithoutExtension(uploadfile.FileName); }

                file.FileExtention = Path.GetExtension(uploadfile.FileName);

                var filelist = from f in _db.Files
                             where f.PublicationPublicationID == file.PublicationPublicationID
                             select f.FileName;

                if (filelist.Contains(file.FileName)) file.FileName = "new_" + file.FileName;

                if (formCollection["Comments"] != null && formCollection["inputfilename"].Length > 0)
                {
                    file.Comments = formCollection["Comments"];
                }

                file.FileUrl = Path.Combine(Server.MapPath(uploadDir), file.FileName+file.FileExtention);

                uploadfile.SaveAs(file.FileUrl);
                file.EntryUser = User.Identity.Name;

                _db.Files.Add(file);
                _db.SaveChanges();

                return RedirectToAction("Upload", file.PublicationPublicationID);
            }
            
            return RedirectToAction("Upload", Convert.ToInt32(TempData["publicationid"]));
        }

        //show file (pdf, image)
        //[AllowAnonymous]
        //public FileResult DownloadDocument(int id, int manuscriptNumber)
        //{
        //    NPQIP.Models.File file = db.Files.Find(id);
        //    if (ModelState.IsValid)
        //    {
        //        //return File(file.FileUrl, file.FileType, file.FileName);
        //        if (file.FileExtention == ".pdf") 
        //        { return File(file.FileUrl, file.FileType); }

        //        return File( file.FileUrl, file.FileType);
        //    }
        //    else
        //    { throw new HttpException(404, "File not Found."); }
        //}

        [AllowAnonymous]
        public FileResult DownloadDocument(int fileNumber, string id)
        {           
            NPQIP.Models.File file = _db.Files.Find(fileNumber);
            if (ModelState.IsValid)
            {
                //return File(file.FileUrl, file.FileType, file.FileName);
                return file.FileExtention == ".pdf" ? File(file.FileUrl, file.FileType) : File(file.FileUrl, file.FileType,  "File" + file.FileID.ToString() + file.FileExtention);
            }
            else
            { throw new HttpException(404, "File not Found."); }
        }

        //
        // GET: /Upload/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Uploader")]
        public ActionResult Delete(int id)
        {
            try
            {
                var file = _db.Files.Find(id);
                TempData["PublicationID"] = file.PublicationPublicationID;
                file.DeleteFile = true;

                _db.SaveChanges();

                return RedirectToAction("Upload", new { id = file.PublicationPublicationID });
            }
            catch
            {
                return View("Error");
            }
        }

        //
        // GET: /Upload/PubDetails/9
         [Authorize(Roles = "Uploader")]
        public ActionResult PubDetails(string publicationnumber) {
            var publication = _db.Publications.FirstOrDefault(p => p.PublicationNumber == "NPG" + publicationnumber);

            if (publication == null) return View("Error");

            var publicationVM = new PublicationViewModel()
            {
                PublicationNumber = publication.PublicationNumber,
                PMID = publication.PMID,
                Country = publication.Country,
                PublicationDate = publication.PublicationDate,
                Species = publication.Species,
                //Keywords = publication.Keywords,
                Comments = publication.Comments,
                ExperimentType = publication.ExperimentType
            };
            

         return View(publicationVM);
        }

        //
        // GET: /Upload/PubDetails/9
         [Authorize(Roles = "Uploader")]
        public ActionResult PubEntry(int id)
        {
            var publication = _db.Publications.Find(id);

            if (publication == null) return View("Error");

            var publicationVM = new PublicationViewModel()
            {
                PublicationID = publication.PublicationID,
                PublicationNumber = publication.PublicationNumber,
                PMID = publication.PMID,
                Country = publication.Country,
                PublicationDate = publication.PublicationDate,
                Species = publication.Species,
                //Keywords = publication.Keywords,
                Comments = publication.Comments,
                ExperimentType = publication.ExperimentType
            };


            return View(publicationVM);
        }

        //
        // GET: /Upload/PubDetails/9
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Uploader")]
        public ActionResult PubEntry(PublicationViewModel publicationVM)
        {
            if (!ModelState.IsValid) return View("Error");
            var publication = _db.Publications.Find(publicationVM.PublicationID);

            publication.PublicationNumber = publicationVM.PublicationNumber;
            publication.PMID = publicationVM.PMID;
            publication.Country = publicationVM.Country;
            publication.PublicationDate = publicationVM.PublicationDate;
            publication.Species = publicationVM.Species;
            //publication.Keywords = publicationVM.Keywords;
            publication.Comments = publicationVM.Comments;
            publication.ExperimentType = publicationVM.ExperimentType;

            publication.LastUpdateTime = DateTime.Now;

            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        //
        // GET: /Upload/Link
        [Authorize(Roles = "Administrator")]
        public ActionResult Link(bool makelink =false)
        {
            var naturepub = from f in _db.Files
                            where f.DeleteFile != true && f.PublicationPublicationNumber.Substring(0, 3) == "NPG" 
                            select f;

            if (makelink != true) return View(naturepub);
            foreach (var file in naturepub)
            {
                var pubid = _db.Publications.Where(p => p.DeletePublication != true && p.PublicationNumber == file.PublicationPublicationNumber).Select(p=>p.PublicationID).FirstOrDefault();

                file.PublicationPublicationID = pubid;
                var filepath = "E:\\ecrf1.clinicaltrials.ed.ac.uk\\Npqip\\Content\\upload\\" + file.PublicationPublicationNumber + "\\" + file.FileName + file.FileExtention;

                file.FileUrl = filepath;

                file.EntryUser = User.Identity.Name;

                switch(file.FileExtention)
                {
                    case ".pdf":
                        file.FileType = "application/pdf";break;
                    case ".zip":
                        file.FileType = "application/zip";break;
                    case ".xlsx":
                        file.FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";break;
                    case ".xls":
                        file.FileType = "application/vnd.ms-excel";break;

                    default:
                        file.FileType = "application/pdf"; break;
                }

            }
            _db.SaveChanges();


            return View(naturepub);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
