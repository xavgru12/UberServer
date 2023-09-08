using System;
using System.Collections.Generic;
using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Common;
using UberStrok.Realtime;
using UberStrok.Realtime.Server;
using UberStrok.Realtime.Server.Game;

public abstract class BaseGameRoomOperationHandler : OperationHandler<GamePeer>
{
    public sealed override byte Id => 0;

    protected abstract void OnPowerUpPicked(GameActor actor, int pickupId, PickupItemType type, byte value);

    protected abstract void OnRemoveProjectile(GameActor actor, int projectileId, bool explode);

    protected abstract void OnEmitProjectile(GameActor actor, Vector3 origin, Vector3 direction, byte slot, int projectileId, bool explode);

    protected abstract void OnEmitQuickItem(GameActor actor, Vector3 origin, Vector3 direction, int itemId, byte playerNumber, int projectileId);

    protected abstract void OnRespawnRequest(GameActor actor);

    protected abstract void OnExplosionDamage(GameActor actor, int target, byte slot, byte distance, Vector3 force);

    protected abstract void OnDirectHitDamage(GameActor actor, int target, byte bodyPart, byte bullets, byte weaponSlot);

    protected abstract void OnDirectDamage(GameActor actor, ushort damage);

    protected abstract void OnDirectDeath(GameActor actor);

    protected abstract void OnSwitchWeapon(GameActor actor, byte slot);

    protected abstract void OnSwitchTeam(GameActor actor);

    protected abstract void OnSingleBulletFire(GameActor actor);

    protected abstract void OnIsPaused(GameActor actor, bool on);

    protected abstract void OnIsInSniperMode(GameActor actor, bool on);

    protected abstract void OnIsFiring(GameActor actor, bool on);

    protected abstract void OnJump(GameActor actor, Vector3 position);

    protected abstract void OnUpdatePositionAndRotation(GameActor actor, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState);

    protected abstract void OnChatMessage(GameActor actor, string message, byte context);

    protected abstract void OnPowerUpRespawnTimes(GameActor actor, List<ushort> respawnTimes);

    protected abstract void OnSpawnPositions(GameActor actor, TeamID team, List<Vector3> positions, List<byte> rotations);

    protected abstract void OnJoinTeam(GameActor actor, TeamID team);

    protected abstract void OnJoinAsSpectator(GameActor actor);

    protected abstract void OnOpenDoor(GameActor actor, int doorId);

    protected abstract void OnHitFeedback(GameActor actor, int targetCmid, Vector3 force);

    public override void OnOperationRequest(GamePeer peer, byte opCode, MemoryStream bytes)
    {
        switch ((IGameRoomOperationsType)opCode)
        {
            case IGameRoomOperationsType.PowerUpPicked:
                PowerUpPicked(peer, bytes);
                break;
            case IGameRoomOperationsType.SingleBulletFire:
                SingleBulletFire(peer, bytes);
                break;
            case IGameRoomOperationsType.RemoveProjectile:
                RemoveProjectile(peer, bytes);
                break;
            case IGameRoomOperationsType.EmitProjectile:
                EmitProjectile(peer, bytes);
                break;
            case IGameRoomOperationsType.EmitQuickItem:
                EmitQuickItem(peer, bytes);
                break;
            case IGameRoomOperationsType.RespawnRequest:
                RespawnRequest(peer, bytes);
                break;
            case IGameRoomOperationsType.ExplosionDamage:
                ExplosionDamage(peer, bytes);
                break;
            case IGameRoomOperationsType.DirectHitDamage:
                DirectHitDamage(peer, bytes);
                break;
            case IGameRoomOperationsType.DirectDamage:
                DirectDamage(peer, bytes);
                break;
            case IGameRoomOperationsType.DirectDeath:
                DirectDeath(peer, bytes);
                break;
            case IGameRoomOperationsType.SwitchWeapon:
                SwitchWeapon(peer, bytes);
                break;
            case IGameRoomOperationsType.IsPaused:
                IsPaused(peer, bytes);
                break;
            case IGameRoomOperationsType.IsInSniperMode:
                IsInSniperMode(peer, bytes);
                break;
            case IGameRoomOperationsType.IsFiring:
                IsFiring(peer, bytes);
                break;
            case IGameRoomOperationsType.Jump:
                Jump(peer, bytes);
                break;
            case IGameRoomOperationsType.UpdatePositionAndRotation:
                UpdatePositionAndRotation(peer, bytes);
                break;
            case IGameRoomOperationsType.ChatMessage:
                ChatMessage(peer, bytes);
                break;
            case IGameRoomOperationsType.PowerUpRespawnTimes:
                PowerUpRespawnTimes(peer, bytes);
                break;
            case IGameRoomOperationsType.SpawnPositions:
                SpawnPositions(peer, bytes);
                break;
            case IGameRoomOperationsType.JoinGame:
                JoinGame(peer, bytes);
                break;
            case IGameRoomOperationsType.JoinAsSpectator:
                JoinAsSpectator(peer, bytes);
                break;
            case IGameRoomOperationsType.OpenDoor:
                OpenDoor(peer, bytes);
                break;
            case IGameRoomOperationsType.SwitchTeam:
                SwitchTeam(peer);
                break;
            case IGameRoomOperationsType.HitFeedback:
                HitFeedback(peer, bytes);
                break;
            default:
                throw new NotSupportedException();
        }
    }

