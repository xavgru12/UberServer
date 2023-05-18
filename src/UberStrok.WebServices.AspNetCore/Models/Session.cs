using System;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string IPAddress { get; set; }
        public string HWD { get; set; }
        public DateTime Creation { get; set; }
        public DateTime Expiration { get; set; }
    }
}
