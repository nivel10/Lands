namespace Lands.BackEnd.Helpers
{
    using Lands.BackEnd.Models;
    using Lands.Domain;
    using Lands.Domain.GetServices;
    using Lands.Domain.Soccer;
    using System.Collections.Generic;
    using System.Linq;

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

        public static List<Nationality> GetNationalities(DataContextLocal dbLocal)
        {
            var nationality = dbLocal.Nationalities
                .OrderBy(n => n.NationalityId)
                .ToList();
            nationality.Add(new Nationality
            {
                 NationalityId = 0, 
                 Abbreviation = "S",
                 Name = "[Select a nationality...!!!]",
            });

            return nationality.OrderBy(n => n.NationalityId).ToList();
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

        public static List<User> GetUsersGetServicesVzLa(DataContextLocal dbLocal)
        {
            var appName = MethodsHelper.GetAppNameGetServices();
            var userList = dbLocal.Users
                .Where(u => u.AppName == appName)
                .ToList();

            userList.Add(new User
            {
                UserId = 0,
                FirstName = "[Select an user...!!!]",
                LastName = string.Empty,
            }
            );

            return userList.OrderBy(u => u.UserId).ToList();
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

        public static List<BoardStatus> GetBoradStatus(DataContextLocal dbLocal)
        {
            var boardStatusList = new List<BoardStatus>();
            boardStatusList = dbLocal.BoardStatus.ToList();
            boardStatusList.Add(new BoardStatus
            {
                BoardStatusId = 0,
                 Name = "[Select a board status...!!!]",
            });

            return boardStatusList.OrderBy(bs => bs.Name).ToList();
        }

        public static List<Board> GetBorads(DataContextLocal dbLocal)
        {
            var boardsList = new List<Board>();
            boardsList = dbLocal.Boards.ToList();
            boardsList.Add(
                new Board
                {
                    BoardId = 0,
                    ImagePath = "[Select a board...!!!]",
                });

            return boardsList.OrderBy(b => b.BoardId).ToList();
        }

        public static List<UserType> GetUserType(DataContextLocal dbLocal)
        {
            var userTypeList = dbLocal.UserTypes
                .ToList();

            userTypeList.Add(new UserType
            {
                Name = "[Select a type...!!!]",
                UserTypeId = 0,
            });

            return userTypeList.OrderBy(ut => ut.Name).ToList();
        }

        public static List<Match> GetMatchs(DataContextLocal dbLocal)
        {
            var matchsList = new List<Match>();
            matchsList = dbLocal.Matches.ToList();
            matchsList.Add(new Match
            {
                 MatchId = 0,
            });

            return matchsList.OrderBy(m => m.MatchId).ToList();
        }

        public static List<User> GetUsers(DataContextLocal dbLocal)
        {
            var userList = new List<User>();
            userList = dbLocal.Users.ToList();
            userList.Add(new User
            {
                UserId = 0,
                FirstName = "[Select a user...!!!]"
            });

            return userList.OrderBy(u => u.UserId).ToList();
        }

        public static List<Team> GetTeams(int groupId, int localId, DataContextLocal dbLocal)
        {
            //  CHEJ - Hace el select de datos de los Teams que pertenecen al grupo
            var team = from Team in dbLocal.Teams
                           join GroupTeam in dbLocal.GroupTeams on Team.TeamId equals GroupTeam.TeamId
                           where GroupTeam.GroupId == groupId
                           select Team;

            //  CHEJ - Transforma el resultado de la consulta en un objeto List<>
            var teamList = new List<Team>(team);

            //  CHEJ - Agrega un registro en blanco
            AddBlankTeam(teamList);

            return teamList.OrderBy(t => t.Name).ToList();
        }

        #endregion Combo Soccer
    }
}