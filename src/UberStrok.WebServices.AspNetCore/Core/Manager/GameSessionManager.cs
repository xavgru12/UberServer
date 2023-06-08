using log4net;
using System;
using System.Collections.Generic;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Session;
using UberStrok.WebServices.AspNetCore.Helper;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class GameSessionManager
    {
        private readonly ILog Log = LogManager.GetLogger(typeof(GameSessionManager));

        private readonly Dictionary<int, GameSession> m_sessions;

        private long m_seed;

        public int Count => m_sessions.Count;

        public HashSet<string> Authed
        {
            get;
            set;
        }

        private readonly ClanManager clanManager;
        public GameSessionManager(ClanManager clanManager)
        {
            Authed = new HashSet<string>();
            m_sessions = new Dictionary<int, GameSession>();
            m_seed = Utils.GetTimestamp() & 0xFFFFFFFFFFFFFF;
            this.clanManager = clanManager;
        }

        public GameSession CreateSession(UserDocument document, string ipAddress, string machineId)
        {
            if (m_sessions.ContainsKey(document.UserId))
            {
                _ = m_sessions.Remove(document.UserId);
                Log.Info($"Kicking player with CMID {document.UserId} cause of new login.");
            }
            string sessionId = CreateSessionId(document.UserId);
            GameSession gameSession = new GameSession(sessionId, document, clanManager)
            {
                IPAddress = ipAddress,
                MachineId = machineId
            };
            if (!m_sessions.TryAdd(document.UserId, gameSession))
            {
                Log.Error("GameSessionManager.m_sessions.TryAdd(id, GameSession) return false");
                return null;
            }
            return gameSession;
        }

        public bool TryGet(int id, out GameSession session)
        {
            return m_sessions.TryGetValue(id, out session);
        }

        public bool TryGet(string sessionId, out GameSession session)
        {
            return m_sessions.TryGetValue(Utils.GetUserId(sessionId), out session);
        }

        public GameSession Remove(string sessionId)
        {
            if (m_sessions.Remove(Utils.GetUserId(sessionId), out GameSession session))
            {
                _ = session.SessionId.Equals(sessionId);
            }
            return session;
        }

        internal string CreateSessionId(int userId)
        {
            byte[] sessionId = new byte[12];
            long seed = m_seed;
            m_seed = (m_seed + 1) & 0xFFFFFFFFFFFFFF;
            sessionId[0] = (byte)(userId >> 24);
            sessionId[1] = (byte)(userId >> 16);
            sessionId[2] = (byte)(userId >> 8);
            sessionId[3] = (byte)userId;
            sessionId[4] = (byte)(seed >> 56);
            sessionId[5] = (byte)(seed >> 48);
            sessionId[6] = (byte)(seed >> 40);
            sessionId[7] = (byte)(seed >> 32);
            sessionId[8] = (byte)(seed >> 24);
            sessionId[9] = (byte)(seed >> 16);
            sessionId[10] = (byte)(seed >> 8);
            sessionId[11] = (byte)seed;
            return Convert.ToBase64String(sessionId);
        }
    }
}
