using MongoDB.Bson.Serialization.Attributes;
using System;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream
{
    [BsonKnownTypes(new Type[]
    {
        typeof(ContactRequestStream),
        typeof(GroupInvitationStream)
    })]
    public abstract class StreamDocument : MongoDocument
    {
        public int StreamId
        {
            get;
            set;
        }

        public abstract StreamType StreamType
        {
            get;
        }
    }
}
