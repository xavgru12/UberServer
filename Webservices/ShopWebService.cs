// Decompiled with JetBrains decompiler
// Type: Webservices.ShopWebService
// Assembly: Webservices, Version=1.0.15.0, Culture=neutral, PublicKeyToken=null
// MVID: CF64CEA0-C459-4923-B49E-7422D0147037
// Assembly location: C:\Users\Xaver\Documents\Uber\localServer\patrik\photon\deploy\Webservice-akash\Webservices.exe

using Cmune.DataCenter.Common.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using UberStrike.Core.Models.Views;
using UberStrike.Core.Serialization;
using UberStrike.Core.Types;
using Webservices.Database.Items;
using Webservices.Manager;

namespace Webservices
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
  public class ShopWebService : IShopWebServiceContract
  {
    private readonly string mysteryBoxesDataPath = Path.Combine(Environment.CurrentDirectory, "Data\\MysteryBoxes.json");
    private readonly string luckyDrawsDataPath = Path.Combine(Environment.CurrentDirectory, "Data\\LuckyDraws.json");
    private static UberStrikeItemShopClientView shopData;

    public ShopWebService(AuthenticationWebService authWebService)
    {
      Console.Write("Initializing ShopWebService...\t\t\t");
      ShopWebService.Initialize();
    }

    public static void Initialize()
    {
      while (true)
      {
        try
        {
          ShopWebService.shopData = JsonConvert.DeserializeObject<UberStrikeItemShopClientView>(File.ReadAllText("Data/Shop.json"));
          break;
        }
        catch
        {
          Thread.Sleep(100);
        }
      }
    }

    public static T DeserializeJsonWithNewtonsoftAt<T>(string path)
    {
      if (path == null)
        throw new ArgumentNullException(nameof (path));
      return !File.Exists(path) ? default (T) : JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
    }

    public static string SerializeJsonWithNewtonsoftAt<T>(T path) => JsonConvert.SerializeObject((object) path);

    public byte[] GetShop(byte[] data)
    {
      MemoryStream memoryStream = new MemoryStream();
      ShopWebService.shopData.ItemsRecommendationPerMap = new Dictionary<int, int>();
      UberStrikeItemShopClientViewProxy.Serialize((Stream) memoryStream, ShopWebService.shopData);
      return memoryStream.ToArray();
    }

    public byte[] BuyItem(byte[] data)
    {
      try
      {
        MemoryStream bytes1 = new MemoryStream(data);
        int itemId = Int32Proxy.Deserialize((Stream) bytes1);
        int id = Int32Proxy.Deserialize((Stream) bytes1);
        UberStrikeCurrencyType currencyType = EnumProxy<UberStrikeCurrencyType>.Deserialize((Stream) bytes1);
        BuyingDurationType buyingDurationType = EnumProxy<BuyingDurationType>.Deserialize((Stream) bytes1);
        UberstrikeItemType uberstrikeItemType = EnumProxy<UberstrikeItemType>.Deserialize((Stream) bytes1);
        EnumProxy<BuyingLocationType>.Deserialize((Stream) bytes1);
        EnumProxy<BuyingRecommendationType>.Deserialize((Stream) bytes1);
        bytes1.Close();
        using (MemoryStream bytes2 = new MemoryStream())
        {
          int instance = 0;
          UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
          if (result == null)
          {
            instance = 14;
          }
          else
          {
            BaseUberStrikeItemView uberStrikeItemView = (BaseUberStrikeItemView) null;
            int num = -1;
            switch (uberstrikeItemType)
            {
              case UberstrikeItemType.Weapon:
                uberStrikeItemView = (BaseUberStrikeItemView) ShopWebService.shopData.WeaponItems.FirstOrDefault<UberStrikeItemWeaponView>((Func<UberStrikeItemWeaponView, bool>) (c => c.ID == itemId));
                break;
              case UberstrikeItemType.Gear:
                uberStrikeItemView = (BaseUberStrikeItemView) ShopWebService.shopData.GearItems.FirstOrDefault<UberStrikeItemGearView>((Func<UberStrikeItemGearView, bool>) (c => c.ID == itemId));
                break;
              case UberstrikeItemType.QuickUse:
                uberStrikeItemView = (BaseUberStrikeItemView) ShopWebService.shopData.QuickItems.FirstOrDefault<UberStrikeItemQuickView>((Func<UberStrikeItemQuickView, bool>) (c => c.ID == itemId));
                break;
              case UberstrikeItemType.Functional:
                uberStrikeItemView = (BaseUberStrikeItemView) ShopWebService.shopData.FunctionalItems.FirstOrDefault<UberStrikeItemFunctionalView>((Func<UberStrikeItemFunctionalView, bool>) (c => c.ID == itemId));
                num = 1;
                break;
            }
            int price = uberStrikeItemView.Prices.FirstOrDefault<ItemPrice>((Func<ItemPrice, bool>) (c => c.Duration == buyingDurationType && c.Currency == currencyType)).Price;
            if (currencyType == UberStrikeCurrencyType.Credits)
            {
              if (result.Member.MemberWallet.Credits >= price)
              {
                result.Member.MemberWallet.Credits -= price;
                instance = 0;
              }
              else
              {
                instance = 8;
                goto label_34;
              }
            }
            else if (currencyType == UberStrikeCurrencyType.Points)
            {
              if (result.Member.MemberWallet.Points >= price)
              {
                result.Member.MemberWallet.Points -= price;
                instance = 0;
              }
              else
              {
                instance = 8;
                goto label_34;
              }
            }
            DateTime? nullable = new DateTime?(DateTime.Now);
            switch (buyingDurationType)
            {
              case BuyingDurationType.OneDay:
                nullable = new DateTime?(nullable.Value.AddDays(1.0));
                break;
              case BuyingDurationType.SevenDays:
                nullable = new DateTime?(nullable.Value.AddDays(7.0));
                break;
              case BuyingDurationType.ThirtyDays:
                nullable = new DateTime?(nullable.Value.AddDays(30.0));
                break;
              case BuyingDurationType.NinetyDays:
                nullable = new DateTime?(nullable.Value.AddDays(90.0));
                break;
              default:
                if (uberStrikeItemView.IsConsumable)
                {
                  nullable = new DateTime?();
                  break;
                }
                nullable = new DateTime?();
                num = -1;
                break;
            }
            if (instance == 0 && result != null)
            {
              bool flag = false;
              for (int index = 0; index < result.ItemInventory.Count; ++index)
              {
                if (result.ItemInventory[index].ItemId == itemId)
                {
                  flag = true;
                  result.ItemInventory[index].ExpirationDate = nullable;
                  result.ItemInventory[index].AmountRemaining = num;
                  break;
                }
              }
              if (!flag)
                result.ItemInventory.Add(new ItemInventoryView()
                {
                  Cmid = id,
                  ExpirationDate = nullable,
                  AmountRemaining = num,
                  ItemId = itemId
                });
              UserManager.Save(result).GetAwaiter().GetResult();
            }
          }
label_34:
          Int32Proxy.Serialize((Stream) bytes2, instance);
          return bytes2.ToArray();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("\n" + ex.ToString() + "\n");
        throw new Exception(ex.ToString());
      }
    }

    public byte[] BuyBundle(byte[] data) => new byte[0];

    public byte[] BuyPack(byte[] data)
    {
      MemoryStream bytes1 = new MemoryStream(data);
      int itemId = Int32Proxy.Deserialize((Stream) bytes1);
      int id = Int32Proxy.Deserialize((Stream) bytes1);
      PackType packType = EnumProxy<PackType>.Deserialize((Stream) bytes1);
      UberStrikeCurrencyType currencyType = EnumProxy<UberStrikeCurrencyType>.Deserialize((Stream) bytes1);
      UberstrikeItemType uberstrikeItemType = EnumProxy<UberstrikeItemType>.Deserialize((Stream) bytes1);
      EnumProxy<BuyingLocationType>.Deserialize((Stream) bytes1);
      EnumProxy<BuyingRecommendationType>.Deserialize((Stream) bytes1);
      bytes1.Close();
      using (MemoryStream bytes2 = new MemoryStream())
      {
        int instance = 0;
        UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
        if (result == null)
        {
          instance = 14;
        }
        else
        {
          int num1 = 0;
          ItemInventoryView itemInventoryView1 = (ItemInventoryView) null;
          foreach (ItemInventoryView itemInventoryView2 in result.ItemInventory)
          {
            if (itemInventoryView2.ItemId == itemId)
              itemInventoryView1 = itemInventoryView2;
          }
          if (itemInventoryView1 != null)
            num1 = itemInventoryView1.AmountRemaining;
          UberStrikeItemQuickView strikeItemQuickView = (UberStrikeItemQuickView) null;
          int num2 = 0;
          if (uberstrikeItemType == UberstrikeItemType.QuickUse)
          {
            strikeItemQuickView = ShopWebService.shopData.QuickItems.FirstOrDefault<UberStrikeItemQuickView>((Func<UberStrikeItemQuickView, bool>) (c => c.ID == itemId));
            if (strikeItemQuickView != null)
            {
              if (packType == PackType.One)
                num2 = 1;
              else if (packType == PackType.Two)
                num2 = 2;
              else if (packType == PackType.Three)
                num2 = 3;
              if (num2 + num1 > strikeItemQuickView.MaxOwnableAmount)
              {
                instance = 13;
                goto label_36;
              }
            }
            else
            {
              instance = 14;
              goto label_36;
            }
          }
          int price = strikeItemQuickView.Prices.FirstOrDefault<ItemPrice>((Func<ItemPrice, bool>) (c => c.Currency == currencyType && c.PackType == packType)).Price;
          if (currencyType == UberStrikeCurrencyType.Credits)
          {
            if (result.Member.MemberWallet.Credits >= price)
            {
              result.Member.MemberWallet.Credits -= price;
              instance = 0;
            }
            else
            {
              instance = 8;
              goto label_36;
            }
          }
          else if (currencyType == UberStrikeCurrencyType.Points)
          {
            if (result.Member.MemberWallet.Points >= price)
            {
              result.Member.MemberWallet.Points -= price;
              instance = 0;
            }
            else
            {
              instance = 8;
              goto label_36;
            }
          }
          if (instance == 0)
          {
            if (itemInventoryView1 == null)
              result.ItemInventory.Add(new ItemInventoryView()
              {
                Cmid = id,
                ExpirationDate = new DateTime?(),
                AmountRemaining = num2 + num1,
                ItemId = itemId
              });
            else
              itemInventoryView1.AmountRemaining += num2;
            UserManager.Save(result).GetAwaiter().GetResult();
          }
        }
label_36:
        Int32Proxy.Serialize((Stream) bytes2, instance);
        return bytes2.ToArray();
      }
    }

    public byte[] GetAllLuckyDraws_1(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        Dictionary<int, LuckyDrawUnityView> dictionary = JsonConvert.DeserializeObject<Dictionary<int, LuckyDrawUnityView>>(File.ReadAllText(this.luckyDrawsDataPath));
        ListProxy<LuckyDrawUnityView>.Serialize((Stream) bytes, (ICollection<LuckyDrawUnityView>) dictionary.Values.ToList<LuckyDrawUnityView>(), new ListProxy<LuckyDrawUnityView>.Serializer<LuckyDrawUnityView>(LuckyDrawUnityViewProxy.Serialize));
        return bytes.ToArray();
      }
    }

    public byte[] GetAllLuckyDraws_2(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        Dictionary<int, LuckyDrawUnityView> dictionary = JsonConvert.DeserializeObject<Dictionary<int, LuckyDrawUnityView>>(File.ReadAllText(this.luckyDrawsDataPath));
        ListProxy<LuckyDrawUnityView>.Serialize((Stream) bytes, (ICollection<LuckyDrawUnityView>) dictionary.Values.ToList<LuckyDrawUnityView>(), new ListProxy<LuckyDrawUnityView>.Serializer<LuckyDrawUnityView>(LuckyDrawUnityViewProxy.Serialize));
        return bytes.ToArray();
      }
    }

    public byte[] GetAllMysteryBoxs_1(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        Dictionary<int, MysteryBoxUnityView> dictionary = JsonConvert.DeserializeObject<Dictionary<int, MysteryBoxUnityView>>(File.ReadAllText(this.mysteryBoxesDataPath));
        ListProxy<MysteryBoxUnityView>.Serialize((Stream) bytes, (ICollection<MysteryBoxUnityView>) dictionary.Values.ToList<MysteryBoxUnityView>(), new ListProxy<MysteryBoxUnityView>.Serializer<MysteryBoxUnityView>(MysteryBoxUnityViewProxy.Serialize));
        return bytes.ToArray();
      }
    }

    public byte[] GetAllMysteryBoxs_2(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
        return memoryStream.ToArray();
    }

    public byte[] GetBundles(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
        return memoryStream.ToArray();
    }

    public byte[] GetLuckyDraw(byte[] data)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        LuckyDrawUnityViewProxy.Serialize((Stream) memoryStream, new LuckyDrawUnityView()
        {
          Id = 20,
          Category = BundleCategoryType.Login,
          Description = "Win something and fuck off",
          IconUrl = "http://localhost/2.0/Images/playerIcon.png",
          IsAvailableInShop = false,
          LuckyDrawSets = new List<LuckyDrawSetUnityView>()
          {
            new LuckyDrawSetUnityView()
            {
              Id = 20,
              ImageUrl = "http://localhost/2.0/Images/redBoxImage.png",
              LuckyDrawId = 1,
              ExposeItemsToPlayers = true,
              SetWeight = 100,
              CreditsAttributed = 1500,
              PointsAttributed = 1500,
              LuckyDrawSetItems = new List<BundleItemView>()
            }
          },
          Name = "Daily Lucky Draw",
          Price = 0,
          UberStrikeCurrencyType = UberStrikeCurrencyType.Points
        });
        return memoryStream.ToArray();
      }
    }

    public byte[] GetMysteryBox(byte[] data) => new byte[0];

    public byte[] RollLuckyDraw(byte[] data)
    {
      using (MemoryStream bytes = new MemoryStream())
      {
        Int32Proxy.Serialize((Stream) bytes, 0);
        return bytes.ToArray();
      }
    }

    public byte[] RollMysteryBox(byte[] data)
    {
      int id;
      int mysteryBoxId;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        id = Int32Proxy.Deserialize((Stream) bytes);
        mysteryBoxId = Int32Proxy.Deserialize((Stream) bytes);
        EnumProxy<ChannelType>.Deserialize((Stream) bytes);
      }
      using (MemoryStream bytes1 = new MemoryStream())
      {
        MysteryBoxUnityView mysteryBoxUnityView = JsonConvert.DeserializeObject<Dictionary<int, MysteryBoxUnityView>>(File.ReadAllText(this.mysteryBoxesDataPath)).Values.FirstOrDefault<MysteryBoxUnityView>((Func<MysteryBoxUnityView, bool>) (c => c.Id == mysteryBoxId));
        if (mysteryBoxUnityView == null)
        {
          ListProxy<MysteryBoxWonItemUnityView>.Serialize((Stream) bytes1, (ICollection<MysteryBoxWonItemUnityView>) new List<MysteryBoxWonItemUnityView>(), new ListProxy<MysteryBoxWonItemUnityView>.Serializer<MysteryBoxWonItemUnityView>(MysteryBoxWonItemUnityViewProxy.Serialize));
          return bytes1.ToArray();
        }
        MysteryBoxWonItemUnityView wonItemUnityView = new MysteryBoxWonItemUnityView();
        int maxValue = mysteryBoxUnityView.MysteryBoxItems.Max<BundleItemView>((Func<BundleItemView, int>) (c => c.BundleId));
        int num = new Random().Next(0, maxValue);
        if (num <= mysteryBoxUnityView.PointsAttributedWeight)
          wonItemUnityView.PointWon = mysteryBoxUnityView.PointsAttributed;
        else if (num > mysteryBoxUnityView.PointsAttributedWeight && num <= mysteryBoxUnityView.CreditsAttributedWeight)
          wonItemUnityView.CreditWon = mysteryBoxUnityView.CreditsAttributed;
        else if (num == maxValue)
          wonItemUnityView.ItemIdWon = mysteryBoxUnityView.MysteryBoxItems[0].ItemId;
        UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
        if (mysteryBoxUnityView.UberStrikeCurrencyType == UberStrikeCurrencyType.Credits)
          result.Member.MemberWallet.Credits -= mysteryBoxUnityView.Price;
        else
          result.Member.MemberWallet.Points -= mysteryBoxUnityView.Price;
        result.Member.MemberWallet.Credits += wonItemUnityView.CreditWon;
        result.Member.MemberWallet.Points += wonItemUnityView.PointWon;
        UserManager.Save(result).GetAwaiter().GetResult();
        MemoryStream bytes2 = bytes1;
        List<MysteryBoxWonItemUnityView> instance = new List<MysteryBoxWonItemUnityView>();
        instance.Add(wonItemUnityView);
        ListProxy<MysteryBoxWonItemUnityView>.Serializer<MysteryBoxWonItemUnityView> serialization = new ListProxy<MysteryBoxWonItemUnityView>.Serializer<MysteryBoxWonItemUnityView>(MysteryBoxWonItemUnityViewProxy.Serialize);
        ListProxy<MysteryBoxWonItemUnityView>.Serialize((Stream) bytes2, (ICollection<MysteryBoxWonItemUnityView>) instance, serialization);
        return bytes1.ToArray();
      }
    }

    public byte[] UseConsumableItem(byte[] data)
    {
      int id;
      using (MemoryStream bytes = new MemoryStream(data))
      {
        id = Int32Proxy.Deserialize((Stream) bytes);
        Int32Proxy.Deserialize((Stream) bytes);
      }
      using (MemoryStream bytes = new MemoryStream())
      {
        UserDocument result = UserManager.GetUser(id).GetAwaiter().GetResult();
        if (result != null)
        {
          for (int index = 0; index < result.ItemInventory.Count; ++index)
          {
            if (result.ItemInventory[index] != null)
            {
              if (result.ItemInventory[index].AmountRemaining > 0)
              {
                --result.ItemInventory[index].AmountRemaining;
                BooleanProxy.Serialize((Stream) bytes, true);
                UserManager.Save(result).GetAwaiter().GetResult();
              }
              else if (result.ItemInventory[index].AmountRemaining <= 0)
                BooleanProxy.Serialize((Stream) bytes, false);
            }
            else
              BooleanProxy.Serialize((Stream) bytes, false);
          }
        }
        return bytes.ToArray();
      }
    }

    public byte[] BuyMasBundle(byte[] data) => new byte[0];

    public byte[] BuyiPadBundle(byte[] data) => new byte[0];

    public byte[] BuyiPhoneBundle(byte[] data) => new byte[0];
  }
}
