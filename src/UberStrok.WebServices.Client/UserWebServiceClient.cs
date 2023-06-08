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

        public LoadoutView GetLoadoutServer(string serviceAuth, string authToken)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, serviceAuth);
                StringProxy.Serialize(memoryStream, authToken);
                using (MemoryStream bytes = new MemoryStream(base.Channel.GetLoadoutServer(memoryStream.ToArray())))
                {
                    return LoadoutViewProxy.Deserialize(bytes);
                }
            }
        }

        public ApplicationConfigurationView GetAppConfig()
        {
            using (new MemoryStream())
            {
                using (MemoryStream bytes = new MemoryStream(base.Channel.GetAppConfig()))
                {
                    return ApplicationConfigurationViewProxy.Deserialize(bytes);
                }
            }
        }

        public void SetWallet(string authToken, MemberWalletView walletView)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                MemberWalletViewProxy.Serialize(memoryStream, walletView);
                base.Channel.SetWallet(memoryStream.ToArray());
            }
        }

        public void SendEndGame(string authToken, StatsCollectionView totalStats, StatsCollectionView bestStats)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                StringProxy.Serialize(memoryStream, authToken);
                StatsCollectionViewProxy.Serialize(memoryStream, totalStats);
                StatsCollectionViewProxy.Serialize(memoryStream, bestStats);
                base.Channel.EndOfMatch(memoryStream.ToArray());
            }
        }
    }

}
