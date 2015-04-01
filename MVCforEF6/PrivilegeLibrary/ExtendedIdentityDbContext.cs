using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivilegeLibrary
{
    public class ExtendedIdentityDbContext :
        Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext<SysUser,
        SysRole, int, SysUserLogin, SysUserRole, SysUserClaim>
    {
        public ExtendedIdentityDbContext(string nameOrConnection)
            : base(nameOrConnection)
        {

            this.DebugOutput(
                "DbContext Inited:" + this.GetHashCode().ToString()
                + " nameOrConnection: " + nameOrConnection);
        }

        private void DebugOutput(string output)
        {
            System.Diagnostics.Debug.WriteLine(output);
        }


        /// <summary>
        /// Handle MySQL index too long exceptions
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SysUser>()
                .Property(h => h.UserName).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<SysRole>()
                .Property(h => h.Name).HasMaxLength(100).IsRequired();
        }


        /// <summary>
        /// Web Menus
        /// </summary>
        public DbSet<SysMenu> SysMenus
        {
            get;
            set;
        }
    }
}
