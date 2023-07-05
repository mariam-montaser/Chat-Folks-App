using System.Threading.Tasks;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Helpers;

namespace SocialApp.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceId, int likedId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
