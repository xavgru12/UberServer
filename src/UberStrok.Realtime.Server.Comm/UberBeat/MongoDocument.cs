using MongoDB.Bson;

namespace UberStrok.Realtime.Server.Comm.UberBeat
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
