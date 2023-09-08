using System.Collections.Generic;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Items
{
    public class SecurityDocument : MongoDocument
    {
        public string Type
        {
            get => "Security";
            set
            {
            }
        }

        public HashSet<int> CmidBans
        {
            get;
            set;
        }

        public HashSet<string> IpBans
        {
            get;
            set;
        }

        public HashSet<string> HwdBans
        {
            get;
            set;
        }
    }
}
