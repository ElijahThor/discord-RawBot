using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;
using RawBotFYP_GUI_.Core.Data;
using System.IO;
using System.Reflection;

//for moderations use.

namespace RawBotFYP_GUI_.Core.Commands
{

    #region Methods
    public class ModerationMethods : ModuleBase<SocketCommandContext>
    {
        public EmbedBuilder CreateEmbled(string Author, string Description)       // Methods to Create a Embled
        {
            EmbedBuilder Embedb = new EmbedBuilder();
            Embedb.WithAuthor(Author);
            Embedb.WithColor(40, 200, 150);
            Embedb.WithDescription(Description);
            Embedb.Build();
            return Embedb;
        }

        public bool UserisAdmin(SocketGuildUser user) // Search for Roles(delete if not in use)//
        {
            string[] targetrolename = { "Moderator" };
            var result = from r in user.Guild.Roles
                         where r.Name == targetrolename[0]
                         select r.Id;
            ulong roleID = result.FirstOrDefault();
            if (roleID == 0) return false;
            var targetrole = user.Guild.GetRole(roleID);
            return user.Roles.Contains(targetrole);
        }
    }
    #endregion

    #region KickUser
    public class KickUser : ModuleBase<SocketCommandContext>
    {
        ModerationMethods getMethods = new ModerationMethods();

        [RequireBotPermission(Discord.GuildPermission.KickMembers)]
        [RequireUserPermission(Discord.GuildPermission.KickMembers)]
        [Command("Kick")] //[!kick][name of the person that you want to kick]
        public async Task kicksomeone(IGuildUser user)
        {
            if (user.GuildPermissions.Administrator)
            {
                await ReplyAsync("Can't Kick Moderator");
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Boot Hammer has spoken", "**" + user.Username + "**" + " was kicked"));
                await Discord.UserExtensions.SendMessageAsync(user, $"Hey, sorry about this but... you got kicked from **{Context.Guild.Name}** by {Context.Message.Author}", false);
                await user.KickAsync();
            }
        }

        [RequireBotPermission(Discord.GuildPermission.KickMembers)]
        [RequireUserPermission(Discord.GuildPermission.KickMembers)]
        [Command("Kick")] //if kick command is incomplete
        public async Task incompletekick()
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Incomplete command", "Please have a Name after the !Kick command. Example: !Kick Name."));
        }

