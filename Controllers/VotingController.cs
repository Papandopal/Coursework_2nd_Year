using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Entities.ModelsForControllers;
using System.Security.Cryptography;
using System.Reflection;

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
            int checkKeyValue;
            if (ModelState.IsValid)
            {
                if (MapVotes.TryGetValue(model.SelectedMapSize, out checkKeyValue))
                {
                    MapVotes[model.SelectedMapSize] += 1;
                }
                else
                {
                    MapVotes.TryAdd(model.SelectedMapSize, 1);
                }

                if (SpeedVotes.TryGetValue(model.SelectedSpeed, out checkKeyValue))
                {
                    SpeedVotes[model.SelectedSpeed] += 1;
                }
                else
                {
                    SpeedVotes.TryAdd(model.SelectedSpeed, 1);
                }
            }
            
        }
        [HttpPost]
        public IActionResult RedirectToWaiting(string Name)
        {
            return RedirectToAction("Waiting", "Waiting", new { name = Name });
        }
    }
}
