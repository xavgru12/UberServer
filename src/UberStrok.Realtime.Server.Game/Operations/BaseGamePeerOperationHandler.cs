using System;
using System.IO;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.Realtime;
using UberStrok.Realtime.Server;
using UberStrok.Realtime.Server.Game;

public abstract class BaseGamePeerOperationHandler : OperationHandler<GamePeer>
{
    public sealed override byte Id => 1;

    protected abstract void OnUpdateKeyState(GamePeer peer, byte state);

    protected abstract void OnGetGameListUpdates(GamePeer peer);

    protected abstract void OnGetServerLoad(GamePeer peer);

    protected abstract void OnCreateRoom(GamePeer peer, GameRoomDataView roomData, string password, string clientVersion, string authToken, string magicHash, bool isMac);

    protected abstract void OnJoinRoom(GamePeer peer, int roomId, string password, string clientVersion, string authToken, string magicHash, bool isMac);

    protected abstract void OnLeaveRoom(GamePeer peer);

    protected abstract void OnUpdatePing(GamePeer peer, ushort ping);

    protected abstract void OnUpdateLoadout(GamePeer peer);

    protected abstract void OnSendHeartbeatResponse(GamePeer peer, string authToken, string responseHash);

    public override void OnOperationRequest(GamePeer peer, byte opCode, MemoryStream bytes)
    {
        switch ((IGamePeerOperationsType)opCode)
        {
            case IGamePeerOperationsType.SendHeartbeatResponse:
                SendHeartbeatResponse(peer, bytes);
                break;
            case IGamePeerOperationsType.UpdateKeyState:
                UpdateKeyState(peer, bytes);
                break;
            case IGamePeerOperationsType.GetGameListUpdates:
                GetGameListUpdates(peer, bytes);
                break;
            case IGamePeerOperationsType.GetServerLoad:
                GetServerLoad(peer, bytes);
                break;
            case IGamePeerOperationsType.CreateRoom:
                CreateRoom(peer, bytes);
                break;
            case IGamePeerOperationsType.EnterRoom:
                EnterRoom(peer, bytes);
                break;
            case IGamePeerOperationsType.LeaveRoom:
                LeaveRoom(peer, bytes);
                break;
            case IGamePeerOperationsType.UpdatePing:
                UpdatePing(peer, bytes);
                break;
            case IGamePeerOperationsType.UpdateLoadout:
                UpdateLoadout(peer, bytes);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private void SendHeartbeatResponse(GamePeer peer, MemoryStream bytes)
    {
        string authToken = StringProxy.Deserialize(bytes);
        string responseHash = StringProxy.Deserialize(bytes);
        OnSendHeartbeatResponse(peer, authToken, responseHash);
    }

    private void UpdateKeyState(GamePeer peer, MemoryStream bytes)
    {
        byte state = ByteProxy.Deserialize(bytes);
        OnUpdateKeyState(peer, state);
    }

    private void GetGameListUpdates(GamePeer peer, MemoryStream bytes)
    {
        OnGetGameListUpdates(peer);
    }

    private void GetServerLoad(GamePeer peer, MemoryStream bytes)
    {
        OnGetServerLoad(peer);
    }

    private void CreateRoom(GamePeer peer, MemoryStream bytes)
    {
        GameRoomDataView roomData = GameRoomDataViewProxy.Deserialize(bytes);
        string password = StringProxy.Deserialize(bytes);
        string clientVersion = StringProxy.Deserialize(bytes);
        string authToken = StringProxy.Deserialize(bytes);
        string magicHash = StringProxy.Deserialize(bytes);
        bool isMac = BooleanProxy.Deserialize(bytes);
        OnCreateRoom(peer, roomData, password, clientVersion, authToken, magicHash, isMac);
    }

    private void EnterRoom(GamePeer peer, MemoryStream bytes)
    {
        int roomId = Int32Proxy.Deserialize(bytes);
        string password = StringProxy.Deserialize(bytes);
        string clientVersion = StringProxy.Deserialize(bytes);
        string authToken = StringProxy.Deserialize(bytes);
        string magicHash = StringProxy.Deserialize(bytes);
        bool isMac = BooleanProxy.Deserialize(bytes);
        OnJoinRoom(peer, roomId, password, clientVersion, authToken, magicHash, isMac);
    }

    private void LeaveRoom(GamePeer peer, MemoryStream bytes)
    {
        OnLeaveRoom(peer);
    }

    private void UpdatePing(GamePeer peer, MemoryStream bytes)
    {
        ushort ping = UInt16Proxy.Deserialize(bytes);
        OnUpdatePing(peer, ping);
    }

    private void UpdateLoadout(GamePeer peer, MemoryStream bytes)
    {
        OnUpdateLoadout(peer);
    }
}