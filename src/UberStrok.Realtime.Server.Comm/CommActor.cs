using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommActor
    {
        public HashSet<string> BIOS = new HashSet<string>();

        public HashSet<string> HDD = new HashSet<string>();

        public HashSet<string> MAC = new HashSet<string>();

        public HashSet<string> MOTHERBOARD = new HashSet<string>();

        public HashSet<string> UNITY = new HashSet<string>();

        public HashSet<string> Modules = new HashSet<string>();

        public HashSet<string> Processes = new HashSet<string>();

        public HashSet<string> Windows = new HashSet<string>();

        public HashSet<string> Exe = new HashSet<string>();

        public HashSet<string> TrustedModules = new HashSet<string>();

        public CommPeer Peer { get; }

        public CommActorInfoView View { get; }

        public bool IsMuted { get; set; }

        public DateTime MuteEndTime { get; set; }

        public int Cmid => View.Cmid;

        public string Name => View.PlayerName;

        public MemberAccessLevel AccessLevel => View.AccessLevel;

        public HashSet<int> ContactList { get; set; }

        public CommActor(CommPeer peer, CommActorInfoView view)
        {
            Peer = peer ?? throw new ArgumentNullException("peer");
            View = view ?? throw new ArgumentNullException("view");
        }
    }

}
