using UberStrok.WebServices.AspNetCore.Core.Db.Items;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Tables
{
    public class SecurityTable
    {
        public SecurityTable()
        {
            Table = new MongoDatabase<SecurityDocument>("Servers");
        }

        public MongoDatabase<SecurityDocument> Table { get; }
    }
}
