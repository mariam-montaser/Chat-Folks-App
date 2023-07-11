using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Extensions;
using SocialApp.Interfaces;

namespace SocialApp.SignalR
{
    public class MessageHub: Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _presenceTracker;

        public MessageHub(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper, IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"];
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = AddConnectionToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();
            var recipientUsername = createMessageDto.RecipientUsername.ToLower();
            if (username == recipientUsername) throw new HubException("You can't message yourself.");
            var sender = await _userRepository.GetUserByNameAsync(username);
            var recipient = await _userRepository.GetUserByNameAsync(recipientUsername);

            if (recipient == null) throw new HubException("Not found User.");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = username,
                RecipientUsername = recipientUsername,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(username, recipientUsername);
            var group = await _messageRepository.GetMessagesGroup(groupName);
            if(group.Connections.Any(c => c.Username == recipientUsername))
            {
                message.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await _presenceTracker.GetConnectionsForUser(recipientUsername);
                if(connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", new { username = username, knownAs = sender.KnownAs });
                }
            }
            _messageRepository.AddMessage(message);
            if(await _messageRepository.SaveAllAsync())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            }
        }

        private async Task<Group> AddConnectionToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessagesGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if(group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }
            group.Connections.Add(connection);
            if (await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("Failed to join group.");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var connectionId = Context.ConnectionId;
            var group = await _messageRepository.GetGroupForConnection(connectionId);
            var connection = group.Connections.FirstOrDefault(c => c.ConnectionId == connectionId);
            _messageRepository.RemoveConnection(connection);
            if(await _messageRepository.SaveAllAsync()) return group;
            throw new HubException("Failed to remove from group.");
        }
        
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other);
            return stringCompare < 0 ? $"{caller}-{other}": $"{other}-{caller}";
        }

    }
}
