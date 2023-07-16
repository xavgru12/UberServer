using Discord;
using Discord.Commands;
using Discord.WebSocket;
using log4net;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Manager;

namespace UberStrok.WebServices.AspNetCore.Core.Discord
{
    public class CoreDiscord
    {
        private DiscordSocketClient client;

        private ulong lobbychannel = 0uL;

        private ulong userloginchannel = 0uL;

        private ulong publicloginchannel = 0uL;

        private readonly List<ulong> allowedChannels = new List<ulong>();

        private string token = null;

        private ulong AltDentifierChannel;

        private ulong LeaderboardChannel;

        private ulong CommandChannel;

        private string prefix;

        private UDPListener UDPListener;

        private readonly ILog Log = LogManager.GetLogger(typeof(CoreDiscord));
        private readonly UberBeatManager uberBeatManager;
        public CoreDiscord(UberBeatManager uberBeatManager)
        {
            this.uberBeatManager = uberBeatManager;
        }

        public Task RunAsync()
        {
            UDPListener = new UDPListener(this);
            return MainAsync();
        }

        public async Task MainAsync()
        {
            try
            {
                GetConfig();
                bool exit = false;
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine(Environment.NewLine + "Discord bot token missing/invalid. Skipping....." + Environment.NewLine);
                    exit = true;
                }
                if (!exit && (lobbychannel == 0L || userloginchannel == 0L || publicloginchannel == 0L))
                {
                    Console.WriteLine(Environment.NewLine + "Invalid Discord Channel ID. Couldnt parse data to ulong. Skipping....." + Environment.NewLine);
                    exit = true;
                }
                if (!exit)
                {
                    Console.WriteLine(token);
                    var config = new DiscordSocketConfig
                    {
                        GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
                    };
                    client = new DiscordSocketClient(config);
                    await client.LoginAsync(TokenType.Bot, token);
                    await client.StartAsync();
                    client.MessageReceived += MessageReceived;
                    Console.WriteLine(Environment.NewLine + "Discord bot initialised." + Environment.NewLine);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void GetConfig()
        {
            try
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "assets", "configs", "discord.config");
                if (File.Exists(path))
                {
                    string[] discord = File.ReadAllLines(path);
                    string[] array = discord;
                    foreach (string line in array)
                    {
                        string data = line.Trim();
                        if (data.StartsWith("lobbychannel:"))
                        {
                            lobbychannel = ulong.Parse(data.Replace("lobbychannel:", ""));
                        }
                        else if (data.StartsWith("userloginchannel:"))
                        {
                            userloginchannel = ulong.Parse(data.Replace("userloginchannel:", ""));
                        }
                        else if (data.StartsWith("publicloginchannel:"))
                        {
                            publicloginchannel = ulong.Parse(data.Replace("publicloginchannel:", ""));
                        }
                        else if (data.StartsWith("token:"))
                        {
                            token = data.Replace("token:", "");
                        }
                        else if (data.StartsWith("prefix:"))
                        {
                            prefix = data.Replace("prefix:", "");
                        }
                        else if (data.StartsWith("AltDentifier:"))
                        {
                            AltDentifierChannel = ulong.Parse(data.Replace("AltDentifier:", ""));
                            allowedChannels.Add(AltDentifierChannel);
                        }
                        else if (data.StartsWith("Leaderboard:"))
                        {
                            LeaderboardChannel = ulong.Parse(data.Replace("Leaderboard:", ""));
                            allowedChannels.Add(LeaderboardChannel);
                        }
                        else if (data.StartsWith("CommandChannel:"))
                        {
                            CommandChannel = ulong.Parse(data.Replace("CommandChannel:", ""));
                            allowedChannels.Add(CommandChannel);
                        }
                        /*else if (data.Contains(":") && ulong.TryParse(data.Substring(data.IndexOf(":") + 1), out id))
						{
							allowedChannels.Add(id);
						}*/
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString() + "\n\nSkipping discord bot.");
            }
        }

        private async Task MessageReceived(SocketMessage message)
        {
            Log.Info(message.Author.Username + ":" +message.CleanContent);
            if (!allowedChannels.Contains(message.Channel.Id))
            {
                return;
            }
            if (message.Channel.Id == AltDentifierChannel)
            {
                if (message.Content != "?verify")
                {
                    await message.DeleteAsync();
                    return;
                }
            }
            if (string.IsNullOrEmpty(prefix) || !message.Content.StartsWith(prefix))
            {
                return;
            }
            string reply = Reply(message.Content.Split(' '));
            if (string.IsNullOrEmpty(reply))
            {
                return;
            }
            if (!reply.Contains("|@@|@|"))
            {
                _ = await message.Channel.SendMessageAsync(reply);
                return;
            }
            _ = await message.Channel.SendMessageAsync(Environment.NewLine + "_ _" + Environment.NewLine);
            string[] replies = reply.Split(new string[1]
            {
                "|@@|@|"
            }, StringSplitOptions.None);
            string[] array = replies;
            foreach (string text in array)
            {
                _ = await message.Channel.SendMessageAsync("```" + text + "```");
            }
        }

        public async Task SendChannel([Remainder] string message)
        {
            SocketTextChannel channel = client.GetChannel(lobbychannel) as SocketTextChannel;
            _ = await channel.SendMessageAsync(message);
        }

        public async Task SendLoginLog([Remainder] string message)
        {
            SocketTextChannel channel = client.GetChannel(userloginchannel) as SocketTextChannel;
            _ = await channel.SendMessageAsync(message);
        }

        public async Task SendPublicLoginLog([Remainder] string message)
        {
            SocketTextChannel channel = client.GetChannel(publicloginchannel) as SocketTextChannel;
            _ = await channel.SendMessageAsync(message);
        }

        public async Task SendGameChannel([Remainder] string message)
        {
            SocketTextChannel channel = client.GetChannel(lobbychannel) as SocketTextChannel;
            _ = await channel.SendMessageAsync(message);
        }

        public string Reply(string[] args)
        {
            try
            {
                int cmid = 0;
                int duration = -1;
                if (args.Length > 1)
                {
                    if(!int.TryParse(args[1], out cmid))
                    {
                        return "Please input valid CMID before using this command";
                    }
                }
                if (args.Length > 2)
                {
                    if (!int.TryParse(args[2], out duration))
                    {
                        return "Please input valid Duration before using this command";
                    }
                }
                string arg = args[0].Replace(prefix, "");
                string message = string.Join(' ', args);
                string retstring5;
                switch (arg)
                {
                    case "alts":
                        {
                            List<string> alts = uberBeatManager.Alts(cmid);
                            retstring5 = string.Join(Environment.NewLine + Environment.NewLine, alts);
                            return retstring5.Length > 1900 ? string.Join("|@@|@|", Trim(retstring5, 1900).ToList()) : "```" + retstring5 + "```";
                        }
                    case "hwid":
                        {
                            string[] hwidlist = uberBeatManager.GetHWID(cmid).ToArray();
                            return string.Join(Environment.NewLine, hwidlist);
                        }
                    case "search":
                        {
                            if (args[1].Length < 2)
                            {
                                return "Search name should be 2 characters atleast";
                            }
                            List<string> searchlist = uberBeatManager.Search(args[1]);
                            return string.Join(Environment.NewLine + Environment.NewLine, searchlist);
                        }
                    case "mute":
                        return uberBeatManager.Mute(cmid, duration);
                    case "unmute":
                        return uberBeatManager.Unmute(cmid);
                    case "ban":
                        {
                            retstring5 = uberBeatManager.Ban(cmid, duration);
                            string returnstr = PhotonSocket.ExecuteClientSocket(message.Replace(prefix, ""));
                            return returnstr + Environment.NewLine + retstring5;
                        }
                    case "unban":
                        return uberBeatManager.Unban(cmid);
                    case "status":
                        return "Disabled.";
                    case "push":
                        return "Disabled.";
                    case "players":
                    case "msg":
                    case "kick":
                        return PhotonSocket.ExecuteClientSocket(message.Replace(prefix, ""));
                    case "processes":
                    case "windows":
                    case "modules":
                        retstring5 = PhotonSocket.ExecuteClientSocket(message.Replace(prefix, ""));
                        if (retstring5.Length > 1900)
                        {
                            return string.Join("|@@|@|", Trim(retstring5, 1900).ToList());
                        }
                        return "```" + retstring5 + "```";
                    case "leaderboardkill":
                    case "leaderboardxp":
                    case "leaderboardkdr":
                        {
                            int count = (cmid == 0) ? 100 : cmid;
                            retstring5 = string.Join(Environment.NewLine, uberBeatManager.Leaderboard(count, arg.Replace("leaderboard", "")));
                            return retstring5.Length > 1900 ? string.Join("|@@|@|", Trim(retstring5, 1900).ToList()) : "```" + retstring5 + "```";
                        }
                    default:
                        return null;
                    case "banned":
                        retstring5 = string.Join(Environment.NewLine, uberBeatManager.bannedUsers());
                        if (retstring5.Length > 1900)
                        {
                            return string.Join("|@@|@|", Trim(retstring5, 1900).ToList());
                        }
                        return "```" + retstring5 + "```";
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        private List<string> Trim(string stringToSplit, int maximumLineLength)
        {
            IEnumerable<string> lines = stringToSplit.Split(new string[1]
            {
                Environment.NewLine
            }, StringSplitOptions.None).Concat<string>(new string[1]
            {
                ""
            });
            return lines.Skip(1).Aggregate(lines.Take(1).ToList(), delegate (List<string> a, string w)
            {
                string text = a.Last();
                while (text.Length > maximumLineLength)
                {
                    a[a.Count() - 1] = text[..maximumLineLength];
                    text = text[maximumLineLength..];
                    a.Add(text);
                }
                string text2 = text + Environment.NewLine + w;
                if (text2.Length > maximumLineLength)
                {
                    a.Add(w);
                }
                else
                {
                    a[a.Count() - 1] = text2;
                }
                return a;
            }).ToList();
        }

        public async void UserLog(UserDocument result, int duration, string hwid)
        {
            if (result == null)
            {
                if (duration == -1)
                {
                    await SendLoginLog(string.Concat(new string[]
                    {
                        "``Permanently Banned user with HWID:``",
                        Environment.NewLine,
                        "```",
                        hwid.Replace("|", Environment.NewLine),
                        "```has been kicked."
                    }));
                    return;
                }
                if (duration == 0)
                {
                    await SendLoginLog(string.Concat(new string[]
                    {
                        "``User with HWID:``",
                        Environment.NewLine,
                        "```",
                        hwid.Replace("|", Environment.NewLine),
                        "```has logged in."
                    }));
                    return;
                }
                await SendLoginLog(string.Concat(new string[]
                {
                    "``Temporarily Banned user ( for ",
                    duration.ToString(),
                    " more minutes ) with HWID:``",
                    Environment.NewLine,
                    "```",
                    hwid.Replace("|", Environment.NewLine),
                    "```has logged in."
                }));
                return;
            }
            else
            {
                if (duration == -1)
                {
                    await SendLoginLog(string.Concat(new string[]
                    {
                        string.Format("``Permanently banned user with CMID {0}, Name: {1}, SteamID {2} and HWID:``", result.Profile.Cmid, result.Profile.Name, result.SteamId),
                        Environment.NewLine,
                        "```",
                        hwid.Replace("|", Environment.NewLine),
                        "```has been kicked."
                    }));
                    return;
                }
                if (duration == 0)
                {
                    await SendLoginLog(string.Concat(new string[] { string.Format("``User with CMID {0}, Name: {1}, SteamID {2} and HWID:``", result.Profile.Cmid, result.Profile.Name, result.SteamId), Environment.NewLine, "```", hwid.Replace("|", Environment.NewLine), "```has logged in." }));
                    await SendPublicLoginLog(string.Concat(new string[] { string.Format("``User with Name: {0}``", result.Profile.Name), Environment.NewLine, "has logged in." }));
                    return;
                }
                await SendLoginLog(string.Concat(new string[] { string.Format("``Temporarily Banned user ( for {0} more minutes ) with CMID {1}, Name {2}, SteamID {3} and HWID:``", new object[] { duration.ToString(), result.Profile.Cmid, result.Profile.Name, result.SteamId }), Environment.NewLine, "```", hwid.Replace("|", Environment.NewLine), "```has logged in." }));
                return;
            }
        }
    }
}
