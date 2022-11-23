using Microsoft.AspNetCore.Identity;
using System.Data;

namespace BookMarket.Infrastracture
{
    public class UserRolesInitializerService
    {
        private readonly IConfiguration config;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserRolesInitializerService(
            IConfiguration config,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            this.config = config;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitBaseRolesAndAdminAccount()
        {
            var roles_in_config = config.GetSection("Roles").Get<IEnumerable<string>>();
            
            foreach(var role_name in roles_in_config)
            {
                var identity_role = await roleManager.FindByNameAsync(role_name);
                if(identity_role is null)
                {
                    await roleManager.CreateAsync(new IdentityRole(role_name));
                }
            }

            var admin_account = await userManager.FindByNameAsync("Admin");

            if(admin_account is null)
            {                
                string admin_password = config.GetSection("Admin_Password").Get<string>();
                var user_admin = new IdentityUser("Admin");
                await userManager.CreateAsync(user_admin, admin_password);

                await userManager.AddToRoleAsync(user_admin, "admin");
            }
        }
    }
}
