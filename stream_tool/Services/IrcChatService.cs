using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamTool.Dal;
using StreamTool.Hubs;
using StreamTool.Models;
using System;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace StreamTool.Services
{
    public class IrcChatService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ChatMessageHub, IChatMessageHub> _hubContext;

        private IServiceScope _scope;
        private ChatContext _context;

        private TwitchClient _client;
        private ConnectionCredentials _credentials;

        private static string _botName = "justinfan1337";
        private static string _channelName = "wubbl0rz";
        private static string _oAuthToken = "";
        private static string _keyWord = "@wubbl0rz";

        public IrcChatService(ILogger<IrcChatService> logger, IServiceScopeFactory scopeFactory, IHubContext<ChatMessageHub, IChatMessageHub> hubContext)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;

            _scope = scopeFactory.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<ChatContext>();

            _credentials = new ConnectionCredentials(_botName, _oAuthToken);

            _logger.LogDebug("Creating new twitch client instance. Bot name: {}", _botName);
            _client = new TwitchClient();

            _logger.LogDebug("Setting credentials for channel {}.", _channelName);
            _client.Initialize(_credentials, _channelName);

            _logger.LogDebug("Registered event handler for \"OnMessageReceived\" event.");
            _client.OnMessageReceived += ClientOnMessageReceived;

        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Chat update service is starting.");

            _client.Connect();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Chat update service is stopping.");

            _client.Disconnect();

            return Task.CompletedTask;
        }

        private async void ClientOnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains(_keyWord))
            {
                var msg = new Models.ChatMessage() { Sender = e.ChatMessage.Username, Message = e.ChatMessage.Message};
                await _context.ChatMessages.AddAsync(msg);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.AddMessage(msg);
            }
        }

        public void Dispose()
        {
            _scope?.Dispose();
            _context?.Dispose();
            _client?.Disconnect();
        }
    }
}
