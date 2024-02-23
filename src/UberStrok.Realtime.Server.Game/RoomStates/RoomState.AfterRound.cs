using System;
using UberStrok.Core;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class AfterRoundState : RoomState
    {
        private readonly Countdown _restartCountdown;

        private TeamID _winner;

        public AfterRoundState(GameRoom room)
            : base(room)
        {
            _restartCountdown = new Countdown(base.Room.Loop, 3, 0);
            _restartCountdown.Completed += OnRestartCountdownCompleted;
        }

        public override void OnEnter()
        {
            _winner = GetWinner();
            foreach (GameActor player in base.Room.Players)
            {
                player.Peer.Events.Game.SendTeamWins(_winner);
                player.State.Set(ActorState.Id.AfterRound);
            }
            _restartCountdown.Restart();
        }

        public override void OnTick()
        {
            _restartCountdown.Tick();
        }

        private void OnRestartCountdownCompleted()
        {
            int roundNumber;
            if (!base.Room.IsTeamElimination)
            {
                GameRoom room = base.Room;
                roundNumber = room.RoundNumber;
                room.RoundNumber = roundNumber + 1;
                base.Room.State.Set(RoomState.Id.End);
                return;
            }
            if (base.Room.GetView().KillLimit - Math.Max(base.Room.BlueTeamScore, base.Room.RedTeamScore) == 0)
            {
                base.Room.State.Set(RoomState.Id.End);
                return;
            }
            GameRoom room2 = base.Room;
            roundNumber = room2.RoundNumber;
            room2.RoundNumber = roundNumber + 1;
            base.Room.State.Set(RoomState.Id.Countdown);
        }

        private TeamID GetWinner()
        {
            if (!base.Room.IsTeamElimination)
            {
                return base.Room.Winner;
            }
            if (base.Room.GetView().KillLimit - Math.Max(base.Room.BlueTeamScore, base.Room.RedTeamScore) == 0)
            {
                if (base.Room.BlueTeamScore > base.Room.RedTeamScore)
                {
                    base.Room.Winner = TeamID.BLUE;
                }
                else if (base.Room.RedTeamScore > base.Room.BlueTeamScore)
                {
                    base.Room.Winner = TeamID.RED;
                }
                else
                {
                    base.Room.Winner = TeamID.NONE;
                }
                return base.Room.Winner;
            }
            return ((TeamEliminationGameRoom)base.Room).RoundWinner;
        }
    }

}
