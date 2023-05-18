using System;
using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ClanView : BasicClanView
    {
        public ClanView()
        {
            this.Members = new List<ClanMemberView>();
        }

        public List<ClanMemberView> Members { get; set; }

        public override string ToString()
        {
            string text = "[Clan: " + base.ToString();
            text += "[Members:";
            foreach (ClanMemberView clanMemberView in this.Members)
            {
                text += clanMemberView.ToString();
            }
            text += "]";
            return text;
        }
    }
}
