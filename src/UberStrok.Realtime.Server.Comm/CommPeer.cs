using Photon.SocketServer;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeer : Peer
    {
        public LobbyRoom Room { get; set; }

        public CommActor Actor { get; set; }

        public CommPeerEvents Events { get; }

        public CommPeer(InitRequest request)
            : base(request)
        {
            Events = new CommPeerEvents(this);
            base.Handlers.Add(new CommPeerOperationHandler());
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
            base.Tick();
            LobbyRoom.CheckMute(Actor.Peer, false);
        }

        protected override void OnAuthenticate(UberstrikeUserView userView)
        {
            CommActorInfoView view = new CommActorInfoView
            {
                AccessLevel = userView.CmuneMemberView.PublicProfile.AccessLevel,
                Channel = (ChannelType)(base.IsMac ? 7 : 4),
                Cmid = userView.CmuneMemberView.PublicProfile.Cmid,
                PlayerName = userView.CmuneMemberView.PublicProfile.Name,
                ClanTag = userView.CmuneMemberView.PublicProfile.GroupTag
            };
            Actor = new CommActor(this, view);
        }
    }

}
