using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework; 
using System.Collections.Generic; 

namespace PrivilegeLibrary
{  
    public class IdentityManager
    {  
        public IdentityManager(ExtendedIdentityDbContext db)
        {
            this._db = db;
            this._userManager =
                new ApplicationUserManager(
                    new UserStore<SysUser, SysRole,
                            int, SysUserLogin, SysUserRole, SysUserClaim>(
                            this._db), this._db);
            this._roleManager = new ApplicationRoleManager(this._db);
        }

        ExtendedIdentityDbContext _db = null;

        // Swap ApplicationRole for IdentityRole:
        ApplicationRoleManager _roleManager = null;

        ApplicationUserManager _userManager = null;

        public bool RoleExists(string name)
        {
            return _roleManager.RoleExists(name);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager;
            }
        }

        public bool CreateRole(string name, int parentId = 0, int privilegeLevel = 50)
        {
            // Swap ApplicationRole for IdentityRole:
            var idResult = _roleManager.Create(new SysRole()
            {
                Name = name,
                ParentId = parentId,
                PrivilegeLevel = privilegeLevel
            });
            return idResult.Succeeded;
        }


        public bool CreateUser(SysUser user, string password)
        {
            var idResult = _userManager.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(int userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(int userId)
        {
            var user = _userManager.FindById(userId);
            var currentRoles = new List<SysUserRole>();

            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                _userManager.RemoveFromRole(userId,
                    _roleManager.FindById(role.RoleId).Name);
            }
        }
    }
}