using SocialApp.Entities;

namespace SocialApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);

    }
}
