using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ClanMemberView
    {
        public ClanMemberView()
        {
        }

        public ClanMemberView(string name, int cmid, GroupPosition position, DateTime joiningDate, DateTime lastLogin)
        {
            Cmid = cmid;
            Name = name;
            Position = position;
            JoiningDate = joiningDate;
            Lastlogin = lastLogin;
        }

        public string Name { get; set; }

        public int Cmid { get; set; }

        public GroupPosition Position { get; set; }

        public DateTime JoiningDate { get; set; }

        public DateTime Lastlogin { get; set; }

        public override string ToString()
        {
            return string.Concat(new object[]
            {
                "[Clan member: [Name: ",
                Name,
                "][Cmid: ",
                Cmid,
                "][Position: ",
                Position,
                "][JoiningDate: ",
                JoiningDate,
                "][Lastlogin: ",
                Lastlogin,
                "]]"
            });
        }
    }
}

