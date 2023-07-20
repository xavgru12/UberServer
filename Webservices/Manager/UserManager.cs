// Decompiled with JetBrains decompiler
// Type: Webservices.Manager.UserManager
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Webservices.Database;
using Webservices.Database.Items;

namespace Webservices.Manager
{
  public static class UserManager
  {
    private static MongoDatabase<UserDocument> sm_database;

    public static bool Init()
    {
      try
      {
        UserManager.sm_database = new MongoDatabase<UserDocument>("Users");
        UserManager.sm_database.Collection.Indexes.CreateOne(new CreateIndexModel<UserDocument>(Builders<UserDocument>.IndexKeys.Ascending((Expression<Func<UserDocument, object>>) (f => f.Member.PublicProfile.Name)), new CreateIndexOptions()
        {
          Name = "Name",
          Unique = new bool?(true)
        }));
        UserManager.sm_database.Collection.Indexes.CreateOne(new CreateIndexModel<UserDocument>(Builders<UserDocument>.IndexKeys.Ascending((Expression<Func<UserDocument, object>>) (f => f.Email)), new CreateIndexOptions()
        {
          Name = "Email",
          Unique = new bool?(true)
        }));
        return true;
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return false;
      }
    }

    internal static Task<UserDocument> CreateUser(string email, string password)
    {
      try
      {
        UserDocument userDocument = UserDocument.Default(UserManager.GetCount().GetAwaiter().GetResult(), email, password);
        UserManager.sm_database.Collection.InsertOne(userDocument);
        return Task.FromResult<UserDocument>(userDocument);
      }
      catch (MongoException ex)
      {
        Console.WriteLine(ex.ToString());
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
      }
      return (Task<UserDocument>) null;
    }

