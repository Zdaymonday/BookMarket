using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookMarket.ViewModels.Admin
{
    public class EditUserViewModel : UserViewModel
    {
        public List<string> AvailableRoles { get; set; } = new();
        public EditUserViewModel
            (string userId = "", string userName = "", string userEmail = "")
            :base(userId, userName, userEmail) { }

        public EditUserViewModel() : base() { }
    }
}
