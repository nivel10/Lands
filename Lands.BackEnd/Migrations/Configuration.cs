namespace Lands.BackEnd.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Lands.BackEnd.Models.DataContextLocal>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //  CHEJ - Habilita las migraciones automaticas
            AutomaticMigrationDataLossAllowed = true;
            ContextKey = "Lands.BackEnd.Models.DataContextLocal";
        }

        protected override void Seed(Lands.BackEnd.Models.DataContextLocal context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
