using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Serialization;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{
    public class RelationshipWebServiceClient : BaseWebServiceClient<IRelationshipWebServiceContract>
    {
        public RelationshipWebServiceClient(string endPoint)
            : base(endPoint, "RelationshipWebService")
        {
        }

        public void SendContactRequest(string authToken, int receiverCmid, string message, Action callback, Action<Exception> handler)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                Int32Proxy.Serialize(memoryStream, receiverCmid);
                StringProxy.Serialize(memoryStream, message);
                base.Channel.SendContactRequest(memoryStream.ToArray());
            }
        }

        public List<ContactRequestView> GetContactRequest(string authToken)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                using (MemoryStream bytes = new MemoryStream(base.Channel.GetContactRequests(memoryStream.ToArray())))
                {
                    return ListProxy<ContactRequestView>.Deserialize(bytes, ContactRequestViewProxy.Deserialize);
                }
            }
        }

        public PublicProfileView AcceptContactRequest(string authToken, int contactRequestId)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                Int32Proxy.Serialize(memoryStream, contactRequestId);
                using (MemoryStream bytes = new MemoryStream(base.Channel.AcceptContactRequest(memoryStream.ToArray())))
                {
                    return PublicProfileViewProxy.Deserialize(bytes);
                }
            }
        }

        public bool DeclineContactRequest(string authToken, int contactRequestId)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                Int32Proxy.Serialize(memoryStream, contactRequestId);
                using (MemoryStream bytes = new MemoryStream(base.Channel.DeclineContactRequest(memoryStream.ToArray())))
                {
                    return BooleanProxy.Deserialize(bytes);
                }
            }
        }

        public MemberOperationResult DeleteContact(string authToken, int contactCmid)
        {
            //IL_002c: Unknown result type (might be due to invalid IL or missing references)
            //IL_0031: Unknown result type (might be due to invalid IL or missing references)
            //IL_0048: Unknown result type (might be due to invalid IL or missing references)
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                Int32Proxy.Serialize(memoryStream, contactCmid);
                using (MemoryStream bytes = new MemoryStream(base.Channel.DeleteContact(memoryStream.ToArray())))
                {
                    return EnumProxy<MemberOperationResult>.Deserialize(bytes);
                }
            }
        }

        public List<ContactGroupView> GetContactsByGroups(string authToken, bool populateFacebookIds)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                BooleanProxy.Serialize(memoryStream, populateFacebookIds);
                using (MemoryStream bytes = new MemoryStream(base.Channel.GetContactsByGroups(memoryStream.ToArray())))
                {
                    return ListProxy<ContactGroupView>.Deserialize(bytes, ContactGroupViewProxy.Deserialize);
                }
            }
        }
    }
}
