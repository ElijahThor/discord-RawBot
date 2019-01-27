using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RawBotFYP_GUI_.Core.Commands
{
    #region HelpCommand
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {

        [Command("help")]
        public async Task AllCommands() // Commands Help
        {
            await ReplyAsync("Commands has been sent to your private message ;)");
            var user = Context.Message.Author;
            RequestOptions options = new RequestOptions();
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("RawBots' Commands");
            Embed.WithColor(40, 200, 150);
            Embed.WithDescription("-----ENTERTAINMENT-----\n!Roll\n!Jokes\n!Pickuser\n!Define\n!Youtube\n-----MODERATION-----\n!Kick\n!Ban\n!Unban\n!Banlist\n!Info");
            Embed.WithFooter("RawBot© 2018");
            await Discord.UserExtensions.SendMessageAsync(user, string.Empty, false, Embed);

        }
    }
    #endregion
}
