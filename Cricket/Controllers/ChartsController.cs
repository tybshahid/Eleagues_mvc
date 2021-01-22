using Cricket.CustomModels;
using Cricket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cricket.Controllers
{
    public class ChartsController : Controller
    {
        private CricketEntities db;
        public ChartsController()
        {
            db = new CricketEntities();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
        }
        [ValidateAntiForgeryToken]
        public ActionResult Charts()
        {
            ChartsData model = new ChartsData();
            model.LeadingScorers = GetLeadingScorers("VETERAN");
            return View(model);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ViewLeadingScorersVet()
        {
            var lstResult = GetLeadingScorers("VETERAN");
            return PartialView("_LeadingRunsScorersVet", lstResult);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ViewLeadingScorersPro()
        {
            var lstResult = GetLeadingScorers("PRO");
            return PartialView("_LeadingRunsScorersPro", lstResult);
        }

        private List<LeadingScorer> GetLeadingScorers(string difficulty)
        {
            var lstWon = db.Games
                        .Where(p => p.LeaguesPlayersMaster.LeaguesMaster.Difficulty.ToUpper() == difficulty)
                        .GroupBy(x => x.LeaguesPlayersMaster.PlayersMaster.Name)
                        .Select(y => new
                        {
                            Name = y.FirstOrDefault().LeaguesPlayersMaster.PlayersMaster.Name,
                            Runs = y.Sum(p => p.Runs),
                            Country = y.FirstOrDefault().LeaguesPlayersMaster.PlayersMaster.Country
                        }).ToList();

            var lstLost = db
                        .Games
                        .Where(p => p.LeaguesPlayersMaster.LeaguesMaster.Difficulty.ToUpper() == difficulty)
                        .GroupBy(x => x.PlayersMaster.Name)
                        .Select(y => new
                        {
                            Name = y.FirstOrDefault().PlayersMaster.Name,
                            Runs = y.Sum(p => p.OpponentRuns),
                            Country = y.FirstOrDefault().LeaguesPlayersMaster.PlayersMaster.Country
                        }).ToList();

            var lstAll = lstWon.Union(lstLost).GroupBy(p => p.Name)
                        .Select(p => new
                        {
                            Player = p.FirstOrDefault().Name,
                            Runs = p.Sum(q => q.Runs),
                            Country = p.FirstOrDefault().Country
                        }).ToList();

            List<LeadingScorer> lstResult = new List<LeadingScorer>();
            foreach (var item in lstAll)
            {
                LeadingScorer obj = new LeadingScorer();
                obj.Name = item.Player;
                obj.Runs = item.Runs;
                obj.Country = item.Country;
                lstResult.Add(obj);
            }

            return lstResult.OrderByDescending(p => p.Runs).Take(20).ToList();
        }
        #region unsed
        //var lstGamesWinnerLoser = db.Games
        //    .Join(db.Games
        //    .Select(l => new
        //    {
        //        RecordID = l.RecordID,
        //        LoserName = l.PlayersMaster.Name,
        //        LoserRuns = l.OpponentRuns
        //    }), w => w.RecordID, l => l.RecordID, (w, l) =>
        //    new
        //    {
        //        RecordID = w.RecordID,
        //        WinnerName = w.LeaguesPlayersMaster.PlayersMaster.Name,
        //        WinnerRuns = w.Runs,
        //        LoserName = l.LoserName,
        //        LoserRun = l.LoserRuns
        //    }).AsEnumerable();
        #endregion

    }
}