    internal static Task<bool> IsNameUsed(string name)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      UserManager.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new UserManager.\u003C\u003Ec__DisplayClass3_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.name = name;
      ParameterExpression parameterExpression;
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      return Task.FromResult<bool>(UserManager.sm_database.Collection.AsQueryable<UserDocument>().Where<UserDocument>(Expression.Lambda<Func<UserDocument, bool>>((Expression) Expression.Equal((Expression) Expression.Call(f.Member.PublicProfile.Name, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass30, typeof (UserManager.\u003C\u003Ec__DisplayClass3_0)), FieldInfo.GetFieldFromHandle(__fieldref (UserManager.\u003C\u003Ec__DisplayClass3_0.name))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>())), parameterExpression)).Count<UserDocument>() != 0);
    }

    internal static Task<UserDocument> GetUser(int id) => UserManager.sm_database.Collection.Find<UserDocument>(Builders<UserDocument>.Filter.Eq<int>((Expression<Func<UserDocument, int>>) (f => f.UserId), id)).FirstOrDefaultAsync<UserDocument, UserDocument>();

    internal static Task<int> GetCount()
    {
      try
      {
        FindOptions<UserDocument, UserDocument> findOptions = new FindOptions<UserDocument, UserDocument>();
        findOptions.Limit = new int?(1);
        findOptions.Sort = Builders<UserDocument>.Sort.Descending((Expression<Func<UserDocument, object>>) (f => (object) f.Member.PublicProfile.Cmid));
        FindOptions<UserDocument, UserDocument> options = findOptions;
        UserDocument userDocument = UserManager.sm_database.Collection.FindAsync<UserDocument>(FilterDefinition<UserDocument>.Empty, options).Result.FirstOrDefault<UserDocument>();
        return userDocument == null ? Task.FromResult<int>(1) : Task.FromResult<int>(userDocument.Member.PublicProfile.Cmid + 1);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return Task.FromResult<int>(0);
      }
    }

    internal static Task<List<UserDocument>> Leaderboard(int limit, string key)
    {
      switch (key)
      {
        case "kill":
          return UserManager.sm_database.Collection.Find<UserDocument>((Expression<Func<UserDocument, bool>>) (x => true)).SortByDescending<UserDocument, UserDocument>((Expression<Func<UserDocument, object>>) (f => (object) f.Kills)).Limit(new int?(limit)).ToListAsync<UserDocument>();
        case "xp":
          return UserManager.sm_database.Collection.Find<UserDocument>((Expression<Func<UserDocument, bool>>) (x => true)).SortByDescending<UserDocument, UserDocument>((Expression<Func<UserDocument, object>>) (f => (object) f.Statistics.Xp)).Limit(new int?(limit)).ToListAsync<UserDocument>();
        default:
          return UserManager.sm_database.Collection.Find<UserDocument>((Expression<Func<UserDocument, bool>>) (x => x.Statistics.Level > 9)).SortByDescending<UserDocument, UserDocument>((Expression<Func<UserDocument, object>>) (f => (object) f.Kdr)).Limit(new int?(limit)).ToListAsync<UserDocument>();
      }
    }

    internal static Task<List<UserDocument>> bannedUsers() => UserManager.sm_database.Collection.Find<UserDocument>((Expression<Func<UserDocument, bool>>) (x => x.UBBan != default (string))).ToListAsync<UserDocument>();

    internal static Task<List<UserDocument>> mutedUsers() => UserManager.sm_database.Collection.Find<UserDocument>((Expression<Func<UserDocument, bool>>) (x => x.UBMute != default (string))).ToListAsync<UserDocument>();

    internal static Task<UserDocument> GetUser(string email) => UserManager.sm_database.Collection.Find<UserDocument>(Builders<UserDocument>.Filter.Eq<string>((Expression<Func<UserDocument, string>>) (f => f.Email), email)).FirstOrDefaultAsync<UserDocument, UserDocument>();

    internal static Task<List<UserDocument>> FindUser(string name)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      UserManager.\u003C\u003Ec__DisplayClass10_0 cDisplayClass100 = new UserManager.\u003C\u003Ec__DisplayClass10_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass100.name = name;
      FilterDefinitionBuilder<UserDocument> filter1 = Builders<UserDocument>.Filter;
      IMongoCollection<UserDocument> collection = UserManager.sm_database.Collection;
      ParameterExpression parameterExpression1 = Expression.Parameter(typeof (UserDocument), "x");
      // ISSUE: method reference
      MethodInfo methodFromHandle = (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (Enumerable.Any));
      // ISSUE: method reference
      Expression[] expressionArray = new Expression[2]
      {
        (Expression) Expression.Property((Expression) parameterExpression1, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (UserDocument.get_Names))),
        null
      };
      ParameterExpression parameterExpression2;
      // ISSUE: method reference
      // ISSUE: method reference
      // ISSUE: field reference
      // ISSUE: method reference
      expressionArray[1] = (Expression) Expression.Lambda<Func<string, bool>>((Expression) Expression.Call((Expression) Expression.Call(t, (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>()), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.Contains)), (Expression) Expression.Call((Expression) Expression.Field((Expression) Expression.Constant((object) cDisplayClass100, typeof (UserManager.\u003C\u003Ec__DisplayClass10_0)), FieldInfo.GetFieldFromHandle(__fieldref (UserManager.\u003C\u003Ec__DisplayClass10_0.name))), (MethodInfo) MethodBase.GetMethodFromHandle(__methodref (string.ToLower)), Array.Empty<Expression>())), parameterExpression2);
      Expression<Func<UserDocument, bool>> filter2 = Expression.Lambda<Func<UserDocument, bool>>((Expression) Expression.Call((Expression) null, methodFromHandle, expressionArray), parameterExpression1);
      return collection.Find<UserDocument>(filter2).ToListAsync<UserDocument>();
    }

    internal static Task Save(UserDocument document)
    {
      try
      {
        return (Task) UserManager.sm_database.Collection.ReplaceOneAsync<UserDocument>((Expression<Func<UserDocument, bool>>) (f => f.Member.PublicProfile.Cmid == document.Member.PublicProfile.Cmid), document);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return (Task) null;
      }
    }

    internal static Task ChangeName(int id, string newName) => (Task) UserManager.sm_database.Collection.UpdateOneAsync(Builders<UserDocument>.Filter.Eq<int>((Expression<Func<UserDocument, int>>) (f => f.UserId), id), Builders<UserDocument>.Update.Set<string>((Expression<Func<UserDocument, string>>) (f => f.Member.PublicProfile.Name), newName));

    internal static Task<HashSet<int>> MatchHWID(string key, HashSet<string> obj)
    {
      List<UserDocument> userDocumentList = new List<UserDocument>();
      FilterDefinitionBuilder<UserDocument> filter = Builders<UserDocument>.Filter;
      switch (key)
      {
        case "hdd":
          userDocumentList = UserManager.sm_database.Collection.Find<UserDocument>(filter.AnyIn<string>((Expression<Func<UserDocument, IEnumerable<string>>>) (x => x.HDD), (IEnumerable<string>) obj)).ToList<UserDocument>();
          break;
        case "bios":
          userDocumentList = UserManager.sm_database.Collection.Find<UserDocument>(filter.AnyIn<string>((Expression<Func<UserDocument, IEnumerable<string>>>) (x => x.BIOS), (IEnumerable<string>) obj)).ToList<UserDocument>();
          break;
        case "motherboard":
          userDocumentList = UserManager.sm_database.Collection.Find<UserDocument>(filter.AnyIn<string>((Expression<Func<UserDocument, IEnumerable<string>>>) (x => x.MOTHERBOARD), (IEnumerable<string>) obj)).ToList<UserDocument>();
          break;
        case "mac":
          userDocumentList = UserManager.sm_database.Collection.Find<UserDocument>(filter.AnyIn<string>((Expression<Func<UserDocument, IEnumerable<string>>>) (x => x.MAC), (IEnumerable<string>) obj)).ToList<UserDocument>();
          break;
        case "unity":
          userDocumentList = UserManager.sm_database.Collection.Find<UserDocument>(filter.AnyIn<string>((Expression<Func<UserDocument, IEnumerable<string>>>) (x => x.UNITY), (IEnumerable<string>) obj)).ToList<UserDocument>();
          break;
      }
      HashSet<int> result = new HashSet<int>();
      foreach (UserDocument userDocument in userDocumentList)
        result.Add(userDocument.Member.PublicProfile.Cmid);
      return Task.FromResult<HashSet<int>>(result);
    }
  }
}
