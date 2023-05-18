using System.Collections.Generic;

namespace UberStrok.Realtime.Server.Comm
{
    public class BufferedAdministratorMessage
    {
        private Dictionary<int, string> _bufferedmsgs;

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
            return _bufferedmsgs.ContainsKey(cmid) ? true : false;
        }

        public string GetBufferedMessages(int cmid)
        {
            if (_bufferedmsgs.TryGetValue(cmid, out string message))
            {
                return message;
            }

            return null;
        }

        public void DeleteBufferedMessage(int cmid)
        {
            _bufferedmsgs.Remove(cmid);
        }
    }
}
