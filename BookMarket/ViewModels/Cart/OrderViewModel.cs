using System.ComponentModel.DataAnnotations;

namespace BookMarket.ViewModels.Cart
{
    public class OrderViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; } = null!;

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string UserPhone { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = null!;

        public string? Name { get; set; }
        public string? Address { get; set; }
    }
}
