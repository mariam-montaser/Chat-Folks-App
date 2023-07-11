﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SocialApp.DTOs;
using SocialApp.Entities;
using SocialApp.Helpers;

namespace SocialApp.Interfaces
{
    public interface IMessageRepository
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessagesGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        void AddMessage(Message message);
        void DeleteMessage(Message message);

        Task<Message> GetMessage(int id);

        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);

        Task<IEnumerable<MessageDto>> GetMessagesThread(string currentUsername, string recipientUsername);

        Task<bool> SaveAllAsync();
    }
}
