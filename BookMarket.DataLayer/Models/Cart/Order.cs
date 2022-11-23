using System.ComponentModel.DataAnnotations;

namespace BookMarket.DataLayer.Models.Cart
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string OwnerId { get; set; } = null!;

        public string? Name { get; set; }
        public string? Address { get; set; }

        public List<OrderPosition> Items { get; set; } = new();

        public PaymentStatus PaymentStatus { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public enum PaymentStatus
    {
        Unpaid,
        Paid,
    }

    public enum OrderStatus
    {
        Canceled,
        Accepted,
        Formed,
        Ready,
        Sent,
        Delivered,
        Completed,
    }
}
