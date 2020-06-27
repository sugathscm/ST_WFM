using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WFM.DAL;
using WFM.UI.Models;

namespace WFM.UI.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AccountUserController : Controller
    {
        private ApplicationDbContext context;

        public AccountUserController()
        {
            context = new ApplicationDbContext();
        }

        // GET: AccountUser
        public ActionResult Index()
        {
            var usersWithRoles = (from user in context.Users
                                  select new
                                  {
                                      UserId = user.Id,
                                      Username = user.UserName,
                                      Email = user.Email,
                                      RoleNames = (from userRole in user.Roles
                                                   join role in context.Roles on userRole.RoleId
                                                   equals role.Id
                                                   select role.Name).ToList()
                                  }).ToList().Select(p => new UsersInRoleViewModel()

                                  {
                                      UserId = p.UserId,
                                      Username = p.Username,
                                      Email = p.Email,
                                      Role = string.Join(",", p.RoleNames)
                                  });
            return View(usersWithRoles);
        }
    }
}