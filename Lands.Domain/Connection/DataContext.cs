namespace Lands.Domain.Connection
{
    using Lands.Domain.Soccer;
    using Lands.Domain.Soccer.RelationsMap;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public class DataContext : DbContext
    {
        #region Properties

        public DbSet<User> Users { get; set; }

        public DbSet<UserType> UserTypes { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<GroupTeam> GroupTeams { get; set; }

        public DbSet<StatusMatch> StatusMatches { get; set; }

        public DbSet<Match> Matches { get; set; }

        public DbSet<BoardStatus> BoardStatus { get; set; }

        public DbSet<Board> Boards { get; set; }

        public DbSet<Prediction> Predictions { get; set; }

        #endregion Properties

        #region Constructor

        public DataContext() : base("DefaultConnection")
        {
        }

        #endregion Constructor

        #region Methods

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //  CHEJ - Deshabilita el borrado de cascada
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //  CHEJ - Mapeo de relaciones de mas en una tabla con el mismo campo varias veces
            modelBuilder.Configurations.Add(new MatchesMap());
        } 

        #endregion Methods
    }
}
