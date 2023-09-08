using System;
using System.Linq;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game;

public sealed class DeathMatchGameRoom : GameRoom
{
    public DeathMatchGameRoom(GameRoomDataView data, ILoopScheduler scheduler)
        : base(data, scheduler)
    {
        //IL_0009: Unknown result type (might be due to invalid IL or missing references)
        //IL_000f: Invalid comparison between Unknown and I4
        if (data.GameMode != GameModeType.DeathMatch)
        {
            throw new ArgumentException("GameRoomDataView is not in deathmatch mode", "data");
        }
    }

    public override bool CanJoin(GameActor actor, TeamID team)
    {
        return actor.Info.AccessLevel >= MemberAccessLevel.Moderator || (team == TeamID.NONE && !base.GetView().IsFull);
    }

    public override bool CanStart()
    {
        return base.Players.Count > 1;
    }

    public override bool CanDamage(GameActor victim, GameActor attacker)
    {
        return base.IsMatchRunning;
    }

    protected override void OnPlayerJoined(PlayerJoinedEventArgs e)
    {
        base.OnPlayerJoined(e);
        e.Player.Peer.Events.Game.SendKillsRemaining(GetKillsRemaining(), 0);
    }

    protected override void OnPlayerLeft(PlayerLeftEventArgs e)
    {
        base.OnPlayerLeft(e);
        if (!base.IsWaitingForPlayers && base.Players.Count <= 1)
        {
            base.State.Set(RoomState.Id.End);
        }
        else
        {
            if (base.State.Current != RoomState.Id.Running)
            {
                return;
            }
            int killsRemaining = GetKillsRemaining();
            foreach (GameActor actor in base.Actors)
            {
                actor.Peer.Events.Game.SendKillsRemaining(killsRemaining, 0);
            }
        }
    }

    protected override void OnPlayerKilled(PlayerKilledEventArgs args)
    {
        base.OnPlayerKilled(args);
        if (!base.IsWaitingForPlayers)
        {
            if (args.Attacker == args.Victim)
            {
                return;
            }
            int killsRemaining = GetKillsRemaining();
            foreach (GameActor player in base.Players)
            {
                player.Peer.Events.Game.SendKillsRemaining(killsRemaining, 0);
            }
            if (killsRemaining <= 0)
            {
                base.State.Set(RoomState.Id.AfterRound);
            }
        }
        else
        {
            OnRespawnRequest(args.Victim);
        }
    }

    private int GetKillsRemaining()
    {
        return GetView().KillLimit - ((base.Players.Count > 0) ? base.Players.Aggregate((GameActor a, GameActor b) => (a.Info.Kills <= b.Info.Kills) ? b : a).Info.Kills : 0);
    }

    protected override void OnSwitchTeam(GameActor actor)
    {
    }
}
