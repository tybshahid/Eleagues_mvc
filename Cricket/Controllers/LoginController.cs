using Cricket.CustomModels;
using Cricket.Models;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Linq;

namespace Cricket.Controllers
{
    public class LoginController : Controller
    {
        private CricketEntities db;
        public LoginController()
        {
            db = new CricketEntities();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //string genPwd = FormsAuthentication.HashPasswordForStoringInConfigFile("@@LU", "MD5");

            LoginModel LM = new LoginModel();
            Random objRandom = new Random();

#pragma warning disable 618
            var Seed = FormsAuthentication.HashPasswordForStoringInConfigFile(Convert.ToString(objRandom.Next()), "MD5");
#pragma warning restore 618

            LM.hdrandomSeed = Seed;
            return View(LM);
        }

        [HttpPost, AllowAnonymous, ValidateAntiForgeryToken]
        public ActionResult DoLogin(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var AdminPwd = ConfigurationManager.AppSettings["AdminPwd"].ToString();
                //if (model.UserName.ToUpper() == "ADMIN" && ReturnHash(AdminPwd, model.hdrandomSeed) == model.Password)
                if (model.UserName.ToUpper() == "ADMIN" && AdminPwd == FormsAuthentication.HashPasswordForStoringInConfigFile(model.Password, "MD5"))
                {
                    Session["IsAuthenticated"] = true;
                    Session["IsAdmin"] = true;

                    // Getting New Guid
                    //string guid = Convert.ToString(Guid.NewGuid());
                    ////Storing new Guid in Session
                    //Session["AuthenticationToken"] = guid;
                    ////Adding Cookie in Browser
                    //Response.Cookies.Add(new HttpCookie("AuthenticationToken", guid));

                    Session["LoginError"] = null;
                    return RedirectToAction("Index", "Home");
                }
                else if (model.UserName != "" && model.Password != "")
                {
                    // Validate User & Pwd from db
                    PlayersMaster objUser = db.PlayersMaster.FirstOrDefault(p => p.UserID.ToUpper() == model.UserName.ToUpper());
                    if (objUser != null)
                    {
                        //if (ReturnHash(objUser.Password, model.hdrandomSeed) == model.Password)
                        if (objUser.Password == model.Password)
                        {
                            Session["IsAuthenticated"] = true;
                            Session["AuthenticatedUser"] = objUser;
                        }
                        else
                        {
                            Session["LoginError"] = "Invalid Credentials!!!";
                        }
                    }
                    else
                    {
                        Session["LoginError"] = "Invalid Credentials!!!";
                    }
                }
                else
                {
                    Session["LoginError"] = "Invalid Credentials!!!";
                }
            }
            else
            {
                Session["LoginError"] = "Invalid Credentials!!!";
            }

            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        public string ReturnHash(string strPassword, string token)
        {
            string strHash = null;
            string RandomNo = token;
#pragma warning disable 618
            return strHash = FormsAuthentication.HashPasswordForStoringInConfigFile((RandomNo + strPassword), "MD5");
#pragma warning restore 618
        }

        public ActionResult Logout()
        {
            //Removing Session
            Session.Abandon();
            Session.Clear();
            Session.RemoveAll();

            //Removing ASP.NET_SessionId Cookie
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-10);
            }

            if (Request.Cookies["AuthenticationToken"] != null)
            {
                Response.Cookies["AuthenticationToken"].Value = string.Empty;
                Response.Cookies["AuthenticationToken"].Expires = DateTime.Now.AddMonths(-10);
            }

            Session["IsAuthenticated"] = null;
            Session["IsAdmin"] = null;
            Session["AuthenticatedUser"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}