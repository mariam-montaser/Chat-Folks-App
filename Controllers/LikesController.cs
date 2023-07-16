using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Extensions;
using SocialApp.Helpers;
using SocialApp.Interfaces;

namespace SocialApp.Controllers
{
    [Authorize]
    public class LikesController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public LikesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams) {

            likesParams.UserId = User.GetUserId();
            var users = await _unitOfWork.LikesRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.PageSize, users.TotalCount, users.CurrentPage, users.TotalPages);

            return Ok(users);

        }


        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username) { 
            var sourceUserId = User.GetUserId();
            var likedUser = await _unitOfWork.UserRepository.GetUserByNameAsync(username);
            var sourceUser = await _unitOfWork.LikesRepository.GetUserWithLikes(sourceUserId);

            if (likedUser == null) return NotFound();

            if (username == sourceUser.UserName) return BadRequest("You can't like yourself.");

            var likeUser = await _unitOfWork.LikesRepository.GetUserLike(sourceUserId, likedUser.Id);

            if (likeUser != null) return BadRequest("You already liked this user");

            likeUser = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(likeUser);

            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Failed to like user");


        }



    }
}
