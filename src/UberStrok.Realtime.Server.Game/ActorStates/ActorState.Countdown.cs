using UberStrok.Realtime.Server.Game;
namespace UberStrok.Realtime.Server.Game
{
    public sealed class CountdownActorState : ActorState
    {
        public CountdownActorState(GameActor actor)
            : base(actor)
        {
        }

        public override void OnEnter()
        {
            base.Actor.Info.Health = 100;
            base.Actor.Info.ArmorPoints = base.Actor.Info.ArmorPointCapacity;
            if (!base.Room.IsTeamElimination)
            {
                base.Actor.Statistics.Reset(hard: true);
                base.Actor.Info.Kills = 0;
                base.Actor.Info.Deaths = 0;
            }
            base.Peer.Events.Game.SendPrepareNextRound();
            base.Peer.Events.Game.SendUpdateRoundScore(base.Room.RoundNumber, (short)base.Room.BlueTeamScore, (short)base.Room.RedTeamScore);
            base.Peer.Events.Game.SendKillsRemaining(0, 0);
            base.Room.Spawn(base.Actor);
        }
    }
}