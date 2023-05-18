using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ClanRequestDeclineViewProxy
    {
        public static ClanRequestDeclineView Deserialize(Stream bytes)
        {
            return new ClanRequestDeclineView
            {
                ActionResult = Int32Proxy.Deserialize(bytes),
                ClanRequestId = Int32Proxy.Deserialize(bytes)
            };
        }

        public static void Serialize(Stream stream, ClanRequestDeclineView instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Int32Proxy.Serialize(memoryStream, instance.ActionResult);
                Int32Proxy.Serialize(memoryStream, instance.ClanRequestId);
                memoryStream.WriteTo(stream);
            }
        }
    }
}
