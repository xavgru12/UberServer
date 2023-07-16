using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
  public static class MemberWalletViewProxy
  {
    public static void Serialize(Stream stream, MemberWalletView instance)
    {
      int num = 0;
      if (instance != null)
      {
        using (MemoryStream bytes = new MemoryStream())
        {
          Int32Proxy.Serialize((Stream) bytes, instance.Cmid);
          Int32Proxy.Serialize((Stream) bytes, instance.Credits);
          DateTimeProxy.Serialize((Stream) bytes, instance.CreditsExpiration);
          Int32Proxy.Serialize((Stream) bytes, instance.Points);
          DateTimeProxy.Serialize((Stream) bytes, instance.PointsExpiration);
          Int32Proxy.Serialize(stream, ~num);
          bytes.WriteTo(stream);
        }
      }
      else
        Int32Proxy.Serialize(stream, 0);
    }

    public static MemberWalletView Deserialize(Stream bytes)
    {
      int num = Int32Proxy.Deserialize(bytes);
      MemberWalletView memberWalletView = (MemberWalletView) null;
      if (num != 0)
      {
        memberWalletView = new MemberWalletView();
        memberWalletView.Cmid = Int32Proxy.Deserialize(bytes);
        memberWalletView.Credits = Int32Proxy.Deserialize(bytes);
        memberWalletView.CreditsExpiration = DateTimeProxy.Deserialize(bytes);
        memberWalletView.Points = Int32Proxy.Deserialize(bytes);
        memberWalletView.PointsExpiration = DateTimeProxy.Deserialize(bytes);
      }
      return memberWalletView;
    }
  }
}
