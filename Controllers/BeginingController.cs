using Agar.io_Alpfa.RulesNameSpace;
using Agar.io_Alpfa.Entities.ModelsForControllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;

namespace Agar.io_Alpfa.Controllers
{
    public class BeginingController : Controller
    {
        [HttpGet]
        public IActionResult Begining()
        {
            if(UrlHolder.Url == "None")UrlHolder.Url = Environment.GetEnvironmentVariable("TUNNEL_URL") ?? "Not find tunnel";
            return View();
        }

        [HttpPost]
        public IActionResult Begining(BeginingModel model)
        {
            if(Rules.NamesOfVoters.Count()==Rules.HowManyPeoplesVote && Rules.HowManyPeoplesVote!=0) return View();
            if (ModelState.IsValid)
            {
                Rules.NamesOfVoters.Add(model.Name);
                return RedirectToAction("Voting", "Voting", new { name = model.Name });
            }
            return View(model);
        }
    }
}
