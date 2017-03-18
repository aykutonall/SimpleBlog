using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        // GET: Admin/Users
        public ActionResult Index()
        {
            return Content("Admin Area Users Controller Index Action");
        }

        public ActionResult List()
        {
            return Content("Admin Area Users Controller List Action");
        }

        public ActionResult Edit(int id = 0)
        {
            return Content("Admin Area Users Controller Edit Action : " + id.ToString());
        }
    }
}