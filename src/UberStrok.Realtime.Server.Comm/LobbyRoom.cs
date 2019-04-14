﻿using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public partial class LobbyRoom : BaseLobbyRoomOperationHandler, IRoom<CommPeer>
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(LobbyRoom));

        private readonly List<CommPeer> _peers;

        public object Sync { get; }
        public IReadOnlyList<CommPeer> Peers { get; }

        public LobbyRoom()
        {
            _peers = new List<CommPeer>();

            Sync = new object();
            Peers = _peers.AsReadOnly();
        }

        public void Join(CommPeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room == null, "CommPeer is joining room, but its already in another room.");

            peer.Room = this;

            lock (Sync)
                _peers.Add(peer);

            Log.Debug($"CommPeer joined the room {peer.Actor.Cmid}");

            peer.Events.SendLobbyEntered();
            peer.AddOperationHandler(this);

            UpdateList();
        }

        public void Leave(CommPeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            lock (Sync)
                _peers.Remove(peer);

            Log.Debug($"CommPeer left the room {peer.Actor.Cmid}");

            UpdateList();
            /* TODO: Tell the web servers to close the user's session or something. */
        }

        /* Update all peer's player list in the lobby. */
        private void UpdateList()
        {
            lock (Sync)
            {
                var actors = new List<CommActorInfoView>(_peers.Count);
                for (int i = 0; i < _peers.Count; i++)
                    actors.Add(_peers[i].Actor.View);

                for (int i = 0; i < _peers.Count; i++)
                    _peers[i].Events.Lobby.SendFullPlayerListUpdate(actors);
            }
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
