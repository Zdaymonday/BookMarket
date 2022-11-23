using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookMarket.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Remote(action: "CheckEmail", controller: "Account", ErrorMessage = "Адрес уже используется")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
