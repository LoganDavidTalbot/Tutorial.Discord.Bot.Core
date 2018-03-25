using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tutorial.Discord.Bot.Core
{
    internal static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
        internal static ulong MessageIdToTrack { get; set; }
    }
}
