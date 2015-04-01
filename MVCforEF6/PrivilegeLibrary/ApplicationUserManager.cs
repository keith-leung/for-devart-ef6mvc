using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PrivilegeLibrary
{
    // 配置此应用程序中使用的应用程序用户管理器。UserManager 在 ASP.NET Identity 中定义，并由此应用程序使用。
    public class ApplicationUserManager : UserManager<SysUser, int>
    {
        public ApplicationUserManager(IUserStore<SysUser, int> store, ExtendedIdentityDbContext db)
            : base(store)
        {
            this.DbContext = db;
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var db = context.Get<ExtendedIdentityDbContext>();
            var manager = new ApplicationUserManager(
                new UserStore<SysUser, SysRole,
                    int, SysUserLogin, SysUserRole, SysUserClaim>(
                    db), db);
            //context.Get<ApplicationDbContext>()));

            // 配置用户名的验证逻辑
            manager.UserValidator = new UserValidator<SysUser, int>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // 配置密码的验证逻辑
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = false,
            };

            // 配置用户锁定默认值
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<SysUser, int>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(SysUser user)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await this.CreateIdentityAsync(
                user, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }

        internal ExtendedIdentityDbContext DbContext { get; set; }

        internal IUserStore<SysUser, int> UserStore
        {
            get
            {
                return this.Store;
            }
        }
    }
}
