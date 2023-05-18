using System;
using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class GroupInvitation
    {
        public int Id { get; set; }

        public GroupInvitationView View { get; set; }
    }
}
