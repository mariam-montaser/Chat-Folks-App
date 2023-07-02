using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApp.Data;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Interfaces;

namespace SocialApp.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserToReturnDto>> Register(RegisterDTO userData)
        {
            if (await isUsernsameExist(userData.Username)) return BadRequest("Username is Taken.");

            var user = _mapper.Map<AppUser>(userData);

            using var hmac = new HMACSHA512();


            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userData.Password));
            user.PasswordSalt = hmac.Key;
           
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserToReturnDto
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToReturnDto>> Login(LoginDTO userData)
        {
            var user = await _context.Users.SingleOrDefaultAsync(user => user.UserName == userData.Username);
            if (user == null) return Unauthorized("Invalid Credentials");

            if (!IsMatchedGivenPassword(user, userData.Password)) return Unauthorized("Invalid Credentials.");

            return new UserToReturnDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = _tokenService.CreateToken(user),
                Gender = user.Gender
            }; 
        }

        private async Task<bool> isUsernsameExist(string username)
        {
            return await _context.Users.AnyAsync(user => user.UserName == username);
        }

        private bool IsMatchedGivenPassword(AppUser user, string givenPass)
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var Hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(givenPass));

            for (int i = 0; i < Hash.Length; i++)
            {
                if (user.PasswordHash[i] != Hash[i]) return false;
            }

            return true;
        }
    }
}
