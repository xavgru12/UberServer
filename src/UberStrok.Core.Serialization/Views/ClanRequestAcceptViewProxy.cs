using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ClanRequestAcceptViewProxy
    {
        public static ClanRequestAcceptView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            ClanRequestAcceptView clanRequestAcceptView = new ClanRequestAcceptView
            {
                ActionResult = Int32Proxy.Deserialize(bytes),
                ClanRequestId = Int32Proxy.Deserialize(bytes)
            };
            if ((num & 1) != 0)
            {
                clanRequestAcceptView.ClanView = ClanViewProxy.Deserialize(bytes);
            }
            return clanRequestAcceptView;
        }

        public static void Serialize(Stream stream, ClanRequestAcceptView instance)
        {
            int num = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Int32Proxy.Serialize(memoryStream, instance.ActionResult);
                Int32Proxy.Serialize(memoryStream, instance.ClanRequestId);
                if (instance.ClanView != null)
                {
                    ClanViewProxy.Serialize(memoryStream, instance.ClanView);
                }
                else
                {
                    num |= 1;
                }
                Int32Proxy.Serialize(stream, ~num);
                memoryStream.WriteTo(stream);
            }
        }
    }
}
