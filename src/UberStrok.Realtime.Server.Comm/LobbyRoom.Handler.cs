using Newtonsoft.Json;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.WebServices.Client;

namespace UberStrok.Realtime.Server.Comm
{
    public partial class LobbyRoom
    {
        public override void OnDisconnect(CommPeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Log.Info($"{peer.Actor.Cmid} Disconnected {reasonCode} -> {reasonDetail}");
            Leave(peer);
        }

        protected override void OnFullPlayerListUpdate(CommPeer peer)
        {
            var actors = new List<CommActorInfoView>(_peers.Count);
            for (int i = 0; i < this._peers.Count; i++)
                actors.Add(this._peers[i].Actor.View);

            peer.Events.Lobby.SendFullPlayerListUpdate(actors);
        }

        protected override void OnUpdatePlayerRoom(CommPeer peer, GameRoomView room)
        {
            var settings = new JsonSerializerSettings();
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            Log.Info(peer.Actor.Cmid + "\n" + JsonConvert.SerializeObject(room, Formatting.Indented, settings));
        }

        protected override void OnResetPlayerRoom(CommPeer peer)
        {
            peer.Actor.View.CurrentRoom = null;

            foreach (var otherPeer in this._peers)
            {
                otherPeer.Events.Lobby.SendPlayerUpdate(peer.Actor.View);
            }
        }

        protected override void OnUpdateFriendsList(CommPeer peer, int cmid)
        {
            CommPeer userPeer = this.Find(cmid);

            if (userPeer != null)
            {
                userPeer.Events.Lobby.SendUpdateFriendsList();
            }
        }

        protected override void OnUpdateClanData(CommPeer peer, int cmid)
        {
            CommPeer userPeer = this.Find(cmid);

            if (userPeer != null)
            {
                userPeer.Events.Lobby.SendUpdateClanData();
            }
        }

        protected override void OnUpdateInboxMessages(CommPeer peer, int cmid, int messageId)
        {
            CommPeer userPeer = this.Find(cmid);

            if (userPeer != null)
            {
                userPeer.Events.Lobby.SendUpdateInboxMessages(messageId);
            }
        }

        protected override void OnUpdateInboxRequests(CommPeer peer, int cmid)
        {
            CommPeer friendPeer = this.Find(cmid);

            if (friendPeer != null)
            {
                friendPeer.Events.Lobby.SendUpdateInboxRequests();
            }
        }

        protected override void OnUpdateClanMembers(CommPeer peer, List<int> clanMembers)
        {
            foreach (var clanMember in clanMembers)
            {
                CommPeer clanMemberPeer = this.Find(clanMember);

                if (clanMemberPeer != null)
                {
                    clanMemberPeer.Events.Lobby.SendUpdateClanMembers();
                }
            }
        }

        protected override void OnGetPlayersWithMatchingName(CommPeer peer, string search)
        {
            throw new NotImplementedException();
        }

        private static readonly char[] _separators = { ' ' };

        protected override void OnChatMessageToAll(CommPeer peer, string message)
        {
            if (peer.Actor.IsMuted)
                return;

            lock (Sync)
            {
                if (peer.Actor.AccessLevel >= MemberAccessLevel.Moderator && message.Length > 0 && message[0] == '?')
                {
                    try
                    {
                        var args = message.Split(_separators, StringSplitOptions.RemoveEmptyEntries);
                        var cmd = args[0];

                        string response;
                        switch (args[0])
                        {
                            case "?ban":
                                {
                                    if (args.Length < 2)
                                    {
                                        response = "Usage: ?ban <cmid>";
                                        break;
                                    }

                                    if (!int.TryParse(args[1], out int cmid))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }

                                    if (cmid == peer.Actor.Cmid)
                                    {
                                        response = "Banning yourself might be a bad idea. :)";
                                        break;
                                    }
                                    try
                                    {
                                        if (DoBan(peer, cmid))
                                            response = $"Banned user with CMID {cmid}.";
                                        else
                                            response = "Error: Failed to ban user.";
                                    }
                                    catch (Exception ex)
                                    {
                                        response = "Error: Exception thrown while trying to ban a user: " + ex.Message;
                                    }
                                    break;
                                }

                            case "?unban":
                                {
                                    if (args.Length < 2)
                                    {
                                        response = "Usage: ?unban <cmid>";
                                        break;
                                    }

                                    if (!int.TryParse(args[1], out int cmid))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }

                                    if (cmid == peer.Actor.Cmid)
                                    {
                                        response = "You can't unban yourself.";
                                        break;
                                    }

                                    if (DoUnban(peer, cmid))
                                        response = $"Unbanned user with CMID {cmid}.";
                                    else
                                        response = "Error: Failed to unban user.";
                                    break;
                                }

                            case "?baniphwd":
                                {
                                    if (peer.Actor.AccessLevel != MemberAccessLevel.Admin)
                                    {
                                        response = $"Error: Only admins can use this command.";
                                        break;
                                    }

                                    if (args.Length < 2)
                                    {
                                        response = "Usage: ?baniphwd <cmid>";
                                        break;
                                    }

                                    if (!int.TryParse(args[1], out int cmid))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }

                                    if (cmid == peer.Actor.Cmid)
                                    {
                                        response = "Banning yourself might be a bad idea. :)";
                                        break;
                                    }

                                    if (DoBanIpHwd(cmid))
                                        response = $"Banned user with CMID {cmid}.";
                                    else
                                        response = "Error: Failed to ban user.";
                                    break;
                                }

                            case "?msg":
                                if (args.Length < 3)
                                {
                                    response = "Usage: ?msg <cmid>/all <message>";
                                    break;
                                }

                                int target = 0;
                                if (args[1] == "all") target = 0;
                                else
                                {
                                    if (!int.TryParse(args[1], out target))
                                    {
                                        response = "Error: <cmid> must be an integer.";
                                        break;
                                    }
                                }

                                var adminMessage = string.Join(" ", args, 2, args.Length - 2);
                                if (target == 0)
                                {
                                    if (peer.Actor.AccessLevel != MemberAccessLevel.Admin)
                                    {
                                        response = $"Error: Only admins can send to all users.";
                                        break;
                                    }

                                    lock (Sync)
                                    {
                                        foreach (var otherPeer in Peers)
                                            otherPeer.Events.Lobby.SendModerationCustomMessage(adminMessage);
                                    }

                                    response = $"Sent message to all users.";
                                }
                                else
                                {
                                    var otherPeer = Find(target);
                                    if (otherPeer != null)
                                    {
                                        otherPeer.Events.Lobby.SendModerationCustomMessage(adminMessage);
                                        response = $"Sent message to user with CMID {target}.";
                                    }
                                    else
                                        response = $"Could not find user with CMID {target} online.";
                                }
                                break;

                            default:
                                response = "Error: Unknown command.";
                                break;
                        }

                        peer.Events.Lobby.SendLobbyChatMessage(0, "Server", response);
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to handle command.", ex);
                        /* Fall through as normal message. */
                    }
                }

