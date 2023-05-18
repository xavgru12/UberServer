using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class MemberSocials
    {
        public Dictionary<int, ContactRequest> IncomingRequests { get; set; }
        public List<int> Contacts { get; set; } // TODO: Update to ISet<int>.
    }
}
