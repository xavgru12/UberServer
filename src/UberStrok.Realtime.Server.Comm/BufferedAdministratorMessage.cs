using System.Collections.Generic;

namespace UberStrok.Realtime.Server.Comm
{
    public class BufferedAdministratorMessage
    {
        private readonly Dictionary<int, string> _bufferedmsgs;

        public BufferedAdministratorMessage()
        {
            _bufferedmsgs = new Dictionary<int, string>();
        }

        public void AddBufferedMessage(int cmid, string message)
        {
            _bufferedmsgs.Add(cmid, message);
        }

        public bool HasBufferedMessage(int cmid)
        {
            return _bufferedmsgs.ContainsKey(cmid);
        }

        public string GetBufferedMessages(int cmid)
        {
            return _bufferedmsgs.TryGetValue(cmid, out string message) ? message : null;
        }

        public void DeleteBufferedMessage(int cmid)
        {
            _ = _bufferedmsgs.Remove(cmid);
        }
    }
}