                foreach (var otherPeer in Peers)
                {
                    if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                        otherPeer.Events.Lobby.SendLobbyChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
                }
            }
        }

        protected override void OnChatMessageToPlayer(CommPeer peer, int cmid, string message)
        {
            if (peer.Actor.IsMuted)
                return;

            Find(cmid)?.Events.Lobby.SendPrivateChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
        }

        protected override void OnChatMessageToClan(CommPeer peer, List<int> clanMembers, string message)
        {
            foreach (int clanMember in clanMembers)
            {
                CommPeer clanMemberPeer = this.Find(clanMember);

                if (clanMemberPeer != null)
                {
                    clanMemberPeer.Events.Lobby.SendClanChatMessage(peer.Actor.Cmid, peer.Actor.Name, message);
                }
            }
        }

        protected override void OnModerationMutePlayer(CommPeer peer, int durationInMinutes, int mutedCmid, bool disableChat)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.Moderator)
                return;

            var mutedPeer = Find(mutedCmid);
            if (mutedPeer != null && mutedPeer.Actor.AccessLevel < MemberAccessLevel.Moderator)
            {
                mutedPeer.Actor.IsMuted = durationInMinutes > 0;
                mutedPeer.Actor.MuteEndTime = DateTime.UtcNow.AddMinutes(durationInMinutes);
                mutedPeer.Events.Lobby.SendModerationMutePlayer(mutedPeer.Actor.IsMuted);
            }
        }

        protected override void OnModerationPermanentBan(CommPeer peer, int cmid)
        {
            /* NOTE: Not reachable from game client. */
            if (peer.Actor.AccessLevel < MemberAccessLevel.SeniorQA)
                return;
            DoBan(peer, cmid);
        }

        protected override void OnModerationBanPlayer(CommPeer peer, int cmid)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.Moderator)
                return;
            var user = Find(cmid);
            if (user != null)
            {
                user.SendError("You have been kicked from the game.");
                Leave(user);
            }
        }

        protected override void OnModerationKickGame(CommPeer peer, int cmid)
        {
            if (peer.Actor.AccessLevel < MemberAccessLevel.Moderator)
                return;

            Find(cmid)?.Events.Lobby.SendModerationKickGame();
        }

        protected override void OnModerationUnbanPlayer(CommPeer peer, int cmid)
        {
            /* NOTE: Not reachable from game client. */
            DoUnban(peer, cmid);
        }

        protected override void OnModerationCustomMessage(CommPeer peer, int cmid, string message)
        {
            /* NOTE: Not reachable from game client. */
        }

        protected override void OnSpeedhackDetection(CommPeer peer)
        {
            /* NOTE: Not reachable from game client. */
            peer.SendError();
        }

        protected override void OnSpeedhackDetectionNew(CommPeer peer, List<float> timeDifferences)
        {
            /* NOTE: Not reachable from game client. */

            if (IsSpeedHacking(timeDifferences))
                peer.SendError();
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
            throw new NotImplementedException();
        }

        private bool DoBan(CommPeer peer, int cmid)
        {
            int code;
            try
            {
                var client = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices);
                code = client.BanCmid(peer.AuthToken, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                throw;
            }
            var user = Find(cmid);
            if (user != null)
            {
                user.SendError("You have been banned.");
                Leave(user);
            }
            return code == 0;
        }

        private bool DoBanIpHwd(int cmid)
        {
            int code;
            try
            {
                var client = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices);
                code = client.Ban(CommApplication.Instance.Configuration.WebServicesAuth, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                code = 1;
            }
            var user = Find(cmid);
            if (user != null)
            {
                user.SendError("You have been banned.");
                Leave(user);
            }
            return code == 0;
        }

        private bool DoUnban(CommPeer peer, int cmid)
        {
            int code;
            try
            {
                var client = new ModerationWebServiceClient(CommApplication.Instance.Configuration.WebServices);
                code = client.UnbanCmid(peer.AuthToken, cmid);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to ban user.", ex);
                code = 1;
            }

            return code == 0;
        }
    }
}
