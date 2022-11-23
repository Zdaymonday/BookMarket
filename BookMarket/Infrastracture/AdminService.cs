using BookMarket.IdentityServer.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace BookMarket.Infrastracture
{
    public class AdminService
    {
        private readonly IConfiguration config;

        public AdminService(IConfiguration config)
        {
            this.config = config;
        }

        public async Task InitAdminRoleAndAccount()
        {
            var ctx = new BookMarketIdentityContext();
            IdentityRole? role = await ctx.Roles.FirstOrDefaultAsync(r => r.Name == "admin");
            if(role == null)
            {
                role = new()
                {
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    Id = Guid.NewGuid().ToString(),
                };

                await ctx.Roles.AddAsync(role);
                await ctx.SaveChangesAsync();
            }

            var pass = config.GetSection("Admin_Password").Value;
            if (String.IsNullOrEmpty(pass)) throw new Exception("Пароль не прочитан из конфигурации");

            PasswordHasher<IdentityUser> hasher = new();

            var admin_acc = await ctx.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
            
            if(admin_acc == null)
            {            
                IdentityUser admin = new IdentityUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                };

                await ctx.Users.AddAsync(admin);
                await ctx.SaveChangesAsync();

                var hash = hasher.HashPassword(admin, pass);
                admin.PasswordHash = hash;

                ctx.Users.Update(admin);
                await ctx.SaveChangesAsync();

                ctx.UserRoles.Add(new IdentityUserRole<string>() { RoleId = role.Id, UserId = admin.Id });
                await ctx.SaveChangesAsync();
            }
        }
    }
}
