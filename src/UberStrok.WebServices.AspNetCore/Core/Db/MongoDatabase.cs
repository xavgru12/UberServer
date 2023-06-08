using MongoDB.Driver;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.WebService;

namespace UberStrok.WebServices.AspNetCore.Core.Db
{
    public class MongoDatabase<T> where T : MongoDocument
    {
        private class Sequence : MongoDocument
        {
            public string Name
            {
                get;
                set;
            }

            public long Number
            {
                get;
                set;
            }
        }

        private readonly IMongoClient m_client;

        private readonly IMongoDatabase m_database;

        private IMongoCollection<Sequence> m_sequenceCollection;

        public IMongoCollection<T> Collection
        {
            get;
        }

        public MongoDatabase(string type)
        {
            WebServiceConfiguration.MongoDatabaseInfo info = Startup.WebServiceConfiguration.ServiceDatabase;
            m_client = new MongoClient(new MongoClientSettings
            {
                Servers = new MongoServerAddress[1]
                {
                    new MongoServerAddress(info.Host)
                },
                Credential = MongoCredential.CreateCredential(null, info.Username, info.Password),
                MinConnectionPoolSize = 10,
                MaxConnectionPoolSize = 1000
            });
            m_database = m_client.GetDatabase(info.Database);
            Collection = m_database.GetCollection<T>(type);
            _ = IMongoCollectionExtensions.FindSync(Collection, (T f) => false, null, default);
        }

        public void InitSequence()
        {
            m_sequenceCollection = m_database.GetCollection<Sequence>("Sequences");
        }

        public async Task<long> IncrementSeed(string name)
        {
            return (await m_sequenceCollection.FindOneAndUpdateAsync(Builders<Sequence>.Filter.Eq((Sequence s) => s.Name, name), Builders<Sequence>.Update.Inc((Sequence f) => f.Number, 1L), new FindOneAndUpdateOptions<Sequence, Sequence>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            })).Number;
        }
    }
}
