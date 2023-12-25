using SocialNet.Models;
using SocialNet.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNet.Controllers
{
    public class CommunityController : Controller
    {
        // GET: Community
        public ActionResult ListOfCommunity()
        {
            List<Community> Communites = new List<Community>();
            using (var db = new Entities())
            {
                Communites = db.CommunityMembers.Where(x => x.Login == User.Identity.Name).Join(db.Communities,
                                   member => member.CommunityID,
                                   comm => comm.CommunityID,
                                   (comm, member) => member)
                             .OrderBy(x => x.Name).ToList();

            }
            return View(Communites);
        }
        [HttpGet]
        [Authorize]
        public ActionResult CommunityDetails(Guid ID_Community)
        {
            Community community = new Community();
            using (var db = new Entities())
            {
                community = db.Communities.Find(ID_Community);
                return View(community);
            }
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddPost(string Title, string Content, Guid CommunityID)
        {
            using (var context = new Entities())
            {
                var newPost = new CommunityPost
                {
                    CommunityPostID = Guid.NewGuid(),
                    Title = Title,
                    Content = Content,
                    CommunityMembersID = context.CommunityMembers.Where(cm => cm.Login == User.Identity.Name && cm.CommunityID == CommunityID).Select(cm => cm.CommunityMembersID).FirstOrDefault()
                };

                context.CommunityPosts.Add(newPost);
                context.SaveChanges();
            }

            return RedirectToAction("CommunityDetails", new { ID_Community = CommunityID });
        }
        [ChildActionOnly]
        public ActionResult CommunityPosts(Guid ID_Community)
        {
            List<CommunityPost> communityPosts = new List<CommunityPost>();
            using (var db = new Entities())
            {
                communityPosts = db.Communities
    .Join(db.CommunityMembers,
        comm => comm.CommunityID,
        commMember => commMember.CommunityID,
        (comm, commMember) => new { Community = comm, CommunityMember = commMember })
    .Join(db.CommunityPosts,
        commMember => commMember.CommunityMember.CommunityMembersID,
        post => post.CommunityMembersID,
        (commMember, post) => new { CommunityMember = commMember, Post = post })
    .Where(x => x.CommunityMember.Community.CommunityID == ID_Community)
    .Select(x => x.Post)
    .OrderBy(x => x.CommunityMembersID)
    .ToList();
            }
            return View(communityPosts);
        }
        [HttpGet]
        [Authorize]
        public ActionResult CreateCommunity()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult CreateCommunity(CommunityVM newCommunity)
        {
            if (ModelState.IsValid)
            {
                using (var context = new Entities())
                {
                    Community comm = new Community
                    {
                        CommunityID=Guid.NewGuid(),
                        Name=newCommunity.CommunityName,
                        IsItBlog=newCommunity.IsItBLog

                    };
                    CommunityMember commMember = new CommunityMember
                    {
                        CommunityMembersID = Guid.NewGuid(),
                        CommunityID = comm.CommunityID,
                        Login = User.Identity.Name,
                        RoleID = 1
                    };
                    context.Communities.Add(comm);
                    context.CommunityMembers.Add(commMember);
                    context.SaveChanges();
                }
                return RedirectToAction("ListOfCommunity","Community");
            }
            return View(newCommunity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult JoinCommunity(string communityName)
        {
            if (ModelState.IsValid)
            {
                
           
                CommunityMember newCommunityMember = null;
                using (var context = new Entities())
                {
                    Community community = context.Communities.Where(x => x.Name == communityName).FirstOrDefault();
                    if (community == null)
                    {
                        return Content("<script language='javascript' type='text/javascript'>alert('Сообщество не найдено!');window.location.href='/Main/ListOfFriends';</script>");
                    }
                    newCommunityMember = new CommunityMember
                    {
                        CommunityMembersID = Guid.NewGuid(),
                        CommunityID = community.CommunityID,
                        Login = User.Identity.Name,
                        RoleID = 2
                    };
                    context.CommunityMembers.Add(newCommunityMember);
                    context.SaveChanges();
                }

            }
            return RedirectToAction("ListOfCommunity", "Community");
        }
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize]
        public ActionResult LeaveComunity(Guid ID_Community)
        {
            using (var context = new Entities())
            {
                var memberToDelete = context.CommunityMembers.Where(r => r.CommunityID== ID_Community&&r.Login==User.Identity.Name).SingleOrDefault();
                if (memberToDelete != null)
                {
                    context.CommunityMembers.Remove(memberToDelete);
                    context.SaveChanges();
                }
            }
            return RedirectToAction("ListOFCommunity");
        }

    }
}