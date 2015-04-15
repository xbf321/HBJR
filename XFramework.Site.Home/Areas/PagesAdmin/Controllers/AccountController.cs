using System;
using System.Web;
using System.Web.Mvc;

using XFramework.Model;
using XFramework.Services;
using XFramework.Site.PagesAdmin.Models;

namespace XFramework.Site.PagesAdmin.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /PagesAdmin/Account/

        #region == 登录 ==
        public ActionResult Login() {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserInfo model, FormCollection fc) {
            if(string.IsNullOrEmpty(model.UserName)){
                ModelState.AddModelError("USERNAME","请输入您的用户名");
            }
            if(string.IsNullOrEmpty(model.UserPwd)){
                ModelState.AddModelError("USERPWD","请输入您的密码");
            }
            if(ModelState.IsValid){
                UserInfo ui = UserService.Get(model.UserName);
                if(ui.Id <=0){
                    ModelState.AddModelError("ERRORUSERNAME", "不存在此用户");
                    return View();
                }
                //处理密码
                string md5Pwd = Controleng.Common.Utils.MD5(model.UserPwd);
                if (ui.UserPwd.ToLower().Equals(md5Pwd.ToLower()))
                {
                    //写cookie
                    string cookieValue = string.Format("{0}#{1}",DateTime.Now.ToString(), ui.UserName);
                    cookieValue = Goodspeed.Library.Security.DESCryptography.Encrypt(cookieValue, System.Configuration.ConfigurationManager.AppSettings["DESKey"]);
                    //Cookie保存2个小时
                    HttpCookie lcookie = new HttpCookie(System.Configuration.ConfigurationManager.AppSettings["COOKIENAME"]);
                    lcookie.Value = cookieValue;
                    lcookie.Expires = DateTime.Now.AddMinutes(1*60*2);

                    Response.Cookies.Add(lcookie);
                    return new RedirectResult("/pagesadmin/");
                }
                else
                {
                    ModelState.AddModelError("ERRORPWD", "密码不正确");
                }
            }
            return View();
        }
        #endregion

        #region == 退出 ==
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout() {
            //清Cookie
            Response.Cookies.Add(new HttpCookie(System.Configuration.ConfigurationManager.AppSettings["COOKIENAME"])
            { 
                Expires = DateTime.Now.AddDays(-1)
            });
            return new RedirectResult(System.Configuration.ConfigurationManager.AppSettings["LoginUrl"]);
        }
        #endregion

        #region == 添加用户 ==
        [PagesAdminAuth]
        public ActionResult Add() {
            return View();
        }
        [HttpPost]
        [PagesAdminAuth]
        public ActionResult Add(UserInfo formModel) {

            if(string.IsNullOrEmpty(formModel.UserName)){
                ModelState.AddModelError("USERNAME","Email不能为空");
            }
            if(string.IsNullOrEmpty(formModel.UserPwd)){
                ModelState.AddModelError("USERPWD","密码不能为空");
            }
            if(!string.IsNullOrEmpty(formModel.UserPwd)){
                if(formModel.UserPwd.Length <6){
                    ModelState.AddModelError("USERPWDLENGTH","密码的长度至少6位");
                }
            }
            if (ModelState.IsValid)
            {
                //1，判断用户名是否存在
                //2，判断密码是否至少六位
                UserInfo ui = UserService.Get(formModel.UserName);
                if (ui.Id > 0)
                {
                    ModelState.AddModelError("USERNAMEEXISTS", "Email存在，请选择其他Email");
                    return View();
                }
                formModel.UserPwd = Controleng.Common.Utils.MD5(formModel.UserPwd);
                UserService.Create(formModel);
                ViewBag.Msg = "添加用户成功";
            }
            
            return View();
        }
        #endregion

        #region == 用户列表 ==
        [PagesAdminAuth]
        public ActionResult List() {
            ViewBag.UserList = UserService.List();
            return View();
        }
        #endregion

        #region == 设置密码 ==
        [PagesAdminAuth]
        public ActionResult SetPwd() {
            return View();
        }
        [HttpPost]
        [PagesAdminAuth]
        public ActionResult SetPwd(FormCollection fc) {
            string txtOldPwd = fc["txtoldPwd"];
            string txtNewPwd = fc["txtNewPwd"];
            string txtNewConfirmPwd = fc["txtNewConfirmPwd"];
            if(string.IsNullOrEmpty(txtOldPwd)){
                ModelState.AddModelError("OLDPWDEMPTY","请输入旧密码");
            }
            if(string.IsNullOrEmpty(txtNewPwd)){
                ModelState.AddModelError("NEWPWDEMPTY","请输入新密码");
            }
            if(string.IsNullOrEmpty(txtNewConfirmPwd)){
                ModelState.AddModelError("NEWCONFIRMEMPTY","请输入确认新密码");
            }
            if(!string.IsNullOrEmpty(txtNewPwd)){
                if(txtNewPwd.Length <6){
                    ModelState.AddModelError("NEWPWDLENGTH","密码至少六位字符");
                }
                if(txtNewPwd != txtNewConfirmPwd){
                    ModelState.AddModelError("CONFIRMPWDERROR","两次输入的新密码不正确");
                }
            }
            if(ModelState.IsValid){
                string userName = PagesAdmin.Models.PagesAdminContext.Current.UserName;
                UserInfo model = UserService.Get(userName);
                
                if(!Controleng.Common.Utils.MD5(txtOldPwd).ToLower().Equals(model.UserPwd.ToLower())){
                    ModelState.AddModelError("OLDPWDERROR","旧密码不正确，请重新输入");
                    return View();
                }
                model.UserPwd = Controleng.Common.Utils.MD5(txtNewConfirmPwd);
                UserService.Create(model);
                ViewBag.Msg = "修改成功";
            }
            return View();
        }
        #endregion
    }
}
