using Cricket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cricket.CustomModels;
using System.Data.Entity;
using Cricket.Utilities;
using System.IO;
using System.Drawing;
using Tweetinvi;

namespace Cricket.Controllers
{
    public class AdministrationController : Controller
    {
        private CricketEntities db;
        public AdministrationController()
        {
            db = new CricketEntities();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }
        [ValidateAntiForgeryToken]
        public ActionResult Administration()
        {
            return View();
        }
        #region LeagueCreator
        [ValidateAntiForgeryToken]
        public ActionResult OpenLeagueCreator()
        {
            LeagueCreatorModel objLeagueCreatorModel = new LeagueCreatorModel();
            objLeagueCreatorModel.LeaguesAll = db.LeaguesMaster.ToList();
            objLeagueCreatorModel.LeagueSelected = new LeaguesMaster();
            return View("LeagueCreator", objLeagueCreatorModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchLeagueData(long? LeagueID)
        {
            return View("_LeagueCreator", LeagueID == null ? new LeaguesMaster() : db.LeaguesMaster.FirstOrDefault(p => p.RecordID == LeagueID));
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult SaveLeague(LeaguesMaster objLeague, long? LeagueID)
        {
            try
            {
                {
                    objLeague.CreatedBy = "System Admin";
                    objLeague.CreatedOn = DateTime.Now;

                    if (LeagueID == null)
                        db.LeaguesMaster.Add(objLeague);
                    else
                    {
                        LeaguesMaster objLeagueExisting = db.LeaguesMaster.FirstOrDefault(p => p.RecordID == LeagueID);
                        objLeagueExisting.Name = objLeague.Name;
                        objLeagueExisting.Difficulty = objLeague.Difficulty;
                        objLeagueExisting.Format = objLeague.Format;
                        objLeagueExisting.Rounds = objLeague.Rounds;
                        objLeagueExisting.TeamFormat = objLeague.TeamFormat;
                        objLeagueExisting.MatchTiming = objLeague.MatchTiming;
                        objLeagueExisting.Powerplay = objLeague.Powerplay;
                        objLeagueExisting.NormalizeSkills = objLeague.NormalizeSkills;
                        objLeagueExisting.RopeSettings = objLeague.RopeSettings;
                        objLeagueExisting.PitchSettings = objLeague.PitchSettings;
                        objLeagueExisting.WinPoints = objLeague.WinPoints;
                        objLeagueExisting.LostPoints = objLeague.LostPoints;
                        objLeagueExisting.ParticipationPoints = objLeague.ParticipationPoints;
                        objLeagueExisting.StartDate = objLeague.StartDate;
                        objLeagueExisting.EndDate = objLeague.EndDate;
                        objLeagueExisting.IsClosed = objLeague.IsClosed;
                        objLeagueExisting.CreatedBy = "System Admin";
                        objLeagueExisting.CreatedOn = DateTime.Now;
                        objLeagueExisting.Winner = objLeague.Winner;
                        objLeagueExisting.IsGroupLeague = objLeague.IsGroupLeague;
                        objLeagueExisting.IsDefaultLeague = objLeague.IsDefaultLeague;
                        objLeagueExisting.IsCategoryLeague = objLeague.IsCategoryLeague;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return OpenLeagueCreator();
        }
        #endregion
        #region RulesManager
        [ValidateAntiForgeryToken]
        public ActionResult OpenRulesManager()
        {
            RulesManagerModel objRulesManagerModel = new RulesManagerModel();
            objRulesManagerModel.LeaguesAll = db.LeaguesMaster.ToList();
            objRulesManagerModel.RulesSelectedLeague = new List<LeaguesRulesMaster>();
            return View("RulesManager", objRulesManagerModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchRules(long? LeagueID)
        {
            return View("_Rules", LeagueID == null ? new List<LeaguesRulesMaster>() : db.LeaguesRulesMaster.Where(p => p.LeagueID == LeagueID).ToList());
        }
        [ValidateAntiForgeryToken]
        public ActionResult AddRule(long? LeagueID, string RuleDescription)
        {
            LeaguesRulesMaster objLeaguesRulesMaster = new LeaguesRulesMaster();
            objLeaguesRulesMaster.LeagueID = LeagueID;
            objLeaguesRulesMaster.Rule = RuleDescription;
            objLeaguesRulesMaster.CreatedBy = "System Admin";
            objLeaguesRulesMaster.CreatedOn = DateTime.Now;
            db.LeaguesRulesMaster.Add(objLeaguesRulesMaster);
            db.SaveChanges();

            return FetchRules(LeagueID);
        }
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRule(long? LeagueID, string ruleIds)
        {
            string[] _ruleIds = ruleIds.Split(new char[] { ',' });

            for (int i = 0; i < _ruleIds.Length; i++)
            {
                Int64 ruleID = Int64.Parse(_ruleIds[i]);
                db.LeaguesRulesMaster.Remove(db.LeaguesRulesMaster.FirstOrDefault(p => p.RecordID == ruleID));
            }

            db.SaveChanges();
            return FetchRules(LeagueID);
        }
        #endregion
        #region PlayerManager
        [ValidateAntiForgeryToken]
        public ActionResult OpenPlayerManager()
        {
            PlayerManagerModel objPlayerManagerModel = new PlayerManagerModel();
            objPlayerManagerModel.PlayersAll = db.PlayersMaster.OrderBy(p => p.Name).ToList();
            objPlayerManagerModel.PlayerSelected = new PlayersMaster();
            objPlayerManagerModel.LeaguesAll = db.LeaguesMaster.Where(p => p.IsClosed != "Yes").OrderBy(p => p.Name).ToList();
            objPlayerManagerModel.LeaguePlayers = new List<LeaguesPlayersMaster>();
            objPlayerManagerModel.Groups = db.Groups.ToList();
            return View("PlayerManager", objPlayerManagerModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchPlayers(long? PlayerID)
        {
            return View("_PlayerManager", PlayerID == null ? new PlayersMaster() : db.PlayersMaster.FirstOrDefault(p => p.RecordID == PlayerID));
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult SavePlayer(long? PlayerID, PlayersMaster objPlayer)
        {
            string uploadResult = "";
            bool isFileUploaded = false;
            string fileName = "";
            try
            {
                if (Request.Files["upload"].ContentLength > 0)
                {
                    isFileUploaded = true;
                    uploadResult = new ImageHelper().Upload(Request.Files["upload"]);
                    if (uploadResult.ToUpper().Contains("ERROR"))
                    {
                        ViewData["ServerError"] = "Unable to upload Photo";
                        return OpenPlayerManager();
                    }
                }

                if (isFileUploaded)
                {
                    HttpPostedFileBase file = Request.Files["upload"];
                    fileName = "~/Uploads/Players/" + objPlayer.Name.Split(' ')[0] + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
                    objPlayer.FileName = objPlayer.Name.Split(' ')[0] + DateTime.Now.Ticks + Path.GetExtension(file.FileName);
                }

                objPlayer.CreatedBy = "System Admin";
                objPlayer.CreatedOn = DateTime.Now;

                if (PlayerID == null)
                    db.PlayersMaster.Add(objPlayer);
                else
                {
                    PlayersMaster objPlayersExisting = db.PlayersMaster.FirstOrDefault(p => p.RecordID == PlayerID);
                    objPlayersExisting.Name = objPlayer.Name;
                    objPlayersExisting.NickName = objPlayer.NickName;
                    objPlayersExisting.PSN = objPlayer.PSN;
                    objPlayersExisting.Country = objPlayer.Country;
                    objPlayersExisting.Email = objPlayer.Email;
                    objPlayersExisting.Mobile = objPlayer.Mobile;
                    objPlayersExisting.IsActive = objPlayer.IsActive;
                    objPlayersExisting.UserID = objPlayer.UserID;
                    objPlayersExisting.Password = objPlayer.Password;
                    objPlayersExisting.CreatedBy = "System Admin";
                    objPlayersExisting.CreatedOn = DateTime.Now;
                    objPlayersExisting.Category = objPlayer.Category;
                    if (isFileUploaded)
                        objPlayersExisting.FileName = objPlayer.FileName;
                }

                db.SaveChanges();

                if (isFileUploaded)
                {
                    Image resizedImage = new Bitmap(Image.FromStream(Request.Files["upload"].InputStream, true, true), new Size(500, 500));
                    resizedImage.Save(Server.MapPath(fileName));
                    //Request.Files["upload"].SaveAs(Server.MapPath(fileName));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return OpenPlayerManager();
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchLeaguePlayers(long? leagueID)
        {
            return View("_PlayerManagerLeague", db.LeaguesPlayersMaster.Where(p => p.LeagueID == leagueID).OrderBy(p => p.PlayersMaster.Name).ToList());
        }
        [ValidateAntiForgeryToken]
        public ActionResult AddPlayertoLeague(long? playerID, long? leagueID, string TeamName, int? GroupID)
        {
            // Check if Valid Group Name is provided
            if (GroupID == null)
                GroupID = 1;

            if (db.LeaguesMaster.FirstOrDefault(p => p.RecordID == leagueID).IsGroupLeague == true && (GroupID == null || GroupID == 1))
            {
                ViewData["ServerError"] = "Group Name not provided!!!";
                return View("_PlayerManagerLeague", db.LeaguesPlayersMaster.Where(p => p.LeagueID == leagueID).ToList());
            }

            // Check if player already exists
            LeaguesPlayersMaster objLeaguesPlayersMaster = db.LeaguesPlayersMaster.FirstOrDefault(p => p.PlayerID == playerID && p.LeagueID == leagueID);
            if (objLeaguesPlayersMaster != null)
            {
                ViewData["ServerError"] = "Player Exists Already!!!";
            }
            else
            {
                // Check if TeamName is provided
                if (string.IsNullOrEmpty(TeamName))
                    TeamName = db.PlayersMaster.FirstOrDefault(p => p.RecordID == playerID).Name.Split(' ')[0] + " 11";

                objLeaguesPlayersMaster = new LeaguesPlayersMaster();
                objLeaguesPlayersMaster.LeagueID = leagueID;
                objLeaguesPlayersMaster.PlayerID = playerID;
                objLeaguesPlayersMaster.TeamName = TeamName;
                objLeaguesPlayersMaster.GroupID = GroupID;
                objLeaguesPlayersMaster.CreatedBy = "System Admin";
                objLeaguesPlayersMaster.CreatedOn = DateTime.Now;
                db.LeaguesPlayersMaster.Add(objLeaguesPlayersMaster);
                db.SaveChanges();
                db.Entry(objLeaguesPlayersMaster).State = EntityState.Detached;
                ViewData["ServerMessage"] = "Player Added Successfully!!!";
            }

            return View("_PlayerManagerLeague", db.LeaguesPlayersMaster.Where(p => p.LeagueID == leagueID).OrderBy(p => p.PlayersMaster.Name).ToList());
        }
        #endregion
        #region SquadsManager
        [ValidateAntiForgeryToken]
        public ActionResult OpenSquadsManager()
        {
            SquadsManagerModel objModel = new SquadsManagerModel();
            objModel.LeaguesPlayersAll = db.LeaguesPlayersMaster.ToList();
            objModel.SquadSelected = new List<LeaguesPlayersTeamMaster>();

            return View("SquadsManager", objModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchSquads(long? LeaguePlayerID)
        {
            ViewData["TeamName"] = db.LeaguesPlayersMaster.FirstOrDefault(p => p.RecordID == LeaguePlayerID).TeamName;

            var result = db.LeaguesPlayersTeamMaster.Where(p => p.LeaguesPlayersID == LeaguePlayerID).ToList();
            return View("_Squads", LeaguePlayerID == null ? new List<LeaguesPlayersTeamMaster>() : result);
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult AddPlayertoSquad(long? LeaguePlayerID, string PlayerName, string PlayerType, string PlayerOrigin, string BowlingStyle)
        {
            try
            {
                {
                    var existingPlayers = db.LeaguesPlayersTeamMaster.Where(p => p.LeaguesPlayersID == LeaguePlayerID).ToList();
                    if (existingPlayers != null && existingPlayers.Count > 15)
                    {
                        ViewData["ServerError"] = "Max 15 Players are Allowed!!!";
                        return FetchSquads(LeaguePlayerID);
                    }

                    foreach (var item in existingPlayers)
                    {
                        if (item.PlayerName == PlayerName)
                        {
                            ViewData["ServerError"] = "Players Exists Already!!!";
                            return FetchSquads(LeaguePlayerID);
                        }
                    }

                    LeaguesPlayersTeamMaster obj = new LeaguesPlayersTeamMaster();
                    obj.LeaguesPlayersID = LeaguePlayerID;
                    obj.PlayerName = PlayerName;
                    obj.PlayerType = PlayerType;
                    obj.PlayerOrigin = PlayerOrigin;
                    obj.BowlingStyle = BowlingStyle;
                    obj.CreatedBy = "System Admin";
                    obj.CreatedOn = DateTime.Now;

                    db.LeaguesPlayersTeamMaster.Add(obj);
                    db.SaveChanges();
                    ViewData["ServerMessage"] = "Added to Squad";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return FetchSquads(LeaguePlayerID);
        }
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFromSquad(long? LeaguePlayerID, long? RecordID)
        {
            db.LeaguesPlayersTeamMaster.Remove(db.LeaguesPlayersTeamMaster.FirstOrDefault(p => p.RecordID == RecordID));
            db.SaveChanges();
            ViewData["ServerMessage"] = "Deleted from Squad";

            return FetchSquads(LeaguePlayerID);
        }
        #endregion
        #region KnockoutsManager
        [ValidateAntiForgeryToken]
        public ActionResult OpenKnockoutsManager()
        {
            KnockoutsManagerModel objModel = new KnockoutsManagerModel();
            objModel.LeaguesAll = db.LeaguesMaster.Where(p => p.IsClosed != "Yes").OrderByDescending(p => p.RecordID).ToList();
            objModel.PlayersAll = db.PlayersMaster.Where(p => p.IsActive == true).OrderBy(p => p.Name).ToList();
            objModel.Knockouts = new List<Knockouts>();

            return View("KnockoutsManager", objModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchKnockouts(long? LeagueID)
        {
            var result = db.Knockouts.Where(p => p.LeagueID == LeagueID).ToList();
            return View("_Knockouts", LeagueID == null ? new List<Knockouts>() : result);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchKnockout(long? RecordID)
        {
            var result = db.Knockouts.FirstOrDefault(p => p.RecordID == RecordID);
            return Json(new { stage = result.Stage, player1 = result.Player1ID, player2 = result.Player2ID, winner = result.WinnerID });
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult SaveKnockout(long? LeagueID, long? Player1ID, long? Player2ID, string Stage, string Winner)
        {
            try
            {
                long? winnerID = null;
                if (Winner == "Player 1")
                    winnerID = Player1ID;
                else if (Winner == "Player 2")
                    winnerID = Player2ID;

                if (Player1ID == Player2ID)
                {
                    ViewData["ServerError"] = "Player1 cannot be same as Player2 !!!";
                    return FetchKnockouts(LeagueID);
                }

                var existingKnockout = db.Knockouts.FirstOrDefault(p => p.LeagueID == LeagueID && p.Player1ID == Player1ID && p.Player2ID == Player2ID && p.Stage == Stage);
                if (existingKnockout != null)
                {
                    existingKnockout.Player1ID = Player1ID;
                    existingKnockout.Player2ID = Player2ID;
                    existingKnockout.WinnerID = winnerID;
                    db.SaveChanges();
                    db.Entry(existingKnockout).State = EntityState.Detached;

                    ViewData["ServerMessage"] = "Knockout Updated";
                    return FetchKnockouts(LeagueID);
                }

                Knockouts obj = new Knockouts();
                obj.Number = db.Knockouts.Where(p => p.LeagueID == LeagueID).Count() + 1;
                obj.LeagueID = LeagueID;
                obj.Player1ID = Player1ID;
                obj.Player2ID = Player2ID;
                obj.Stage = Stage;
                obj.WinnerID = winnerID;
                obj.CreatedOn = DateTime.Now;

                db.Knockouts.Add(obj);
                db.SaveChanges();
                db.Entry(obj).State = EntityState.Detached;
                ViewData["ServerMessage"] = "Knockout Added";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return FetchKnockouts(LeagueID);
        }
        [ValidateAntiForgeryToken]
        public ActionResult DeleteKnockout(long? LeagueID, long? RecordID)
        {
            db.Knockouts.Remove(db.Knockouts.FirstOrDefault(p => p.RecordID == RecordID));
            db.SaveChanges();
            ViewData["ServerMessage"] = "Knockout Deleted";

            return FetchKnockouts(LeagueID);
        }
        #endregion
        #region GameManager
        [ValidateAntiForgeryToken]
        public ActionResult OpenGameManager()
        {
            GameManagerModel objGameManagerModel = new GameManagerModel();
            objGameManagerModel.LeaguesAll = db.LeaguesMaster.Where(p => p.IsClosed != "Yes").OrderByDescending(p => p.RecordID).ToList();
            objGameManagerModel.Winner = new List<LeaguesPlayersMaster>();
            objGameManagerModel.Opponents = new List<LeaguesPlayersMaster>();
            return View("GameManager", objGameManagerModel);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchWinners(long? LeagueID)
        {
            var result = db.LeaguesPlayersMaster
                            .Where(p => p.LeagueID == LeagueID && p.LeaguesMaster.IsClosed != "Yes" && p.PlayersMaster.IsActive == true)
                            .OrderBy(p => p.PlayersMaster.Name).ToList();

            if (Session["AuthenticatedUser"] != null)
            {
                PlayersMaster AuthenticatedUser = (PlayersMaster)Session["AuthenticatedUser"];
                result = result.Where(p => p.PlayerID == AuthenticatedUser.RecordID).ToList();
            }
            else if (Session["IsAdmin"] != null && (bool)Session["IsAdmin"])
            {
                result = result.ToList();
            }
            else
            {
                result = result.Where(p => p.PlayerID == 0).ToList();
            }

            return View("_GameManagerWinner", LeagueID == null ? new List<LeaguesPlayersMaster>() : result);
        }
        [ValidateAntiForgeryToken]
        public ActionResult FetchOpponents(long? WinnerLeaguePlayerID)
        {
            if (WinnerLeaguePlayerID == null)
                return View("_GameManagerOpponent", new List<LeaguesPlayersMaster>());

            // Identify League & Player
            LeaguesPlayersMaster obj = db.LeaguesPlayersMaster.FirstOrDefault(p => p.RecordID == WinnerLeaguePlayerID);

            List<LeaguesPlayersMaster> result = new List<LeaguesPlayersMaster>();
            if (obj.LeaguesMaster.IsGroupLeague == true)
            {
                result = db
                            .LeaguesPlayersMaster
                            .Where(p => p.LeagueID == obj.LeagueID && p.PlayerID != obj.PlayerID && p.GroupID != obj.GroupID && p.PlayersMaster.IsActive == true)
                            .OrderBy(p => p.PlayersMaster.Name)
                            .ToList();
            }
            else
            {
                result = db.LeaguesPlayersMaster
                            .Where(p => p.LeagueID == obj.LeagueID && p.PlayerID != obj.PlayerID && p.PlayersMaster.IsActive == true)
                            .OrderBy(p => p.PlayersMaster.Name).ToList();
            }

            return View("_GameManagerOpponent", WinnerLeaguePlayerID == null ? new List<LeaguesPlayersMaster>() : result);
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult SaveGame(Games Game)
        {
            string uploadResult = "";
            try
            {
                {
                    if (Request.Files["upload"].ContentLength > 0)
                    {
                        uploadResult = new ImageHelper().Upload(Request.Files["upload"]);
                        if (uploadResult.ToUpper().Contains("ERROR"))
                        {
                            ViewData["ServerError"] = "Unable to upload Scorecard";
                            return OpenGameManager();
                        }
                    }
                    else
                    {
                        ViewData["ServerError"] = "Scorecard not attached";
                        return OpenGameManager();
                    }

                    long WinnerLeaguePlayerID = 0;
                    if (long.TryParse(Request.Form["ddlWinner"], out WinnerLeaguePlayerID)) { }
                    else
                    {
                        ViewData["ServerError"] = "Invalid Data - Winner ID";
                        return OpenGameManager();
                    }

                    long OpponentID = 0;
                    if (long.TryParse(Request.Form["ddlOpponent"], out OpponentID)) { }
                    else
                    {
                        ViewData["ServerError"] = "Invalid Data - Opponent ID";
                        return OpenGameManager();
                    }

                    decimal Overs = 0;
                    if (decimal.TryParse(Game.Overs, out Overs)) { }
                    else
                    {
                        ViewData["ServerError"] = "Invalid Data - Overs";
                        return OpenGameManager();
                    }

                    if (decimal.TryParse(Game.OpponentOvers, out Overs)) { }
                    else
                    {
                        ViewData["ServerError"] = "Invalid Data - Opponent Overs";
                        return OpenGameManager();
                    }

                    Game.Fifties = Game.Fifties == null ? 0 : Game.Fifties;
                    Game.OpponentFifties = Game.OpponentFifties == null ? 0 : Game.OpponentFifties;
                    Game.FiveWickets = Game.FiveWickets == null ? 0 : Game.FiveWickets;
                    Game.OpponentFiveWickets = Game.OpponentFiveWickets == null ? 0 : Game.OpponentFiveWickets;
                    Game.Hundreds = Game.Hundreds == null ? 0 : Game.Hundreds;
                    Game.OpponentHundreds = Game.OpponentHundreds == null ? 0 : Game.OpponentHundreds;

                    Game.LeaguesPlayersID = WinnerLeaguePlayerID;
                    Game.OpponentPlayerID = OpponentID;
                    Game.IsApproved = false;
                    Game.InningsHigh = 0;
                    Game.CreatedBy = Session["AuthenticatedUser"] == null ? "System Admin" : ((PlayersMaster)Session["AuthenticatedUser"]).UserID;
                    Game.CreatedOn = DateTime.Now;
                    db.Games.Add(Game);
                    db.SaveChanges();

                    db.Entry(Game).State = EntityState.Detached;
                    Game = db.Games.FirstOrDefault(p => p.RecordID == Game.RecordID);

                    int roundsPlayed = db.Games.Where(p => p.LeaguesPlayersMaster.LeagueID == Game.LeaguesPlayersMaster.LeagueID && p.Stage == Game.Stage &&
                                       (p.LeaguesPlayersMaster.PlayerID == Game.LeaguesPlayersMaster.PlayerID || p.LeaguesPlayersMaster.PlayerID == Game.OpponentPlayerID)
                                       && (p.OpponentPlayerID == Game.OpponentPlayerID || p.OpponentPlayerID == Game.LeaguesPlayersMaster.PlayerID)).Count();

                    int LeagueRounds = int.Parse(db.LeaguesPlayersMaster.FirstOrDefault(p => p.RecordID == WinnerLeaguePlayerID).LeaguesMaster.Rounds);
                    if (roundsPlayed > LeagueRounds)
                    {
                        // delete the game
                        db.Games.Remove(Game);
                        db.SaveChanges();

                        ViewData["ServerError"] = "Rounds already completed!!!";
                        return OpenGameManager();
                    }

                    Game.Round = roundsPlayed;

                    HttpPostedFileBase file = Request.Files["upload"];
                    string fileName = "~/Uploads/" + Game.RecordID + Path.GetExtension(file.FileName);
                    Game.FileName = Game.RecordID + Path.GetExtension(file.FileName);

                    db.SaveChanges();

                    Request.Files["upload"].SaveAs(Server.MapPath(fileName));
                    string url = new TwitterHandler().SendTweet(Game, file);

                    ViewData["ServerMessage"] = "Game Saved";

                    string message = String
                        .Format("{0}\n\n*{1}* Defeats *{2}* in Round {3}\n\n{1} Scored {4} for {5} in {6} Overs\n{2} Scored {7} for {8} in {9} Overs\n\n{10}",
                        Game.LeaguesPlayersMaster.LeaguesMaster.Name,
                        Game.LeaguesPlayersMaster.PlayersMaster.Name,
                        Game.PlayersMaster.Name,
                        Game.Round,
                        Game.Runs, Game.Wickets, Game.Overs,
                        Game.OpponentRuns,
                        Game.OpponentWickets,
                        Game.OpponentOvers,
                        url);

                    ViewData["WinMessage"] = message;
                }
            }
            catch (Exception ex)
            {
                ViewData["ServerError"] = ex.InnerException;
                return OpenGameManager();
            }

            return OpenGameManager();
        }
        #endregion
    }
}