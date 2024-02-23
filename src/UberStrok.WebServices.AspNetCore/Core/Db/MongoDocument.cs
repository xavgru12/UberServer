using MongoDB.Bson;

namespace UberStrok.WebServices.AspNetCore.Core.Db
{
    public abstract class MongoDocument
    {
        public ObjectId Id
        {
            get;
            set;
        }
    }
}
