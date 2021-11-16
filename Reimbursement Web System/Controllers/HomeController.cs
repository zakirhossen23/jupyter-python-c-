using Reimbursement_Web_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Reimbursement_Web_System.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(User user)
        {
            using (var context = new ReimbursementContext()) //initialize the database
            {
                String hashedPassword = UtilityClass.GetHash(user.Password); //convert the password to hash
                var query = context.User.Where(s => s.Username.Equals(user.Username) && s.Password.Equals(hashedPassword)).Select(p => p); ; //verify the username and password
                if (query.Count() > 0) //if count > 0 then the user is valid
                {

                    //save the info in session
                    User dbUser = query.FirstOrDefault<User>();
                    Session["UserId"] = dbUser.Id;
                    Session["Username"] = dbUser.Username;
                    Session["FirstName"] = dbUser.FirstName;
                    Session["LastName"] = dbUser.LastName;
                    Session["Role"] = dbUser.Role;

                    //show apporiate page depending on the user type
                    if (dbUser.Role == Role.Standard)
                    {
                        return RedirectToAction("PendingTickets", "User", null);
                    }
                    else {
                        return RedirectToAction("Home", "Admin", null);
                    }
                }
                else
                {
                    //user not existing or wrong password
                    ViewBag.LoginError = true;
                    return View("Index");
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return View("Index");
        }
    }
}