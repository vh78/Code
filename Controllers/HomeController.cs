using Login.Models;
using Login.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Kursova.Controllers
{
    #region Admin class
    // This code is only for testing
    // In real application, you should have your own classes User and UserRepository
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdministrator { get; set; }
    }

    public class UserRepository
    {
        public User GetUserByNameAndPassword(string username, string password)
        {
            if (username == "hrisi" && password == "hrisiadmin")
            {
                return new User() { Username = username };
            }
            else
            {
                return null;
            }
        }
    }
    #endregion
    public class HomeController : Controller
    {

        private readonly WoodEntities db = new WoodEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View(db.Woods.ToList());
        }


        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Login")]
        public ActionResult LoginPost(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // here we have to check if the username exists in the database
                UserRepository userRepository = new UserRepository();
                User dbUser = userRepository.GetUserByNameAndPassword(viewModel.Username, viewModel.Password);

                bool isUserExists = dbUser != null;
                if (isUserExists)
                {
                    LoginUserSession.Current.SetCurrentUser(dbUser.ID, dbUser.Username, dbUser.IsAdministrator);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username and/or password");
                }
            }

            // if we are here, this means there is some validation error and we have to show the login screen again
            return View();
        }

  

        public ActionResult Logout()
        {
            LoginUserSession.Current.Logout();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Wood wood)
        {
            if (ModelState.IsValid)
            {
                db.Woods.Add(wood);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(wood);
        }
  


        // this is LINQ
        public ActionResult OrderByName() 
        {
            var wood = from p in db.Woods
                           orderby p.WoodsName ascending
                           select p;
            return View(wood);
        }

        public ActionResult OrderByCubage()
        {
            var w = from p in db.Woods
                    orderby p.Cubage ascending
                    select p;

            return View(w);
        }

        public ActionResult OrderByDateOfReceipt()
        {
            var wo = from p in db.Woods
                     orderby p.DateOfReceipt ascending
                     select p;
            return View(wo);
        }

 


        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Wood wood = db.Woods.Find(id);
            if (wood == null)
                return HttpNotFound();
            return View(wood);
        }

        [HttpPost]
        public ActionResult Edit(Wood wood)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(wood).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(wood);
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Wood w = db.Woods.Find(id);
            if (w == null)
                return HttpNotFound();
            return View(w);
        }

        [HttpPost]
        public ActionResult Delete(int? id, Wood w)
        {
            try
            {
                Wood wood = new Wood();
                if (ModelState.IsValid)
                {
                    if (id == null)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    w = db.Woods.Find(id);
                    if (w == null)
                        return HttpNotFound();
                    db.Woods.Remove(w);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(w);
            }
            catch
            {
                return View();
            }
        }
    }


}