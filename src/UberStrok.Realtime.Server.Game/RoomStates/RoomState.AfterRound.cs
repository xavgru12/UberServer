using System;
using UberStrok.Core;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class AfterRoundState : RoomState
    {
        private readonly Countdown _restartCountdown;
        private TeamID _winner;

        public AfterRoundState(GameRoom room) : base(room)
        {
            _restartCountdown = new Countdown(Room.Loop, 3, 0);
            _restartCountdown.Completed += OnRestartCountdownCompleted;
        }

        public override void OnEnter()
        {
            _winner = GetWinner();

            foreach (var otherActor in Room.Players)
            {
                otherActor.Peer.Events.Game.SendTeamWins(_winner);
                otherActor.State.Set(ActorState.Id.AfterRound);
            }

            _restartCountdown.Restart();
        }

        public override void OnTick()
        {
            _restartCountdown.Tick();
        }


        private void OnRestartCountdownCompleted()
        {
            if (Room.IsTeamElimination)
            {
                if (this.Room.GetView().KillLimit - Math.Max(this.Room.BlueTeamScore, this.Room.RedTeamScore) == 0)
                {
                    Room.State.Set(Id.End);
                }
                else
                {
                    Room.RoundNumber++;
                    Room.State.Set(Id.Countdown);
                }
            }
            else
            {
                Room.RoundNumber++;
                Room.State.Set(Id.End);
            }
        }

        private TeamID GetWinner()
        {
            if (Room.IsTeamElimination)
            {
                if (this.Room.GetView().KillLimit - Math.Max(this.Room.BlueTeamScore, this.Room.RedTeamScore) == 0)
                {
                    if (this.Room.BlueTeamScore > this.Room.RedTeamScore)
                    {
                        Room.Winner = TeamID.BLUE;
                    }
                    else if (this.Room.RedTeamScore > this.Room.BlueTeamScore)
                    {
                        Room.Winner = TeamID.RED;
                    }
                    else
                    {
                        Room.Winner = TeamID.NONE;
                    }

                    return Room.Winner;
                }
                else
                {
                    return ((TeamEliminationGameRoom)Room).RoundWinner;
                }
            }

            return Room.Winner;
        }
    }
}
