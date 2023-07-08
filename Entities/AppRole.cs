using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SocialApp.Entities
{
    public class AppRole: IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
