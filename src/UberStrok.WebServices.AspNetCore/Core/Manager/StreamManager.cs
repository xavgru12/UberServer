using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Core.Db;
using UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream;
using UberStrok.WebServices.AspNetCore.Core.Db.Tables;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class StreamManager
    {
        private readonly MongoDatabase<StreamDocument> sm_database;

        public StreamManager(StreamTable table)
        {
            sm_database = table.Table;
        }

        internal async Task<int> GetNextId()
        {
            return (int)await sm_database.IncrementSeed("StreamID");
        }

        internal Task Create(StreamDocument document)
        {
            return sm_database.Collection.InsertOneAsync(document);
        }

        internal Task Save(StreamDocument document)
        {
            return IMongoCollectionExtensions.ReplaceOneAsync(sm_database.Collection, (StreamDocument f) => f.Id == document.Id, document, (ReplaceOptions)null, default);
        }

        internal Task<StreamDocument> Get(int id)
        {
            return sm_database.Collection.Find(Builders<StreamDocument>.Filter.Eq((StreamDocument f) => f.StreamId, id)).FirstOrDefaultAsync();
        }

        internal Task<StreamDocument[]> Get(int[] ids)
        {
            Task<StreamDocument>[] docs = new Task<StreamDocument>[ids.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                docs[i] = Get(ids[i]);
            }
            return Task.WhenAll<StreamDocument>(docs);
        }

        internal Task<StreamDocument[]> Get(List<int> ids)
        {
            Task<StreamDocument>[] docs = new Task<StreamDocument>[ids.Count];
            for (int i = 0; i < ids.Count; i++)
            {
                docs[i] = Get(ids[i]);
            }
            return Task.WhenAll<StreamDocument>(docs);
        }
    }
}
