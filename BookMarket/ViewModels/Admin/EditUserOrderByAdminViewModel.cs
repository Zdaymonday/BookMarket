using BookMarket.DataLayer.Models.Cart;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookMarket.ViewModels.Admin
{
    public class EditUserOrderByAdminViewModel
    {
        public Order Order { get; set; }
        public SelectList PaymentStatusList { get; set; }
        public SelectList OrderStatusList { get; set; }
    }
}
