using System;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class ClanMember
    {
        public ClanMember()
        {

        }
        public ClanMember(Member member)
        {
            CmId = member.Id;
        }
        public int CmId { get; set; }
        public GroupPosition Position { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
