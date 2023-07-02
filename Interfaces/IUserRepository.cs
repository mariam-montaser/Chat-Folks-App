using System.Collections.Generic;
using System.Threading.Tasks;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Helpers;

namespace SocialApp.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetAllUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string username);

        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userparams);
        Task<MemberDto> GetMemberAsync(string username);
    }
}
