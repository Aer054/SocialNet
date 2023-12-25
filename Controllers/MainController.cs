using SocialNet.Models;
using SocialNet.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNet.Controllers
{
    public class MainController : Controller
    {
        // GET: Main
        public ActionResult ListOfFriends()
        {
            List<User> Friends = new List<User>();
            using (var db = new Entities())
            {
                Friends = db.Friend_Requests.Where(x => x.Status == true && (x.FromUserLogin == User.Identity.Name))
                             .Join(db.Users,
                                   request => request.ToUserLogin,
                                   user => user.Login,
                                   (request, user) => user)
                             .Union(db.Friend_Requests.Where(x => x.Status == true && (x.ToUserLogin == User.Identity.Name))
                             .Join(db.Users,
                                   request => request.FromUserLogin,
                                   user => user.Login,
                                   (request, user) => user))
                             .OrderBy(x => x.First_Name)
                             .ThenBy(x => x.Last_Name)
                             .ToList();

            }
            return View(Friends);
        }
        public ActionResult ListOfFriendsRequest()
        {
            List<User> FriendsReq = new List<User>();
            using (var db = new Entities())
            {
                FriendsReq = db.Friend_Requests.Where(x => x.Status == null && x.ToUserLogin == User.Identity.Name)
                             .Join(db.Users,
                                   request => request.FromUserLogin,
                                   user => user.Login,
                                   (request, user) => user)
                             .OrderBy(x => x.First_Name)
                             .ThenBy(x => x.Last_Name)
                             .ToList();

            }
            return View(FriendsReq);
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult DeclineRequest(string fromUserLogin)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
                {
                    var request = context.Friend_Requests
                        .Where(r => r.FromUserLogin == fromUserLogin && r.ToUserLogin == User.Identity.Name).FirstOrDefault();

                    if (request != null)
                    {
                        request.Status = false;
                        context.SaveChanges();
                    }

                }
            }
            return RedirectToAction("ListOfFriendsRequest", "Main");
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult AcceptRequest(string fromUserLogin)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
                {
                    var request = context.Friend_Requests
                        .Where(r => r.FromUserLogin == fromUserLogin && r.ToUserLogin == User.Identity.Name).FirstOrDefault();

                    if (request != null)
                    {
                        request.Status = true;
                        context.SaveChanges();
                    }

                }
            }
            return RedirectToAction("ListOfFriendsRequest", "Main");
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddFriend()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult AddFriend(string friendLogin)
        {
            if (ModelState.IsValid)
            {
                Friend_Request friend_Request = null;
                using (var context = new Entities())
                {
                    var existingLogin = context.Users.FirstOrDefault(x => x.Login == friendLogin);
                    if (existingLogin == null)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Пользователь не найден!');window.location.href='/Main/ListOfFriends';</script>");

                    }
                    var existingFriend = context.Friend_Requests.FirstOrDefault(f => f.ToUserLogin == friendLogin && f.FromUserLogin == User.Identity.Name);
                    if (existingFriend != null)
                    {

                        return Content("<script language='javascript' type='text/javascript'>alert('Запрос уже отправлен или пользователь уже в друзьях!');window.location.href='/Main/ListOfFriends';</script>");
                    }

                    friend_Request = new Friend_Request
                    {
                        RequestID = Guid.NewGuid(),
                        Status = null,
                        FromUserLogin = User.Identity.Name,
                        ToUserLogin = friendLogin
                    };
                    context.Friend_Requests.Add(friend_Request);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("ListOfFriends", "Main");
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult DeleteConfirmed(string FriendLogin)
        {
            using (var context = new Entities())
            {
                var requestToDelete = context.Friend_Requests.Where(r => (r.ToUserLogin == FriendLogin && r.FromUserLogin == User.Identity.Name) ||
                (r.FromUserLogin == FriendLogin && r.ToUserLogin == User.Identity.Name)).SingleOrDefault();
                if (requestToDelete != null)
                {
                    context.Friend_Requests.Remove(requestToDelete);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("ListOfFriends");
        }
        [HttpGet]
        [Authorize]
        public ActionResult Profile(string userLogin)
        {
            if (userLogin == null)
            {
                return View("Error");

            }
            User user = null;
            using (var db = new Entities())
            {
                user = db.Users.Where(x => x.Login == userLogin).FirstOrDefault();
            }
            return View(user);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddUserPost(string Title, string Content)
        {
            using (var context = new Entities())
            {
                var newUserPost = new UserPost
                {
                    UserPostID = Guid.NewGuid(),
                    Title = Title,
                    Content = Content,
                    Login = User.Identity.Name
                };

                context.UserPosts.Add(newUserPost);
                context.SaveChanges();
            }

            return RedirectToAction("Profile", new { userLogin = User.Identity.Name });
        }

        [ChildActionOnly]
        public ActionResult UserPosts(string Login)
        {
            List<UserPost> userPosts = new List<UserPost>();
            using (var db = new Entities())
            {
                userPosts = db.UserPosts
                              .Where(x => x.Login == Login)
                              .OrderBy(x => x.UserPostID)
                              .ToList();
            }
            return View(userPosts);
        }


    }
}