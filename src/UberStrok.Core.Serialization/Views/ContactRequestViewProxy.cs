using System;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    // Token: 0x0200001D RID: 29
    public static class ContactRequestViewProxy
    {
        // Token: 0x06000037 RID: 55 RVA: 0x0000388C File Offset: 0x00001A8C
        public static ContactRequestView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            ContactRequestView contactRequestView = new ContactRequestView();
            contactRequestView.InitiatorCmid = Int32Proxy.Deserialize(bytes);
            if ((num & 1) != 0)
            {
                contactRequestView.InitiatorMessage = StringProxy.Deserialize(bytes);
            }
            if ((num & 2) != 0)
            {
                contactRequestView.InitiatorName = StringProxy.Deserialize(bytes);
            }
            contactRequestView.ReceiverCmid = Int32Proxy.Deserialize(bytes);
            contactRequestView.RequestId = Int32Proxy.Deserialize(bytes);
            contactRequestView.SentDate = DateTimeProxy.Deserialize(bytes);
            contactRequestView.Status = EnumProxy<ContactRequestStatus>.Deserialize(bytes);
            return contactRequestView;
        }

        // Token: 0x06000038 RID: 56 RVA: 0x00003904 File Offset: 0x00001B04
        public static void Serialize(Stream stream, ContactRequestView instance)
        {
            int num = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Int32Proxy.Serialize(memoryStream, instance.InitiatorCmid);
                if (instance.InitiatorMessage != null)
                {
                    StringProxy.Serialize(memoryStream, instance.InitiatorMessage);
                }
                else
                {
                    num |= 1;
                }
                if (instance.InitiatorName != null)
                {
                    StringProxy.Serialize(memoryStream, instance.InitiatorName);
                }
                else
                {
                    num |= 2;
                }
                Int32Proxy.Serialize(memoryStream, instance.ReceiverCmid);
                Int32Proxy.Serialize(memoryStream, instance.RequestId);
                DateTimeProxy.Serialize(memoryStream, instance.SentDate);
                EnumProxy<ContactRequestStatus>.Serialize(memoryStream, instance.Status);
                Int32Proxy.Serialize(stream, ~num);
                memoryStream.WriteTo(stream);
            }
        }
    }
}
