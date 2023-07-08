using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserToReturnDto>> Register(RegisterDTO userData)
        {
            if (await isUsernsameExist(userData.Username)) return BadRequest("Username is Taken.");

            var user = _mapper.Map<AppUser>(userData);

            user.UserName = user.UserName.ToLower();
           
            var result = await _userManager.CreateAsync(user, userData.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if(!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserToReturnDto
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserToReturnDto>> Login(LoginDTO userData)
        {
            var user = await _userManager.Users.Include(user => user.Photos).SingleOrDefaultAsync(user => user.UserName == userData.Username.ToLower());
            if (user == null) return Unauthorized("Invalid Credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, userData.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid Credentials.");

            return new UserToReturnDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Token = await _tokenService.CreateToken(user),
                Gender = user.Gender
            }; 
        }

        private async Task<bool> isUsernsameExist(string username)
        {
            return await _userManager.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

       
    }
}
