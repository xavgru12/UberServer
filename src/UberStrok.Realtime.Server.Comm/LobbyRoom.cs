using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Views;

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
            _loopScheduler = new LoopScheduler(5);
            _peers = new List<CommPeer>();
            _failedPeers = new List<CommPeer>();

            BufferedMessage = new BufferedAdministratorMessage();
            Sync = new object();
            Peers = _peers.AsReadOnly();
            Instance = this;
            _loopScheduler.Schedule(_loop);
            _loopScheduler.Start();
        }

        private void OnTeardown()
        {
            foreach (var peer in _peers)
            {
                peer.SendError("Uberstrike servers are being restarted, kindly restart the game");
                peer.Disconnect();
                peer.Dispose();
            }
        }

        public void Join(CommPeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room == null, "CommPeer is joining room, but its already in another room.");

            peer.Room = this;

            CommPeer oldPeer;
            lock (Sync)
                oldPeer = Find(peer.Actor.Cmid);

            if (oldPeer != null)
                Leave(oldPeer);

            lock (Sync)
                _peers.Add(peer);

            Log.Debug($"CommPeer joined the room {peer.Actor.Cmid}");

            peer.Handlers.Add(this);
            peer.Events.SendLobbyEntered();

            if (BufferedMessage.HasBufferedMessage(peer.Actor.Cmid))
            {
                string buffered = BufferedMessage.GetBufferedMessages(peer.Actor.Cmid);

                if (buffered != null)
                {
                    peer.Events.Lobby.SendModerationCustomMessage(buffered);
                }

                BufferedMessage.DeleteBufferedMessage(peer.Actor.Cmid);
            }
            UpdateList("Peer joined");
        }

        public string UpdateMute(int cmid, int muteDuration)
        {
            var peer = Find(cmid);
            if (peer == null)
                return "In-game status: Peer offline.";
            if (muteDuration > 0)
            {
                var message = "Your account has been temporarily muted!" + Environment.NewLine + "Duration left: " + (muteDuration / 60) + " hours " + (muteDuration % 60) + " minutes.";
                peer.Events.Lobby.SendModerationCustomMessage(message);
                peer.Actor.MuteEndTime = DateTime.UtcNow.AddMinutes(muteDuration);
            }
            else if (muteDuration == -1)
            {
                var message = "Your account has been muted permanently!";
                peer.Events.Lobby.SendModerationCustomMessage(message);
                peer.Actor.MuteEndTime = DateTime.MaxValue;
            }
            peer.Actor.IsMuted = true;
            peer.Events.Lobby.SendModerationMutePlayer(true);
            return "In-game status. Peer muted";
        }
        public string RemoveMute(int cmid)
        {
            var peer = Find(cmid);
            if (peer == null)
                return "In-game status: Peer offline.";
            peer.Actor.MuteEndTime = DateTime.UtcNow;
            peer.Actor.IsMuted = false;
            peer.Events.Lobby.SendModerationMutePlayer(false);
            return "In-game status: Peer unmuted.";
        }
        public string Kick(int cmid, string message)
        {
            if (message == null)
            {
                message = "You have been disconnected from the server";
            }
            var peer = Find(cmid);
            if (peer == null)
                return "Cant find cmid";
            peer.SendError(message);
            return "``User has been kicked from the game.``";
        }

        public string KickAll(string message)
        {
            foreach (var peer in _peers)
            {
                peer.SendError(message);
            }
            return "``Everyone has been kicked.``";
        }

        public string KickGame(int cmid)
        {
            var peer = Find(cmid);
            if (peer == null)
                return "Cant find cmid";
            peer.Events.Lobby.SendModerationKickGame();
            return "``User has been kicked from the game.``";
        }
        public void Leave(CommPeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            lock (Sync)
                _peers.Remove(peer);

            Log.Debug($"CommPeer left the room {peer.Actor.Cmid}");

            peer.Handlers.Remove(Id);
            UpdateList("Peer Leave");
            /* TODO: Tell the web servers to close the user's session or something. */
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            OnTeardown();
            _loopScheduler.Dispose();
            _disposed = true;
        }

        private void OnTick()
        {
            _failedPeers.Clear();
            bool changed = false;
            lock (Sync)
            {
                foreach (var peer in Peers)
                {
                    if (peer.HasError)
                    {
                        peer.Disconnect();
                        changed = true;
                        _failedPeers.Add(peer);
                    }
                    else
                    {
                        try
                        {
                            peer.Tick();
                        }
                        catch (Exception ex)
                        {
                            /* NOTE: This should never happen, but just incase. */
                            Log.Error("Failed to update peer.", ex);
                            _failedPeers.Add(peer);
                        }
                    }
                }
            }
            if (_failedPeers.Count > 0)
            {
                changed = true;
            }
            foreach (var peer in _failedPeers)
                Leave(peer);
            if (changed)
            {
                Log.Debug("Peers disconnect detected");
            }
        }

        private void OnTickError(Exception ex)
        {
            Log.Error("Failed to tick.", ex);
        }

        /* Update all peer's player list in the lobby. */
        private void UpdateList(string updateReason)
        {
            Log.Debug("Sending update room list because of " + updateReason);
            lock (Sync)
            {
                var actors = new List<CommActorInfoView>(_peers.Count);
                for (int i = 0; i < _peers.Count; i++)
                    actors.Add(_peers[i].Actor.View);

                for (int i = 0; i < _peers.Count; i++)
                    _peers[i].Events.Lobby.SendFullPlayerListUpdate(actors);
            }
        }

        private CommPeer Find(int cmid)
        {
            lock (Sync)
            {
                for (int i = 0; i < _peers.Count; i++)
                {
                    var peer = _peers[i];
                    if (peer.Actor.Cmid == cmid)
                        return peer;
                }
            }

            return null;
        }

        private static bool IsSpeedHacking(List<float> td)
        {
            float mean = 0;
            for (int i = 0; i < td.Count; i++)
                mean += td[i];

            mean /= td.Count;
            if (mean > 2f)
                return true;

            float variance = 0;
            for (int i = 0; i < td.Count; i++)
                variance += (float)Math.Pow(td[i] - mean, 2);

            variance /= td.Count - 1;
            return mean > 1.1f && variance <= 0.05f;
        }
    }
}