        [RequireBotPermission(Discord.GuildPermission.KickMembers)]
        [RequireUserPermission(Discord.GuildPermission.KickMembers)]
        [Command("kick")] // If kick command has wrong name
        public async Task wrongnamekick([Remainder] string empty)
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Incomplete command", $"{empty} can't be found.\nPlease have a valid user after the !kick command. Example: !kick name."));
        }

    }

    #endregion

    #region BanUser
    public class BanUser : ModuleBase<SocketCommandContext>
    {
        ModerationMethods getMethods = new ModerationMethods();

        [RequireBotPermission(Discord.GuildPermission.BanMembers)]
        [RequireUserPermission(Discord.GuildPermission.BanMembers)]
        [Command("Ban")] //for banning the user
        public async Task banUser(Discord.IGuildUser user)
        {
            if (user.GuildPermissions.Administrator)
            {
                await ReplyAsync("Can't Ban Moderator");
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Ban Hammer has spoken", "**" + user.Username + "**" + " was banned"));
                await Discord.UserExtensions.SendMessageAsync(user, $"Hey, sorry about this but... you got banned from **{Context.Guild.Name}** by {Context.Message.Author}", false);
                await user.Guild.AddBanAsync(user, 1);
            }
        }

        [RequireBotPermission(Discord.GuildPermission.BanMembers)]
        [RequireUserPermission(Discord.GuildPermission.BanMembers)]
        [Command("Ban")] //if command is not complete
        public async Task inccommandBan()
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Incomplete Command", "Please have a Name after the !ban command. Example: !ban Name"));
        }

        [RequireBotPermission(Discord.GuildPermission.BanMembers)]
        [RequireUserPermission(Discord.GuildPermission.BanMembers)]
        [Command("Ban")] // If ban command has wrong name
        public async Task wrongnameBan([Remainder] string empty)
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Incomplete Command", $"{empty} can't be found.\nPlease have a valid username after the !ban command. Example: !ban name."));
        }
    }
    #endregion

    #region UnbanUser
    public class UnBanUser : ModuleBase<SocketCommandContext>
    {
        ModerationMethods getMethods = new ModerationMethods();

        [Command("UnBan")]
        [RequireContext(ContextType.Guild)]
        [RequireBotPermission(Discord.GuildPermission.BanMembers)]
        [RequireUserPermission(Discord.GuildPermission.BanMembers)]
        public async Task UnbanInternal(string username)
        {
            var ban = (await Context.Guild.GetBansAsync()).FirstOrDefault(b => b.User.Username == username);
            if (ban != null)
            {
                await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("The Ban Hammer Have Spoken", " was unbanned."));
                await Context.Guild.RemoveBanAsync(ban.User);
            }
            else
            {
                await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Invalid Command", $"{username} is not banned.\nPlease have a banned username after the !unban command. Example: !ban name ."));
            }
        }
        [RequireBotPermission(Discord.GuildPermission.BanMembers)]
        [RequireUserPermission(Discord.GuildPermission.BanMembers)]
        [Command("UnBan")]
        public async Task ifnonameUnBan()
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Incomplete Command", "Please have a Name after the !unban command. Example: !unban Name."));
        }
    }
    #endregion

    #region BanList
    public class BanList : ModuleBase<SocketCommandContext>
    {
        [Command("Banlist")]//to show everyone who is banned
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Banned()
        {
            var bans = await Context.Guild.GetBansAsync();
            var usersBanned = bans.Select(ban => ban.User);
            var temp = 1;
            var answer = new EmbedBuilder();
            answer.WithColor(40, 200, 150);
            answer.WithTitle("*All Who Are Banned*");
            foreach (var usernames in usersBanned)
            {
                answer.Description += temp++ + ". " + usernames + "\n";
            }
            answer.WithFooter($"Requested by {this.Context.User.Username}");
            await Context.Channel.SendMessageAsync("", false, answer.Build());
        }
    }
    #endregion

    #region ShowUserInformation
    public class ShowUserInformation : ModuleBase<SocketCommandContext>
    {
        [Command("info")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserInfo(SocketGuildUser user)
        {
            var perms = user.GuildPermissions.ToList();
            var allPerms = string.Join(", ", perms);
            if (user != null)
            {
                var answer = new Discord.EmbedBuilder();
                answer.WithColor(40, 200, 150);
                answer.ThumbnailUrl = user.GetAvatarUrl(ImageFormat.Auto);
                answer.AddField("User Info for " + user, "UserID | " + user.Id);
                //answer.AddInlineField("User Nickname" + "", user.Nickname);
                answer.AddInlineField("Server Permissions ", allPerms);
                answer.AddInlineField("Joined Server at", user.JoinedAt);
                answer.AddInlineField("Joined Discord at ", user.CreatedAt.DateTime.ToLongDateString());
                answer.AddInlineField("User Status ", user.Status);
                await Context.Channel.SendMessageAsync("", false, answer.Build());
            }
        }

        [Command("info")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserInfoerror()
        {
            var errorms = new EmbedBuilder();
            errorms.WithTitle("Incomplete Command");
            errorms.WithDescription("Please Have a Valid name behind the info command!! example !info @user");
            await Context.Channel.SendMessageAsync("", false, errorms.Build());
        }

        [Command("info")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task UserInfoifnum(string parameter)
        {
            var errorms = new EmbedBuilder();
            errorms.WithTitle("Error with Command");
            errorms.WithDescription($"'{parameter}' is not found. Please Have a Valid name behind the info command!! example !info @user");
            await Context.Channel.SendMessageAsync("", false, errorms.Build());
        }
    }
    #endregion

    #region blacklist

    public class blacklist : ModuleBase<SocketCommandContext>
    {
        [Command("Blacklist")]
        public async Task blacklists([Remainder]string URL)
        {
            StoreBlackList list = new StoreBlackList();
            var relpath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location).Replace(@"bin\Debug", @"Core\Data\BlackList.txt");
            string[] blackcheck = File.ReadAllLines(relpath);

            if (blackcheck.Contains(URL.ToString()))
            {
                await ReplyAsync("Already exist");
            }
            if (URL != list.blackList.ToString())
            {
                using (var tw = new StreamWriter(relpath, true))
                {
                    tw.WriteLine(URL.ToString());
                }
                await Context.Message.DeleteAsync();
                await ReplyAsync("Added to the Blacklist");
            }
        }
    }
    #endregion

    #region pm
    public class Privatemessage : ModuleBase<SocketCommandContext>
    {
        private bool IsPrivateMessage(SocketMessage msg)
        {
            return (msg.Channel.GetType() == typeof(SocketDMChannel));
        }
        //public async Task Privatemessages([Remainder]string URL)
        //{
        //    if (ReplyAsync = IsPrivateMessage)
        //    {
               
        //    }
        //}
    }
            #endregion
        }
