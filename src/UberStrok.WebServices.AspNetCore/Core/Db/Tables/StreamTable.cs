using UberStrok.WebServices.AspNetCore.Core.Db.Items.Stream;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Tables
{
    public class StreamTable
    {
        public StreamTable()
        {
            Table = new MongoDatabase<StreamDocument>("Stream");
            Table.InitSequence();
        }

        public MongoDatabase<StreamDocument> Table { get; }
    }
}
