using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ClanMemberViewProxy
    {
        public static ClanMemberView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            ClanMemberView clanMemberView = new ClanMemberView
            {
                Cmid = Int32Proxy.Deserialize(bytes),
                JoiningDate = DateTimeProxy.Deserialize(bytes),
                Lastlogin = DateTimeProxy.Deserialize(bytes)
            };
            if ((num & 1) != 0)
            {
                clanMemberView.Name = StringProxy.Deserialize(bytes);
            }
            clanMemberView.Position = EnumProxy<GroupPosition>.Deserialize(bytes);
            return clanMemberView;
        }

        public static void Serialize(Stream stream, ClanMemberView instance)
        {
            int num = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Int32Proxy.Serialize(memoryStream, instance.Cmid);
                DateTimeProxy.Serialize(memoryStream, instance.JoiningDate);
                DateTimeProxy.Serialize(memoryStream, instance.Lastlogin);
                if (instance.Name != null)
                {
                    StringProxy.Serialize(memoryStream, instance.Name);
                }
                else
                {
                    num |= 1;
                }
                EnumProxy<GroupPosition>.Serialize(memoryStream, instance.Position);
                Int32Proxy.Serialize(stream, ~num);
                memoryStream.WriteTo(stream);
            }
        }
    }
}
