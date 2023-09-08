using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Comm.UberBeat;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Comm
{
    public partial class LobbyRoom
    {
        private static readonly char[] _separators = new char[1] { ' ' };
        public override void OnDisconnect(CommPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info($"{peer.Actor.Cmid} Disconnected {reasonCode} -> {reasonDetail}");
            Leave(peer);
        }

        protected override void OnFullPlayerListUpdate(CommPeer peer)
        {
            List<CommActorInfoView> list = new List<CommActorInfoView>(_peers.Count);
            for (int i = 0; i < _peers.Count; i++)
            {
                list.Add(_peers[i].Actor.View);
            }
            peer.Events.Lobby.SendFullPlayerListUpdate(list);
        }

        protected override void OnUpdatePlayerRoom(CommPeer peer, GameRoomView room)
        {
            peer.Actor.View.CurrentRoom = room;
            foreach (CommPeer peer2 in _peers)
            {
                peer2.Events.Lobby.SendPlayerUpdate(peer.Actor.View);
            }
        }

        protected override void OnResetPlayerRoom(CommPeer peer)
        {
            peer.Actor.View.CurrentRoom = null;
            foreach (CommPeer peer2 in _peers)
            {
                peer2.Events.Lobby.SendPlayerUpdate(peer.Actor.View);
            }
        }

        protected override void OnUpdateFriendsList(CommPeer peer, int cmid)
        {
            Find(cmid)?.Events.Lobby.SendUpdateFriendsList();
        }

        protected override void OnModulesSignatureRequest(CommPeer peer, string umodules)
        {
            string[] array = umodules.Split(',');
            foreach (string item in array)
            {
                _ = peer.Actor.TrustedModules.Add(item);
            }
        }

        protected override void OnUpdateClanData(CommPeer peer, int cmid)
        {
            Find(cmid)?.Events.Lobby.SendUpdateClanData();
        }

        protected override void OnUpdateInboxMessages(CommPeer peer, int cmid, int messageId)
        {
            Find(cmid)?.Events.Lobby.SendUpdateInboxMessages(messageId);
        }

        protected override void OnUpdateInboxRequests(CommPeer peer, int cmid)
        {
            Find(cmid)?.Events.Lobby.SendUpdateInboxRequests();
        }

        protected override void OnUpdateClanMembers(CommPeer peer, List<int> clanMembers)
        {
            foreach (int clanMember in clanMembers)
            {
                Find(clanMember)?.Events.Lobby.SendUpdateClanMembers();
            }
        }

        protected override void OnGetPlayersWithMatchingName(CommPeer peer, string search)
        {
            throw new NotImplementedException();
        }

        protected override void OnChatMessageToAll(CommPeer peer, string message)
        {
            lock (Sync)
            {
                if (peer.Actor.IsMuted)
                {
                    peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "You are muted!");
                    return;
                }
                if ((int)peer.Actor.AccessLevel >= 6 && message.Length > 0 && message[0] == '?')
                {
                    Log.Debug("Command executed by User " + peer.Actor.Name + ":" + message);
                    try
                    {
                        string[] array = message.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                        _ = array[0];
                        string text;
                        switch (array[0])
                        {
                            case "?ban":
                                {
                                    text = (array.Length >= 2) ? (int.TryParse(array[1], out int result11) ? ((result11 != peer.Actor.Cmid) ? ((!DoBan(peer, result11)) ? "Error: Failed to ban user." : $"Banned user with CMID {result11}.") : "Banning yourself might be a bad idea. :)") : "Error: <cmid> must be an integer.") : "Usage: ?ban <cmid>";
                                    break;
                                }
                            case "?unban":
                                {
                                    text = (array.Length >= 2) ? (int.TryParse(array[1], out int result4) ? ((result4 != peer.Actor.Cmid) ? ((!DoUnban(peer, result4)) ? "Error: Failed to unban user." : $"Unbanned user with CMID {result4}.") : "You can't unban yourself.") : "Error: <cmid> must be an integer.") : "Usage: ?unban <cmid>";
                                    break;
                                }
                            case "?baniphwd":
                                {
                                    text = ((int)peer.Actor.AccessLevel == 10) ? ((array.Length >= 2) ? (int.TryParse(array[1], out int result2) ? ((result2 != peer.Actor.Cmid) ? ((!DoBanIpHwd(result2)) ? "Error: Failed to ban user." : $"Banned user with CMID {result2}.") : "Banning yourself might be a bad idea. :)") : "Error: <cmid> must be an integer.") : "Usage: ?baniphwd <cmid>") : "Error: Only admins can use this command.";
                                    break;
                                }
                            case "?msg":
                                {
                                    if (array.Length < 3)
                                    {
                                        text = "Usage: ?msg <cmid>/all <message>";
                                        break;
                                    }
                                    int result9 = 0;
                                    if (array[1] == "all")
                                    {
                                        result9 = 0;
                                    }
                                    else if (!int.TryParse(array[1], out result9))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    string message2 = string.Join(" ", array, 2, array.Length - 2);
                                    if (result9 == 0)
                                    {
                                        if ((int)peer.Actor.AccessLevel != 10)
                                        {
                                            text = "Error: Only admins can send to all users.";
                                            break;
                                        }
                                        lock (Sync)
                                        {
                                            foreach (CommPeer peer2 in Peers)
                                            {
                                                peer2.Events.Lobby.SendModerationCustomMessage(message2);
                                            }
                                        }
                                        text = "Sent message to all users.";
                                    }
                                    else
                                    {
                                        CommPeer commPeer3 = Find(result9);
                                        if (commPeer3 != null)
                                        {
                                            commPeer3.Events.Lobby.SendModerationCustomMessage(message2);
                                            text = $"Sent message to user with CMID {result9}.";
                                        }
                                        else
                                        {
                                            text = $"Could not find user with CMID {result9} online.";
                                        }
                                    }
                                    break;
                                }
                            case "?modules":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?modules <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result10))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    if (Find(result10) == null)
                                    {
                                        text = $"Could not find user with CMID {result10} online.";
                                        break;
                                    }
                                    text = null;
                                    FetchModules(peer, result10);
                                    break;
                                }
                            case "?processes":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?processes <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result12))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    if (Find(result12) == null)
                                    {
                                        text = $"Could not find user with CMID {result12} online.";
                                        break;
                                    }
                                    text = null;
                                    FetchProcesses(peer, result12);
                                    break;
                                }
                            case "?windows":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?processes <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result5))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    if (Find(result5) == null)
                                    {
                                        text = $"Could not find user with CMID {result5} online.";
                                        break;
                                    }
                                    text = null;
                                    FetchWindows(peer, result5);
                                    break;
                                }
                            case "?eban":
                                text = null;
                                if (array.Length == 2)
                                {
                                    if (!int.TryParse(array[1], out int result14))
                                    {
                                        text = "Usage: ?eban <integer:cmid> <optional:duration in minutes>";
                                        break;
                                    }
                                    CommPeer target = Find(result14);
                                    peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Trying to ban user..");
                                    BanUberBeatUser(peer, target, result14);
                                }
                                else if (array.Length == 3)
                                {
                                    if (!int.TryParse(array[1], out int result15))
                                    {
                                        text = "Usage: ?eban <integer:cmid> <optional:duration in minutes>";
                                        break;
                                    }
                                    if (!int.TryParse(array[2], out int result16))
                                    {
                                        text = "Usage: ?eban <integer:cmid> <optional:duration in minutes>";
                                        break;
                                    }
                                    CommPeer target2 = Find(result15);
                                    peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Trying to ban user..");
                                    BanUberBeatUser(peer, target2, result15, result16);
                                }
                                else
                                {
                                    text = "Usage: ?eban <integer:cmid> <optional:duration in minutes>";
                                }
                                break;
                            case "?eunban":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?eban <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result6))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    text = null;
                                    peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Trying to unban user..");
                                    UnBanUberBeatUser(peer, result6);
                                    break;
                                }
                            case "?alts":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?alts <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result13))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    text = null;
                                    peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Trying to Find alts");
                                    FindAlts(peer, result13);
                                    break;
                                }
                            case "?search":
                                if (array.Length < 2 || array[1].Length < 3)
                                {
                                    text = "Usage: ?search <some_text(minimum 3 characters)>";
                                    break;
                                }
                                text = null;
                                peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Trying to Find Accounts");
                                FindAccounts(peer, array[1]);
                                break;
                            case "?hwid":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?hwid <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result3))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    text = null;
                                    peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Trying to get all hwids of user");
                                    GetHWID(peer, result3);
                                    break;
                                }
                            case "?help":
                                text = null;
                                peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "List of available Commands:");
                                SendHelp(peer);
                                break;
                            case "?emute":
                                {
                                    if (array.Length < 3)
                                    {
                                        text = "Usage: ?emute <cmid> <duration_in_minutes>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result7))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    if (!int.TryParse(array[2], out int result8))
                                    {
                                        text = "Error: <duration_in_minutes> must be an integer.";
                                        break;
                                    }
                                    text = "";
                                    CommPeer commPeer2 = Find(result7);
                                    if (commPeer2 == null)
                                    {
                                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Muting offline user with cmid " + result7 + " for " + result8 + " minutes.");
                                        OnModerationMuteOfflinePlayer(result7, result8);
                                    }
                                    else
                                    {
                                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Muting user " + commPeer2.Actor.Name + " for " + result8 + " minutes.");
                                        OnModerationMutePlayer(peer, result8, result7, true);
                                    }
                                    break;
                                }
                            case "?eunmute":
                                {
                                    if (array.Length < 2)
                                    {
                                        text = "Usage: ?eumute <cmid>";
                                        break;
                                    }
                                    if (!int.TryParse(array[1], out int result))
                                    {
                                        text = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                    text = null;
                                    CommPeer commPeer = Find(result);
                                    if (commPeer == null)
                                    {
                                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Unmuting offline user with cmid " + result);
                                        OnModerationUnMuteOfflinePlayer(result);
                                    }
                                    else
                                    {
                                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Unmuting user " + commPeer.Actor.Name);
                                        OnModerationUnMutePlayer(commPeer);
                                    }
                                    break;
                                }
                            default:
                                text = "Error: Unknown command.";
                                break;
                        }
                        if (!string.IsNullOrEmpty(text))
                        {
                            peer.Events.Lobby.SendLobbyChatMessage(0, "Server", text);
                        }
                        return;
                    }
                    catch (Exception ex)
                    {
                        string text2 = "Unhandled exception while handling command.";
                        Log.Error(text2, ex);
                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", text2);
                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", "Error Report:\n" + ex.ToString());
                        return;
                    }
                }
                foreach (CommPeer peer3 in Peers)
                {
                    if (peer3.Actor.Cmid != peer.Actor.Cmid)
                    {
                        peer3.Events.Lobby.SendLobbyChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
                    }
                }
                LobbyMessage(peer, message);
            }
        }

        protected override void OnChatMessageToPlayer(CommPeer peer, int cmid, string message)
        {
            if (!peer.Actor.IsMuted)
            {
                Find(cmid)?.Events.Lobby.SendPrivateChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
            }
        }

        protected override void OnChatMessageToClan(CommPeer peer, List<int> clanMembers, string message)
        {
            foreach (int clanMember in clanMembers)
            {
                Find(clanMember)?.Events.Lobby.SendClanChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
            }
            Instance.ClanMessage(peer.Actor.Cmid, peer.Actor.Name, message);
        }

        protected override void OnModerationMutePlayer(CommPeer peer, int durationInMinutes, int mutedCmid, bool disableChat)
        {
            if ((int)peer.Actor.AccessLevel >= 6)
            {
                CommPeer commPeer = Find(mutedCmid);
                if (!disableChat)
                {
                    OnModerationUnMutePlayer(commPeer);
                }
                else if (commPeer != null && (int)commPeer.Actor.AccessLevel < 10)
                {
                    UberBeatManager ubm = new UberBeatManager();
                    commPeer.Actor.IsMuted = durationInMinutes > 0;
                    commPeer.Actor.MuteEndTime = DateTime.UtcNow.AddMinutes(durationInMinutes);
                    commPeer.Events.Lobby.SendModerationMutePlayer(disableChat);
                    File.AppendAllText(ubm.MuteData, commPeer.Actor.Cmid + "=" + commPeer.Actor.MuteEndTime.ToString() + Environment.NewLine);
                }
            }
        }

        public void OnModerationMuteOfflinePlayer(int Cmid, int minutes)
        {
            UberBeatManager ubm = new UberBeatManager();
            DateTime dateTime = DateTime.UtcNow.AddMinutes(minutes);
            File.AppendAllText(ubm.MuteData, Cmid + "=" + dateTime.ToString() + Environment.NewLine);
        }

        public void OnModerationUnMutePlayer(CommPeer mutedPeer)
        {
            UberBeatManager ubm = new UberBeatManager();
            string[] array = File.ReadAllLines(ubm.MuteData);
            foreach (string text in array)
            {
                if (text.StartsWith(mutedPeer.Actor.Cmid + "="))
                {
                    string[] array2 = text.Split('=');
                    try
                    {
                        _ = DateTime.Parse(array2[1]);
                        string text2 = File.ReadAllText(ubm.MuteData);
                        text2 = text2.Replace(text, "");
                        text2 = Regex.Replace(text2, "^\\s+$[\\r\\n]*", string.Empty, RegexOptions.Multiline);
                        mutedPeer.Actor.MuteEndTime = DateTime.UtcNow;
                        mutedPeer.Actor.IsMuted = false;
                        File.WriteAllText(ubm.MuteData, text2);
                        mutedPeer.Events.Lobby.SendModerationMutePlayer(false);
                    }
                    catch (Exception)
                    {
                        Log.Error("Failed to do unmute for player :" + mutedPeer.Actor.Cmid);
                    }
                }
            }
        }

        private void LobbyMessage(CommPeer peer, string message)
        {
            string text = string.IsNullOrEmpty(peer.Actor.View.ClanTag) ? "[" + peer.Actor.View.Cmid + "] " + peer.Actor.Name + " ``UTC Time:" + DateTime.UtcNow.ToString() + "``" : "[" + peer.Actor.View.Cmid + "] {" + peer.Actor.View.ClanTag + "} " + peer.Actor.Name + " ``UTC Time:" + DateTime.UtcNow.ToString() + "``";
            SendMessageUDP(text + Environment.NewLine + "```" + message + "```");
        }

        public void ClanMessage(int cmid, string name, string message)
        {
            try
            {
                CommPeer commPeer = Find(cmid);
                if (commPeer != null)
                {
                    string text = "{" + commPeer.Actor.View.ClanTag + "}";
                    if (!string.IsNullOrEmpty(text))
                    {
                        string arg = $"[{cmid}] {name}";
                        string text2 = $"Clan tag : ``{text}`` , User : ``{arg}`` UTC Time: ``{DateTime.UtcNow}``";
                        SendMessageUDP(text2 + Environment.NewLine + "```" + message + "```");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to log clan message : ", ex);
            }
        }

        public void SendMessageUDP(string message)
        {
            using (UdpClient udpClient = new UdpClient())
            {
                if (!File.Exists("udphost.txt"))
                {
                    File.WriteAllText("udphost.txt", "127.0.0.1");
                }
                udpClient.Connect(File.ReadAllText("udphost.txt"), 5070);
                byte[] bytes = Encoding.UTF8.GetBytes("comm:" + message);
                _ = udpClient.Send(bytes, bytes.Length);
            }
        }

        public void OnModerationUnMuteOfflinePlayer(int Cmid)
        {
            UberBeatManager ubm = new UberBeatManager();
            string[] array = File.ReadAllLines(ubm.MuteData);
            foreach (string text in array)
            {
                if (text.StartsWith(Cmid + "="))
                {
                    string[] array2 = text.Split('=');
                    try
                    {
                        _ = DateTime.Parse(array2[1]);
                        string text2 = File.ReadAllText(ubm.MuteData);
                        text2 = text2.Replace(text, "");
                        text2 = Regex.Replace(text2, "^\\s+$[\\r\\n]*", string.Empty, RegexOptions.Multiline);
                        File.WriteAllText(ubm.MuteData, text2);
                    }
                    catch (Exception)
                    {
                        Log.Error("Failed to check unmute on player :" + Cmid);
                    }
                }
            }
        }

        protected override void OnModerationPermanentBan(CommPeer peer, int cmid)
        {
        }

        protected override void OnModerationBanPlayer(CommPeer peer, int cmid)
        {
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Invalid comparison between Unknown and I4
            if ((int)peer.Actor.AccessLevel >= 6)
            {
                Find(cmid)?.SendError("You have been kicked from the game.");
            }
        }

        protected override void OnModerationKickGame(CommPeer peer, int cmid)
        {
            //IL_0006: Unknown result type (might be due to invalid IL or missing references)
            //IL_000c: Invalid comparison between Unknown and I4
            if ((int)peer.Actor.AccessLevel >= 4)
            {
                Find(cmid)?.Events.Lobby.SendModerationKickGame();
            }
        }

        protected override void OnModerationUnbanPlayer(CommPeer peer, int cmid)
        {
        }

        protected override void OnModerationCustomMessage(CommPeer peer, int cmid, string message)
        {
        }

        protected override void OnSpeedhackDetection(CommPeer peer)
        {
            peer.SendError("An error has occured tht forced Uberstrike to halt.");
        }

        protected override void OnSpeedhackDetectionNew(CommPeer peer, List<float> timeDifferences)
        {
            if (IsSpeedHacking(timeDifferences))
            {
                peer.SendError("An error has occured tht forced Uberstrike to halt.");
            }
        }

        protected override void OnPlayersReported(CommPeer peer, List<int> cmids, int type, string details, string logs)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateNaughtyList(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnClearModeratorFlags(CommPeer peer, int cmid)
        {
            throw new NotImplementedException();
        }

        protected override void OnSetContactList(CommPeer peer, List<int> cmids)
        {
            peer.Actor.ContactList = new HashSet<int>(cmids);
        }

        protected override void OnUpdateAllActors(CommPeer peer)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdateContacts(CommPeer peer)
        {
            foreach (int contact in peer.Actor.ContactList)
            {
                _ = contact;
            }
            throw new NotImplementedException();
        }

        private bool DoBan(CommPeer peer, int cmid)
        {
            int num;
            try
            {
                num = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices).BanCmid(peer.AuthToken, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                num = 1;
            }
            Find(cmid)?.SendError("You have been banned.");
            return num == 0;
        }

        private bool DoBanIpHwd(int cmid)
        {
            int num;
            try
            {
                num = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices).Ban(CommApplication.Instance.Configuration.WebServicesAuth, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                num = 1;
            }
            Find(cmid)?.SendError("You have been banned.");
            return num == 0;
        }

        private bool DoUnban(CommPeer peer, int cmid)
        {
            int num;
            try
            {
                num = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices).UnbanCmid(peer.AuthToken, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                num = 1;
            }
            return num == 0;
        }

        private void FetchModules(CommPeer peer, int cmid)
        {
            List<string> list = new List<string>(Find(cmid).Actor.Modules);
            if (list.Count < 1)
            {
                peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat", "Error getting List. Data Length is 0");
                return;
            }
            int num = 0;
            foreach (string item in list)
            {
                num++;
                peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat: Modules of CMID " + cmid, $"[{num}] Module Name: {item} ");
            }
        }

        private void SendHelp(CommPeer peer)
        {
            foreach (string item in new List<string>
        {
            "?ban <cmid> -> regular ban", "?unban <cmid> -> regular unban", "?baniphwd <cmid> -> regular ip ban", "?msg <optional:all> <cmid> -> sends server popup message to users (optional for admins)", "?modules <cmid> -> get dll list loaded into target client uberstrike.exe", "?processes <cmid> -> get list of running process and path from target client pc", "?windows <cmid> -> get list of running windows from target client pc", "?eban <cmid> <optional:duration in minutes> -> hardware ban the target user (works for offline user aswell)", "?eunban <cmid> -> unbans hardware of target user (works for offline user aswell)", "?alts <cmid> -> finds all the alternative account of target user (works for offline user aswell)",
            "?hwid <cmid> -> finds all the hwids of target user (works for offline user aswell)", "?search <some_text(minimum 3 characters)> -> searches account and cmid using the given input", "?help -> lol", "?emute <cmid> -> mutes the target user (works for offline user aswell)", "?eunmute <cmid> -> munmutes the target user (works for offline user aswell)"
        })
            {
                peer.Events.Lobby.SendLobbyChatMessage(0, "", item);
            }
        }

        private void FetchProcesses(CommPeer peer, int cmid)
        {
            List<string> list = new List<string>(Find(cmid).Actor.Processes);
            if (list.Count < 1)
            {
                peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat", "Error getting List. Data Length is 0");
                return;
            }
            int num = 0;
            foreach (string item in list)
            {
                num++;
                peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat: Processes of CMID " + cmid, $"[{num}] Process Name: {item} ");
            }
        }

        protected override void OnUberBeatAuthenticate(CommPeer peer, string HWID)
        {
        }

        protected override void OnUberBeatReport(CommPeer peer, string report)
        {
            if (report.StartsWith("TRUSTED:"))
            {
                report = report.Replace("TRUSTED:", "");
                string[] array = report.Split('|');
                foreach (string item in array)
                {
                    _ = peer.Actor.TrustedModules.Add(item);
                }
            }
            else if (report.StartsWith("DETECTED:"))
            {
                UberBeatManager ubm = new UberBeatManager();
                report = report.Replace("DETECTED:", "");
                File.AppendAllText(ubm.HackReport, peer.Actor.Cmid + ":" + report + Environment.NewLine);
                peer.Disconnect();
                peer.Dispose();
            }
            else if (report.StartsWith("REPORT:"))
            {
                report = report.Replace("REPORT:", "");
                new UberBeat.UberBeat().Initialize(report, peer);
            }
        }

    }
}
