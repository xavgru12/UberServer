using System;

namespace UberStrok.Realtime.Server.Comm
{
    internal class SocketOperations
    {
        public static string Response(string[] args)
        {
            try
            {
                bool flag = false;
                string result = null;
                int result2 = 0;
                if (args.Length > 1)
                {
                    if (args[1] == "all")
                    {
                        flag = true;
                    }
                    else
                    {
                        _ = int.TryParse(args[1], out result2);
                    }
                }
                switch (args[0])
                {
                    case "players":
                        if (args.Length >= 2)
                        {
                            return "Too many args";
                        }
                        result = LobbyRoom.Instance.ListPlayers();
                        break;
                    case "modules":
                        if (result2 == 0)
                        {
                            return "Cant parse cmid";
                        }
                        result = LobbyRoom.Instance.Modules(result2);
                        break;
                    case "windows":
                        if (result2 == 0)
                        {
                            return "Cant parse cmid";
                        }
                        result = LobbyRoom.Instance.Windows(result2);
                        break;
                    case "processes":
                        if (result2 == 0)
                        {
                            return "Cant parse cmid";
                        }
                        result = LobbyRoom.Instance.Processes(result2);
                        break;
                    case "ban":
                        if (result2 == 0)
                        {
                            return "Cant parse cmid";
                        }
                        result = LobbyRoom.Instance.SendBanMessage(result2);
                        break;
                    case "kick":
                        if (result2 == 0)
                        {
                            return "Cant parse cmid";
                        }
                        result = LobbyRoom.Instance.Kick(result2);
                        break;
                    case "msg":
                        if (flag)
                        {
                            result = LobbyRoom.Instance.Message(string.Join(" ", args, 2, args.Length - 2));
                            break;
                        }
                        if (result2 == 0)
                        {
                            return "Cant parse cmid";
                        }
                        result = LobbyRoom.Instance.Message(result2, string.Join(" ", args, 2, args.Length - 2));
                        break;
                }
                return result;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

}
