using StreamTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamTool.Hubs
{
    public interface IChatMessageHub
    {
        Task AllMessages(List<ChatMessage> messages);
        Task AddMessage(ChatMessage message);
        Task DeleteMessage(int? id);
        Task DeleteMessages(List<int> ids);
    }
}
