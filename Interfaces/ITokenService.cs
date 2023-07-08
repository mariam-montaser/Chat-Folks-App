using System.Threading.Tasks;
using SocialApp.Entities;

namespace SocialApp.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);

    }
}
