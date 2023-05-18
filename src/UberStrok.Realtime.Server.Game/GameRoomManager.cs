using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class GameRoomManager : IDisposable, IEnumerable<GameRoom>
    {
        private int _roomId;
        private bool _disposed;

        private readonly object _sync;
        private readonly Dictionary<int, GameRoom> _rooms;

        private readonly List<int> _removedRooms;
        private readonly List<GameRoomDataView> _updatedRooms;
        private readonly BalancingLoopScheduler _loopScheduler;

        protected ILog Log { get; }
        public int Count => _rooms.Count;

        public GameRoomManager()
        {
            _loopScheduler = new BalancingLoopScheduler(64);

            _rooms = new Dictionary<int, GameRoom>();

            _sync = new object();
            _updatedRooms = new List<GameRoomDataView>();
            _removedRooms = new List<int>();

            Log = LogManager.GetLogger(GetType().Name);
        }

        public GameRoom Get(int roomId)
        {
            lock (_sync)
            {
                if (!_rooms.TryGetValue(roomId, out GameRoom room))
                    return null;
                return room;
            }
        }

        public GameRoom Create(GameRoomDataView data, string password)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            /* Set those to 0, so the client knows there is no level restriction. */
            if (data.LevelMin == 1 && data.LevelMax == 80)
            {
                data.LevelMin = 0;
                data.LevelMax = 0;
            }

            GameRoom room = null;
            try
            {
                switch (data.GameMode)
                {
                    case GameModeType.DeathMatch:
                        room = new DeathMatchGameRoom(data, _loopScheduler);
                        break;
                    case GameModeType.TeamDeathMatch:
                        room = new TeamDeathMatchGameRoom(data, _loopScheduler);
                        break;
                    case GameModeType.EliminationMode:
                        room = new TeamEliminationGameRoom(data, _loopScheduler);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            catch
            {
                room?.Dispose();
                throw;
            }

            lock (_sync)
            {
                room.RoomId = _roomId++;
                room.Password = password;

                _rooms.Add(room.RoomId, room);
                _updatedRooms.Add(room.GetView());

                Log.Info($"Created {room.GetDebug()}");
            }

            return room;
        }

        public void Tick()
        {
            lock (_sync)
            {
                foreach (var kv in _rooms)
                {
                    var room = kv.Value;
                    var view = room.GetView();

                    if (room.EmptyTickTime > 10000 / 15)
                    {
                        Log.Info("Removing empty room " + room.RoomId);
                        _removedRooms.Add(room.RoomId);
                    }
                    else if (room.Updated)
                    {
                        _updatedRooms.Add(room.GetView());
                        room.Updated = false;
                    }
                }
                if (_removedRooms.Count > 0 || _updatedRooms.Count > 0)
                {
                    foreach (var peer in GameApplication.Instance.Lobby.Peers)
                        peer.Events.SendGameListUpdate(_updatedRooms, _removedRooms);
                }


                foreach (var roomId in _removedRooms)
                {
                    if (_rooms.TryGetValue(roomId, out GameRoom room))
                    {
                        _rooms.Remove(roomId);
                        Log.Info($"Removed {room.GetDebug()}");
                        room.Dispose();
                    }
                }

                _updatedRooms.Clear();
                _removedRooms.Clear();
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _loopScheduler.Dispose();
            _disposed = true;
        }

        public IEnumerator<GameRoom> GetEnumerator()
        {
            lock (_sync)
            {
                foreach (var kv in _rooms)
                {
                    yield return kv.Value;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
