using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BookMarket.DataLayer.Models.Cart
{
    [Index(nameof(OwnerId), IsUnique = true)]
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OwnerId { get; set; } = null!;
        public List<CartItem> Items { get; set; } = new();
    }
}
