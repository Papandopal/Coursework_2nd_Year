using System.ComponentModel.DataAnnotations;

namespace Agar.io_Alpfa.Entities.ModelsForControllers
{
    public class RegistrModel
    {
        [Required(ErrorMessage = "Требуется email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Требуется пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать от {2} до {1} символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
