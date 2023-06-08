using log4net;
using System;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;
using UberStrok.Realtime.Server.Game.Helper;

namespace UberStrok.Realtime.Server.Game
{
    public class GameActor
    {
        private StatisticsManager _statistics;

        protected ILog Log { get; }

        public int Cmid => Info.Cmid;

        public int PlayerId
        {
            get => Info.PlayerId;
            set
            {
                Info.PlayerId = (byte)value;
                Movement.PlayerId = (byte)value;
            }
        }

        public string PlayerName
        {
            get => Info.PlayerName;
            set => Info.PlayerName = value;
        }

        public string PlayerFullName => string.IsNullOrEmpty(Info.ClanTag) ? PlayerName : $"[{Info.ClanTag}] {PlayerName}";

        public float TimePlayed { get; set; }
        public float DurationPlayed => Math.Max(Room.Loop.Time - TimePlayed, 0);

        public StatisticsManager Statistics
        {
            get => _statistics;
            set
            {
                _statistics = value;
                Info.Kills = (short)(_statistics.Total.GetKills() - _statistics.Total.Suicides);
                Info.Deaths = (short)(_statistics.Total.Deaths + _statistics.Total.Suicides);
            }
        }

        public DamageEventView Damages { get; }
        public PlayerMovement Movement { get; }
        public GameActorInfo Info { get; }

        public ProjectileManager Projectiles { get; }
        public PingManager Ping { get; }

        public TrustManager Trust { get; }
        public LoadoutManager Loadout { get; }

        public GamePeer Peer { get; }
        public GameRoom Room { get; }
        public StateMachine<ActorState.Id> State { get; }

        public GameActor(GamePeer peer, GameRoom room)
        {
            Peer = peer ?? throw new ArgumentNullException(nameof(peer));
            Room = room ?? throw new ArgumentNullException(nameof(room));

            Log = LogManager.GetLogger(nameof(GameActor));

            Info = new GameActorInfo();
            Movement = new PlayerMovement();
            Damages = new DamageEventView();

            Ping = new PingManager();
            Projectiles = new ProjectileManager();
            Statistics = new StatisticsManager();

            Trust = new TrustManager();
            Loadout = new LoadoutManager();

            State = new StateMachine<ActorState.Id>();
            State.Register(ActorState.Id.None, null);
            State.Register(ActorState.Id.Overview, new OverviewActorState(this));
            State.Register(ActorState.Id.WaitingForPlayers, new WaitingForPlayersActorState(this));
            State.Register(ActorState.Id.Countdown, new CountdownActorState(this));
            State.Register(ActorState.Id.Playing, new PlayingActorState(this));
            State.Register(ActorState.Id.Killed, new KilledActorState(this));
            State.Register(ActorState.Id.End, new EndActorState(this));
            State.Register(ActorState.Id.AfterRound, new PlayerAfterRoundState(this));
            State.Register(ActorState.Id.Spectator, new PlayerSpectatingState(this));
            Reset();
        }

        public void Tick()
        {
            Peer.Tick();
            State.Tick();
            Trust.Tick();
        }

        public void Reset()
        {
            var userView = Peer.GetUser(retrieve: false);
            var loadoutView = Peer.GetLoadout(retrieve: false);
            var profileView = userView.CmuneMemberView.PublicProfile;
            var statsView = userView.UberstrikeMemberView.PlayerStatisticsView;

            Loadout.Update(Room.Shop, loadoutView);

            PlayerId = 0;

            Info.PlayerName = profileView.Name;
            Info.PlayerState = PlayerStates.None;
            Info.TeamID = TeamID.NONE;
            Info.Channel = ChannelType.Steam;
            Info.Level = statsView.Level == 0 ? 1 : statsView.Level;
            Info.Cmid = profileView.Cmid;
            Info.ClanTag = profileView.GroupTag;
            Info.AccessLevel = profileView.AccessLevel;

            Info.Gear = Loadout.Gear.GetAsList();
            Info.Weapons = Loadout.Weapons.GetAsList();
            Info.QuickItems = Loadout.QuickItems.GetAsList();

            Info.Health = 100;
            Info.ArmorPointCapacity = Loadout.Gear.GetArmorCapacity();
            Info.ArmorPoints = Info.ArmorPointCapacity;

            Info.Kills = 0;
            Info.Deaths = 0;

            /* Ignore these changes. */
            Info.GetViewDelta().Reset();

            State.Reset();
            Loadout.Reset();

            Statistics.Reset(hard: true);

            Log.Info($"{GetDebug()} has been reset with armor capacity {Info.ArmorPointCapacity}.");
        }

