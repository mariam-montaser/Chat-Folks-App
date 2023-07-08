using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using SocialApp.Extensions;

namespace SocialApp.Entities
{
    public class AppUser: IdentityUser<int>
    {

        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;

        public string Country { get; set; }
        public string City { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
        public string LookingFor { get; set; }
        public string Introduction { get; set; }
        public string Interests { get; set; }

        public ICollection<Photo> Photos { get; set; }
        public ICollection<UserLike> LikedUsers { get; set; }
        public ICollection<UserLike> LikedByUsers { get; set; }

        public ICollection<Message> SentMessages { get; set; }
        public ICollection<Message> ReceviedMessages { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; }


        //public int GetAge()
        //{
        //    return DateOfBirth.CalculateAge();
        //}
    }
}
