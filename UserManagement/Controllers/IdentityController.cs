using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using UserManagement.Models;

namespace UserManagement.Controllers
{
    public class IdentityController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        //------------User-------------
        // 显示用户
        public ActionResult ListUsers()
        {
            var q = from u in db.Users
                    select new UserViewModel
                    {
                        UserId = u.Id,
                        UserName = u.UserName
                    };
            return View(q);
        }
        // 编辑用户
        [HttpGet]
        public ActionResult EditUser(string id)
        {
            var user = db.Users.Find(id);//查找主键
            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new UserViewModel(user);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditUser(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.UserId);//根据主键查找
                user.Id = model.UserId;//把视图模型属性值赋给ApplicationUser类（即基类IdentityUser。Id、UserName是基类的属性）
                user.UserName = model.UserName;
                //改变实体状态
                db.Entry(user).State = EntityState.Modified;
                //保存数据
                db.SaveChanges();
                return RedirectToAction("ListUsers");
            }
            //编辑失败时返回视图模型
            return View(model);
        }
        // 删除用户
        public ActionResult DeleteUser(string Id = null)
        {
            //根据View传递给Controller的Id参数找到对象
            var user = db.Users.Where(r => r.Id == Id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new UserViewModel(user);//创建一个视图模型返回给页面
            return View(model);
        }
        [HttpPost, ActionName("DeleteUser")]
        public ActionResult DeleteConfirm(string Id)
        {
            var user = db.Users.Where(r => r.Id == Id).FirstOrDefault();
            db.Users.Remove(user);
            db.SaveChanges();
            //return View("Index"); 只是传View的话，@foreach (var item in Model) 中的 Model 是 null ！
            return RedirectToAction("ListUsers");
        }

        //---------Role--------------
        // 创建角色
        public ActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateRole(RoleViewModel role)
        {
            if (ModelState.IsValid)
            {
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var result = roleManager.Create(new IdentityRole(role.RoleName));
                if (result.Succeeded)
                    return RedirectToAction("ListRoles");
            }
            return View(role);
        }
        // 显示角色列表
        public ActionResult ListRoles()
        {
            var q = from r in db.Roles
                    select new RoleViewModel
                    {
                        RoleId = r.Id,
                        RoleName = r.Name
                    };
            return View(q);
        }
        // 编辑角色
        [HttpGet]
        public ActionResult EditRole(string id)
        {
            var role = db.Roles.Find(id);
            if (role == null)
                return HttpNotFound();
            var model = new RoleViewModel(role);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditRole(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                //找出IdentityDbContext<ApplicationUser>类的对象
                var role = db.Roles.Find(model.RoleId);
                //把视图模型的属性值赋给IdentityRole类
                role.Id = model.RoleId;
                role.Name = model.RoleName;
                //改变实体状态
                db.Entry(role).State = EntityState.Modified;
                //保存数据
                db.SaveChanges();
                return RedirectToAction("ListRoles");
            }
            //编辑失败时返回视图模型
            return View(model);
        }
        // 删除角色
        public ActionResult DeleteRole(string Id = null)
        {
            //根据View传递给Controller的Id参数找到对象
            var role = db.Roles.Where(r => r.Id == Id).FirstOrDefault();
            if (role == null)
            {
                return HttpNotFound();
            }
            var model = new RoleViewModel(role);//创建一个视图模型返回给页面
            return View(model);
        }
        [HttpPost, ActionName("DeleteRole")]
        public ActionResult DeleteRoleConfirm(string Id)
        {
            var role = db.Roles.Where(r => r.Id == Id).FirstOrDefault();
            db.Roles.Remove(role);
            db.SaveChanges();
            //return View("Index"); 只是传View的话，@foreach (var item in Model) 中的 Model 是 null ！
            return RedirectToAction("ListRoles");
        }

        //----------UserRole---------
        //添加用户到角色
        public ActionResult CreateUserRole(string userId)
        {
            //角色列表(添加用户到角色用的是角色名称Name)
            ViewBag.RoleName = new SelectList(db.Roles, "Name", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult CreateUserRole(string userName, string roleName)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var userId = userManager.FindByName(userName).Id;
            userManager.AddToRole(userId, roleName);
            return RedirectToAction("ListUserRoles");
        }
        //show user role
        public ActionResult ListUserRoles()
        {
            //左外连接显示所有用户
            var query = from u in db.Users
                        join r in db.UserRoles
                        on u.Id equals r.UserId
                        into ur
                        from list in ur.DefaultIfEmpty()
                        select new ListUserAndRoleViewModel
                        {
                            UserId = u.Id,
                            UserName = u.UserName,
                            RoleId = list == null ? "" : list.RoleId,
                            RoleName = list == null ? "" : list.Role.Name
                        };
            //var q = query.ToList<UserAndRoleViewModel>();
            var q = query.ToList();
            return View(q);
        }
        //改变用户的角色
        public ActionResult EditUserRole(string userId, string roleId)
        {
            //如果没有角色则添加
            if (roleId == null)
            {
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                var userName = userManager.FindById(userId).UserName;
                var userModel = new UserRoleViewModel(userName);
                return View(userModel);
            }
            var userRole = db.UserRoles.Where(u => u.UserId == userId && u.RoleId == roleId).FirstOrDefault();
            if (userRole == null)
                return HttpNotFound();
            var model = new UserRoleViewModel(userRole);
            return View(model);
        }
        [HttpPost]
        public ActionResult EditUserRole(UserRoleViewModel model, string userId, string roleId)
        {
            if (ModelState.IsValid)
            {
                //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

                // 使中文用户名可编辑
                var userManager = new ApplicationUserManager();

                //当编辑角色时如果没有角色则直接添加角色
                if (roleId == null)
                {
                    userManager.AddToRole(userId, model.RoleName);
                }
                //有角色则先移除角色再添加角色（RemoveFromRole->AddUserToRole)
                else
                {
                    //var userRole = db.UserRoles.Where(u => u.UserId == userId && u.RoleId==roleId).FirstOrDefault();
                    var userRole = db.UserRoles.Find(userId, roleId);//复合主键
                    userManager.RemoveFromRole(userId, userRole.Role.Name);//移除已有角色
                    userManager.AddToRole(userId, model.RoleName);//添加新角色
                }
                return RedirectToAction("ListUserRoles");
            }
            return View(model);
        }
        //移除用户的角色
        [HttpGet]
        public ActionResult DeleteUserRole(string userId = null, string roleId = null)
        {
            var userRole = db.UserRoles.Where(u => u.UserId == userId && u.RoleId == roleId).FirstOrDefault();
            if (userRole == null)
                return HttpNotFound();
            var model = new UserRoleViewModel(userRole);
            return View(model);
        }
        [HttpPost, ActionName("DeleteUserRole")]
        public ActionResult DeleteUserRoleConfirm(string userId, string roleId)
        {
            var userRole = db.UserRoles.Where(u => u.UserId == userId && u.RoleId == roleId).FirstOrDefault();
            db.UserRoles.Remove(userRole);
            db.SaveChanges();
            return RedirectToAction("ListUserRoles");
        }
        public ActionResult Password()
        {
            return View();
        }

        //重置密码（没有Reset的方法，只有先移除再添加一个新密码。）
        public ActionResult ResetPassword(string userName)
        {
            var db = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var user = userManager.FindByName(userName);

            //判断是否有此用户
            if (user == null)
                return Json(new { msg = "no user！" + userName });

            //移除密码
            userManager.RemovePassword(user.Id);
            //添加新密码
            var newPassword = "111111";
            userManager.AddPassword(user.Id, newPassword);

            return Json(new { success = true, msg = "Reset password is:" + newPassword });
        }

        //解锁用户
        //根本就不会锁定用户何来解锁？！
	}
}