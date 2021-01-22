using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Cricket.Models;

namespace Cricket.CustomModels
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string hdrandomSeed { get; set; }
    }
    public class DashboardsData
    {
        public List<LeaguesMaster> Leagues { get; set; }
    }
    public class LeagueCreatorModel
    {
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public LeaguesMaster LeagueSelected { get; set; }
    }
    public class RulesManagerModel
    {
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public List<LeaguesRulesMaster> RulesSelectedLeague { get; set; }
    }
    public class PlayerManagerModel
    {
        public List<PlayersMaster> PlayersAll { get; set; }
        public PlayersMaster PlayerSelected { get; set; }
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public List<LeaguesPlayersMaster> LeaguePlayers { get; set; }
        public List<Groups> Groups { get; set; }
    }
    public class SquadsManagerModel
    {
        public List<LeaguesPlayersMaster> LeaguesPlayersAll { get; set; }
        public List<LeaguesPlayersTeamMaster> SquadSelected { get; set; }
    }
    public class GameManagerModel
    {
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public List<LeaguesPlayersMaster> Winner { get; set; }
        public List<LeaguesPlayersMaster> Opponents { get; set; }
    }

    public class KnockoutsManagerModel
    {
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public List<PlayersMaster> PlayersAll { get; set; }
        public List<Knockouts> Knockouts { get; set; }
    }

    #region DashboardModels
    public class LeaguePlayersModel
    {
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public List<PlayersMaster> PlayersAll { get; set; }
        public List<LeaguesPlayersMaster> LeaguePlayers { get; set; }
        public List<Groups> Groups { get; set; }
        public List<StarsModel> Stars { get; set; }
    }
    public class GameViewerModel
    {
        public List<LeaguesMaster> Leagues { get; set; }
        public List<PlayersMaster> Players { get; set; }
        public List<Games> Games { get; set; }
    }

    public class LeaderBoardModel
    {
        public List<LeaguesMaster> Leagues { get; set; }
        public List<PlayersMaster> Players { get; set; }
        public List<LeaderBoard> LeaderBoard { get; set; }
    }
    public class LeaderBoard
    {
        public string Player { get; set; }
        public string Country { get; set; }
        public int? Played { get; set; }
        public int? Won { get; set; }
        public int? Lost { get; set; }
        public int? Points { get; set; }
        public string History { get; set; }

        public int? RunsScored { get; set; }
        public decimal? OversPlayed { get; set; }
        public int? WicketsLost { get; set; }
        public int? RunsConceded { get; set; }
        public decimal? OversBowled { get; set; }
        public int? WicketsTaken { get; set; }

        public int? Fifties { get; set; }
        public int? Hundreds { get; set; }
        public int? FiveWickets { get; set; }

        public decimal? Average { get; set; }
        public decimal? Economy { get; set; }

        public double? WinPercentage { get; set; }
        public decimal? NRR { get; set; }

        public string GroupName { get; set; }

        public string FileName { get; set; }
    }

    public class KnockoutsViewerModel
    {
        public List<LeaguesMaster> LeaguesAll { get; set; }
        public List<Knockouts> Knockouts { get; set; }
    }

    #endregion

    public class ChartsData
    {
        public List<LeadingScorer> LeadingScorers { get; set; }
    }
    public class LeadingScorer
    {
        public string Name { get; set; }
        public int? Runs { get; set; }
        public string Country { get; set; }
    }

    public class StarsModel
    {
        public string Name { get; set; }
        public int? Category { get; set; }
        public string Country { get; set; }
        public string FileName { get; set; }
        public int? Games { get; set; }
        public int? Wins { get; set; }
        public int? Runs { get; set; }
        public int? Wickets { get; set; }
        public decimal? Average { get; set; }
        public decimal? Economy { get; set; }
        public double? WinPercentage { get; set; }

        public StarsModel()
        {
            Category = 0;
            Games = 0;
            Wins = 0;
            Runs = 0;
            Wickets = 0;
            Average = 0;
            WinPercentage = 0;
            Economy = 0;
        }

    }
}
