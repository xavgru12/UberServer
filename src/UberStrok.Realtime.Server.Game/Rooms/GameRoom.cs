using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Uberstrok.Core.Common;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract partial class GameRoom : IRoom<GamePeer>, IDisposable
    {
        private bool _disposed;

        private byte _nextPlayer;
        private string _password;

        private ushort _frame;
        private readonly Timer _frameTimer;
        //A time that players had retrain the same for long time
        private readonly GameRoomDataView _view;
        public int EmptyTickTime = 0;
        public int LastTickTime;
        /* 
         * Dictionary mapping player CMIDs to StatisticsManager instances.
         * This is used for when a player leaves and joins the game again; so
         * as to retain his stats.
         */
        private readonly Dictionary<int, StatisticsManager> _stats;
        /* List of cached player stats for end game. */
        private List<StatsSummaryView> _mvps;

        /* List of actor info delta. */
        private readonly List<GameActorInfoDeltaView> _actorDeltas;
        /* List of actor movement. */
        private readonly List<PlayerMovement> _actorMovements;

        private readonly List<GameActor> _actors;
        private readonly List<GameActor> _players;

        protected ILog ReportLog { get; }

        public Loop Loop { get; }
        public ILoopScheduler Scheduler { get; }

        public ICollection<GameActor> Players
        {
            get
            {
                return _players;
            }
        }

        public ICollection<GameActor> Actors
        {
            get
            {
                return _actors;
            }
        }

        public StateMachine<RoomState.Id> State { get; }

        public ShopManager Shop { get; }
        public SpawnManager Spawns { get; }
        public PowerUpManager PowerUps { get; }

        public bool Updated { get; set; }
        public bool IsTeamElimination => _view.GameMode == GameModeType.EliminationMode;
        public int RoundNumber { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public TeamID Winner { get; set; }
        public int BlueTeamScore { get; set; }
        public int RedTeamScore { get; set; }
        /* 
         * Room ID but we call it number since we already defined Id &
         * thats how UberStrike calls it too. 
         */
        public int RoomId
        {
            get => _view.RoomId;
            set => _view.RoomId = value;
        }

        public string Password
        {
            get => _password;
            set
            {
                /* 
                 * If the password is null or empty it means its not
                 * password protected. 
                 */
                _view.IsPasswordProtected = !string.IsNullOrEmpty(value);
                _password = _view.IsPasswordProtected ? value : null;
            }
        }

        public GameRoom(GameRoomDataView data, ILoopScheduler scheduler)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            this._view = data;
            if (scheduler == null)
            {
                throw new ArgumentNullException("scheduler");
            }
            this.Scheduler = scheduler;
            this.ReportLog = LogManager.GetLogger("Report");
            this._stats = new Dictionary<int, StatisticsManager>();
            int capacity = data.PlayerLimit / 2;
            this._players = new List<GameActor>(capacity);
            this._actors = new List<GameActor>(capacity);
            this._actorDeltas = new List<GameActorInfoDeltaView>(capacity);
            this._actorMovements = new List<PlayerMovement>(capacity);
            this.Loop = new Loop(new Action(this.OnTick), new Action<Exception>(this.OnTickError));
            this.Shop = new ShopManager();
            this.Spawns = new SpawnManager();
            this.PowerUps = new PowerUpManager(this);
            this.State = new StateMachine<RoomState.Id>();
            this.State.Register(RoomState.Id.None, null);
            this.State.Register(RoomState.Id.WaitingForPlayers, new WaitingForPlayersRoomState(this));
            this.State.Register(RoomState.Id.Countdown, new CountdownRoomState(this));
            this.State.Register(RoomState.Id.Running, new RunningRoomState(this));
            this.State.Register(RoomState.Id.End, new EndRoomState(this));
            this.State.Register(RoomState.Id.AfterRound, new AfterRoundState(this));
            this._frameTimer = new Timer(this.Loop, 105.263161f);
            this.Reset();
            this.Scheduler.Schedule(this.Loop);
        }

        public void Join(GamePeer peer)
        {
            if (peer == null)
            {
                throw new ArgumentNullException("peer");
            }
            if (peer.Actor != null)
            {
                throw new InvalidOperationException("Peer already in another room");
            }

            Enqueue(delegate
            {
                DoJoin(peer);
            });
        }

        public void Leave(GamePeer peer)
        {
            if (peer == null)
            {
                throw new ArgumentNullException("peer");
            }
            if (peer.Actor == null)
            {
                throw new InvalidOperationException("Peer is not in a room");
            }
            if (peer.Actor.Room != this)
            {
                throw new InvalidOperationException("Peer is not leaving the correct room");
            }
            Enqueue(delegate
            {
                DoLeave(peer);
            });
        }

        public void Spawn(GameActor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }
            Enqueue(delegate
            {
                DoSpawn(actor);
            });
        }

        private struct Achievement
        {
            public Dictionary<AchievementType, Tuple<StatsSummaryView, ushort>> All;
            public AchievementType Type;
            public int Value;

            public Achievement(AchievementType type, Dictionary<AchievementType, Tuple<StatsSummaryView, ushort>> all)
            {
                All = all;
                Type = type;
                Value = int.MinValue;
            }

            public void Check(StatsSummaryView summary, int value)
            {
                if (Value == value)
                {
                    All.Remove(Type);
                }
                else if (value > Value)
                {
                    Value = value;

                    if (Value > 0)
                        All[Type] = new Tuple<StatsSummaryView, ushort>(summary, (ushort)value);
                }
            }
        }

        public List<StatsSummaryView> GetMvps(bool force = false)
        {
            if (_mvps == null || force)
            {
                _mvps = new List<StatsSummaryView>();

                var achievements = new Dictionary<AchievementType, Tuple<StatsSummaryView, ushort>>();
                var mostValuable = new Achievement(AchievementType.MostValuable, achievements);
                var mostAggressive = new Achievement(AchievementType.MostAggressive, achievements);
                var costEffective = new Achievement(AchievementType.CostEffective, achievements);
                var hardestHitter = new Achievement(AchievementType.HardestHitter, achievements);
                var sharpestShooter = new Achievement(AchievementType.SharpestShooter, achievements);
                var triggerHappy = new Achievement(AchievementType.TriggerHappy, achievements);

                foreach (var player in Players)
                {
                    var summary = new StatsSummaryView
                    {
                        Cmid = player.Cmid,
                        Name = player.PlayerFullName,
                        Kills = player.Info.Kills,
                        Deaths = player.Info.Deaths,
                        Level = player.Info.Level,
                        Team = player.Info.TeamID,
                        Achievements = new Dictionary<byte, ushort>()
                    };

                    _mvps.Add(summary);

                    int kills = player.Statistics.Total.GetKills();
                    int deaths = player.Statistics.Total.Deaths;
                    int kdr;

                    if (kills == deaths)
                        kdr = 10;
                    else
                        kdr = (int)Math.Floor((float)kills / Math.Max(1, deaths)) * 10;

                    int shots = player.Statistics.Total.GetShots();
                    int hits = player.Statistics.Total.GetHits();

                    int accuracy = (int)Math.Floor((float)hits / Math.Max(1, shots) * 1000f);
                    int damageDealt = player.Statistics.Total.GetDamageDealt();
                    int criticalHits = player.Statistics.Total.Headshots + player.Statistics.Total.Nutshots;
                    int consecutiveKills = player.Statistics.MostConsecutiveKills;

                    mostValuable.Check(summary, kdr);
                    mostAggressive.Check(summary, kills);
                    costEffective.Check(summary, accuracy);
                    hardestHitter.Check(summary, damageDealt);
                    sharpestShooter.Check(summary, criticalHits);
                    triggerHappy.Check(summary, consecutiveKills);
                }

                foreach (var kv in achievements)
                {
                    var tuple = kv.Value;
                    var achievement = kv.Key;

                    tuple.Item1.Achievements.Add((byte)achievement, tuple.Item2);
                }

                _mvps = _mvps.OrderByDescending(x => x.Kills).ToList();
            }

            return _mvps;
        }

        public virtual void Reset()
        {
            this._frame = 6;
            this._frameTimer.Restart();
            this._nextPlayer = 0;
            foreach (GameActor player in this.Players)
            {
                foreach (GameActor otherActor in this.Actors)
                {
                    otherActor.Peer.Events.Game.SendPlayerLeftGame(player.Cmid);
                    int rtt;
                    int rttVar;
                    int numFailures;
                    otherActor.Peer.GetStats(out rtt, out rttVar, out numFailures);
                    base.Log.Info(string.Format("{0} RTT: {1} var<RTT>: {2} NumFailures: {3}", new object[]
                    {
                        otherActor.GetDebug(),
                        rtt,
                        rttVar,
                        numFailures
                    }));
                }
            }
            this._mvps = null;
            this._stats.Clear();
            this._players.Clear();
            this.PowerUps.Reset();
            this.State.Reset();
            this.State.Set(RoomState.Id.WaitingForPlayers);
            base.Log.Info(this.GetDebug() + " has been reset.");
        }

        private void OnTick()
        {
            bool updateMovements = this._frameTimer.Tick();
            if (updateMovements)
            {
                this._frame += 1;
            }
            this.State.Tick();
            this.PowerUps.Tick();
            foreach (GameActor actor in this.Actors)
            {
                if (actor.Peer.HasError)
                {
                    actor.Peer.Disconnect();
                }
                else
                {
                    try
                    {
                        actor.Tick();
                    }
                    catch (Exception ex)
                    {
                        base.Log.Error("Failed to tick " + actor.GetDebug() + ".", ex);
                        actor.Peer.Disconnect();
                        continue;
                    }
                    if (this.Players.Contains(actor) || actor.State.Current == ActorState.Id.Spectator)
                    {
                        GameActorInfoDeltaView delta = actor.Info.GetViewDelta();
                        if (delta.Changes.Count > 0)
                        {
                            delta.Update();
                            this._actorDeltas.Add(delta);
                        }
                        if (actor.Damages.Count > 0)
                        {
                            actor.Peer.Events.Game.SendDamageEvent(actor.Damages);
                            actor.Damages.Clear();
                        }
                        if (updateMovements && actor.Info.IsAlive)
                        {
                            this._actorMovements.Add(actor.Movement);
                        }
                    }
                }
            }
            if (this._actorDeltas.Count > 0)
            {
                foreach (GameActor gameActor in this.Actors)
                {
                    gameActor.Peer.Events.Game.SendAllPlayerDeltas(this._actorDeltas);
                }
                foreach (GameActorInfoDeltaView gameActorInfoDeltaView in this._actorDeltas)
                {
                    gameActorInfoDeltaView.Reset();
                }
                this._actorDeltas.Clear();
            }
            if (this._actorMovements.Count > 0 && updateMovements)
            {
                foreach (GameActor gameActor2 in this.Actors)
                {
                    gameActor2.Peer.Events.Game.SendAllPlayerPositions(this._actorMovements, this._frame);
                }
                this._actorMovements.Clear();
            }
        }

        private void OnTickError(Exception ex)
        {
            Log.Error("Failed to tick game loop.", ex);
        }

        /* This is executed on the game room loop thread. */
        private void DoJoin(GamePeer peer, bool reJoin = false)
        {
            try
            {
                peer.Handlers.Add(this);
                GameRoomDataView view = this.GetView();
                GameActor actor = new GameActor(peer, this);
                if (view.IsFull && actor.Info.AccessLevel < MemberAccessLevel.QA)
                {
                    peer.Events.SendRoomEnterFailed(null, 0, "The game is full.");
                }
                else
                {
                    peer.Events.SendRoomEntered(view, false);
                    peer.Actor = actor;
                    peer.Actor.State.Set(ActorState.Id.Overview);
                    this._actors.Add(peer.Actor);
                    base.Log.Info(peer.Actor.GetDebug() + " joined.");
                }
            }
            catch (Exception ex)
            {
                peer.Actor = null;
                peer.Handlers.Remove(this.Id);
                peer.Events.SendRoomEnterFailed(null, 0, "Failed to join room.");
                base.Log.Error("Failed to join " + this.GetDebug() + ".", ex);
            }
        }

        private void DoLeave(GamePeer peer)
        {
            if (peer == null)
            {
                Log.Error("Peer was null when tried to leave the room");
                return;
            }
            bool donotResetActor = false;
            if (peer.Actor != null && peer.Actor.Room != this)
            {
                Log.Error("Peer tried to leave a room, but doesnt belong to this room");
                donotResetActor = true;
            }
            var actor = peer.Actor;
            try
            {
                if(actor != null)
                {
                    if (_actors.Remove(actor))
                    {
                        if (_players.Contains(actor))
                        {
                            OnPlayerLeft(new PlayerLeftEventArgs
                            {
                                Player = actor
                            });
                        }
                        base.Log.Info((object)(actor.GetDebug() + " left."));
                    }
                    else
                    {
                        base.Log.Warn((object)(actor.GetDebug() + " tried to leave but was not in the list of Actors."));
                    }
                }
            }
            finally
            {
                /* Clean up. */
                if (!donotResetActor)
                {
                    peer.Actor = null;
                    _ = peer.Handlers.Remove(Id);
                }
                if (peer.OnLeaveRoom != null)
                {
                    peer.OnLeaveRoom();
                    peer.OnLeaveRoom = null;
                }
            }
        }

        private void DoSpawn(GameActor actor)
        {
            SpawnPoint spawn = this.Spawns.Get(actor.Info.TeamID);
            PlayerMovement movement = actor.Movement;
            movement.Position = spawn.Position;
            movement.HorizontalRotation = spawn.Rotation;
            foreach (GameActor gameActor in this.Actors)
            {
                gameActor.Peer.Events.Game.SendPlayerRespawned(actor.Cmid, spawn.Position, spawn.Rotation);
            }
            base.Log.Debug(string.Format("{0} spawned at {1}.", actor.GetDebug(), spawn));
        }

        /* Determine if state of the room can be switched to RunningRoomState. */
        public abstract bool CanStart();

        /*
         * Determines if the actor can join the specified team.
         * 
         * It would seem there is a client side bug where the client uses
         * GameRoomData.ConnectedPlayers to determined if the room is full or
         * not, however it never updates ConntectedPlayers once it has created
         * the BaseGameRoom instance, so this results in HUDJoinButtons being
         * broken.
         */
        public abstract bool CanJoin(GameActor actor, TeamID team);

        /* Determines if the vicitim can get damaged by the attcker. */
        public abstract bool CanDamage(GameActor victim, GameActor attacker);

        /* Does damage and returns true if victim is killed; otherwise false. */
        protected bool DoDamage(GameActor victim, GameActor attacker, Weapon weapon, short damage, BodyPart part, out Vector3 direction)
        {
            bool selfDamage = victim.Cmid == attacker.Cmid;

            /* Calculate the direction of the hit. */
            var victimPos = victim.Movement.Position;
            var attackerPos = attacker.Movement.Position;
            direction = attackerPos - victimPos;
            
            /* Chill time, game has ended; we don't do damage. */
            if (State.Current == RoomState.Id.End)
                return false;

            /* We can't kill someone who's already dead. */
            if (!victim.Info.IsAlive)
                return false;

            /* Check if we can apply the damage on the players. */
            if (!CanDamage(victim, attacker))
                return false;

            float angle = Vector3.Angle(direction, new Vector3(0, 0, -1));
            if (direction.x < 0)
                angle = 360 - angle;

            byte byteAngle = Conversions.Angle2Byte(angle);

            /* Check if not self-damage. */
            if (!selfDamage)
                victim.Damages.Add(byteAngle, damage, part, 0, 0);
            else
                damage /= 2;

            /* Calculate armor absorption. */
            int armorDamage;
            int healthDamage;
            if (victim.Info.ArmorPoints > 0)
            {
                armorDamage = (byte)(victim.Info.GetAbsorptionRate() * damage);
                healthDamage = (short)(damage - armorDamage);
            }
            else
            {
                armorDamage = 0;
                healthDamage = damage;
            }

            int newArmor = victim.Info.ArmorPoints - armorDamage;
            int newHealth = victim.Info.Health - healthDamage;

            if (newArmor < 0)
                newHealth += newArmor;

            victim.Info.ArmorPoints = (byte)Math.Max(0, newArmor);
            victim.Info.Health = (short)Math.Max(0, newHealth);

            /* Record some statistics. */
            if (!selfDamage)
            {
                victim.Statistics.RecordDamageReceived(damage);
                attacker.Statistics.RecordHit(weapon.GetView().ItemClass);
                attacker.Statistics.RecordDamageDealt(weapon.GetView().ItemClass, damage);
            }

            /* Check if the player is dead. */
            if (victim.Info.Health <= 0)
            {
                
                if (victim.Damages.Count > 0)
                {
                    /* 
                     * Force a push of damage events to the victim peer, so he
                     * gets the feedback of where he was hit from aka red hit
                     * marker HUD.
                     */
                    victim.Peer.Events.Game.SendDamageEvent(victim.Damages);
                    victim.Peer.Flush();

                    victim.Damages.Clear();
                }

                if (selfDamage)
                    attacker.Info.Kills--;
                else
                    attacker.Info.Kills++;

                victim.Info.Deaths++;

                /* Record statistics. */
                victim.Statistics.RecordDeath();

                if (selfDamage)
                {
                    attacker.Statistics.RecordSuicide();
                }
                else
                {
                    if (part == BodyPart.Head)
                        attacker.Statistics.RecordHeadshot();
                    else if (part == BodyPart.Nuts)
                        attacker.Statistics.RecordNutshot();

                    attacker.Statistics.RecordKill(weapon.GetView().ItemClass);
                }

                return true;
            }

            return false;
        }

        public virtual void EndMatch()
        {
            foreach (GameActor player in Players)
            {
                player.EndMatch(Winner == player.Info.TeamID);
            }
        }

        public string GetDebug()
        {
            return $"(room \"{GetView().Name}\":{RoomId} {GetView().ConnectedPlayers}/{GetView().PlayerLimit} state {State.Current})";
        }

        public GameRoomDataView GetView()
        {
            if (_view.ConnectedPlayers != _players.Count)
            {
                _view.ConnectedPlayers = _players.Count;
                Updated = true;
            }

            return _view;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Scheduler.Unschedule(Loop);

                /* Best effort clean up. */
                foreach (var player in Players)
                {
                    foreach (var otherActor in Actors)
                        otherActor.Peer.Events.Game.SendPlayerLeftGame(player.Cmid);
                }

                /* Clean up actors. */
                foreach (var actor in Actors)
                {
                    var peer = actor.Peer;
                    peer.Actor = null;
                    peer.Handlers.Remove(Id);

                    peer.Disconnect();
                    peer.Dispose();
                }

                /* Clear to lose refs to GameActor objects. */
                _actors.Clear();
                _players.Clear();
            }

            _disposed = true;
        }
    }
}
