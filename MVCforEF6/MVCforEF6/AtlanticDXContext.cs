using PrivilegeLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCforEF6
{
    public class AtlanticDXContext : ExtendedIdentityDbContext
    {
        public AtlanticDXContext()
            : base("DefaultConnection")
        {

        }

        public AtlanticDXContext(string nameOrConnection)
            : base(nameOrConnection)
        {

        }
    }
}