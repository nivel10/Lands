namespace Lands.BackEnd.Helpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Lands.BackEnd.Models;
    using Lands.Domain.Soccer;

    public class CombosHelper
    {
        #region Combo Soccer

        public static List<Team> GetTeams(DataContextLocal dbLocal)
        {
            var teams = new List<Team>();

            teams = dbLocal.Teams
                .OrderBy(t => t.Name)
                .ToList();

            //  CHEJ - Agrega un registro en blanco
            AddBlankTeam(teams);

            //teams.Add(new Team
            //{
            //    TeamId = 0,
            //    Name = "[Select a team...!!!]",
            //    ImagePath = string.Empty,
            //});

            return teams.OrderBy(t => t.Name).ToList();
        }

        public static List<Team> GetTeams(DataContextLocal dbLocal, Group group)
        {
            var teams = new List<Team>();
            var teamsList = GetListTeam(group);

            //teams = dbLocal.Teams
            //    .Except(teamsList)
            //    .OrderBy(t => t.Name)
            //    .ToList();

            teams = dbLocal.Teams
               .OrderBy(t => t.Name)
               .ToList();

            //  CHEJ - Agrega un registro en blanco
            AddBlankTeam(teams);

            //teams.Add(new Team
            //{
            //    TeamId = 0,
            //    Name = "[Select a team...!!!]",
            //    ImagePath = string.Empty,
            //});

            return teams.OrderBy(t => t.Name).ToList();
        }

        private static List<Team> GetListTeam(Group group)
        {
            var teamList = new List<Team>();
            foreach (var team in group.GroupTeams)
            {
                teamList.Add(team.Team);
            }

            return teamList.ToList();
        }

        private static void AddBlankTeam(List<Team> teams)
        {
            teams.Add(new Team
            {
                TeamId = 0,
                Name = "[Select a team...!!!]",
                ImagePath = string.Empty,
            });
        }

        #endregion Combo Soccer
    }
}