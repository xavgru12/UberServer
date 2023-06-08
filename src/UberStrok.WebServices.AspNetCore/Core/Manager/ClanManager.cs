using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.Core.Views;
using UberStrok.WebServices.AspNetCore.Core.Db;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Db.Tables;
using UberStrok.WebServices.AspNetCore.Helper;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class ClanManager
    {
        private readonly MongoDatabase<ClanDocument> sm_database;

        public ClanManager(ClanTable table)
        {
            sm_database = table.Table;
        }

        internal async Task<ClanDocument> Create(GroupCreationView groupCreation, PublicProfileView profile)
        {
            _ = 1;
            try
            {
                int id = (int)await sm_database.IncrementSeed("ClanID");
                ClanDocument clan = new ClanDocument
                {
                    ClanId = id,
                    Clan = ClanHelper.GetClanView(groupCreation, id, profile)
                };
                await sm_database.Collection.InsertOneAsync(clan);
                return clan;
            }
            catch (MongoWriteException)
            {
                return null;
            }
        }

        internal Task<bool> IsClanNameUsed(string name)
        {
            return Task.FromResult<bool>(sm_database.Collection.AsQueryable().Where((ClanDocument f) => f.Clan.Name.ToLower() == name.ToLower()).Count() != 0);
        }

        internal Task<bool> IsClanTagUsed(string tag)
        {
            return Task.FromResult<bool>(sm_database.Collection.AsQueryable().Where((ClanDocument f) => f.Clan.Tag.ToLower() == tag.ToLower()).Count() != 0);
        }

        internal Task Save(ClanDocument document)
        {
            return IMongoCollectionExtensions.ReplaceOneAsync(sm_database.Collection, (ClanDocument f) => f.Id == document.Id, document, (ReplaceOptions)null, default);
        }

        internal Task Remove(ClanDocument document)
        {
            return IMongoCollectionExtensions.DeleteOneAsync(sm_database.Collection, (ClanDocument f) => f.Id == document.Id, default);
        }

        internal Task<ClanDocument> Get(int id)
        {
            return sm_database.Collection.Find(Builders<ClanDocument>.Filter.Eq((ClanDocument f) => f.ClanId, id)).FirstOrDefaultAsync();
        }
    }
}
