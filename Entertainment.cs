using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.WebSocket;
using Discord.Commands;

using Google.Apis.YouTube.v3;

using RawBotFYP_GUI_.Core.Data;
using Newtonsoft.Json.Linq;
using RawBotFYP.Core.Services;
using Google.Apis.Services;
using System.Diagnostics;
using Discord.Audio;
using RawBotFYP_GUI_.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Discord.Rest;
using System.Collections.Concurrent;

//entertainment commands for fun.

namespace RawBotFYP_GUI_.Core.Commands 
{
    #region Methods
    public class EntertainMethods
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
    }
    #endregion

    #region Jokes
    public class Jokes : ModuleBase<SocketCommandContext>
    {
        StoreJokes CallJoke = new StoreJokes();

        [Command("jokes")]
        public async Task sayJoke() // Execute jokes
        {
            Random randjoke = new Random();
            int randomgen = randjoke.Next(1, CallJoke.JokeRNG.Count);
            string CallrandJoke = CallJoke.JokeRNG[randomgen];
            await ReplyAsync($"**Jokes Number :** {randomgen} \n{CallrandJoke}", true);
        }
    }
    #endregion

    #region Ping
    public class Ping : ModuleBase<SocketCommandContext>
    {
        [Command("Ping")] // Ping , Pong.
        public async Task PingPong()
        {
            await Context.Channel.SendMessageAsync($"Pong :ping_pong: {Context.Client.Latency}");
        }
    }
    #endregion

    #region RollDice
    public class RollDice : ModuleBase<SocketCommandContext>
    {
        ModerationMethods getMethods = new ModerationMethods();

        [Command("Roll")] // Roll Dice with number
        public async Task sayRollNum([Remainder] int rNum)
        {
            Random RNG = new Random();
            int random = RNG.Next(1, (rNum + 1));
            await ReplyAsync($":game_die: Number Rolled: {random}");
        }
        [Command("roll")] // If Dice command has string parameter
        public async Task ifRollstring([Remainder] string rNum)
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Invalid Command", $"{rNum} is not a Integer.\nPlease have an Integer after the !roll command. Example: !roll 30."));
        }
        [Command("roll")] // if Dice command has no paramater
        public async Task ifnorollnum()
        {
            await Context.Channel.SendMessageAsync("", false, getMethods.CreateEmbled("Invalid Command", "Please have an Integer after the !roll command. Example: !roll 30."));
        }
    }
    #endregion

    #region Pickuser/LuckyDraw
    public class PickUser : ModuleBase<SocketCommandContext>
    {
        [RequireBotPermission(GuildPermission.Administrator)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("PickUser")]
        public async Task userpick()
        {
            Random RNGUser = new Random();
            List<string> UserArrayFiltered = new List<string>();

            foreach (SocketGuildUser User in Context.Guild.Users)
            {
                if (!User.IsBot)
                {
                    if (User.Status.Equals(UserStatus.Online) && !User.Equals(Context.Message.Author))
                    {
                        UserArrayFiltered.Add(User.Username);
                    }
                }
            }

            int numofusers = RNGUser.Next(0, UserArrayFiltered.Count);

            await Context.Channel.SendMessageAsync("Congrats to " + UserArrayFiltered[numofusers] + " for winning the Giveaway!!");
        }
        [Command("PickUser")]
        public async Task UserRequire()
        {
            await ReplyAsync("You do not have the required permissions to run the bot!");
        }
    }
    #endregion

    #region Define
    public class defining : ModuleBase<SocketCommandContext>
    {
        [Command("define")]
        public async Task Dictionary([Remainder] string word = null)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                //if word is empty/missing
                var embed = new EmbedBuilder();
                embed.Title = "Defining " + word;
                embed.WithColor(52, 152, 219);
                embed.Description = ":warning: A word or phrase is required!";
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
            else
            {
                var data = await Commonservice.DefinitionService.GetDefinitionForTermAsync(word);
                if (data.Results.Count == 0)
                {
                    //if word can't be found
                    var embed = new EmbedBuilder();
                    embed.Title = "can't find the explantion for " + word;
                    embed.WithColor(52, 152, 219);
                    embed.Description = "No results found!";
                    await Context.Channel.SendMessageAsync("", false, embed.Build());
                }
                else
                    for (var index = 0; index < data.Results.Count; index++)
                        foreach (var value in data.Results[index].Senses)
                            if (value.Definition != null)
                            {
                                var definition = value.Definition.ToString();
                                if (!(value.Definition is string))
                                    definition = ((JArray)JToken.Parse(value.Definition.ToString())).First.ToString();
                                var output = new EmbedBuilder()
                                    .WithTitle($"Dictionary definition for **{word}**")
                                    .WithDescription(definition.Length < 500 ? definition : definition.Take(500) + "...")
                                    .WithColor(52, 152, 219);
                                if (value.Examples != null)
                                    output.AddField("Example", value.Examples.First().text);
                                await Context.Channel.SendMessageAsync("", false, output.Build());
                                index = data.Results.Count;
                                break;
                            }
            }
        }
    }
    #endregion

    #region youtubeSearch
    public class youtubeSearch : ModuleBase<SocketCommandContext>
    {
        [Command("Youtube")]
        public async Task Youtube([Remainder] string Ysearching)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyCiDVoN9suAf6QR97UC6Ca_88RBwVi_yAI",
                ApplicationName = GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = Ysearching;
            searchListRequest.MaxResults = 1;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync();

            List<string> videos = new List<string>();
            List<string> channels = new List<string>();

            // Add each result to the appropriate list, and then display the lists of
            // matching videos and channels.
            foreach (var searchResult in searchListResponse.Items)
            {
                switch (searchResult.Id.Kind)
                {
                    case "youtube#video":
                        videos.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.VideoId));
                        await Context.Channel.SendMessageAsync("https://youtube.com/watch?v=" + searchResult.Id.VideoId);
                        break;

                    case "youtube#channel":
                        channels.Add(String.Format("{0} ({1})", searchResult.Snippet.Title, searchResult.Id.ChannelId));
                        await Context.Channel.SendMessageAsync("https://www.youtube.com/channel/" + searchResult.Id.ChannelId);
                        break;
                }
            }
            Console.WriteLine(String.Format("Videos:\n{0}\n", string.Join("\n", videos)));
            Console.WriteLine(String.Format("Channels:\n{0}\n", string.Join("\n", channels)));
        }
    }
    #endregion

    #region poll
    public class poll : ModuleBase<SocketCommandContext>
    {
        [Command("Poll")]
        public async Task Polling([Remainder]string text = null)
        {
            var embed = new EmbedBuilder();
            embed.WithColor(10, 10, 10);
            embed.WithTitle("Poll Created By " + Context.User);
            embed.WithDescription($"*{text}*");
            embed.WithFooter("React to vote.");
            RestUserMessage notice = await Context.Channel.SendMessageAsync("", false, embed.Build());
            await notice.AddReactionAsync(new Emoji("✅"));
            await notice.AddReactionAsync(new Emoji("❌"));
        }
    }
    #endregion

    #region calculator
    public class cal : ModuleBase<SocketCommandContext>
    {
        [Command("Math")]
        public async Task Math(double num1, string symbol, double num2)
        {

            double result;

            switch (symbol)
            {
                case "+":
                    result = num1 + num2;
                    break;

                case "-":
                    result = num1 - num2;
                    break;

                case "*":
                    result = num1 * num2;
                    break;

                case "/":
                    result = num1 / num2;
                    break;

                case "%":
                    result = num1 % num2;
                    break;

                default:
                    result = num1 + num2;
                    break;
            }
            var answer = new EmbedBuilder();
            answer.WithTitle($":white_check_mark: The result is {result:#,##0.00}");
            answer.WithColor(40, 200, 150);
            await Context.Channel.SendMessageAsync("", false, answer.Build());
        }
        [Command("Math")]
        public async Task invalid(string word = null, string symbol = null, string word2 = null)
        {
            await Context.Channel.SendMessageAsync("Invalid! Please follow this format : number + number :slight_smile:");
        }
    }
    #endregion

    #region catfact
    public class catfacts : ModuleBase<SocketCommandContext>
    {
        [Command("Catfact")]
        public async Task Catfact()
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetStringAsync($"https://catfact.ninja/fact").ConfigureAwait(false);
                if (response == null)
                    return;

                var fact = JObject.Parse(response)["fact"].ToString();
                await Context.Channel.SendMessageAsync("🐈" + ("catfact! ") + fact).ConfigureAwait(false);
            }
        }
    }
    #endregion

    #region dogfact
    public class dogfacts : ModuleBase<SocketCommandContext>
    {
        [Command("Dogfact")]
        public async Task dogfact()
        {
            using (var http = new HttpClient())
            {
                var response = await http.GetStringAsync($"http://dog-api.kinduff.com/api/facts").ConfigureAwait(false);
                if (response == null)
                    return;

                var fact = JObject.Parse(response)["facts"].ToString();
                await Context.Channel.SendMessageAsync("🐕 Dogfact!" + fact).ConfigureAwait(false);
            }
        }
    }
    #endregion

    #region catPic
    public class Catpic : ModuleBase<SocketCommandContext>
    {
        [Command("Catpic")]
        public async Task CatPic()
        {

            using (HttpClient client = new HttpClient())
            {
                var Picresponse = new EmbedBuilder();
                string url = null;
                string data = await client.GetStringAsync("https://aws.random.cat/meow");
                var dData = JObject.Parse(data);
                url = dData["file"].ToString();
                Picresponse.WithImageUrl(url);
                await Context.Channel.SendMessageAsync("", false, Picresponse.Build());
            }
        }
    }
    #endregion

    #region dogPic
    public class Docpic : ModuleBase<SocketCommandContext>
    {
        [Command("Dogpic")]
        public async Task CatPic()
        {

            using (HttpClient client = new HttpClient())
            {
                var Picresponse = new EmbedBuilder();
                string url = null;
                string data = await client.GetStringAsync("https://random.dog/woof.json");
                var dData = JObject.Parse(data);
                url = dData["url"].ToString();
                Picresponse.WithImageUrl(url);
                await Context.Channel.SendMessageAsync("", false, Picresponse.Build());
            }
        }
    }
    #endregion

    #region BirdPic
    public class Birdpic : ModuleBase<SocketCommandContext>
    {
        [Command("Birdpic")]
        public async Task BirdPic()
        {

            using (HttpClient client = new HttpClient())
            {
                var Picresponse = new EmbedBuilder();
                string url = null;
                string data = await client.GetStringAsync("https://birdsare.cool/bird.json");
                var dData = JObject.Parse(data);
                url = dData["url"].ToString();
                Picresponse.WithImageUrl(url);
                await Context.Channel.SendMessageAsync("", false, Picresponse.Build());
            }
        }
    }
    #endregion

    #region music
    //public class joining : ModuleBase<SocketCommandContext>
    //{
    //    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();
        
    //    //public AudioService _service;

    //    //Remember to add an instance of the AudioService
    //    // to your IServiceCollection when you initialize your bot
    //    //public joining(AudioService service)
    //    //{
    //    //    _service = service;
    //    //}

    //    [Command("Create", RunMode = RunMode.Async)]
    //public async Task CreateVoiceChannel([Remainder]string name = "General")
    //{
    //    await Context.Guild.CreateVoiceChannelAsync(name);
    //}

    //[Command("Join", RunMode = RunMode.Async)]
    //public async Task JoinChannel(IVoiceChannel channel = null)
    //{
    //    // Get the audio channel
    //    channel = channel ?? (Context.Message.Author as IGuildUser)?.VoiceChannel;
    //    if (channel == null)
    //    {
    //        await Context.Channel.SendMessageAsync("User must be in a voice channel.");
    //        return;
    //    }
    //    // For the next step with transmitting audio, you would want to pass this Audio Client in to a service.
    //    var audioClient = await channel.ConnectAsync();
    //}
    //       // [Command("play", RunMode = RunMode.Async)]
    //       //public async Task PlayCmd([Remainder] string song)
    //       //{
    //       //   await (Context.Guild, Context.Channel, song);
    //       //}
    //    private Process CreateStream(string path)
    //{
    //    try
    //    {
    //        return Process.Start(new ProcessStartInfo
    //        {
    //            FileName = "ffmpeg",
    //            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
    //            UseShellExecute = false,
    //            RedirectStandardOutput = true,
    //            CreateNoWindow = true
    //        });
    //    }
    //    catch
    //    {
    //        Console.WriteLine($"Error while opening local stream : {path}");
    //        return null;
    //    }
    //}
    //private async Task SendAsync(IAudioClient client, string path)
    //{
    //    // Create FFmpeg using the previous example
    //    using (var ffmpeg = CreateStream(path))
    //    using (var output = ffmpeg.StandardOutput.BaseStream)
    //    using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
    //    {
    //        try { await output.CopyToAsync(discord); }
    //        finally { await discord.FlushAsync(); }
    //    }
    //}
}


public class AudioModule : ModuleBase<ICommandContext>
{
    // Scroll down further for the AudioService.
    // Like, way down
    private readonly AudioService _service;

    // Remember to add an instance of the AudioService
    // to your IServiceCollection when you initialize your bot
    public AudioModule(AudioService service)
    {
        _service = service;
    }

    // You *MUST* mark these commands with 'RunMode.Async'
    // otherwise the bot will not respond until the Task times out.
    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinCmd()
    {
        await _service.JoinAudio(Context.Guild, (Context.User as IVoiceChannel));
    }

    //Remember to add preconditions to your commands,
    // this is merely the minimal amount necessary.
    // Adding more commands of your own is also encouraged.


        [Command("play", RunMode = RunMode.Async)]
        public async Task PlayCmd([Remainder] string song)
    {
        await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
    }

    #endregion
}
