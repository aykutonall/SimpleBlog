using NHibernate.Linq;
using SimpleBlog.Areas.Admin.ViewModels;
using SimpleBlog.Infrastructure;
using SimpleBlog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimpleBlog.Areas.Admin.Controllers
{

    [Authorize(Roles = "admin")]
    [SelectedTabAttribute("users")]
    public class UsersController : Controller
    {
        private void SyncRoles(IList<RoleCheckBox> checkBoxes, IList<Role> roles)
        {
            var selectedRoles = new List<Role>();

            foreach (var role in Database.Session.Query<Role>())
            {
                var checkBox = checkBoxes.Single(c => c.Id == role.Id);
                checkBox.Name = role.Name;

                if (checkBox.IsChecked)
                    selectedRoles.Add(role);
            }

            foreach (var toAdd in selectedRoles.Where(p => !roles.Contains(p)))
                roles.Add(toAdd);

            foreach (var toRemove in roles.Where(p => !selectedRoles.Contains(p)).ToList())
                roles.Remove(toRemove);
        }

        public ActionResult Index()
        {
            var users = Database.Session.Query<User>().ToList();
            return View(new UsersIndex(){ Users = users });
        }


        public ActionResult New()
        {
            return View(new UsersNew
            {
                Roles = Database.Session.Query<Role>().Select(role => new RoleCheckBox
                {
                    Id = role.Id,
                    IsChecked = false,
                    Name = role.Name
                }).ToList()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult New(UsersNew form)
        {
            if (Database.Session.Query<User>().Any(u => u.Username == form.Username))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);

            var user = new User()
            {
                Email = form.Email,
                Username = form.Username
            };
            
            user.SetPassword(form.Password);
            SyncRoles(form.Roles, user.Roles);

            Database.Session.Save(user);
            Database.Session.Flush();
            return RedirectToAction("index");
        }

        public ActionResult Edit(int id)
        {
            var user = Database.Session.Load<User>(id);

            if (user == null)
                return HttpNotFound();

            return View(
                new UsersEdit()
                {
                    Username = user.Username,
                    Email = user.Email,

                    Roles = Database.Session.Query<Role>().Select(
                        role => new RoleCheckBox()
                        {
                            Id = role.Id,
                            Name = role.Name,
                            IsChecked = user.Roles.Contains(role)

                        }).ToList()
                }
            );
        }



        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UsersEdit form)
        {
            var user = Database.Session.Load<User>(id);

            if (user == null)
                return HttpNotFound();
                

            if (Database.Session.Query<User>().Any(u => u.Username == form.Username && u.Id != id))
                ModelState.AddModelError("Username", "Username must be unique");

            if (!ModelState.IsValid)
                return View(form);
               
            user.Username = form.Username;
            user.Email = form.Email;

            SyncRoles(form.Roles, user.Roles);

            Database.Session.Update(user);
            Database.Session.Flush();
            return RedirectToAction("index");
        }


        public ActionResult ResetPassword(int id)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();
            return View(new UsersResetPassword
            {
                Username = user.Username
            });
        }

        [HttpPost]
        public ActionResult ResetPassword(int id, UsersResetPassword form)
        {
            var user = Database.Session.Load<User>(id);
            if (user == null)
                return HttpNotFound();

            form.Username = user.Username;

            if (!ModelState.IsValid)
                return View(form);

            user.SetPassword(form.Password);
            Database.Session.Update(user);
            Database.Session.Flush();
            return RedirectToAction("index");
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var user = Database.Session.Query<User>().FirstOrDefault(p => p.Id == id);
            if (user == null)
                return HttpNotFound();

            Database.Session.Delete(user);
            Database.Session.Flush();
            return RedirectToAction("index");
        }
    }
}