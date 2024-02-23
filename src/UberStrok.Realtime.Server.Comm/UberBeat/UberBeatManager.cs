using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using System.Threading.Tasks;
using MongoDB.Bson;
using UberStrok.Realtime.Server;
using log4net;

namespace UberStrok.Realtime.Server.Comm.UberBeat
{
    public partial class UberBeatManager
    {
        private ILog Log;

        static string connectionString = "mongodb://localhost:27017";

        static string SetNotNull = "-1";

        static string SetNull = null;

        // Connect to the MongoDB server
        static MongoClient client = new MongoClient(connectionString);

        static IMongoDatabase userdatabase = client.GetDatabase("admin");

        static IMongoDatabase securitybans = client.GetDatabase("Security");

        // Access a specific collection within the database
        IMongoCollection<BsonDocument> collection = userdatabase.GetCollection<BsonDocument>("Users");

        private class UserData
        {
            public int Cmid;

            public HashSet<string> Name = new HashSet<string>();

            public HashSet<string> HDD = new HashSet<string>();

            public HashSet<string> BIOS = new HashSet<string>();

            public HashSet<string> MAC = new HashSet<string>();

            public HashSet<string> MOTHERBOARD = new HashSet<string>();

            public HashSet<string> UNITY = new HashSet<string>();

            public bool isBanned;

            public string Date;
        }
        public async Task GetHWID(int cmid)
        {
            try
            {
                var Filter = Builders<BsonDocument>.Filter.Eq("UserId", cmid);
                var User = await collection.Find(Filter).FirstOrDefaultAsync();
                if (User != null)
                {
                    BsonDocument bsonUser = User.ToBsonDocument();
                    string hdd = bsonUser.GetValue("HDD").ToString();
                    Log.Info($"HDD for User with CMID {cmid}: {hdd}");
                }
                else
                {
                }

            }
            catch (Exception ex)
            {
                Log.Error($"Error Failed to Ban User with CMID: {cmid}", ex);
            }
        }
        public async Task BanCmid(int cmid)
        {
            try
            {
                var Filter = Builders<BsonDocument>.Filter.Eq("UserId", cmid);
                var User = await collection.Find(Filter).FirstOrDefaultAsync();
                if (User != null)
                {
                    var Update = Builders<BsonDocument>.Update.Set("UBBan", SetNotNull);
                    await collection.UpdateOneAsync(Filter, Update);
                }
                else
                {
                }
                Log.Info($"Banned User with CMID: {cmid}");

            }
            catch (Exception ex)
            {
                Log.Error($"Error Failed to Ban User with CMID: {cmid}", ex);
            }
        }

        public async Task UnbanCmid(int cmid)
        {
            try
            {
                var Filter = Builders<BsonDocument>.Filter.Eq("UserId", cmid);
                var User = await collection.Find(Filter).FirstOrDefaultAsync();
                if (User != null)
                {
                    var Update = Builders<BsonDocument>.Update.Set("UBBan", SetNull);
                    await collection.UpdateOneAsync(Filter, Update);
                }
                else
                {
                }
                Log.Info($"Unbanned User with CMID: {cmid}");

            }
            catch (Exception ex)
            {
                Log.Error($"Error Failed to Unban User with CMID: {cmid}", ex);
            }
        }

    }

}
