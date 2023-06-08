using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomEvents : EventSender
    {
        public LobbyRoomEvents(Peer peer)
            : base(peer)
        {
        }

        public void SendModulesRequest(string untrusted)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, untrusted);
                _ = SendEvent(24, bytes);
            }
        }

        public void SendPrivateChatMessage(int cmid, string name, string message)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);
                _ = SendEvent(14, bytes);
            }
        }

        public void SendPlayerUpdate(CommActorInfoView commActor)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            using (MemoryStream memoryStream = new MemoryStream())
            {
                CommActorInfoViewProxy.Serialize(memoryStream, commActor);
                _ = SendEvent(7, memoryStream);
            }
        }

        public void SendClanChatMessage(int cmid, string name, string message)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);
                _ = SendEvent(11, bytes);
            }
        }

        public void SendFullPlayerListUpdate(List<CommActorInfoView> actors)
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                ListProxy<CommActorInfoView>.Serialize(bytes, actors, CommActorInfoViewProxy.Serialize);
                _ = SendEvent(9, bytes);
            }
        }

        public void SendLobbyChatMessage(int cmid, string name, string message)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);
                _ = SendEvent(13, bytes);
            }
        }

        public void SendModerationCustomMessage(string message)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);
                _ = SendEvent(21, bytes);
            }
        }

        public void SendModerationMutePlayer(bool isPlayerMuted)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                BooleanProxy.Serialize(bytes, isPlayerMuted);
                _ = SendEvent(22, bytes);
            }
        }

        public void SendModerationKickGame()
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                _ = SendEvent(23, bytes);
            }
        }

        public void SendUpdateFriendsList()
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                _ = SendEvent(16, bytes);
            }
        }

        public void SendUpdateClanMembers()
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                _ = SendEvent(18, bytes);
            }
        }

        public void SendUpdateInboxRequests()
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                _ = SendEvent(15, bytes);
            }
        }

        public void SendUpdateInboxMessages(int messageId)
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, messageId);
                _ = SendEvent(17, bytes);
            }
        }

        public void SendUpdateClanData()
        {

            using (MemoryStream bytes = new MemoryStream())
            {
                _ = SendEvent(19, bytes);
            }
        }
    }
}