    private void PowerUpPicked(GamePeer peer, MemoryStream bytes)
    {
        int pickupId = Int32Proxy.Deserialize(bytes);
        byte type = ByteProxy.Deserialize(bytes);
        byte value = ByteProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnPowerUpPicked(peer.Actor, pickupId, (PickupItemType)type, value);
        });
    }

    private void IsInSniperMode(GamePeer peer, MemoryStream bytes)
    {
        bool on = BooleanProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnIsInSniperMode(peer.Actor, on);
        });
    }

    private void SingleBulletFire(GamePeer peer, MemoryStream _)
    {
        Enqueue(delegate
        {
            OnSingleBulletFire(peer.Actor);
        });
    }

    private void RemoveProjectile(GamePeer peer, MemoryStream bytes)
    {
        int projectileId = Int32Proxy.Deserialize(bytes);
        bool explode = BooleanProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnRemoveProjectile(peer.Actor, projectileId, explode);
        });
    }

    private void EmitProjectile(GamePeer peer, MemoryStream bytes)
    {
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0022: Unknown result type (might be due to invalid IL or missing references)
        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
        Vector3 origin = Vector3Proxy.Deserialize(bytes);
        Vector3 direction = Vector3Proxy.Deserialize(bytes);
        byte slot = ByteProxy.Deserialize(bytes);
        int projectileId = Int32Proxy.Deserialize(bytes);
        bool explode = BooleanProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            OnEmitProjectile(peer.Actor, origin, direction, slot, projectileId, explode);
        });
    }

    private void EmitQuickItem(GamePeer peer, MemoryStream bytes)
    {
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0022: Unknown result type (might be due to invalid IL or missing references)
        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
        Vector3 origin = Vector3Proxy.Deserialize(bytes);
        Vector3 direction = Vector3Proxy.Deserialize(bytes);
        int itemId = Int32Proxy.Deserialize(bytes);
        byte playerNumber = ByteProxy.Deserialize(bytes);
        int projectileId = Int32Proxy.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            OnEmitQuickItem(peer.Actor, origin, direction, itemId, playerNumber, projectileId);
        });
    }

    private void RespawnRequest(GamePeer peer, MemoryStream _)
    {
        Enqueue(delegate
        {
            OnRespawnRequest(peer.Actor);
        });
    }

    private void ExplosionDamage(GamePeer peer, MemoryStream bytes)
    {
        //IL_003a: Unknown result type (might be due to invalid IL or missing references)
        //IL_003f: Unknown result type (might be due to invalid IL or missing references)
        int target = Int32Proxy.Deserialize(bytes);
        byte slot = ByteProxy.Deserialize(bytes);
        byte distance = ByteProxy.Deserialize(bytes);
        Vector3 force = Vector3Proxy.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0024: Unknown result type (might be due to invalid IL or missing references)
            OnExplosionDamage(peer.Actor, target, slot, distance, force);
        });
    }

    private void DirectHitDamage(GamePeer peer, MemoryStream bytes)
    {
        int target = Int32Proxy.Deserialize(bytes);
        byte bodyPart = ByteProxy.Deserialize(bytes);
        byte bullets = ByteProxy.Deserialize(bytes);
        byte weaponSlot = ByteProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnDirectHitDamage(peer.Actor, target, bodyPart, bullets, weaponSlot);
        });
    }

    private void DirectDamage(GamePeer peer, MemoryStream bytes)
    {
        ushort damage = UInt16Proxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnDirectDamage(peer.Actor, damage);
        });
    }

    private void DirectDeath(GamePeer peer, MemoryStream _)
    {
        Enqueue(delegate
        {
            OnDirectDeath(peer.Actor);
        });
    }

    private void SwitchWeapon(GamePeer peer, MemoryStream bytes)
    {
        byte slot = ByteProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnSwitchWeapon(peer.Actor, slot);
        });
    }

    private void IsPaused(GamePeer peer, MemoryStream bytes)
    {
        bool on = BooleanProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnIsPaused(peer.Actor, on);
        });
    }

    private void IsFiring(GamePeer peer, MemoryStream bytes)
    {
        bool on = BooleanProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnIsFiring(peer.Actor, on);
        });
    }

    private void Jump(GamePeer peer, MemoryStream bytes)
    {
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        Vector3 position = Vector3Proxy.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            OnJump(peer.Actor, position);
        });
    }

    private void UpdatePositionAndRotation(GamePeer peer, MemoryStream bytes)
    {
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        //IL_0022: Unknown result type (might be due to invalid IL or missing references)
        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
        Vector3 position = ShortVector3Proxy.Deserialize(bytes);
        Vector3 velocity = ShortVector3Proxy.Deserialize(bytes);
        byte horizontalRotation = ByteProxy.Deserialize(bytes);
        byte verticalRotation = ByteProxy.Deserialize(bytes);
        byte moveState = ByteProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            OnUpdatePositionAndRotation(peer.Actor, position, velocity, horizontalRotation, verticalRotation, moveState);
        });
    }

    private void ChatMessage(GamePeer peer, MemoryStream bytes)
    {
        string message = StringProxy.Deserialize(bytes);
        byte context = ByteProxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnChatMessage(peer.Actor, message, context);
        });
    }

    private void PowerUpRespawnTimes(GamePeer peer, MemoryStream bytes)
    {
        List<ushort> respawnTimes = ListProxy<ushort>.Deserialize(bytes, UInt16Proxy.Deserialize);
        Enqueue(delegate
        {
            OnPowerUpRespawnTimes(peer.Actor, respawnTimes);
        });
    }

    private void SpawnPositions(GamePeer peer, MemoryStream bytes)
    {
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        TeamID team = EnumProxy<TeamID>.Deserialize(bytes);
        List<Vector3> positions = ListProxy<Vector3>.Deserialize(bytes, Vector3Proxy.Deserialize);
        List<byte> rotations = ListProxy<byte>.Deserialize(bytes, ByteProxy.Deserialize);
        Enqueue(delegate
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            OnSpawnPositions(peer.Actor, team, positions, rotations);
        });
    }

    private void JoinGame(GamePeer peer, MemoryStream bytes)
    {
        //IL_0016: Unknown result type (might be due to invalid IL or missing references)
        //IL_001b: Unknown result type (might be due to invalid IL or missing references)
        TeamID team = EnumProxy<TeamID>.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0012: Unknown result type (might be due to invalid IL or missing references)
            OnJoinTeam(peer.Actor, team);
        });
    }

    private void JoinAsSpectator(GamePeer peer, MemoryStream bytes)
    {
        Enqueue(delegate
        {
            OnJoinAsSpectator(peer.Actor);
        });
    }

    private void OpenDoor(GamePeer peer, MemoryStream bytes)
    {
        int doorId = Int32Proxy.Deserialize(bytes);
        Enqueue(delegate
        {
            OnOpenDoor(peer.Actor, doorId);
        });
    }

    private void SwitchTeam(GamePeer peer)
    {
        Enqueue(delegate
        {
            OnSwitchTeam(peer.Actor);
        });
    }

    private void HitFeedback(GamePeer peer, MemoryStream bytes)
    {
        //IL_0022: Unknown result type (might be due to invalid IL or missing references)
        //IL_0027: Unknown result type (might be due to invalid IL or missing references)
        int targetCmid = Int32Proxy.Deserialize(bytes);
        Vector3 force = Vector3Proxy.Deserialize(bytes);
        Enqueue(delegate
        {
            //IL_0018: Unknown result type (might be due to invalid IL or missing references)
            OnHitFeedback(peer.Actor, targetCmid, force);
        });
    }
}