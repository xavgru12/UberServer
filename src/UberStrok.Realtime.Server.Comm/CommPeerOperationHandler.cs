using log4net;
using System;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Comm
{
    public class CommPeerOperationHandler : BaseCommPeerOperationHandler
    {
        private static new readonly ILog Log = LogManager.GetLogger("CommPeerOperationHandler");

        public override void OnAuthenticationRequest(CommPeer peer, string authToken, string magicHash, bool isMac)
        {
            try
            {
                peer.IsMac = isMac;
                if (!peer.Authenticate(authToken, magicHash))
                {
                    peer.SendError();
                }
            }
            catch (Exception ex)
            {
                peer.Disconnect();
                Log.Error("Failed to authenticate user.", ex);
                throw;
            }
            CommApplication.Instance.Rooms.Global.Join(peer);
        }

        public override void OnSendHeartbeatResponse(CommPeer peer, string authToken, string responseHash)
        {
            try
            {
                if (!peer.HeartbeatCheck(responseHash))
                {
                    peer.SendError();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception while checking heartbeat", ex);
                peer.SendError();
            }
        }

        public override void OnSetLoadout(CommPeer peer, string authToken, LoadoutView view)
        {
            peer.Events.SendLoadoutUpdateResult(peer.SetLoadout(authToken, view));
        }
    }

}
