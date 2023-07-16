using System.IO;
using System.Net.Mail;
using System.Runtime.InteropServices;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{


    public class UserWebServiceClient : BaseWebServiceClient<IUserWebServiceContract>
    {
        public UserWebServiceClient(string endPoint)
            : base(endPoint, "UserWebService")
        {
        }

        public UberstrikeUserView GetMember(string authToken)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                using (MemoryStream bytes = new MemoryStream(base.Channel.GetMember(memoryStream.ToArray())))
                {
                    return UberstrikeUserViewProxy.Deserialize(bytes);
                }
            }
        }

        public MemberOperationResult SetLoadout(string serviceauth, string authToken, LoadoutView view)
        {
            using (MemoryStream bytes = new MemoryStream())
            {
                StringProxy.Serialize(bytes, serviceauth);
                StringProxy.Serialize(bytes, authToken);
                LoadoutViewProxy.Serialize(bytes, view);

                byte[] data = Channel.SetLoadout(bytes.ToArray());
                using (MemoryStream inBytes = new MemoryStream(data))
                {
                    return EnumProxy<MemberOperationResult>.Deserialize(inBytes);
                }
            }
        }

        public LoadoutView GetLoadout(string authToken)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                using (MemoryStream bytes = new MemoryStream(base.Channel.GetLoadout(memoryStream.ToArray())))
                {
                    return LoadoutViewProxy.Deserialize(bytes);
                }
            }
        }
    }

}
