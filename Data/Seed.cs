using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialApp.Entities;

namespace SocialApp.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if(await context.Users.AnyAsync()) return;
            var usersData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");
            var users = JsonSerializer.Deserialize<List<AppUser>>(usersData);

            if(users == null) return;
            foreach (var user in users)
            {
                var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                await context.Users.AddAsync(user);
            }
            await context.SaveChangesAsync();

        }

    }
}
