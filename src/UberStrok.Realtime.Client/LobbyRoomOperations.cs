using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Serialization;

namespace UberStrok.Realtime.Client
{
    public class LobbyRoomOperations
    {
        public LobbyRoomOperations(BasePeer peer, byte id)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            _id = id;
            _peer = peer;
        }

        private byte _id;
        private readonly BasePeer _peer;

        public void SendChatToAll(string message)
        {
            using (var bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, message);

                var parameter = new Dictionary<byte, object>
                {
                    {_id, bytes.ToArray() }
                };
                _peer._peer.OpCustom((byte)ILobbyRoomOperationsType.ChatMessageToAll, parameter, true);
            }
        }
        public void SendPrivateChatMessage(int cmid, string message)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, message);

                var parameter = new Dictionary<byte, object>
                {
                    {_id, bytes.ToArray() }
                };
                _peer._peer.OpCustom((byte)ILobbyRoomOperationsType.ChatMessageToPlayer, parameter, true);
            }
        }

        public void SendClanChatMessage(List<int> clanMembers, string message)
        {
            using (var bytes = new MemoryStream())
            {
                ListProxy<int>.Serialize(bytes, clanMembers, new ListProxy<int>.Serializer<int>(Int32Proxy.Serialize));
                StringProxy.Serialize(bytes, message);

                var parameter = new Dictionary<byte, object>
                {
                    {_id, bytes.ToArray() }
                };
                _peer._peer.OpCustom((byte)ILobbyRoomOperationsType.ChatMessageToClan, parameter, true);
            }
        }

        public void SendFullPlayerListUpdate()
        {
            using (var bytes = new MemoryStream())
            {
                var parameter = new Dictionary<byte, object>
                {
                    {_id, bytes.ToArray() }
                };
                _peer._peer.OpCustom((byte)ILobbyRoomOperationsType.FullPlayerListUpdate, parameter, true);
            }
        }
    }
}
