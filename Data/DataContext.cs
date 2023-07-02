using System.Data;
using Microsoft.EntityFrameworkCore;
using SocialApp.Entities;

namespace SocialApp.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {

        }

        public DbSet<AppUser> Users { get; set; }
    }
}
