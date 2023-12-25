using SocialNet.Models;
using SocialNet.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SocialNet.Controllers
{
    public class AuthController : Controller
    {
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserVM webUser)
        {
            if (ModelState.IsValid)
            {
                using (Entities Context = new Entities())
                {
                    User user = null;
                    user = Context.Users.Where(u => u.Login == webUser.Login).FirstOrDefault();
                    if (user != null)
                    {
                        string passwordHash = ReturnHashCode(webUser.Password + user.Salt.ToString().ToUpper());
                        if (passwordHash == user.PasswordHash)
                        {
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                1,
                                user.Login,
                                DateTime.Now,
                                DateTime.Now.AddDays(1),
                                false,
                                "");
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));
                            return RedirectToAction("Profile",new {Controller= "Main",userLogin = user.Login });
                        }

                    }
                }
            }
            ViewBag.Error = "Пользователь с таким логином или паролем не существует";
            return View(webUser);
        }
        [AllowAnonymous, ActionName("Login")]
        [HttpGet]//////Можно проще
        public ActionResult LoginIn(UserVM webUser)
        {
            User user;
            using (Entities Context = new Entities())
            {
                user = Context.Users.Find(webUser.Login);
            }
            return View(user);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registration()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Registration(UserRegisterVM userRegVM)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (Entities Context = new Entities())
                {
                    string newSalt = GenerateSalt();
                    user = new User
                    {
                        Login = userRegVM.Login,
                        Salt = newSalt,
                        PasswordHash = GeneratePasswordHash(userRegVM.Password, newSalt),
                        Last_Name = userRegVM.Last_Name,
                        First_Name = userRegVM.First_Name,
                        Patronymic = userRegVM.Patronomic
                    };
                    Context.Users.Add(user);
                    Context.SaveChanges();
                    return RedirectToAction("Login", "Auth");
                }
            }
            return View(userRegVM);
        }
        public string GenerateSalt()
        {
            Guid newGuid = Guid.NewGuid();
            return newGuid.ToString().ToUpper();
        }

        public string GeneratePasswordHash(string password, string salt)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // "x2" for lowercase hex
                    sb.Append(b.ToString("X2"));
                }

                return sb.ToString();
            }
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        string ReturnHashCode(string loginAndSalt)
        {
            string hash = "";
            using (SHA1 sha1Hash = SHA1.Create())
            {
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(loginAndSalt));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                hash = sBuilder.ToString().ToUpper();
            }
            return hash;
        }
    }
}