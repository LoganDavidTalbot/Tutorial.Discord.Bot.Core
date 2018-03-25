using Discord.WebSocket;
using Discord;
using System;
using System.Threading.Tasks;
using Tutorial.Discord.Bot.Core.Core;

namespace Tutorial.Discord.Bot.Core
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;
        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();
        public async Task StartAsync()
        {
            string name = "Tom";
            string bot = "Test Server";
            string message = Utilities.GetFormattedAlert("WELCOME_$NAME_$BOTNAME", name, bot);
            Console.WriteLine(message);
            if (Config.bot.token == "" || Config.bot.token == null) return;
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });

            _client.Log += Log;
            _client.Ready += RepeatingTimer.StartTimer;
            _client.ReactionAdded += OnReactionAdded;
            await _client.LoginAsync(TokenType.Bot, Config.bot.token);
            await _client.StartAsync();
            Global.Client = _client;
            _handler = new CommandHandler();
            await _handler.InitializeAsyc(_client);
            await Task.Delay(-1);

        }

        private async Task OnReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            if (reaction.MessageId == Global.MessageIdToTrack)
            {
                if (reaction.Emote.Name == "👌")
                {
                    await channel.SendMessageAsync(reaction.User.Value.Username + " says OK.");
                }
            }
        }

        private async Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);

            //return Task.CompletedTask;
        }
    }
}
