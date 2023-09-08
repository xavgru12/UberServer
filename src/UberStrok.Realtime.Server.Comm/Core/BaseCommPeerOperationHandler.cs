using System;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{

    public abstract class BaseCommPeerOperationHandler : OperationHandler<CommPeer>
    {
        public override byte Id => 1;

        public abstract void OnAuthenticationRequest(CommPeer peer, string authToken, string magicHash, bool isMac);

        public abstract void OnSendHeartbeatResponse(CommPeer peer, string authToken, string responseHash);

        public abstract void OnSetLoadout(CommPeer peer, string authToken, LoadoutView view);

        public override void OnOperationRequest(CommPeer peer, byte opCode, MemoryStream bytes)
        {
            switch ((ICommPeerOperationsType)opCode)
            {
                case ICommPeerOperationsType.AuthenticationRequest:
                    AuthenticationRequest(peer, bytes);
                    break;
                case ICommPeerOperationsType.SendHeartbeatResponse:
                    SendHeartbeatResponse(peer, bytes);
                    break;
                case ICommPeerOperationsType.SetLoadoutRequest:
                    UpdateLoadout(peer, bytes);
                    break;
                default:
                    Log.Error("Get opcode as " + opCode + " which is invalid");
                    throw new NotSupportedException();
            }
        }

        private void AuthenticationRequest(CommPeer peer, MemoryStream bytes)
        {
            string authToken = StringProxy.Deserialize(bytes);
            string magicHash = StringProxy.Deserialize(bytes);
            bool isMac = BooleanProxy.Deserialize(bytes);
            OnAuthenticationRequest(peer, authToken, magicHash, isMac);
        }

        private void SendHeartbeatResponse(CommPeer peer, MemoryStream bytes)
        {
            string authToken = StringProxy.Deserialize(bytes);
            string responseHash = StringProxy.Deserialize(bytes);
            OnSendHeartbeatResponse(peer, authToken, responseHash);
        }

        private void UpdateLoadout(CommPeer peer, MemoryStream bytes)
        {
            string authToken = StringProxy.Deserialize(bytes);
            LoadoutView loadoutView = LoadoutViewProxy.Deserialize(bytes);

            OnSetLoadout(peer, authToken, loadoutView);
        }
    }

}
