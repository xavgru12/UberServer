using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ClanCreationReturnView
    {
        public int ResultCode { get; set; }
        public ClanView ClanView { get; set; }
    }
}
