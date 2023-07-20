// Decompiled with JetBrains decompiler
// Type: Webservices.Database.MongoDatabase`1
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Webservices.Manager;

namespace Webservices.Database
{
  public class MongoDatabase<T> where T : MongoDocument
  {
    private readonly IMongoClient m_client;
    private readonly IMongoDatabase m_database;
    private IMongoCollection<MongoDatabase<T>.Sequence> m_sequenceCollection;

    public IMongoCollection<T> Collection { get; }

    public MongoDatabase(string data)
    {
      try
      {
        Configuration config = Program.Config;
        this.m_client = (IMongoClient) new MongoClient(new MongoClientSettings()
        {
          Servers = (IEnumerable<MongoServerAddress>) new MongoServerAddress[1]
          {
            new MongoServerAddress(config.Host, config.Port)
          },
          Credential = MongoCredential.CreateCredential((string) null, config.Username, config.Password),
          MinConnectionPoolSize = 10,
          MaxConnectionPoolSize = 1000
        });
        this.m_database = this.m_client.GetDatabase(config.DbName);
        this.Collection = this.m_database.GetCollection<T>(data);
        this.Collection.FindSync<T>((Expression<Func<T, bool>>) (f => false));
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
    }

    public void InitSequence() => this.m_sequenceCollection = this.m_database.GetCollection<MongoDatabase<T>.Sequence>("Sequences");

    public async Task<long> IncrementSeed(string name)
    {
      IMongoCollection<MongoDatabase<T>.Sequence> sequenceCollection = this.m_sequenceCollection;
      FilterDefinition<MongoDatabase<T>.Sequence> filter = Builders<MongoDatabase<T>.Sequence>.Filter.Eq<string>((Expression<Func<MongoDatabase<T>.Sequence, string>>) (s => s.Name), name);
      UpdateDefinition<MongoDatabase<T>.Sequence> update = Builders<MongoDatabase<T>.Sequence>.Update.Inc<long>((Expression<Func<MongoDatabase<T>.Sequence, long>>) (f => f.Number), 1L);
      FindOneAndUpdateOptions<MongoDatabase<T>.Sequence, MongoDatabase<T>.Sequence> options = new FindOneAndUpdateOptions<MongoDatabase<T>.Sequence, MongoDatabase<T>.Sequence>();
      options.IsUpsert = true;
      options.ReturnDocument = ReturnDocument.After;
      CancellationToken cancellationToken = new CancellationToken();
      MongoDatabase<T>.Sequence oneAndUpdateAsync = await sequenceCollection.FindOneAndUpdateAsync<MongoDatabase<T>.Sequence>(filter, update, options, cancellationToken);
      return oneAndUpdateAsync.Number;
    }

    private class Sequence : MongoDocument
    {
      public string Name { get; set; }

      public long Number { get; set; }
    }
  }
}
