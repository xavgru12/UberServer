namespace UberStrok.Realtime.Server.Game
{
    public sealed class WaitingForPlayersRoomState : RoomState
    {
        public WaitingForPlayersRoomState(GameRoom room)
            : base(room)
        {
            /* Space */
        }

        public override void OnEnter()
        {
            Room.PlayerJoined += OnPlayerJoined;
        }

        public override void OnExit()
        {
            Room.PlayerJoined -= OnPlayerJoined;
        }

        public override void OnTick()
        {
            /* 
             * If we got more than 1 player we start the countdown;
             */
            if (Room.CanStart())
                Room.State.Set(Id.Countdown);
        }

        private void OnPlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            /* 
             * Set the player in the `waiting for players` state.
             */
            e.Player.State.Set(ActorState.Id.WaitingForPlayers);
        }
    }
}