        public string GetDebug()
        {
            return $"(actor <{Cmid}> \"{PlayerName}\":{PlayerId} state {State.Current} in {Room.GetDebug()})";
        }

        public void EndMatch(bool hasWon)
        {
            Peer.Events.Game.SendTeamWins(Room.Winner);
            Peer.SendEndGame(Statistics.Total, Statistics.Best);
        }

        private int CalculateXp(bool hasWonMatch)
        {
            int num = 0;
            if (Room.Players.Contains(this) && TimePlayed != 0f)
            {
                num = (int)Math.Ceiling(DurationPlayed / 1000f);
            }
            if (Statistics.Total.GetDamageDealt() > 0)
            {
                int num2;
                int num3;
                if (hasWonMatch)
                {
                    num2 = XpPointsUtil.Config.XpBaseWinner;
                    num3 = XpPointsUtil.Config.XpPerMinuteWinner;
                }
                else
                {
                    num2 = XpPointsUtil.Config.XpBaseLoser;
                    num3 = XpPointsUtil.Config.XpPerMinuteLoser;
                }
                int num4 = MathUtils.Max(0, Statistics.Total.GetKills()) * XpPointsUtil.Config.XpKill + MathUtils.Max(0, Statistics.Total.Nutshots) * XpPointsUtil.Config.XpNutshot + MathUtils.Max(0, Statistics.Total.Headshots) * XpPointsUtil.Config.XpHeadshot + MathUtils.Max(0, Statistics.Total.MeleeKills) * XpPointsUtil.Config.XpSmackdown;
                int num5 = MathUtils.CeilToInt((float)(num / 60 * num3));
                int num6 = MathUtils.CeilToInt((float)(num / 60 * num3) * CalculateBoost((ItemPropertyType)1));
                return num2 + num4 + num5 + num6;
            }
            return 0;
        }

        private int CalculatePoints(bool hasWonMatch)
        {
            int num = 0;
            if (Room.Players.Contains(this) && TimePlayed != 0f)
            {
                num = (int)Math.Ceiling(DurationPlayed / 1000f);
            }
            int num2;
            int num3;
            if (hasWonMatch)
            {
                num2 = XpPointsUtil.Config.PointsBaseWinner;
                num3 = XpPointsUtil.Config.PointsPerMinuteWinner;
            }
            else
            {
                num2 = XpPointsUtil.Config.PointsBaseLoser;
                num3 = XpPointsUtil.Config.PointsPerMinuteLoser;
            }
            int num4 = MathUtils.Max(0, Statistics.Total.GetKills()) * XpPointsUtil.Config.PointsKill + MathUtils.Max(0, Statistics.Total.Nutshots) * XpPointsUtil.Config.PointsNutshot + MathUtils.Max(0, Statistics.Total.Headshots) * XpPointsUtil.Config.PointsHeadshot + MathUtils.Max(0, Statistics.Total.MeleeKills) * XpPointsUtil.Config.PointsSmackdown;
            int num5 = MathUtils.CeilToInt((float)(num / 60 * num3));
            int num6 = MathUtils.CeilToInt((float)(num / 60 * num3) * CalculateBoost((ItemPropertyType)2));
            return num2 + num4 + num5 + num6;
        }

        private float CalculateBoost(ItemPropertyType propType)
        {
            return 0f;
        }
    }
}
