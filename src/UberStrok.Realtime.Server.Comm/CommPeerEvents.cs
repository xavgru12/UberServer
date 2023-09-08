using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeerEvents : EventSender
    {
        public LobbyRoomEvents Lobby { get; }

        public CommPeerEvents(Peer peer)
            : base(peer)
        {
            Lobby = new LobbyRoomEvents(peer);
        }

        public void SendDisconnectAndDisablePhoton(string message)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);
                _ = SendEvent(4, bytes);
            }
        }

        public void SendHeartbeatChallenge(string challengeHash)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, challengeHash);
                _ = SendEvent(1, bytes);
            }
        }

        public void SendLoadData(ServerConnectionView serverConnection)
        {

            using (MemoryStream memoryStream = new MemoryStream())
            {
                ServerConnectionViewProxy.Serialize(memoryStream, serverConnection);
                _ = SendEvent(2, memoryStream);
            }
        }

        public void SendLobbyEntered()
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                _ = SendEvent(3, bytes);
            }
        }

        public void SendLoadoutUpdateResult(MemberOperationResult result)
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                EnumProxy<MemberOperationResult>.Serialize(bytes, result);
                _ = SendEvent((byte)ICommPeerEventsType.SetLoadoutResult, bytes);
            }
        }
    }
}
