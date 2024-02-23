
using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ClanRequestDeclineView
    {
        public int ActionResult { get; set; }
        public int ClanRequestId { get; set; }
    }
}
