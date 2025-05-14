using Agar.io_Alpfa.Models;
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
