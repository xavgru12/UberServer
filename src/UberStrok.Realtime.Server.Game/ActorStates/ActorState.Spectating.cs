using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class PlayerSpectatingState : ActorState
    {
        public PlayerSpectatingState(GameActor actor) : base(actor)
        {
        }

        public override void OnEnter()
        {
            Actor.Info.PlayerState |= PlayerStates.Spectator;

            if (Room.State.Current == RoomState.Id.Running)
            {
                Peer.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);
                Peer.Events.Game.SendUpdateRoundScore(Room.RoundNumber, (short)Room.BlueTeamScore, (short)Room.RedTeamScore);
            }

            Peer.Events.Game.SendJoinedAsSpectator();
        }

        public override void OnExit()
        {
            Actor.Info.PlayerState &= ~PlayerStates.Spectator;
        }
    }
}
