using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TaskAssign.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string username = collection["username"];
                    string password = collection["password"];

                    var db = new TaskAssign.Models.DatabaseContext();

                    var result = (from l in db.Members
                                  where l.UserName == username && l.Password == password
                                  select l).ToList();

                    if (result.Count > 0)
                    {
                        Session["UserName"] = result[0].UserName;
                        Session["UserId"] = result[0].UserId;
                        Session["Role"] = result[0].RoleId;
                        Session["Name"] = result[0].Name;


                        FormsAuthentication.SetAuthCookie(result[0].UserName, false);

                        return RedirectToAction("Create","Task");
                    }


                    return RedirectToAction("Index");
                }
                catch
                {
                    return View();
                }
            }

            return View();
        }
    }
}
