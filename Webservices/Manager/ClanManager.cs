// Decompiled with JetBrains decompiler
// Type: Webservices.Manager.ClanManager
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Webservices.Database;
using Webservices.Database.Items;
using Webservices.Helper;

namespace Webservices.Manager
{
  public static class ClanManager
  {
    private static MongoDatabase<ClanDocument> sm_database;

    internal static void Init()
    {
      ClanManager.sm_database = new MongoDatabase<ClanDocument>("Clans");
      ClanManager.sm_database.Collection.Indexes.CreateOne(new CreateIndexModel<ClanDocument>(Builders<ClanDocument>.IndexKeys.Ascending((Expression<Func<ClanDocument, object>>) (f => f.Clan.Name)), new CreateIndexOptions()
      {
        Name = "Name",
        Unique = new bool?(true)
      }));
      ClanManager.sm_database.Collection.Indexes.CreateOne(new CreateIndexModel<ClanDocument>(Builders<ClanDocument>.IndexKeys.Ascending((Expression<Func<ClanDocument, object>>) (f => f.Clan.Tag)), new CreateIndexOptions()
      {
        Name = "Tag",
        Unique = new bool?(true)
      }));
    }

    internal static async Task<ClanDocument> Create(
      GroupCreationView groupCreation,
      PublicProfileView profile)
    {
      try
      {
        int id = ClanManager.GetCount().GetAwaiter().GetResult();
        ClanDocument clan = new ClanDocument()
        {
          ClanId = id,
          Clan = ClanHelper.GetClanView(groupCreation, id, profile)
        };
        await ClanManager.sm_database.Collection.InsertOneAsync(clan);
        return clan;
      }
      catch (MongoWriteException ex)
      {
        return (ClanDocument) null;
      }
    }

    internal static Task<int> GetCount()
    {
      try
      {
        FindOptions<ClanDocument, ClanDocument> findOptions = new FindOptions<ClanDocument, ClanDocument>();
        findOptions.Limit = new int?(1);
        findOptions.Sort = Builders<ClanDocument>.Sort.Descending((Expression<Func<ClanDocument, object>>) (f => (object) f.ClanId));
        FindOptions<ClanDocument, ClanDocument> options = findOptions;
        ClanDocument clanDocument = ClanManager.sm_database.Collection.FindAsync<ClanDocument>(FilterDefinition<ClanDocument>.Empty, options).Result.FirstOrDefault<ClanDocument>();
        return clanDocument == null ? Task.FromResult<int>(1) : Task.FromResult<int>(clanDocument.ClanId + 1);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return Task.FromResult<int>(1);
      }
    }

    internal static Task<bool> IsClanNameUsed(string name)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ClanManager.\u003C\u003Ec__DisplayClass4_0 cDisplayClass40 = new ClanManager.\u003C\u003Ec__DisplayClass4_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass40.name = name;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      return Task.FromResult<bool>(ClanManager.sm_database.Collection.AsQueryable<ClanDocument>().Where<ClanDocument>(Expression.Lambda<Func<ClanDocument, bool>>((Expression) Expression.Equal((Expression) Expression.Call(f.Clan.Name, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass40, typeof (ClanManager.\u003C\u003Ec__DisplayClass4_0)), FieldInfo.GetFieldFromHandle(__fieldref (ClanManager.\u003C\u003Ec__DisplayClass4_0.name))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>())), parameterExpression)).Count<ClanDocument>() != 0);
    }

    internal static Task<bool> IsClanTagUsed(string tag)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      ClanManager.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50 = new ClanManager.\u003C\u003Ec__DisplayClass5_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass50.tag = tag;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      return Task.FromResult<bool>(ClanManager.sm_database.Collection.AsQueryable<ClanDocument>().Where<ClanDocument>(Expression.Lambda<Func<ClanDocument, bool>>((Expression) Expression.Equal((Expression) Expression.Call(f.Clan.Tag, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass50, typeof (ClanManager.\u003C\u003Ec__DisplayClass5_0)), FieldInfo.GetFieldFromHandle(__fieldref (ClanManager.\u003C\u003Ec__DisplayClass5_0.tag))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>())), parameterExpression)).Count<ClanDocument>() != 0);
    }

    internal static Task Save(ClanDocument document) => (Task) ClanManager.sm_database.Collection.ReplaceOneAsync<ClanDocument>((Expression<Func<ClanDocument, bool>>) (f => f.Id == document.Id), document);

    internal static Task Remove(ClanDocument document) => (Task) ClanManager.sm_database.Collection.DeleteOneAsync<ClanDocument>((Expression<Func<ClanDocument, bool>>) (f => f.Id == document.Id));

    internal static Task<ClanDocument> Get(int id) => ClanManager.sm_database.Collection.Find<ClanDocument>(Builders<ClanDocument>.Filter.Eq<int>((Expression<Func<ClanDocument, int>>) (f => f.ClanId), id)).FirstOrDefaultAsync<ClanDocument, ClanDocument>();
  }
}
