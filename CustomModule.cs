﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawBotFYP_GUI_.Core.Commands
{
    /**
     * CustomModule
     * Base class that adds overloaded and custom functions to the ModuleBase.
     * Shared functions like reply and set playing.
     */
    public class CustomModule : ModuleBase
    {
        // Reply will allow the AudioService to reply in the correct text channel.
        public async Task ServiceReplyAsync(string s)
        {
            await ReplyAsync(s);
        }

        public async Task ServiceReplyAsync(string title, EmbedBuilder emb)
        {
            await ReplyAsync(title, false, emb);
        }

        // Playing will allow the AudioService to set the current game.
        public async Task ServicePlayingAsync(string s)
        {
            try
            {
                await (Context.Client as DiscordSocketClient).SetGameAsync(s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


    }
}
