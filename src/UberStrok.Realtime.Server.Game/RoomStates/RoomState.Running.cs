using System;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class RunningRoomState : RoomState
    {
        public RunningRoomState(GameRoom room)
            : base(room)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;

            Room.StartTime = Environment.TickCount;
            Room.EndTime = Environment.TickCount + Room.GetView().TimeLimit * 1000;

            if (Room.IsTeamElimination)
            {
                ((TeamEliminationGameRoom)Room).StartRound();
            }

            foreach (var player in Room.Players)
                player.State.Set(ActorState.Id.Playing);

            foreach (var player in Room.Actors)
            {
                if (player.State.Current == ActorState.Id.Spectator)
                {
                    player.Peer.Events.Game.SendMatchStart(Room.RoundNumber, Room.EndTime);
                }
            }
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        public override void OnTick()
        {
            /* Check if match has ended. */
            if (Environment.TickCount >= Room.EndTime)
            {
                if (Room.IsTeamElimination)
                {
                    ((TeamEliminationGameRoom)Room).EndRound();
                }
                else
                {
                    Room.State.Set(Id.End);
                }
            }
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            if (!Room.IsTeamElimination)
            {
                /* Sync the power ups. */
                e.Player.Peer.Events.Game.SendSetPowerUpState(Room.PowerUps.Respawning);
                e.Player.State.Set(ActorState.Id.Playing);
                Room.Spawn(e.Player);
            }
        }
    }
}
