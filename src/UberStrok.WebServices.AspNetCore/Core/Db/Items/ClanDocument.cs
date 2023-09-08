using UberStrok.Core.Views;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Items
{
    public class ClanDocument : MongoDocument
    {
        public int ClanId
        {
            get;
            set;
        }

        public ClanView Clan
        {
            get;
            set;
        }
    }
}
