using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialApp.Data;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Extensions;
using SocialApp.Helpers;
using SocialApp.Interfaces;

namespace SocialApp.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _unitOfWork.UserRepository.GetUserByNameAsync(User.GetUsername());
            userParams.CurrentUserName = user.UserName;
            if (string.IsNullOrEmpty(userParams.Gender)) 
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users.PageSize, users.TotalCount, users.CurrentPage, users.TotalPages);

            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUserByName(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username);
        }



        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto member)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _unitOfWork.UserRepository.GetUserByNameAsync(username);
           
            _mapper.Map(member, user);
            _unitOfWork.UserRepository.Update(user);

            if(await _unitOfWork.Complete()) return NoContent();

            return BadRequest("Failed to update user");
        }


    }
}
    