using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
  public static class PublicProfileViewProxy
  {
    public static void Serialize(Stream stream, PublicProfileView instance)
    {
      int num = 0;
      if (instance != null)
      {
        using (MemoryStream bytes = new MemoryStream())
        {
          EnumProxy<MemberAccessLevel>.Serialize((Stream) bytes, instance.AccessLevel);
          Int32Proxy.Serialize((Stream) bytes, instance.Cmid);
          EnumProxy<EmailAddressStatus>.Serialize((Stream) bytes, instance.EmailAddressStatus);
          if (instance.GroupTag != null)
            StringProxy.Serialize((Stream) bytes, instance.GroupTag);
          else
            num |= 1;
          BooleanProxy.Serialize((Stream) bytes, instance.IsChatDisabled);
          DateTimeProxy.Serialize((Stream) bytes, instance.LastLoginDate);
          if (instance.Name != null)
            StringProxy.Serialize((Stream) bytes, instance.Name);
          else
            num |= 2;
          Int32Proxy.Serialize(stream, ~num);
          bytes.WriteTo(stream);
        }
      }
      else
        Int32Proxy.Serialize(stream, 0);
    }

    public static PublicProfileView Deserialize(Stream bytes)
    {
      int num = Int32Proxy.Deserialize(bytes);
      PublicProfileView publicProfileView = (PublicProfileView) null;
      if (num != 0)
      {
        publicProfileView = new PublicProfileView();
        publicProfileView.AccessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);
        publicProfileView.Cmid = Int32Proxy.Deserialize(bytes);
        publicProfileView.EmailAddressStatus = EnumProxy<EmailAddressStatus>.Deserialize(bytes);
        if ((num & 1) != 0)
          publicProfileView.GroupTag = StringProxy.Deserialize(bytes);
        publicProfileView.IsChatDisabled = BooleanProxy.Deserialize(bytes);
        publicProfileView.LastLoginDate = DateTimeProxy.Deserialize(bytes);
        if ((num & 2) != 0)
          publicProfileView.Name = StringProxy.Deserialize(bytes);
      }
      return publicProfileView;
    }
  }
}
