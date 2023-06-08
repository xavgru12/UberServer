using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.AspNetCore.Contracts
{
    [ServiceContract]
    public interface IUserAsyncWebServiceContract
    {
        [OperationContract]
        Task<byte[]> ChangeMemberName(byte[] data);

        [OperationContract]
        Task<byte[]> IsDuplicateMemberName(byte[] data);

        [OperationContract]
        Task<byte[]> GenerateNonDuplicatedMemberNames(byte[] data);

        [OperationContract]
        Task<byte[]> GetMemberWallet(byte[] data);

        [OperationContract]
        Task<byte[]> GetInventory(byte[] data);

        // Token: 0x06000042 RID: 66
        [OperationContract]
        Task<byte[]> GetCurrencyDeposits(byte[] data);

        // Token: 0x06000043 RID: 67
        [OperationContract]
        Task<byte[]> GetItemTransactions(byte[] data);

        // Token: 0x06000044 RID: 68
        [OperationContract]
        Task<byte[]> GetPointsDeposits(byte[] data);

        // Token: 0x06000045 RID: 69
        [OperationContract]
        Task<byte[]> GetLoadout(byte[] data);

        // Token: 0x06000046 RID: 70
        [OperationContract]
        Task<byte[]> GetLoadoutServer(byte[] data);

        // Token: 0x06000047 RID: 71
        [OperationContract]
        Task<byte[]> SetLoadout(byte[] data);

        // Token: 0x06000048 RID: 72
        [OperationContract]
        Task EndOfMatch(byte[] data);

        // Token: 0x06000049 RID: 73
        [OperationContract]
        Task<byte[]> SetWallet(byte[] data);

        // Token: 0x0600004A RID: 74
        [OperationContract]
        Task<byte[]> GetMember(byte[] data);

        // Token: 0x0600004B RID: 75
        [OperationContract]
        Task<byte[]> GetMemberSessionData(byte[] data);

        // Token: 0x0600004C RID: 76
        [OperationContract]
        Task<byte[]> GetMemberListSessionData(byte[] data);

        // Token: 0x0600004D RID: 77
        [OperationContract]
        Task<byte[]> GetAppConfig();
    }
}