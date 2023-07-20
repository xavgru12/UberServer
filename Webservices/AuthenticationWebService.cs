// Decompiled with JetBrains decompiler
// Type: Webservices.AuthenticationWebService
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using UberStrike.Core.Serialization;
using UberStrike.Core.ViewModel;
using UberStrike.DataCenter.Common.Entities;
using Webservices.Database.Items;
using Webservices.Manager;

namespace Webservices
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class AuthenticationWebService : IAuthenticationWebServiceContract
  {
    public static List<AuthenticationView> authenticationView;
    public static object Sync = new object();

    public AuthenticationWebService()
    {
      Console.Write("Initializing AuthenticationWebService...\t");
      AuthenticationWebService.authenticationView = new List<AuthenticationView>();
    }

    public byte[] CreateUser(byte[] data)
    {
      string email;
      string password;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        bytes.Seek(0L, SeekOrigin.Begin);
        email = StringProxy.Deserialize((Stream) bytes);
        password = this.Hash(StringProxy.Deserialize((Stream) bytes));
        EnumProxy<ChannelType>.Deserialize((Stream) bytes);
        StringProxy.Deserialize((Stream) bytes);
        StringProxy.Deserialize((Stream) bytes);
      }
      MemberRegistrationResult instance = !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password) ? (UserManager.GetUser(email).GetAwaiter().GetResult() == null ? (UserManager.CreateUser(email, password) == null ? MemberRegistrationResult.InvalidData : MemberRegistrationResult.Ok) : MemberRegistrationResult.DuplicateEmail) : MemberRegistrationResult.InvalidData;
      using (MemoryStream bytes = new MemoryStream())
      {
        EnumProxy<MemberRegistrationResult>.Serialize((Stream) bytes, instance);
        return bytes.ToArray();
      }
    }

    public byte[] CompleteAccount(byte[] data)
    {
      int id;
      string name;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        bytes.Seek(0L, SeekOrigin.Begin);
        id = Int32Proxy.Deserialize((Stream) bytes);
        name = StringProxy.Deserialize((Stream) bytes);
        EnumProxy<ChannelType>.Deserialize((Stream) bytes);
        StringProxy.Deserialize((Stream) bytes);
        StringProxy.Deserialize((Stream) bytes);
      }
      int num = 1;
      List<string> stringList = new List<string>();
      UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      if (UserManager.IsNameUsed(name).Result)
        num = 2;
      else if (result != null)
      {
        result.isAccountComplete = true;
        result.Member.PublicProfile.Name = name;
        result.Statistics.Xp = 80;
        dictionary.Add(1000, 5);
        dictionary.Add(1002, 5);
        dictionary.Add(1094, 5);
        if (num == 1)
        {
          if (result.ItemInventory == null)
            result.ItemInventory = new List<ItemInventoryView>();
          for (int index = 1084; index <= 1089; ++index)
            result.ItemInventory.Add(new ItemInventoryView()
            {
              Cmid = id,
              ExpirationDate = new DateTime?(),
              AmountRemaining = -1,
              ItemId = index
            });
          result.ItemInventory.Add(new ItemInventoryView()
          {
            Cmid = id,
            ExpirationDate = new DateTime?(),
            AmountRemaining = -1,
            ItemId = 1000
          });
          result.ItemInventory.Add(new ItemInventoryView()
          {
            Cmid = id,
            ExpirationDate = new DateTime?(),
            AmountRemaining = -1,
            ItemId = 1002
          });
          result.ItemInventory.Add(new ItemInventoryView()
          {
            Cmid = id,
            ExpirationDate = new DateTime?(),
            AmountRemaining = -1,
            ItemId = 1094
          });
        }
        UserManager.Save(result).GetAwaiter().GetResult();
      }
      else
        num = 3;
      AccountCompletionResultView instance = new AccountCompletionResultView()
      {
        ItemsAttributed = dictionary,
        NonDuplicateNames = new List<string>(),
        Result = num
      };
      using (MemoryStream memoryStream = new MemoryStream())
      {
        AccountCompletionResultViewProxy.Serialize((Stream) memoryStream, instance);
        return memoryStream.ToArray();
      }
    }

    public byte[] LoginMemberEmail(byte[] data)
    {
      try
      {
        string str1;
        string str2;
        string authToken;
        using (MemoryStream bytes = new MemoryStream(data))
        {
          bytes.Seek(0L, SeekOrigin.Begin);
          str1 = StringProxy.Deserialize((Stream) bytes);
          str2 = this.Hash(StringProxy.Deserialize((Stream) bytes));
          EnumProxy<ChannelType>.Deserialize((Stream) bytes);
          StringProxy.Deserialize((Stream) bytes);
          authToken = StringProxy.Deserialize((Stream) bytes);
        }
        MemberAuthenticationResult authenticationResult = MemberAuthenticationResult.InvalidData;
        int num = -1;
        UserDocument userDocument = (UserDocument) null;
        if (!string.IsNullOrWhiteSpace(str1) && !string.IsNullOrWhiteSpace(str2))
        {
          userDocument = UserManager.GetUser(str1.Trim()).GetAwaiter().GetResult();
          if (userDocument != null)
          {
            if (userDocument.Password == str2)
            {
              if (this.Authenticate(authToken) || userDocument.Member.PublicProfile.AccessLevel == MemberAccessLevel.Admin)
              {
                authenticationResult = MemberAuthenticationResult.Ok;
                num = userDocument.Member.PublicProfile.Cmid;
              }
              else
                authenticationResult = MemberAuthenticationResult.InvalidHandle;
            }
            else
              authenticationResult = MemberAuthenticationResult.InvalidPassword;
          }
          else
            authenticationResult = MemberAuthenticationResult.InvalidEmail;
        }
        using (MemoryStream memoryStream = new MemoryStream())
        {
          MemberAuthenticationResultView instance = new MemberAuthenticationResultView()
          {
            MemberAuthenticationResult = authenticationResult
          };
          if (num > 0)
          {
            bool isAccountComplete1 = userDocument.isAccountComplete;
            bool isAccountComplete2 = userDocument.isAccountComplete;
            instance = new MemberAuthenticationResultView()
            {
              MemberView = userDocument.Member,
              PlayerStatisticsView = userDocument.Statistics,
              ServerTime = DateTime.Now,
              LuckyDraw = (LuckyDrawUnityView) null,
              WeeklySpecial = new WeeklySpecialView()
              {
                Id = 1,
                EndDate = new DateTime?(DateTime.MaxValue),
                ItemId = 1377,
                StartDate = DateTime.UtcNow,
                Text = "UberKill is Back!",
                Title = "UberKill",
                ImageUrl = Program.AssetBaseUrl + "/india.png"
              },
              IsAccountComplete = isAccountComplete2,
              IsTutorialComplete = isAccountComplete1,
              MemberAuthenticationResult = MemberAuthenticationResult.Ok
            };
          }
          MemberAuthenticationResultViewProxy.Serialize((Stream) memoryStream, instance);
          return memoryStream.ToArray();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        return (byte[]) null;
      }
    }

    public byte[] FacebookSingleSignOn(byte[] data) => new byte[0];

    public byte[] LoginMemberFacebook(byte[] data) => new byte[0];

    public byte[] LoginMemberCookie(byte[] data) => new byte[0];

    public byte[] UncompleteAccount(byte[] data)
    {
      UserDocument result = UserManager.GetUser(Int32Proxy.Deserialize((Stream) new MemoryStream(data))).GetAwaiter().GetResult();
      bool instance = result == null || string.IsNullOrWhiteSpace(result.Member.PublicProfile.Name);
      using (MemoryStream bytes = new MemoryStream())
      {
        BooleanProxy.Serialize((Stream) bytes, instance);
        return bytes.ToArray();
      }
    }

    private string Hash(string pass)
    {
      using (SHA256Managed shA256Managed = new SHA256Managed())
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte num in shA256Managed.ComputeHash(Encoding.UTF8.GetBytes(pass)))
          stringBuilder.Append(num.ToString("x2"));
        return stringBuilder.ToString();
      }
    }

    private bool Authenticate(string authToken)
    {
      bool flag = false;
      if (string.IsNullOrEmpty(Program.Config.Hash))
        return true;
      if (authToken == null)
        return false;
      lock (AuthenticationWebService.Sync)
      {
        AuthenticationView authenticationView = AuthenticationWebService.authenticationView.Where<AuthenticationView>((Func<AuthenticationView, bool>) (x => x.authToken == authToken)).FirstOrDefault<AuthenticationView>();
        if (authenticationView == null)
          return false;
        if (authenticationView.isAuth)
        {
          AuthenticationWebService.authenticationView.Remove(authenticationView);
          flag = true;
        }
      }
      return flag;
    }
  }
}
