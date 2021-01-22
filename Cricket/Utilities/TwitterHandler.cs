using Cricket.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Cricket.Utilities
{
    public class TwitterHandler
    {
        const string consumerKey = "hI8ckFrhbHClGfN8lMWhDIh52";
        const string consumerSecret = "dshwZaWvDbdN0BXTqbRWj2NdSBCvoIoi3lxvkphgIgLYBlkih8";
        const string userAccessToken = "1167214577928560640-Z7d37Re9jVvRbVjRUcb9ymcFS1PDUW";
        const string userAccessSecret = "O0RhvblDNROE0cCxkubl0dF9lcLNrm9dQjy6Ji7a5VgWJ";

        public TwitterHandler()
        {
            Auth.SetUserCredentials(consumerKey, consumerSecret, userAccessToken, userAccessSecret);
        }

        public string TweetText()
        {
            string response = "";
            try
            {
                //string league = "Test League";
                //string round = "2";
                //string winner = "haseeb";
                //string runs = "199";
                //string wkts = "4";
                //string overs = "20";
                //string loser = "tyb";
                //string loserRuns = "189";
                //string loserWkts = "10";
                //string loserOvers = "19";

                //string message = String.Format("{0} - {1} Defeats {2} in Round {3}\n\n{1} Scored {4} for {5} in {6} Overs\n{2} Scored {7} for {8} in {9} Overs",
                //                                league, winner, loser, round, runs, wkts, overs, loserRuns, loserWkts, loserOvers);

                string message = Guid.NewGuid().ToString();

                var tweet = Tweet.PublishTweet(message);
                response = tweet.Text;
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }

            return response;
        }

        public string SendTweet(Games game, HttpPostedFileBase file)
        {
            byte[] data;
            using (System.IO.Stream inputStream = file.InputStream)
            {
                inputStream.Position = 0;

                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }

            var media = Upload.UploadBinary(data);

            string message = String.Format("{0} - {1} Defeats {2} in Round {3}\n\n{1} Scored {4} for {5} in {6} Overs\n{2} Scored {7} for {8} in {9} Overs\n\nCongratulations {1} #Cricket19",
            game.LeaguesPlayersMaster.LeaguesMaster.Name,
            game.LeaguesPlayersMaster.PlayersMaster.Name,
            game.PlayersMaster.Name,
            game.Round,
            game.Runs, game.Wickets, game.Overs,
            game.OpponentRuns, game.OpponentWickets, game.OpponentOvers);

            var tweet = Tweet.PublishTweet(message, new PublishTweetOptionalParameters
            {
                Medias = new List<IMedia> { media }
            });

            return tweet.Url;

        }
    }
}