namespace MVCforEF6.Migrations
{
    using Microsoft.AspNet.Identity;
    using PrivilegeLibrary;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;

    internal sealed class Configuration : DbMigrationsConfiguration<
        MVCforEF6.AtlanticDXContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("Devart.Data.MySql",
                new Devart.Data.MySql.Entity.Migrations.MySqlEntityMigrationSqlGenerator());

            SetHistoryContextFactory("Devart.Data.MySql",
                (conn, schema) => new MySqlHistoryContext(conn, schema));
        }

        protected override void Seed(MVCforEF6.AtlanticDXContext context)
        {
            try
            {
                InitUsers(context);
            }
            catch (Exception e)
            {
                DebugLogException(e);
            }
        }

        private static void DebugLogException(Exception e)
        {
            System.Diagnostics.Debug.WriteLine(
                "LogException InitUserErrors: ", e.Message);
            if (e.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine(
                    "LogInnerException InitUserErrors: ", e.InnerException.Message);
            }
        }

        private void InitUsers(AtlanticDXContext context)
        {
            IdentityManager manager = new IdentityManager(context);
            var userManager = manager.UserManager;
            var roleManager = manager.RoleManager;

            InitRoles(roleManager, context);

#if DEBUG
            var user = new SysUser()
            {
                UserName = "admin",
                Email = "gzkeith@163.com",
                Name = "梁达文",
                PhoneNumber = "13826403668",
            };
            FindCreateAssignRole(userManager, roleManager, user, "Admin@12345", "系统管理员", context);
            user = new SysUser()
           {
               //Id = Guid.NewGuid().ToString(),
               UserName = "manager",
               Email = "iris2013@live.cn",
               Name = "Iris Xiao",
               PhoneNumber = "18666031989",
           };
            FindCreateAssignRole(userManager, roleManager, user, "Iris2013", "Manager", context);
            user = new SysUser()
            {
                //Id = Guid.NewGuid().ToString(),
                UserName = "user",
                Email = "common_user@live.cn",
                Name = "Common User",
                PhoneNumber = "18666031989",
            };
            FindCreateAssignRole(userManager, roleManager, user, "CommonUser", "一般用户", context);

#endif

        }

        private void FindCreateAssignRole(ApplicationUserManager userManager,
           ApplicationRoleManager roleManager, SysUser user, string password,
            string roleName, AtlanticDXContext context)
        {
            int userid = userManager.Users.AsNoTracking().Where(
                c => c.UserName == user.UserName).Select(u => u.Id).FirstOrDefault();
            if (userid < 1)
            {
                user.CTIME = DateTime.Now;
                var result1 = userManager.Create(user, password);
                if (result1.Succeeded)
                {
                    userid = userManager.Users.AsNoTracking().Where(
                    c => c.UserName == user.UserName).Select(u => u.Id).FirstOrDefault();
                }
            }

            int flag = 2;
            while (flag > 0)
            {
                try
                {
                    flag--;
                    var result2 = userManager.AddToRole(userid, roleName);

                    OutputInitError(result2);
                }
                catch (Exception ex2)
                {
                    DebugLogException(ex2);
                    if (ex2 is DbUpdateConcurrencyException)
                    {
                        System.Diagnostics.Debug.WriteLine("DbUpdateConcurrencyException once begins: ");
                        DbUpdateConcurrencyException dbe = ex2 as DbUpdateConcurrencyException;

                        var list = dbe.Entries.ToList();
                        foreach (var item in list)
                        {
                            DebugOutputConcurrencyValues(item, context);
                            item.Reload();
                        }

                        System.Diagnostics.Debug.WriteLine(
                            "DbUpdateConcurrencyException once Ended. ");

                        continue;
                    }
                }

                break;
            }
        }

        private string DebugOutputConcurrencyValues(DbEntityEntry item,
            AtlanticDXContext context)
        {
            string templateFormat = "ItemState: {0} ; DbValueNow: {1} ; item Current Value: {2}; item original values: {3} ";
            string outputString = string.Format(templateFormat, item.State,
                DebugPrintValuesString(item.GetDatabaseValues()),
                DebugPrintValuesString(item.CurrentValues),
                DebugPrintValuesString(item.OriginalValues));
            System.Diagnostics.Debug.WriteLine(templateFormat);

            return outputString;
        }

        private string DebugPrintValuesString(DbPropertyValues dbPropertyValues)
        {
            StringBuilder build = new StringBuilder();

            foreach (var item in dbPropertyValues.PropertyNames)
            {
                build.AppendFormat("{0}:{1}; ", item,
                dbPropertyValues.GetValue<object>(item));
            }

            build.Append("    ");
            return build.ToString();
        }

        private void InitRoles(ApplicationRoleManager roleManager, AtlanticDXContext context)
        {
            if (roleManager.RoleExists("系统管理员") == false)
            {
                IdentityResult result1 = roleManager.Create<SysRole, int>(new SysRole()
                {
                    Name = "系统管理员",
                    ParentId = -1,
                    PrivilegeLevel = 99
                });
                this.OutputInitError(result1);
            }

            SysRole sysadmin = roleManager.FindByName("系统管理员");
            if (roleManager.RoleExists("Boss") == false)
            {
                IdentityResult result2 = roleManager.Create<SysRole, int>(new SysRole()
                {
                    Name = "Boss",
                    ParentId = sysadmin.Id,
                    PrivilegeLevel = 98
                });
                this.OutputInitError(result2);
            }

            SysRole boss = roleManager.FindByName("Boss");
            if (roleManager.RoleExists("Manager") == false)
            {
                IdentityResult result3 = roleManager.Create<SysRole, int>(new SysRole()
                {
                    Name = "Manager",
                    ParentId = boss.Id,
                    PrivilegeLevel = 75
                });
                this.OutputInitError(result3);
            }

            SysRole manager = roleManager.FindByName("Manager");
            if (roleManager.RoleExists("一般用户") == false)
            {
                IdentityResult result4 = roleManager.Create<SysRole, int>(new SysRole()
                {
                    Name = "一般用户",
                    ParentId = manager.Id,
                    PrivilegeLevel = SysRole.DEFAULT_PRIVILEGE_LEVEL
                });
                this.OutputInitError(result4);
            }

            SysRole userrole = roleManager.FindByName("一般用户");
            if (roleManager.RoleExists("游客") == false)
            {
                IdentityResult result5 = roleManager.Create<SysRole, int>(new SysRole()
                {
                    Name = "游客",
                    ParentId = userrole.Id,
                    PrivilegeLevel = 1
                });
                this.OutputInitError(result5);
            }
        }

        private void OutputInitError(IdentityResult result2)
        {
            if (result2 != null && result2.Succeeded == false)
            {
                if (result2.Errors != null && result2.Errors.Count() > 0)
                {
                    string resMsg = result2.Errors.FirstOrDefault();
                    System.Diagnostics.Debug.WriteLine("IdentityResult Errors: " + resMsg);
                }
            }
        }
    }

    public class MySqlHistoryContext :
       System.Data.Entity.Migrations.History.HistoryContext
    {
        public MySqlHistoryContext(System.Data.Common.DbConnection connection,
            string defaultSchema)
            : base(connection, defaultSchema)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<System.Data.Entity.Migrations.History.HistoryRow>()
                .Property(h => h.MigrationId).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<System.Data.Entity.Migrations.History.HistoryRow>()
                .Property(h => h.ContextKey).HasMaxLength(200).IsRequired();
        }
    }
}
