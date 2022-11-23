using System.Security.Policy;

namespace BookMarket.ViewModels.Admin
{
    public class UserViewModel
    {
        public string UserId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public List<string> UserRoles { get; set; } = new();
        public UserViewModel(string userId, string userName, string userEmail)
        {
            UserName = userName;
            UserId = userId;
            UserEmail = userEmail;
        }

        public UserViewModel(): this("", "", "") { }

        public string AllRoles => String.Join("\r\n", UserRoles);
    }
}
