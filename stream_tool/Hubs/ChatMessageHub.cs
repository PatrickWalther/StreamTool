using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StreamTool.Dal;
using StreamTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StreamTool.Hubs
{
    public class ChatMessageHub : Hub<IChatMessageHub>
    {
        private readonly ChatContext _context;

        public ChatMessageHub(ChatContext context)
        {
            _context = context;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();

            var chatMessages = await _context.ChatMessages.ToListAsync();

            await Clients.Caller.AllMessages(chatMessages);
        }

        public async Task DeleteMessage(int? id)
        {
            if (id == null)
                return;

            var message = await _context.ChatMessages.FindAsync(id);

            if (message == null)
                return;

            _context.ChatMessages.Remove(message);
            await _context.SaveChangesAsync();

            await Clients.All.DeleteMessage(id);
        }

        public async Task DeleteMessages()
        {
            var messageIds = await _context.ChatMessages.Select(chatMessage => chatMessage.MsgId).ToListAsync();
            var messages = await _context.ChatMessages.ToListAsync();

            _context.ChatMessages.RemoveRange(messages);
            await _context.SaveChangesAsync();

            await Clients.All.DeleteMessages(messageIds);
        }
    }
}
