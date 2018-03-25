using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using NReco.ImageGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Discord.Bot.Core.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        [Command("warn")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task WarnUser(IGuildUser user)
        {
            //var userAccount = UserAccounts.GetAccount((SocketUser)user);
            //userAccount.NumberOfWarnings++;
            //UserAccounts.SaveAccounts();

            //if (userAccount.NumberOfWarnings >= 3)
            //{
            //    await user.Guild.AddBanAsync(user, 5);
            //}
            //else if (userAccount.NumberOfWarnings == 2)
            //{
            //    // perhaps kick
            //}
            //else if (userAccount.NumberOfWarnings == 1)
            //{
            //    // perhaps send warning message
            //}
        }

        [Command("kick")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task KickUser(IGuildUser user, string reason = "No reason provided.")
        {
            await user.KickAsync(reason);
        }

        [Command("ban")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanUser(IGuildUser user, string reason = "No reason provided.")
        {
            await user.Guild.AddBanAsync(user, 1, reason);
        }

        [Command("mute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task MuteUser(IGuildUser user)
        {
            await user.ModifyAsync(u => u.Mute = true);
        }

        [Command("unmute")]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        public async Task UnmuteUser(IGuildUser user)
        {
            await user.ModifyAsync(u => u.Mute = false);
        }

        [Command("move")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        [RequireBotPermission(GuildPermission.MoveMembers)]
        public async Task Move(IGuildUser user, [Remainder] string channelName = "General")
        {
            var channels = await user.Guild.GetVoiceChannelsAsync();

            var channel = channels.Where(c => c.Name == channelName).FirstOrDefault();
            if (channel == null)
            {
                await Context.Channel.SendMessageAsync(":x: Cannot find the channel:" + channelName);
            }
            else
            {
                await user.ModifyAsync(u => u.ChannelId = channel.Id);
            }
        }

        [Command("move -all")]
        [RequireUserPermission(GuildPermission.MoveMembers)]
        [RequireBotPermission(GuildPermission.MoveMembers)]
        public async Task MoveAll(string from, [Remainder] string to = "General")
        {
            var channels = Context.Guild.VoiceChannels;

            var toChannel = channels.Where(c => c.Name == to).FirstOrDefault();

            var fromChannel = channels.Where(c => c.Name == from).FirstOrDefault();

            if (toChannel == null)
            {
                await Context.Channel.SendMessageAsync(":x: Cannot find the channel:" + to);
            }
            else if (fromChannel == null)
            {
                await Context.Channel.SendMessageAsync(":x: Cannot find the channel:" + from);
            }
            else if (!fromChannel.Users.Any())
            {
                await Context.Channel.SendMessageAsync(":x: No users in channel:" + from);
            }
            else
            {
                foreach (var user in fromChannel.Users)
                {
                    await user.ModifyAsync(u => u.ChannelId = toChannel.Id);
                }
            }
        }

        [Command("react")]
        public async Task HandleReactionMessage()
        {
            RestUserMessage msg = await Context.Channel.SendMessageAsync("React to me!");
            Global.MessageIdToTrack = msg.Id;
        }

        [Command("hello")]
        public async Task Hello(string color = "red")
        {
            string css = "<style>\n    h1{\n        color: " + color + ";\n    }\n</style>\n";
            string html = String.Format("<h1>Hello {0}!</h1>", Context.User.Username);
            var converter = new HtmlToImageConverter
            {
                Width = 250,
                Height = 70
            };
            var jpgBytes = converter.GenerateImage(css + html, NReco.ImageGenerator.ImageFormat.Jpeg);
            await Context.Channel.SendFileAsync(new MemoryStream(jpgBytes), "hello.jpg");
        }

        [Command("echo")]
        public async Task Echo([Remainder]string message)
        {
            var embed = new EmbedBuilder();
            embed.WithTitle("Echoed message by " + Context.User.Username);
            embed.WithDescription(message);
            embed.WithColor(new Color(0, 255, 0));

            await Context.Channel.SendMessageAsync("", false, embed);
        }

        [Command("pick")]
        public async Task PickOne([Remainder]string message)
        {
            string[] options = message.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Random r = new Random();
            string selection = options[r.Next(0, options.Length)];

            var embed = new EmbedBuilder();
            embed.WithTitle("Choice for " + Context.User.Username);
            embed.WithDescription(selection);
            embed.WithColor(new Color(255, 255, 0));
            embed.WithThumbnailUrl("https://orig00.deviantart.net/3033/f/2016/103/0/c/mercy_by_raichiyo33-d9yufl4.jpg");

            await Context.Channel.SendMessageAsync("", false, embed);
        }


        //[RequireUserPermission(GuildPermission.Administrator)] //RequireBotPermissionAttribute - 
        [Command("secret")]
        public async Task RevealSecret([Remainder]string arg = "")
        {
            if (!UserIsSecretOwner((SocketGuildUser)Context.User))
            {
                await Context.Channel.SendMessageAsync(":x: You need the SecretOwner role to do that. " + Context.User.Mention);
                return;
            }
            var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
            await dmChannel.SendMessageAsync(Utilities.GetAlert("SECRET"));
        }

        private bool UserIsSecretOwner(SocketGuildUser user)
        {
            string targetRoleName = "SecretOwner";
            var result = user.Guild.Roles.Where(r => r.Name == targetRoleName).Select(r => r.Id);

            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetRole = user.Guild.GetRole(roleID);
            //user.ModifyAsync(x => x.Channel = )
            return user.Roles.Contains(targetRole);
        }
    }
}
