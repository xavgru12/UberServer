using Photon.SocketServer;
using System.Diagnostics;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommApplication : Application
    {
        public static new CommApplication Instance => (CommApplication)Application.Instance;

        public LobbyRoomManager Rooms { get; private set; }

        protected override void OnSetup()
        {
            Rooms = new LobbyRoomManager();
            _ = Debugger.Launch();
        }

        protected override void OnTearDown()
        {
        }

        protected override Peer OnCreatePeer(InitRequest initRequest)
        {
            return new CommPeer(initRequest);
        }
    }

}
