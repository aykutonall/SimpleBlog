using NHibernate.Linq;
using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Controllers
{
    public class PostsController:Controller
    {
        public ActionResult Index()
        {
            List<User> users = Database.Session.Query<User>().ToList().Where(p => p.Email.Contains("yahoo")).ToList();
            return View();
        }
    }
}