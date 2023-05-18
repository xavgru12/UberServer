using System;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class BasicClanView
    {
        public BasicClanView()
        {
        }

        public int GroupId { get; set; }

        public int MembersCount { get; set; } = 50;

        public string Description { get; set; }

        public string Name { get; set; }

        public string Motto { get; set; }

        public string Address { get; set; }

        public DateTime FoundingDate { get; set; }

        public string Picture { get; set; }

        public GroupType Type { get; set; }

        public DateTime LastUpdated { get; set; }

        public string Tag { get; set; }

        public int MembersLimit { get; set; }

        public GroupColor ColorStyle { get; set; }

        public GroupFontStyle FontStyle { get; set; }

        public int ApplicationId { get; set; }

        public int OwnerCmid { get; set; }

        public string OwnerName { get; set; }


        public override string ToString()
        {
            string text = string.Concat(new object[]
            {
                "[Clan: [Id: ",
                this.GroupId,
                "][Members count: ",
                this.MembersCount,
                "][Description: ",
                this.Description,
                "]"
            });
            string text2 = text;
            text = string.Concat(new string[]
            {
                text2,
                "[Name: ",
                this.Name,
                "][Motto: ",
                this.Name,
                "][Address: ",
                this.Address,
                "]"
            });
            text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "[Creation date: ",
                this.FoundingDate,
                "][Picture: ",
                this.Picture,
                "][Type: ",
                this.Type,
                "][Last updated: ",
                this.LastUpdated,
                "]"
            });
            text2 = text;
            text = string.Concat(new object[]
            {
                text2,
                "[Tag: ",
                this.Tag,
                "][Members limit: ",
                this.MembersLimit,
                "][Color style: ",
                this.ColorStyle,
                "][Font style: ",
                this.FontStyle,
                "]"
            });
            text2 = text;
            return string.Concat(new object[]
            {
                text2,
                "[Application Id: ",
                this.ApplicationId,
                "][Owner Cmid: ",
                this.OwnerCmid,
                "][Owner name: ",
                this.OwnerName,
                "]]"
            });
        }
    }
}

