using WebSaleDistribute.Models;

namespace WebSaleDistribute.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Core;

    internal sealed class Configuration : DbMigrationsConfiguration<WebSaleDistribute.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "WebSaleDistribute.Models.ApplicationdbContext";
        }

        protected override void Seed(WebSaleDistribute.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            bool itWorks = WriteReferenceData(ApplicationDbContext.Create());
            base.Seed(context);
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }



        public static bool WriteReferenceData(ApplicationDbContext ctx)
        {
            DbContextTransaction transaction = null;
            bool succeeded = false;
            try
            {
                transaction = ctx.Database.BeginTransaction();
                
                CreateRoles(ctx);
                CreateUsers(ctx);
                ctx.SaveChanges();

                transaction.Commit();
                succeeded = true;
            }
            catch (Exception exp)
            {
                if (transaction != null) { transaction.Rollback(); transaction.Dispose(); }
                succeeded = false;
                Elmah.ErrorSignal.FromCurrentContext().Raise(exp);
            }
            return succeeded;
        }

        private static void CreateRoles(ApplicationDbContext ctx)
        {
            // Out of the box
            //ctx.Roles.AddOrUpdate(
            //      new IdentityRole { Name = "Admin" },
            //      new IdentityRole { Name = "Developer" },
            //      new IdentityRole { Name = "NoAccess" }
            //    );
        }

        private static void CreateUsers(ApplicationDbContext ctx)
        {
            // Out of the box approach
            // ctx.Users.AddOrUpdate(
            //     new ApplicationUser { Email = "foo@xyz.com", UserName = "foo@xyz.com" }
            //     );

            // Another approach
            //var manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(ctx));
            //var user = new ApplicationUser() { UserName = "foo@xyz.com", Email = "foo@xyz.com" };
            //var password = "Wh@tever777";
            //var adminresult = manager.Create(user, password);

            ////Add User Admin to Role Administrator
            //if (adminresult.Succeeded)
            //{
            //    var result = manager.AddToRole(user.Id, "Administrator");
            //}




            //if (!context.Users.Any())
            //{
            //    var userStore = new UserStore<ApplicationUser>(context);
            //    var userManager = new ApplicationUserManager(userStore);

            //    var user = new ApplicationUser
            //    {
            //        Email = "foo@bar.com",
            //        UserName = "SuperUser"
            //    };
            //ApplicationUserManager.Create(user, "MySecretPassword1234");
            //    userManager.AddToRole(user.Id, "Administrator");
            //}

            //if (!context.Users.Any(u => u.UserName == "founder"))
            //{
            //    var store = new UserStore<ApplicationUser>(context);
            //    var manager = new UserManager<ApplicationUser>(store);
            //    var user = new ApplicationUser { UserName = "founder" };

            //    manager.Create(user, "ChangeItAsap!");
            //    manager.AddToRole(user.Id, "AppAdmin");
            //}
        }

        public static void InitializeUsersManagements(ApplicationDbContext ctx)
        {
            if (!ctx.Database.Exists())
                ctx.Database.Initialize(force: true);

            // insert fixed data in database [SaleDistributeIdentity]
            var insertUsersManagersDataScript = HelperExtensions.ReadResourceFile(Properties.Settings.Default.QueryUpdateDatabase);
            insertUsersManagersDataScript = insertUsersManagersDataScript.Replace("SaleBranch", Connections.SaleBranch.Connection.DatabaseName);
            Connections.SaleDistributeIdentity.ExecuteNonQuery(insertUsersManagersDataScript);
            ctx.Database.ExecuteSqlCommand("Exec sp_UpdateDatabase");

            // create elmah tables
            var elmahDbScript = HelperExtensions.ReadResourceFile(Properties.Settings.Default.QueryElmah);
            Connections.SaleDistributeIdentity.ExecuteBatchNonQuery(elmahDbScript);
        }
    }
}
