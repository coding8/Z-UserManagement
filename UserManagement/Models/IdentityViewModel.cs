using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace UserManagement.Models
{
    //用户列表
    public class UserViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public UserViewModel() { }
        public UserViewModel(ApplicationUser user)
        {
            this.UserId = user.Id;
            this.UserName = user.UserName;
        }
        //public DateTime CreatedOn { get; set; }
    }
    //角色列表
    public class RoleViewModel
    {
        public RoleViewModel() { }
        public RoleViewModel(IdentityRole role)
        {
            this.RoleId = role.Id;
            this.RoleName = role.Name;
        }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        // public DateTime CreatedOn { get; set; }
    }
    //显示用户和角色列表
    public class ListUserAndRoleViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
    //添加用户到角色
    public class UserRoleViewModel
    {
        public string UserName { get; set; }
        [Required]
        public string RoleName { get; set; }
        //在模型里绑定角色列表
        public SelectList RoleNameList
        {
            get
            {
                var db = new ApplicationDbContext();
                return new SelectList(db.Roles, "Name", "Name");
            }
        }
        public UserRoleViewModel() { }
        //用户添加角色时使用
        public UserRoleViewModel(string userName)
        {
            this.UserName = userName;
        }
        public UserRoleViewModel(IdentityUserRole ur)
        {
            this.UserName = ur.User.UserName;
            this.RoleName = ur.Role.Name;
        }
    }
}