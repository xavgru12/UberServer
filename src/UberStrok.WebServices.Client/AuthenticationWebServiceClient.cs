using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Serialization;
using UberStrok.Core.Serialization.Views;
using UberStrok.Core.Views;
using UberStrok.WebServices.Contracts;

namespace UberStrok.WebServices.Client
{
    public class AuthenticationWebServiceClient : BaseWebServiceClient<IAuthenticationWebServiceContract>
    {
        public AuthenticationWebServiceClient(string endPoint) : base(endPoint, "AuthenticationWebService")
        {
            // Space
        }

        public AccountCompletionResultView CompleteAccount(int cmid, string name, ChannelType channelType, string locale, string machineId)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, cmid);
                StringProxy.Serialize(bytes, name);
                EnumProxy<ChannelType>.Serialize(bytes, channelType);
                StringProxy.Serialize(bytes, locale);
                StringProxy.Serialize(bytes, machineId);

                var data = Channel.CompleteAccount(bytes.ToArray());
                using (var inBytes = new MemoryStream(data))
                    return AccountCompletionResultViewProxy.Deserialize(inBytes);
            }
        }

        public MemberAuthenticationResultView LoginSteam(string steamId, string authToken, string machineId, string version)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, version);
                StringProxy.Serialize(memoryStream, steamId);
                StringProxy.Serialize(memoryStream, authToken);
                StringProxy.Serialize(memoryStream, machineId);
                StringProxy.Serialize(memoryStream, "192.168.0.105");
                using (MemoryStream bytes = new MemoryStream(base.Channel.LoginSteam(memoryStream.ToArray())))
                {
                    return MemberAuthenticationResultViewProxy.Deserialize(bytes);
                }
            }
        }
    }
}
