using System;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace XFramework.Site.PagesAdmin.Controllers
{
    public class UploadController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.StatusCode = 200;
            base.OnActionExecuting(filterContext);
        }
        //
        // GET: /PagesAdmin/Upload/
        [HttpPost]
        public ActionResult SWF(FormCollection fc)
        {
            var folder = System.Configuration.ConfigurationManager.AppSettings["IMAGESERVERFOLDER"];
            HttpFileCollectionBase files = Request.Files;
            FileStream fs = null;
            BinaryWriter bw = null;
            string returnImage = string.Empty;
            try
            {
                HttpPostedFileBase postedFile = files["attachFile"];
                if (postedFile.ContentLength > 0)
                {
                    string originalFileName = postedFile.FileName;
                    string originalExtension = System.IO.Path.GetExtension(originalFileName);
                    string newFileName = string.Format("{2}_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), originalExtension, originalFileName.Replace(originalExtension, string.Empty));
                    string imageServerFolder = String.Concat(folder, string.Format("{0}\\{1}\\{2}\\", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00")));
                    if (!System.IO.Directory.Exists(imageServerFolder))
                    {
                        System.IO.Directory.CreateDirectory(imageServerFolder);
                    }
                    postedFile.SaveAs(String.Concat(imageServerFolder, newFileName));

                    /*
                    //以流模式保存文件
                    // 把 Stream 转换成 byte[]
                    byte[] bytes = new byte[postedFile.InputStream.Length];
                    postedFile.InputStream.Read(bytes, 0, bytes.Length);
                    // 设置当前流的位置为流的开始
                    postedFile.InputStream.Seek(0, SeekOrigin.Begin);
                    // 把 byte[] 写入文件
                    fs = new FileStream(String.Concat(imageServerFolder, newFileName), FileMode.Create);
                    bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();*/
                    returnImage = string.Format("/Upload/{0}/{1}/{2}/{3}", DateTime.Now.Year, DateTime.Now.Month.ToString("00"), DateTime.Now.Day.ToString("00"), newFileName);

                }
            }
            catch(Exception ex)
            {
                log4net.LogManager.GetLogger(typeof(UploadController))
                            .Error("上传附件出错！", ex);
                returnImage = "An error occured";
            }
            finally
            {
                if (fs != null) { fs.Close(); fs.Dispose(); }
                if (bw != null) { bw.Close(); bw.Dispose(); }
            }
            return Content(returnImage);
        }

    }
}
