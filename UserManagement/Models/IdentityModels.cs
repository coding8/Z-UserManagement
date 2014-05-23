using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace UserManagement.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
        //用户及角色表-显示用户及角色时会用到。
        public DbSet<IdentityUserRole> UserRoles { get; set; }
    }
    //修改密码长度,同时记得更改 AccountViewModel 里的 MinimumLength
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager()
            : base(new UserStore<ApplicationUser>(new ApplicationDbContext()))
        {
            PasswordValidator = new MinimumLengthValidator(3);
            // 允许使用中文名注册
            UserValidator = new UserValidator<ApplicationUser>(this) { AllowOnlyAlphanumericUserNames = false };
        }
    }
}