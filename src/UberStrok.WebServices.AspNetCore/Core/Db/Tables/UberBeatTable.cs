namespace UberStrok.WebServices.AspNetCore.Core.Db.Tables
{
    public class UberBeatTable
    {
        public UberBeatTable()
        {
            Table = new MongoDatabase<UberBeatDocument>("ExceptionData");
        }

        public MongoDatabase<UberBeatDocument> Table { get; }
    }
}
