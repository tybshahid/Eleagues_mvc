using Cricket.CustomModels;
using Cricket.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Cricket.Controllers
{
    public class DashboardsController : Controller
    {
        private CricketEntities db;
        private DateTime dateFilter = DateTime.Now.AddDays(-1000);
        LeaguesMaster defaultLeague = new LeaguesMaster();

        public DashboardsController()
        {
            db = new CricketEntities();

            if (db.LeaguesMaster.FirstOrDefault(p => p.IsDefaultLeague == true) != null)
                defaultLeague = db.LeaguesMaster.OrderByDescending(p => p.RecordID).FirstOrDefault(p => p.IsDefaultLeague == true);
            else
                defaultLeague = db.LeaguesMaster.OrderByDescending(p => p.RecordID).FirstOrDefault();

            ViewData["defaultLeague"] = defaultLeague.RecordID;
            ViewData["defaultLeagueName"] = defaultLeague.Name;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }
        [ValidateAntiForgeryToken]
        public ActionResult Dashboards()
        {
            LeaderBoardModel objModel = new LeaderBoardModel();
            objModel.Leagues = db.LeaguesMaster.OrderByDescending(p => p.RecordID).ToList();
            objModel.Players = db.PlayersMaster.OrderBy(p => p.Name).ToList();
            objModel.LeaderBoard = GetLeaderboard(defaultLeague.RecordID, null);
            return View(objModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult ViewLeagues()
        {
            var lstResult = db.LeaguesMaster.ToList();
            if (lstResult.Count > 0)
            {
                lstResult = lstResult.OrderByDescending(p => p.RecordID).ToList();
            }

            return PartialView("_Leagues", lstResult);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ViewRules()
        {
            RulesManagerModel objRulesManagerModel = new RulesManagerModel();
            objRulesManagerModel.LeaguesAll = db.LeaguesMaster.Where(p => p.IsClosed != "Yes").ToList();
            objRulesManagerModel.RulesSelectedLeague = db.LeaguesRulesMaster.Where(p => p.LeagueID == defaultLeague.RecordID).ToList();
            return View("_Rules", objRulesManagerModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchRules(long? LeagueID)
        {
            return View("_Rules2", LeagueID == null ? new List<LeaguesRulesMaster>() : db.LeaguesRulesMaster.Where(p => p.LeagueID == LeagueID).ToList());
        }

        [ValidateAntiForgeryToken]
        public ActionResult ViewLeaguePlayers()
        {
            LeaguePlayersModel objModel = new LeaguePlayersModel();
            objModel.LeaguesAll = db.LeaguesMaster.OrderByDescending(p => p.RecordID).ToList();
            objModel.LeaguePlayers = db.LeaguesPlayersMaster.Where(p => p.LeaguesMaster.RecordID == defaultLeague.RecordID).OrderBy(p => p.PlayersMaster.Name).ToList();
            objModel.Groups = db.Groups.ToList();
            return View("_LeaguePlayers", objModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchLeaguePlayers(long? leagueID, int? GroupID)
        {
            var result = db.LeaguesPlayersMaster.Where(p => p.LeagueID == leagueID).OrderBy(p => p.PlayersMaster.Name).ToList();
            if (GroupID != null)
                result = result.Where(p => p.GroupID == GroupID).OrderBy(p => p.PlayersMaster.Name).ToList();

            return View("_LeaguePlayers2", result);
        }

        #region Games
        private List<Games> GetGames(long? LeagueID, long? WinnerID, long? OpponentID)
        {
            var totalGames = db.Games
                .Where(p => p.CreatedOn > dateFilter
                            && p.LeaguesPlayersID != null
                            && p.OpponentPlayerID != null).ToList();

            if (LeagueID != null)
                totalGames = totalGames.Where(p => p.LeaguesPlayersMaster.LeagueID == LeagueID).ToList();

            if (WinnerID != null)
                totalGames = totalGames.Where(p => p.LeaguesPlayersMaster.PlayerID == WinnerID || p.OpponentPlayerID == WinnerID).ToList();

            if (OpponentID != null)
                totalGames = totalGames.Where(p => p.LeaguesPlayersMaster.PlayerID == OpponentID || p.OpponentPlayerID == OpponentID).ToList();

            ViewData["TotalRecords"] = totalGames.Count();
            ViewData["NewGames"] = totalGames.Where(p => DateTime.Compare(p.CreatedOn.Value.Date, DateTime.Now.Date) == 0).Count();

            return totalGames;
        }
        [ValidateAntiForgeryToken]
        public ActionResult ViewGames()
        {
            GameViewerModel objModel = new GameViewerModel();
            objModel.Leagues = db.LeaguesMaster.OrderByDescending(p => p.RecordID).ToList();
            objModel.Players = db.PlayersMaster.OrderBy(p => p.Name).ToList();

            var lstResult = GetGames(defaultLeague.RecordID, null, null);
            if (lstResult.Count > 0)
            {
                lstResult = lstResult.OrderByDescending(p => p.RecordID).Take(15).ToList();
            }
            objModel.Games = lstResult;

            return PartialView("_Games", objModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult DeleteGame(long? RecordID, long? LeagueID, long? WinnerID, long? OpponentID)
        {
            db.Games.Remove(db.Games.FirstOrDefault(p => p.RecordID == RecordID));
            db.SaveChanges();

            var lstResult = GetGames(LeagueID, WinnerID, OpponentID);
            if (lstResult.Count > 0)
            {
                lstResult = lstResult.OrderByDescending(p => p.RecordID).Take(15).ToList();
            }

            return View("_Games2", lstResult);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FilterGames(long? LeagueID, long? WinnerID, long? OpponentID)
        {
            var lstResult = GetGames(LeagueID, WinnerID, OpponentID);
            if (lstResult.Count > 0)
            {
                lstResult = lstResult.OrderByDescending(p => p.RecordID).Take(15).ToList();
            }

            return PartialView("_Games2", lstResult);
        }
        [ValidateAntiForgeryToken]
        public ActionResult PagingGames(int pageIndex, long? LeagueID, long? WinnerID, long? OpponentID)
        {
            pageIndex = pageIndex - 1;

            var lstResult = GetGames(LeagueID, WinnerID, OpponentID);
            lstResult = lstResult.OrderByDescending(p => p.RecordID).Skip(pageIndex * 15).Take(15).ToList();

            return PartialView("_Games2", lstResult);
        }
        #endregion
        #region Leaderboard
        [ValidateAntiForgeryToken]
        public ActionResult ViewLeaderboard()
        {
            LeaderBoardModel objModel = new LeaderBoardModel();
            objModel.Leagues = db.LeaguesMaster.OrderByDescending(p => p.RecordID).ToList();
            objModel.Players = db.PlayersMaster.OrderBy(p => p.Name).ToList();
            objModel.LeaderBoard = GetLeaderboard(defaultLeague.RecordID, null);
            return PartialView("_Leaderboard", objModel);
        }
        private List<LeaderBoard> GetLeaderboard(long? LeagueID, long? PlayerID, int? ExcludeCategory = null, bool? SortByPlayed = false)
        {
            List<LeaderBoard> lstResult = new List<LeaderBoard>();

            List<PlayersMaster> Players = new List<PlayersMaster>();
            List<Games> Games = new List<Games>();

            if (PlayerID == null)
            {
                if (ExcludeCategory.HasValue)
                {
                    Players = db.PlayersMaster.Where(p => p.IsActive == true && p.Category != ExcludeCategory).ToList();
                }
                else
                {
                    Players = db.PlayersMaster.Where(p => p.IsActive == true).ToList();
                }
            }
            else
                Players = db.PlayersMaster.Where(p => p.RecordID == PlayerID && p.IsActive == true).ToList();

            if (LeagueID == null)
            {
                Games = db.Games
               .Where(p => p.CreatedOn > dateFilter
                          && p.LeaguesPlayersID != null
                          && p.OpponentPlayerID != null).ToList();
            }
            else
            {
                Games = db.Games
                .Where(p => p.CreatedOn > dateFilter && p.LeaguesPlayersMaster.LeagueID == LeagueID
                           && p.LeaguesPlayersID != null
                           && p.OpponentPlayerID != null).ToList();
            }

            if (Players.Count > 0)
            {
                foreach (var player in Players)
                {
                    int? Played = 0;
                    int? Won = 0;
                    int? Lost = 0;
                    int? Points = 0;
                    string History = "";

                    int? RunsScored = 0;
                    decimal? OversPlayed = 0;
                    int? WicketsLost = 0;

                    int? Fifties = 0;
                    int? Hundreds = 0;
                    int? FiveWickets = 0;

                    int? RunsConceded = 0;
                    decimal? OversBowled = 0;
                    int? WicketsTaken = 0;

                    if (Games.Count > 0)
                    {
                        foreach (var game in Games)
                        {
                            // Games Won
                            if (game.LeaguesPlayersMaster.PlayerID == player.RecordID)
                            {
                                Played += 1;
                                Won += 1;
                                History += "W|";
                                Points += 2;

                                if ((game.LeaguesPlayersMaster.LeaguesMaster.ParticipationPoints ?? 0) > 0)
                                    Points += 1;

                                Fifties += game.Fifties;
                                Hundreds += game.Hundreds;
                                FiveWickets += game.FiveWickets;

                                RunsScored += game.Runs;
                                OversPlayed += Convert.ToDecimal(game.Overs);
                                WicketsLost += game.Wickets;
                                RunsConceded += game.OpponentRuns;
                                OversBowled += Convert.ToDecimal(game.OpponentOvers);
                                WicketsTaken += game.OpponentWickets;

                                // For NRR
                                //if (game.Wickets != 10)
                                //    OversPlayedNRR += Convert.ToDecimal(game.Overs);
                                //else
                                //    OversPlayedNRR += Convert.ToDecimal(Regex.Match(game.LeaguesPlayersMaster.LeaguesMaster.Format, @"\d+").Value);

                                //OversBowledNRR += Convert.ToDecimal(Regex.Match(game.LeaguesPlayersMaster.LeaguesMaster.Format, @"\d+").Value);
                            }

                            // Games Lost
                            if (game.OpponentPlayerID == player.RecordID)
                            {
                                Played += 1;
                                Lost += 1;
                                History += "L|";

                                if ((game.LeaguesPlayersMaster.LeaguesMaster.ParticipationPoints ?? 0) > 0)
                                    Points += 1;

                                Fifties += game.OpponentFifties;
                                Hundreds += game.OpponentHundreds;
                                FiveWickets += game.OpponentFiveWickets;

                                RunsScored += game.OpponentRuns;
                                OversPlayed += Convert.ToDecimal(game.OpponentOvers);
                                WicketsLost += game.OpponentWickets;
                                RunsConceded += game.Runs;
                                OversBowled += Convert.ToDecimal(game.Overs);
                                WicketsTaken += game.Wickets;

                                // For NRR
                                //OversPlayedNRR += Convert.ToDecimal(Regex.Match(game.LeaguesPlayersMaster.LeaguesMaster.Format, @"\d+").Value);
                            }

                            // Category League Points logic
                            if (game.LeaguesPlayersMaster.LeaguesMaster.IsCategoryLeague.HasValue && game.LeaguesPlayersMaster.LeaguesMaster.IsCategoryLeague.Value)
                            {
                                if (game.LeaguesPlayersMaster.PlayersMaster.Category.HasValue && game.PlayersMaster.Category.HasValue)
                                {
                                    //if(player.Name.ToUpper().Contains("AHMED WA") || player.Name.ToUpper().Contains("TYB")) for testing only
                                    {
                                        int winnerCategory = game.LeaguesPlayersMaster.PlayersMaster.Category.Value;
                                        int opponentCategory = game.PlayersMaster.Category.Value;

                                        // If player won and won against higher cateogry
                                        if (game.LeaguesPlayersMaster.PlayerID == player.RecordID)
                                        {
                                            if (winnerCategory > opponentCategory)
                                                Points += 1;
                                        }

                                        // If player lost and lost against lower category
                                        if (game.OpponentPlayerID == player.RecordID)
                                        {
                                            if (winnerCategory > opponentCategory)
                                                Points -= 1;
                                        }
                                    }
                                }

                            }

                        }
                    }

                    if (Played > 0)
                    {
                        LeaderBoard objLeaderBoard = new LeaderBoard();
                        objLeaderBoard.Player = player.Name;
                        objLeaderBoard.FileName = player.FileName;
                        objLeaderBoard.Country = player.Country;
                        objLeaderBoard.GroupName = LeagueID == null ? "N/A" : db.LeaguesPlayersMaster.FirstOrDefault(p => p.LeagueID == LeagueID && p.PlayerID == player.RecordID).Groups.GroupName;
                        objLeaderBoard.Played = Played;
                        objLeaderBoard.Won = Won;
                        objLeaderBoard.Lost = Lost;
                        objLeaderBoard.Points = Points;
                        objLeaderBoard.History = History;
                        objLeaderBoard.RunsScored = RunsScored;
                        objLeaderBoard.OversPlayed = OversPlayed;
                        objLeaderBoard.WicketsLost = WicketsLost;
                        objLeaderBoard.Fifties = Fifties;
                        objLeaderBoard.Hundreds = Hundreds;
                        objLeaderBoard.FiveWickets = FiveWickets;
                        objLeaderBoard.RunsConceded = RunsConceded;
                        objLeaderBoard.OversBowled = OversBowled;
                        objLeaderBoard.WicketsTaken = WicketsTaken;
                        objLeaderBoard.History = History;
                        objLeaderBoard.WinPercentage = Won > 0 ? (Won * 100.00) / Played : 0.00;

                        //try
                        //{
                        //    objLeaderBoard.NRR = (RunsScored / OversPlayedNRR) - (RunsConceded / OversBowledNRR);
                        //}
                        //catch
                        //{
                        //    objLeaderBoard.NRR = 0;
                        //}

                        objLeaderBoard.Average = RunsScored / OversPlayed;
                        objLeaderBoard.Economy = RunsConceded / OversBowled;

                        lstResult.Add(objLeaderBoard);
                    }

                }
            }

            ViewData["TotalRecords"] = lstResult.Count();

            if (SortByPlayed.HasValue && SortByPlayed.Value)
                return lstResult
                    .OrderByDescending(p => p.Played)
                    .ThenByDescending(p => p.Points)
                    .ThenByDescending(p => p.Average)
                    .ThenByDescending(p => p.Economy).ToList();

            return lstResult
                .OrderByDescending(p => p.Points)
                .ThenByDescending(p => p.Average)
                .ThenByDescending(p => p.Economy)
                .ToList();
        }

        [ValidateAntiForgeryToken]
        public ActionResult FilterLeaderboard(long? LeagueID, long? PlayerID, string GroupView)
        {
            List<LeaderBoard> result = GetLeaderboard(LeagueID, PlayerID);

            if (GroupView.ToUpper() == "YES" && LeagueID != null && db.LeaguesMaster.FirstOrDefault(p => p.RecordID == LeagueID).IsGroupLeague == true)
            {
                //Group based on Group Name
                result = result.GroupBy(x => x.GroupName)
                        .Select(y => new LeaderBoard
                        {
                            Player = y.Max(p => p.GroupName),
                            Played = y.Sum(p => p.Played),
                            Won = y.Sum(p => p.Won),
                            Lost = y.Sum(p => p.Lost),
                            Points = y.Sum(p => p.Points),
                            RunsScored = y.Sum(p => p.RunsScored),
                            OversPlayed = y.Sum(p => p.OversPlayed),
                            WicketsLost = y.Sum(p => p.WicketsLost),
                            RunsConceded = y.Sum(p => p.RunsConceded),
                            OversBowled = y.Sum(p => p.OversBowled),
                            WicketsTaken = y.Sum(p => p.WicketsTaken),
                            Fifties = y.Sum(p => p.Fifties),
                            Hundreds = y.Sum(p => p.Hundreds),
                            FiveWickets = y.Sum(p => p.FiveWickets),
                            WinPercentage = y.Sum(p => p.Won) > 0 ? (y.Sum(p => p.Won) * 100.00) / y.Sum(p => p.Played) : 0.00,
                            NRR = (y.Sum(p => p.RunsScored) / y.Sum(p => p.OversPlayed)) - (y.Sum(p => p.RunsConceded) / y.Sum(p => p.OversBowled)),
                            GroupName = y.Max(p => p.GroupName)
                        }).OrderByDescending(p => p.Points).ThenByDescending(p => p.NRR).ToList();
            }

            return PartialView("_Leaderboard2", result);
        }
        #endregion

        public ActionResult TopPerformers()
        {
            dateFilter = DateTime.Now.AddDays(-7);
            var objModel = GetLeaderboard(null, null).Take(12).ToList();
            return View(objModel);
        }

        public ActionResult TopActive()
        {
            dateFilter = DateTime.Now.AddDays(-7);
            var objModel = GetLeaderboard(null, null, 0, true).Take(16).ToList();
            return View(objModel);
        }

        public ActionResult Knockouts()
        {
            var objModel = new CustomModels.KnockoutsViewerModel();

            objModel.LeaguesAll = db.LeaguesMaster.OrderByDescending(p => p.RecordID).ToList();
            objModel.Knockouts = db.Knockouts.Where(p => p.LeagueID == defaultLeague.RecordID).OrderBy(p => p.RecordID).ToList();

            //ViewData["defaultLeague"] = defaultLeague.RecordID;
            //ViewData["defaultLeagueName"] = defaultLeague.Name;
            return View(objModel);
        }

        public ActionResult FetchKnockouts(long? leagueID)
        {
            var result = db.Knockouts.Where(p => p.LeagueID == leagueID).OrderBy(p => p.RecordID).ToList();
            //ViewData["defaultLeagueName"] = db.LeaguesMaster.FirstOrDefault(p => p.RecordID == leagueID).Name;
            return View("_Knockouts", result);
        }

        #region Stars
        [ValidateAntiForgeryToken]
        public ActionResult ViewLeagueStars()
        {
            LeaguePlayersModel objModel = new LeaguePlayersModel();
            objModel.LeaguesAll = db.LeaguesMaster.OrderByDescending(p => p.RecordID).ToList();
            objModel.PlayersAll = db.PlayersMaster.OrderBy(p => p.Name).ToList();
            objModel.LeaguePlayers = db.LeaguesPlayersMaster.Where(p => p.LeaguesMaster.RecordID == defaultLeague.RecordID).OrderBy(p => p.PlayersMaster.Name).ToList();
            objModel.Groups = db.Groups.ToList();
            objModel.Stars = FetchLeagueStars(defaultLeague.RecordID, null);
            return View("AllStars", objModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchLeagueStarsView(long? LeagueID, long? PlayerID)
        {
            return View("_AllStars", FetchLeagueStars(LeagueID, PlayerID));
        }
        public List<StarsModel> FetchLeagueStars(long? LeagueID, long? PlayerID)
        {
            var leaguesPlayersMaster = new List<LeaguesPlayersMaster>();

            if (PlayerID == null)
                leaguesPlayersMaster = db.LeaguesPlayersMaster.Where(p => p.LeagueID == LeagueID).OrderBy(p => p.PlayersMaster.Name).ToList();
            else
                leaguesPlayersMaster = db.LeaguesPlayersMaster
                    .Where(p => p.LeagueID == LeagueID && p.PlayersMaster.RecordID == PlayerID)
                    .OrderBy(p => p.PlayersMaster.Name)
                    .ToList();


            List<StarsModel> objStars = new List<StarsModel>();
            foreach (var leaguePlayer in leaguesPlayersMaster)
            {
                StarsModel objStar = new StarsModel();
                objStar.Name = leaguePlayer.PlayersMaster.Name;
                objStar.FileName = leaguePlayer.PlayersMaster.FileName;
                objStar.Category = leaguePlayer.PlayersMaster.Category;
                objStar.Country = leaguePlayer.PlayersMaster.Country;

                var gameData = db.Games
                    .Where(p => p.LeaguesPlayersMaster.PlayersMaster.RecordID == leaguePlayer.PlayersMaster.RecordID
                    || p.PlayersMaster.RecordID == leaguePlayer.PlayersMaster.RecordID);

                objStar.Games = gameData.Count();

                decimal? OversPlayed = 0;
                decimal? OversBowled = 0;
                decimal? RunsConceded = 0;

                if (gameData.Count() > 0)
                {
                    foreach (var game in gameData)
                    {
                        if (game.PlayersMaster != null)
                        {
                            if (game.PlayersMaster.RecordID == leaguePlayer.PlayersMaster.RecordID)
                            {
                                objStar.Runs += game.OpponentRuns;
                                objStar.Wickets += game.Wickets ?? 0;
                                OversPlayed += Convert.ToDecimal(game.OpponentOvers);
                                OversBowled += Convert.ToDecimal(game.Overs);
                                RunsConceded += game.Runs;
                            }
                            else
                            {
                                objStar.Runs += game.Runs;
                                objStar.Wins += 1;
                                objStar.Wickets += game.OpponentWickets ?? 0;
                                OversPlayed += Convert.ToDecimal(game.Overs);
                                OversBowled += Convert.ToDecimal(game.OpponentOvers);
                                RunsConceded += game.OpponentRuns;
                            }
                        }
                    }

                    objStar.WinPercentage = objStar.Wins > 0 ? (objStar.Wins * 100.00) / objStar.Games : 0.00;
                    objStar.Average = objStar.Runs / OversPlayed;
                    objStar.Economy = RunsConceded / OversBowled;

                    objStars.Add(objStar);
                }
            }

            return objStars;
        }
        #endregion

    }
}