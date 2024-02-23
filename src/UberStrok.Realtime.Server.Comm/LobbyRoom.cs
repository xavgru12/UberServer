using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UberStrok.Core;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Comm.UberBeat;

namespace UberStrok.Realtime.Server.Comm
{
    public partial class LobbyRoom : BaseLobbyRoomOperationHandler, IRoom<CommPeer>, IDisposable
    {
        private bool _disposed;
        private readonly Loop _loop;
        private readonly LoopScheduler _loopScheduler;
        private readonly List<CommPeer> _peers;
        public static LobbyRoom Instance;
        private readonly List<CommPeer> _failedPeers;
        public BufferedAdministratorMessage BufferedMessage;
        public object Sync { get; }
        public ICollection<CommPeer> Peers { get; }
        public LobbyRoom()
        {
            _loop = new Loop(OnTick, OnTickError);
            _loopScheduler = new LoopScheduler(5f);
            _peers = new List<CommPeer>();
            _failedPeers = new List<CommPeer>();
            SocketCommunicaion.Initialize();
            Sync = new object();
            Peers = _peers.AsReadOnly();
            Instance = this;
            _loopScheduler.Schedule(_loop);
            _loopScheduler.Start();
        }

        public void Join(CommPeer peer)
        {
            if (peer == null)
            {
                throw new ArgumentNullException("peer");
            }
            peer.Room = this;
            CommPeer commPeer;
            lock (Sync)
            {
                commPeer = Find(peer.Actor.Cmid);
            }
            if (commPeer != null)
            {
                Leave(commPeer);
            }
            lock (Sync)
            {
                _peers.Add(peer);
            }
            Log.Debug($"CommPeer joined the room {peer.Actor.Cmid}");
            peer.Handlers.Add(this);
            peer.Events.SendLobbyEntered();
            CheckMute(peer);
            UpdateList();
        }

        public string Kick(int cmid)
        {
            CommPeer commPeer = Find(cmid);
            if (commPeer == null)
            {
                return "Cant find cmid";
            }
            commPeer.SendError("You have been kicked from game");
            return "``User has been kicked from the game.``";
        }

        public static void CheckMute(CommPeer peer, bool nottick = true)
        {
            
        }

        public void Leave(CommPeer peer)
        {
            if (peer == null)
            {
                throw new ArgumentNullException("peer");
            }
            lock (Sync)
            {
                _ = _peers.Remove(peer);
            }
            Log.Debug($"CommPeer left the room {peer.Actor.Cmid}");
            _ = peer.Handlers.Remove(Id);
            UpdateList();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _loopScheduler.Dispose();
                _disposed = true;
            }
        }


        private void OnTick()
        {
            _failedPeers.Clear();
            lock (Sync)
            {
                foreach (CommPeer peer in Peers)
                {
                    if (peer.HasError)
                    {
                        peer.Disconnect();
                        break;
                    }
                    try
                    {
                        peer.Tick();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to update peer.", ex);
                        _failedPeers.Add(peer);
                    }
                }
            }
            foreach (CommPeer failedPeer in _failedPeers)
            {
                Leave(failedPeer);
            }
        }

        private void OnTickError(Exception ex)
        {
            Log.Error("Failed to tick.", ex);
        }

        /* Update all peer's player list in the lobby. */
        private void UpdateList()
        {
            lock (Sync)
            {
                List<CommActorInfoView> list = new List<CommActorInfoView>(_peers.Count);
                for (int i = 0; i < _peers.Count; i++)
                {
                    list.Add(_peers[i].Actor.View);
                }
                for (int j = 0; j < _peers.Count; j++)
                {
                    _peers[j].Events.Lobby.SendFullPlayerListUpdate(list);
                }
            }
        }

        public string ListPlayers()
        {
            lock (Sync)
            {
                if (_peers.Count < 1)
                {
                    return "``No currently active players``";
                }
                List<string> list = new List<string>
                {
                    "``Currently active players : " + _peers.Count + "``",
                    "```"
                };
                foreach (CommPeer peer in _peers)
                {
                    if (!string.IsNullOrEmpty(peer.Actor.View.ClanTag))
                    {
                        list.Add("[" + peer.Actor.View.Cmid + "] {" + peer.Actor.View.ClanTag + "} " + peer.Actor.Name);
                    }
                    else
                    {
                        list.Add("[" + peer.Actor.View.Cmid + "] " + peer.Actor.Name);
                    }
                }
                list.Add("```");
                return string.Join(Environment.NewLine, list);
            }
        }

        private CommPeer Find(int cmid)
        {
            lock (Sync)
            {
                for (int i = 0; i < _peers.Count; i++)
                {
                    CommPeer commPeer = _peers[i];
                    if (commPeer.Actor.Cmid == cmid)
                    {
                        return commPeer;
                    }
                }
            }
            return null;
        }

