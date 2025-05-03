using Agar.io_Alpfa.RulesNameSpace;
using Agar.io_Alpfa.Entities.ModelsForControllers;
using Microsoft.AspNetCore.Mvc;

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
            if (ModelState.IsValid)
            {
                Rules.NamesOfVoters.Add(model.Name);
                return RedirectToAction("Voting", "Voting", new { name = model.Name });
            }
            return View(model);
        }
    }
}
