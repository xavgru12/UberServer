using MongoDB.Driver;
using System;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;

namespace UberStrok.WebServices.AspNetCore.Core.Db.Tables
{
    public class UserTable
    {
        public UserTable()
        {
            Table = new MongoDatabase<UserDocument>("Users");
            Table.InitSequence();
            try
            {
                _ = Table.Collection.Indexes.CreateOne(new CreateIndexModel<UserDocument>(Builders<UserDocument>.IndexKeys.Ascending((UserDocument f) => f.Profile.Name), new CreateIndexOptions<UserDocument>
                {
                    Name = "Name",
                    Unique = new bool?(true),
                    Collation = new Collation("en", default, default, CollationStrength.Secondary, default, default, default, default, default)
                }), null, default);
                _ = Table.Collection.Indexes.CreateOne(new CreateIndexModel<UserDocument>(Builders<UserDocument>.IndexKeys.Ascending((UserDocument f) => f.SteamId), new CreateIndexOptions
                {
                    Name = "SteamId",
                    Unique = new bool?(true)
                }), null, default);
                _ = Table.Collection.Indexes.CreateOne(new CreateIndexModel<UserDocument>(Builders<UserDocument>.IndexKeys.Ascending((UserDocument f) => f.UBBan), new CreateIndexOptions<UserDocument>
                {
                    Name = "Banned",
                    PartialFilterExpression = Builders<UserDocument>.Filter.Exists((UserDocument u) => u.UBBan, true)
                }), null, default);
            }
            catch (Exception ex)
            {
                //ignore if indexes can't be created
                Console.WriteLine(ex.Message);
            }
        }

        public MongoDatabase<UserDocument> Table { get; }
    }
}
