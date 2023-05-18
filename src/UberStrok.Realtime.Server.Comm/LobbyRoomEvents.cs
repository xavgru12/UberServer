using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class LobbyRoomEvents : EventSender
    {
        public LobbyRoomEvents(Peer peer) : base(peer)
        {
            // Space
        }

        public void SendPrivateChatMessage(int cmid, string name, string message)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);
                SendEvent((byte)ILobbyRoomEventsType.PrivateChatMessage, bytes);
            }
        }
        public void SendPlayerUpdate(CommActorInfoView commActor)
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                CommActorInfoViewProxy.Serialize(bytes, commActor);
                base.SendEvent(7, bytes);
            }
        }

        public void SendClanChatMessage(int cmid, string name, string message)
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);
                base.SendEvent(11, bytes);
            }
        }


        public void SendFullPlayerListUpdate(List<CommActorInfoView> actors)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<CommActorInfoView>.Serialize(bytes, actors, CommActorInfoViewProxy.Serialize);
                SendEvent((byte)ILobbyRoomEventsType.FullPlayerListUpdate, bytes);
            }
        }

        public void SendLobbyChatMessage(int cmid, string name, string message)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                StringProxy.Serialize(bytes, message);

                SendEvent((byte)ILobbyRoomEventsType.LobbyChatMessage, bytes);
            }
        }

        public void SendModerationCustomMessage(string message)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);

                SendEvent((byte)ILobbyRoomEventsType.ModerationCustomMessage, bytes);
            }
        }

        public void SendModerationMutePlayer(bool isPlayerMuted)
        {
            using (var bytes = new MemoryStream())
            {
                BooleanProxy.Serialize(bytes, isPlayerMuted);

                SendEvent((byte)ILobbyRoomEventsType.ModerationMutePlayer, bytes);
            }
        }

        public void SendModerationKickGame()
        {
            using (var bytes = new MemoryStream())
            {
                SendEvent((byte)ILobbyRoomEventsType.ModerationKickGame, bytes);
            }
        }

        public void SendUpdateFriendsList()
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                base.SendEvent((byte)ILobbyRoomEventsType.UpdateFriendsList, bytes);
            }
        }

        public void SendUpdateClanMembers()
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                base.SendEvent((byte)ILobbyRoomEventsType.UpdateClanMembers, bytes);
            }
        }

        public void SendUpdateInboxRequests()
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                base.SendEvent((byte)ILobbyRoomEventsType.UpdateInboxRequests, bytes);
            }
        }

        public void SendUpdateInboxMessages(int messageId)
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, messageId);
                base.SendEvent((byte)ILobbyRoomEventsType.UpdateInboxMessages, bytes);
            }
        }

        public void SendUpdateClanData()
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                base.SendEvent((byte)ILobbyRoomEventsType.UpdateClanData, bytes);
            }
        }
    }
}
