using MongoDB.Driver;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Tables
{
    public class ClanTable
    {
        public ClanTable()
        {
            Table = new MongoDatabase<ClanDocument>("Clans");
            Table.InitSequence();
            _ = Table.Collection.Indexes.CreateOne(new CreateIndexModel<ClanDocument>(Builders<ClanDocument>.IndexKeys.Ascending((ClanDocument f) => f.Clan.Name), new CreateIndexOptions
            {
                Name = "Name",
                Unique = true
            }));
            _ = Table.Collection.Indexes.CreateOne(new CreateIndexModel<ClanDocument>(Builders<ClanDocument>.IndexKeys.Ascending((ClanDocument f) => f.Clan.Tag), new CreateIndexOptions
            {
                Name = "Tag",
                Unique = true
            }));
        }

        public MongoDatabase<ClanDocument> Table { get; }
    }
}
