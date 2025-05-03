using System.ComponentModel.DataAnnotations;

namespace Agar.io_Alpfa.Entities.ModelsForControllers
{
    public class BeginingModel
    {
        [Required(ErrorMessage = "Требуется ваше игровое имя")]
        [StringLength(100, ErrorMessage = "Имя должено содержать от {2} до {1} символов", MinimumLength = 6)]
        public string Name { get; set; }
    }
}
