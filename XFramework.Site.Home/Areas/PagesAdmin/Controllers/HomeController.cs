using System;
using System.Web;
using System.Web.Mvc;
using System.IO;

using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    [PagesAdminAuth]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/PagesAdmin/Category/List?lang=-1");
        }
        public ActionResult Main() {
            return Redirect("/PagesAdmin/Category/List?lang=-1");
            return View();
        }
        public ActionResult UploadImage()
        {
            string imageUrl = SaveImageToFile();
            if(!string.IsNullOrEmpty(imageUrl)){
                ViewBag.Msg = "success";
                ViewBag.ImageUrl = imageUrl;
            }
            return View();
        }

        #region == 上传图片 ==
        [HttpPost]
        public void AjaxUploadImage()
        {
            Response.Write(SaveImageToFile());
            Response.End();
        }
        private string SaveImageToFile()
        {
            string returnImage = string.Empty;
            var folder = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERFOLDER"];
            var serverDomain = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERDOMAIN"];
            HttpFileCollectionBase files = Request.Files;
            try
            {
                HttpPostedFileBase postedFile = files[0];
                //
                if (postedFile.ContentLength > 0)
                {
                    string originalFileName = postedFile.FileName;
                    string originalExtension = System.IO.Path.GetExtension(originalFileName);
                    string newFileName = string.Format("{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), originalExtension);
                    string imageServerFolder = String.Concat(folder, string.Format("{0}\\{1}\\{2}\\", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));
                    if (!System.IO.Directory.Exists(imageServerFolder))
                    {
                        System.IO.Directory.CreateDirectory(imageServerFolder);
                    }
                    postedFile.SaveAs(String.Concat(imageServerFolder, newFileName));

                    returnImage = string.Format("/Upload/{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), newFileName);
                }
            }
            catch
            {
                //Logger.Error(ex.ToString());
            }
            finally
            {
                files = null;
            }
            return returnImage;
        }
        #endregion
    }
}
