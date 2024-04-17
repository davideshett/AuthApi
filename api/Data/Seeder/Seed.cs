using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;


namespace api.Data.Seeder
{
    public class Seed
    {

        public static async Task SeedRoles(RoleManager<AppRole> roleManager)
        {

            if (await roleManager.Roles.AnyAsync()) return;

            var roleData = await System.IO.File.ReadAllTextAsync("Data/Seeder/RoleTableSeeder.json");
            var roles = JsonConvert.DeserializeObject<List<AppRole>>(roleData);
            if (roles == null) return;
            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
                Console.WriteLine(role.Name);
            }
        }

        public static async Task SeedUsers(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var userData = await System.IO.File.ReadAllTextAsync("Data/Seeder/UserTableSeeder.json");
            var users = JsonConvert.DeserializeObject<List<AppUser>>(userData);
            if (users == null) return;

            foreach (var user in users)
            {
                user.UserName = user.Email.ToLower();
                user.EmailConfirmed = true;
                var result = await userManager.CreateAsync(user,"Jamesharden13*");

                if (result.Succeeded)
                {
                   await userManager.AddToRoleAsync(user,"Admin");
                }
            }
        }
    }

}