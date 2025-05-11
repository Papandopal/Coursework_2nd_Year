using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Entities.ModelsForControllers;
using Agar.io_Alpfa.RulesNameSpace;

namespace Agar.io_Alpfa.Controllers
{
    public class VotingController : Controller
    {
        static ConcurrentDictionary<string, int> MapVotes = new ConcurrentDictionary<string, int>();
        static ConcurrentDictionary<string, int> SpeedVotes = new ConcurrentDictionary<string, int>();

        [HttpGet]
        public IActionResult Voting(string name)
        {
            ViewBag.Name = name;
            return View();
        }

        [HttpPost]
        public void Voting([FromBody] VoteModel model)
        {

            SaveAnthver(MapVotes, model.SelectedMapSize, Rules.MapSizes, ref Rules.MapSize);

            SaveAnthver(SpeedVotes, model.SelectedSpeed, Rules.PlayerSpeeds, ref Rules.PlayerBasicSpeed);

        }

        void SaveAnthver(IDictionary<string, int> DictWithVotes, string SelectedItem, IDictionary<string, int> DictFromRules, ref int SelectedItemFromRules)
        {
            int checkKeyValue;
            if (DictWithVotes.TryGetValue(SelectedItem, out checkKeyValue))
            {
                DictWithVotes[SelectedItem] += 1;
            }
            else
            {
                DictWithVotes.TryAdd(SelectedItem, 1);
            }

            string MaxVotesSpeed = string.Empty;

            try
            {
                MaxVotesSpeed = DictWithVotes.Max().Key;
            }
            catch (ArgumentException ex)
            {
                var max_votes = DictWithVotes.Values.Max();
                ConcurrentList<string> max_votes_speeds = new ConcurrentList<string>();

                foreach (var item in SpeedVotes)
                {
                    if (item.Value == max_votes) max_votes_speeds.Add(item.Key);
                }

                var rand = new Random();
                MaxVotesSpeed = max_votes_speeds[rand.Next(0, max_votes_speeds.Count)];

            }
            finally
            {
                SelectedItemFromRules = DictFromRules[MaxVotesSpeed];
            }
        }

        [HttpPost]
        public IActionResult RedirectToWaiting(string Name)
        {
            return RedirectToAction("Waiting", "Waiting", new { name = Name });
        }
    }
}
