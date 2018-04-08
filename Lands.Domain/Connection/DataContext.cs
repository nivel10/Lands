namespace Lands.Domain.Connection
{
    using Lands.Domain.Soccer;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
        }

        //  CHEJ - Deshabilita el borrado de cascada
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        //  CEHJ - Crea los DataSet<>
        public DbSet<User> Users  { get; set; }

        public DbSet<UserType> UserTypes { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupTeam> GroupTeams { get; set; }
    }
}
