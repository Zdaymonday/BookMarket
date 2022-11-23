using System.ComponentModel.DataAnnotations;

namespace BookMarket.ViewModels.Account
{
    public class UserProfileViewModel
    {
        [Required]
        public string UserLogin { get; set; } = null!;

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = null!;
    }
}
