using Photon.SocketServer;
using System;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : Peer
    {
        public LobbyRoom Room { get; set; }
        public CommActor Actor { get; set; }
        public CommPeerEvents Events { get; }

        public CommPeer(InitRequest request) : base(request)
        {
            Events = new CommPeerEvents(this);
            Handlers.Add(new CommPeerOperationHandler());
        }

        public override void SendHeartbeat(string hash)
        {
            Events.SendHeartbeatChallenge(hash);
        }

        public override void SendError(string message = "An error occured that forced UberStrike to halt.")
        {
            base.SendError(message);
            Events.SendDisconnectAndDisablePhoton(message);
        }

        public override void Tick()
        {
            try
            {
                base.Tick();
                if (Actor.IsMuted)
                {
                    if (Actor.MuteEndTime < DateTime.UtcNow)
                    {
                        Events.Lobby.SendModerationMutePlayer(false);
                        Actor.IsMuted = false;
                    }
                }
            }
            catch
            {
                HasError = true;
            }
        }

        protected override void OnAuthenticate(UberstrikeUserView userView)
        {
            var actorView = new CommActorInfoView
            {
                AccessLevel = userView.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = ChannelType.Steam,
                Cmid = userView.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = userView.CmuneMemberView.PublicProfile.Name,
                ClanTag = userView.CmuneMemberView.PublicProfile.GroupTag,
            };

            Actor = new CommActor(this, actorView);
        }

        protected override void OnDisconnect(global::PhotonHostRuntimeInterfaces.DisconnectReason reasonCode, string reasonDetail)
        {
            HasError = true;
        }
    }
}
