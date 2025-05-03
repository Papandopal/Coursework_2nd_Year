using Microsoft.AspNetCore.Mvc;
using Agar.io_Alpfa.Entities.ModelsForControllers;

namespace Agar.io_Alpfa.Controllers
{
    public class RegistrationController : Controller
    {
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Registration(RegistrModel model)
        {
            if (ModelState.IsValid)
            {
               //save model in database
                
                return RedirectToAction("Begining", "Begining");
            }

            return View(model);
        }
       
    }
}
