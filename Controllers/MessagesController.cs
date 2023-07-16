using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Extensions;
using SocialApp.Helpers;
using SocialApp.Interfaces;

namespace SocialApp.Controllers
{

    public class MessagesController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            var recipientUsername = createMessageDto.RecipientUsername;

            if (username == recipientUsername) return BadRequest("You can't message yourself");

            var sender = await _unitOfWork.UserRepository.GetUserByNameAsync(username);
            var recipient = await _unitOfWork.UserRepository.GetUserByNameAsync(recipientUsername);

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                RecipientUsername = recipientUsername,
                Content = createMessageDto.Content
            };

            _unitOfWork.MessageRepository.AddMessage(message);

            if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send the message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.PageSize, messages.TotalCount, messages.CurrentPage, messages.TotalPages);
            return Ok(messages);
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _unitOfWork.MessageRepository.GetMessageThread(currentUsername, username));

        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<MessageDto>> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            var message = await _unitOfWork.MessageRepository.GetMessage(id);
            var senderUsername = message.Sender.UserName;
            var recipientUsername = message.Recipient.UserName;

            if (username != senderUsername && username  != recipientUsername) return Unauthorized();

            if (username == senderUsername) message.SenderDeleted = true;
            if (username == recipientUsername) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted) _unitOfWork.MessageRepository.DeleteMessage(message);

            if (await _unitOfWork.Complete()) return Ok();
            return BadRequest("Problem to delete message.");
        }

    }
}
