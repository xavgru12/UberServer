using System;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public sealed class TeamEliminationGameRoom : GameRoom
    {
        public bool FriendlyFire { get; set; }

        public int BlueTeamTotalPlayer { get; private set; }
        public int RedTeamTotalPlayer { get; private set; }

        public int BlueTeamAlivePlayer { get; private set; }
        public int RedTeamAlivePlayer { get; private set; }

        public TeamID RoundWinner { get; private set; }

        public TeamEliminationGameRoom(GameRoomDataView data, ILoopScheduler scheduler) : base(data, scheduler)
        {
            if (data.GameMode != GameModeType.EliminationMode)
                throw new ArgumentException("GameRoomDataView is not in team elimination mode", nameof(data));

            FriendlyFire = false;
        }

        public override bool CanStart()
        {
            return BlueTeamTotalPlayer > 0 && RedTeamTotalPlayer > 0;
        }

        public override void Reset()
        {
            base.Reset();

            BlueTeamTotalPlayer = 0;
            BlueTeamAlivePlayer = 0;
            BlueTeamScore = 0;

            RedTeamTotalPlayer = 0;
            RedTeamAlivePlayer = 0;
            RedTeamScore = 0;
        }

        protected sealed override void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            if (args.Team == TeamID.BLUE)
                BlueTeamTotalPlayer++;
            else if (args.Team == TeamID.RED)
                RedTeamTotalPlayer++;

            if (State.Current == RoomState.Id.Running)
            {
                args.Player.State.Set(ActorState.Id.Killed);
            }

            base.OnPlayerJoined(args);

            /* This is to reset the top scoreboard to not display "STARTS IN". */
            if (State.Current == RoomState.Id.Running)
            {
                args.Player.Peer.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
            }
        }

        protected sealed override void OnPlayerLeft(PlayerLeftEventArgs args)
        {
            if (args.Player.Info.TeamID == TeamID.BLUE)
            {
                BlueTeamTotalPlayer--;

                if (args.Player.State.Current == ActorState.Id.Playing)
                {
                    BlueTeamAlivePlayer--;
                }
            }
            else if (args.Player.Info.TeamID == TeamID.RED)
            {
                RedTeamTotalPlayer--;

                if (args.Player.State.Current == ActorState.Id.Playing)
                {
                    RedTeamAlivePlayer--;
                }
            }

            base.OnPlayerLeft(args);

            if (State.Current != RoomState.Id.WaitingForPlayers)
            {
                if (RedTeamTotalPlayer == 0 || BlueTeamTotalPlayer == 0)
                {
                    if (this.BlueTeamScore > this.RedTeamScore)
                    {
                        this.Winner = TeamID.BLUE;
                    }
                    else if (this.RedTeamScore > this.BlueTeamScore)
                    {
                        this.Winner = TeamID.RED;
                    }
                    else
                    {
                        this.Winner = TeamID.NONE;
                    }

                    State.Set(RoomState.Id.End);
                }
                else if (RedTeamAlivePlayer == 0 || BlueTeamAlivePlayer == 0)
                {
                    EndRound();
                }
            }
        }

        protected sealed override void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            base.OnPlayerKilled(args);
            Log.Debug(State.Current);
            if (State.Current == RoomState.Id.WaitingForPlayers)
            {
                OnRespawnRequest(args.Victim);
                return;
            }
            if (args.Victim.Info.TeamID == TeamID.BLUE)
                BlueTeamAlivePlayer--;
            else if (args.Victim.Info.TeamID == TeamID.RED)
                RedTeamAlivePlayer--;

            if (BlueTeamAlivePlayer == 0 || RedTeamAlivePlayer == 0)
            {
                EndRound();
            }
        }

        public sealed override bool CanJoin(GameActor actor, TeamID team)
        {
            /*
             * There is a client side bug in TeamEliminationGameRoom where the
             * client does not restore the RoomState to PregameLoadout in
             * OnJoinGameFailed, unlike in DeathMatchRoom where it properly
             * restores RoomState.
             */

            if (actor.Info.AccessLevel >= MemberAccessLevel.Moderator)
                return true;

            if (GetView().IsFull)
                return false;

            var diff = BlueTeamTotalPlayer - RedTeamTotalPlayer;
            if (team == TeamID.BLUE)
                return diff <= 0;
            else if (team == TeamID.RED)
                return diff >= 0;
            else
                return false;
        }

        public sealed override bool CanDamage(GameActor victim, GameActor attacker)
        {
            if (State.Current != RoomState.Id.Running)
                return false;

            if (FriendlyFire)
                return true;

            return victim == attacker || victim.Info.TeamID != attacker.Info.TeamID;
        }

        protected override void OnSwitchTeam(GameActor actor)
        {
            if (State.Current == RoomState.Id.Running)
            {
                if (this.BlueTeamTotalPlayer != this.RedTeamTotalPlayer)
                {
                    TeamID targetTeam = actor.Info.TeamID == TeamID.BLUE ? TeamID.RED : TeamID.BLUE;

                    if (targetTeam == TeamID.RED && this.BlueTeamTotalPlayer > this.RedTeamTotalPlayer)
                    {
                        this.BlueTeamTotalPlayer--;
                        this.BlueTeamAlivePlayer--;

                        this.RedTeamTotalPlayer++;
                    }
                    else if (targetTeam == TeamID.BLUE && this.RedTeamTotalPlayer > this.BlueTeamTotalPlayer)
                    {
                        this.RedTeamTotalPlayer--;
                        this.RedTeamAlivePlayer--;

                        this.BlueTeamTotalPlayer++;
                    }
                    else
                        return;

                    actor.Info.TeamID = targetTeam;

                    base.OnPlayerKilled(new PlayerKilledEventArgs
                    {
                        Attacker = actor,
                        Victim = actor,
                        ItemClass = UberStrikeItemClass.WeaponMelee,
                        Part = BodyPart.Nuts,
                    });

                    foreach (var player in Players)
                        player.Peer.Events.Game.SendPlayerChangedTeam(actor.Cmid, targetTeam);

                    if (BlueTeamAlivePlayer == 0 || RedTeamAlivePlayer == 0)
                    {
                        EndRound();
                    }
                }
            }
        }

        public void StartRound()
        {
            BlueTeamAlivePlayer = BlueTeamTotalPlayer;
            RedTeamAlivePlayer = RedTeamTotalPlayer;
            
            Winner = TeamID.NONE;
            RoundWinner = TeamID.NONE;
        }

        public void EndRound()
        {
            if (RedTeamAlivePlayer == 0)
            {
                BlueTeamScore++;
                RoundWinner = TeamID.BLUE;
            }
            else if (BlueTeamAlivePlayer == 0)
            {
                RedTeamScore++;
                RoundWinner = TeamID.RED;
            }
            else
            {
                RoundWinner = TeamID.NONE;
            }

            foreach (var actor in Actors)
            {
                actor.Peer.Events.Game.SendUpdateRoundScore(RoundNumber, (short)BlueTeamScore, (short)RedTeamScore);
            }

            State.Set(RoomState.Id.AfterRound);
        }
    }
}
