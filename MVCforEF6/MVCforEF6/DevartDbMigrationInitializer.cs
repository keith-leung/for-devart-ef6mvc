using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCforEF6
{
    internal class DevartDbMigrationInitializer: 
        System.Data.Entity.MigrateDatabaseToLatestVersion<
                    AtlanticDXContext, Migrations.Configuration>
    { 

    }
}