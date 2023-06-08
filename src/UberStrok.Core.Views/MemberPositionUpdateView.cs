using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class MemberPositionUpdateView
    {
        public MemberPositionUpdateView()
        {
        }

        public MemberPositionUpdateView(int groupId, string authToken, int memberCmid, GroupPosition position)
        {
            GroupId = groupId;
            AuthToken = authToken;
            MemberCmid = memberCmid;
            Position = position;
        }

        public int GroupId { get; set; }

        public string AuthToken { get; set; }

        public int MemberCmid { get; set; }

        public GroupPosition Position { get; set; }

        public override string ToString()
        {
            string text = string.Concat(new object[]
            {
                "[MemberPositionUpdateView: [GroupId:",
                GroupId,
                "][AuthToken:",
                AuthToken,
                "][MemberCmid:",
                MemberCmid
            });
            string text2 = text;
            return string.Concat(new object[]
            {
                text2,
                "][Position:",
                Position,
                "]]"
            });
        }
    }
}