        private static bool IsSpeedHacking(List<float> td)
        {
            float num = 0f;
            for (int i = 0; i < td.Count; i++)
            {
                num += td[i];
            }
            num /= td.Count;
            if (num > 2f)
            {
                return true;
            }
            float num2 = 0f;
            for (int j = 0; j < td.Count; j++)
            {
                num2 += (float)Math.Pow(td[j] - num, 2.0);
            }
            num2 /= td.Count - 1;
            return num > 1.1f && num2 <= 0.05f;
        }

        public string SendBanMessage(int cmid)
        {
            CommPeer commPeer = Find(cmid);
            if (commPeer == null)
            {
                return "In-game status:Peer offline.";
            }
            commPeer.SendError("You have been banned!");
            return "In-game status:Successfully kicked from game.";
        }

        public string Modules(int cmid)
        {
            CommPeer commPeer = Find(cmid);
            if (commPeer == null)
            {
                return "Peer is null";
            }
            HashSet<string> hashSet = new HashSet<string>(commPeer.Actor.Modules);
            return hashSet.Count < 1 ? "Dataset is null" : string.Join(Environment.NewLine, hashSet);
        }

        public string Processes(int cmid)
        {
            CommPeer commPeer = Find(cmid);
            if (commPeer == null)
            {
                return "Peer is null";
            }
            HashSet<string> hashSet = new HashSet<string>(commPeer.Actor.Processes);
            return hashSet.Count < 1 ? "Dataset is null" : string.Join(Environment.NewLine, hashSet);
        }

        public string Windows(int cmid)
        {
            CommPeer commPeer = Find(cmid);
            if (commPeer == null)
            {
                return "Peer is null";
            }
            HashSet<string> hashSet = new HashSet<string>(commPeer.Actor.Windows);
            return hashSet.Count < 1 ? "Dataset is null" : string.Join(Environment.NewLine, hashSet);
        }

        public string DiscordProcess(int cmid)
        {
            List<string> list = new List<string>(Find(cmid).Actor.Processes);
            return list.Count >= 1 ? string.Join(Environment.NewLine, list) : null;
        }

        public string DiscordWindows(int cmid)
        {
            List<string> list = new List<string>(Find(cmid).Actor.Windows);
            return list.Count >= 1 ? string.Join(Environment.NewLine, list) : null;
        }

        public string DiscordModules(int cmid)
        {
            List<string> list = new List<string>(Find(cmid).Actor.Modules);
            return list.Count >= 1 ? string.Join(Environment.NewLine, list) : null;
        }

        private void FetchWindows(CommPeer peer, int cmid)
        {
            List<string> list = new List<string>(Find(cmid).Actor.Windows);
            if (list.Count < 1)
            {
                peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat", "Error getting List. Data Length is 0");
                return;
            }
            int num = 0;
            foreach (string item in list)
            {
                num++;
                peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat: GUI Windows of CMID " + cmid, $"[{num}] Window Name: {item} ");
            }
        }

        private void FindAlts(CommPeer peer, int cmid)
        {
           
        }

        private void GetHWID(CommPeer peer, int cmid)
        {

            CommPeer commPeer = Find(cmid);
            if (commPeer != null)
            {
                List<string> list = new List<string>(Find(cmid).Actor.HDD);
                foreach (string item in list)
                {
                    peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat Server", $"HDD {item}");
                }
                List<string> list2 = new List<string>(Find(cmid).Actor.MOTHERBOARD);
                foreach (string item in list2)
                {
                    peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat Server", $"Motherboard {item}");
                }
                List<string> list3 = new List<string>(Find(cmid).Actor.MAC);
                foreach (string item in list3)
                {
                    peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat Server", $"MAC {item}");
                }
                List<string> list4 = new List<string>(Find(cmid).Actor.UNITY);
                foreach (string item in list4)
                {
                    peer.Events.Lobby.SendLobbyChatMessage(0, "UberBeat Server", $"Unity {item}");
                }
            }
            
        }

        private void FindAccounts(CommPeer peer, string query)
        {
            
        }

        public string Message(int cmid, string message)
        {
            CommPeer commPeer = Find(cmid);
            if (commPeer == null)
            {
                return "Cant find cmid";
            }
            commPeer.Events.Lobby.SendModerationCustomMessage(message);
            return "``Message has been sent to target user``";
        }
        public string Message(string message)
        {
            lock (Sync)
            {
                foreach (CommPeer peer in _peers)
                {
                    peer.Events.Lobby.SendModerationCustomMessage(message);
                }
            }
            return "``Message has been sent to all online users``";
        }


    }
}
