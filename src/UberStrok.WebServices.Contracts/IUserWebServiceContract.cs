using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IUserWebServiceContract
    {
        [OperationContract]
        Task<byte[]> ChangeMemberName(byte[] data);

        [OperationContract]
        Task<byte[]> IsDuplicateMemberName(byte[] data);

        [OperationContract]
        Task<byte[]> GenerateNonDuplicatedMemberNames(byte[] data);

        [OperationContract]
        byte[] GetMemberWallet(byte[] data);

        [OperationContract]
        byte[] GetInventory(byte[] data);

        [OperationContract]
        byte[] GetCurrencyDeposits(byte[] data);

        [OperationContract]
        byte[] GetItemTransactions(byte[] data);

        [OperationContract]
        byte[] GetPointsDeposits(byte[] data);

        [OperationContract]
        byte[] GetLoadout(byte[] data);

        [OperationContract]
        byte[] GetLoadoutServer(byte[] data);

        [OperationContract]
        byte[] SetLoadout(byte[] data);

        [OperationContract]
        void EndOfMatch(byte[] data);

        [OperationContract]
        byte[] SetWallet(byte[] data);

        [OperationContract]
        byte[] GetMember(byte[] data);

        [OperationContract]
        byte[] GetMemberSessionData(byte[] data);

        [OperationContract]
        byte[] GetMemberListSessionData(byte[] data);

        [OperationContract]
        byte[] GetAppConfig();
    }

}