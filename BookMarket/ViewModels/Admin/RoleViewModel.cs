using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookMarket.ViewModels.Admin
{
    public class RoleViewModel
    {
        [Required]
        [StringLength(15, MinimumLength = 4)]
        [Remote(action:"IsRoleNameExist", controller:"Admin")]
        public string Name { get; set; } = null!;

        public string Id { get; set; } = null!;
    }
}
