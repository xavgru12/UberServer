using UberStrok.Core;
using UberStrok.Realtime.Server.Game;

public sealed class CountdownRoomState : RoomState
{
    private readonly Countdown _countdown;

    public CountdownRoomState(GameRoom room)
        : base(room)
    {
        _countdown = new Countdown(base.Room.Loop, 5, 0);
        _countdown.Counted += OnCountdownCounted;
        _countdown.Completed += OnCountdownCompleted;
    }

    public override void OnEnter()
    {
        base.Room.PlayerJoined += OnPlayerJoined;
        foreach (GameActor player in base.Room.Players)
        {
            player.State.Set(ActorState.Id.Countdown);
        }
        base.Room.PowerUps.Reset();
        foreach (GameActor actor in base.Room.Actors)
        {
            actor.Peer.Events.Game.SendResetAllPowerUps();
        }
        _countdown.Restart();
    }

    public override void OnTick()
    {
        _countdown.Tick();
    }

    public override void OnExit()
    {
        base.Room.PlayerJoined -= OnPlayerJoined;
    }

    private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
    {
        e.Player.State.Set(ActorState.Id.Countdown);
    }

    private void OnCountdownCounted(int count)
    {
        foreach (GameActor actor in base.Room.Actors)
        {
            actor.Peer.Events.Game.SendMatchStartCountdown(count);
        }
    }

    private void OnCountdownCompleted()
    {
        base.Room.State.Set(Id.Running);
    }
}