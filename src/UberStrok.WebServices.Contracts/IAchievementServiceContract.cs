using System.ServiceModel;
using System.Threading.Tasks;

namespace UberStrok.WebServices.Contracts
{
    [ServiceContract]
    public interface IAchievementServiceContract
    {
        [OperationContract]
        Task<byte[]> AddItemToInventory(byte[] data);

        [OperationContract]
        Task<byte[]> AddToWallet(byte[] data);

        [OperationContract]
        Task<byte[]> GetUserAchievement(byte[] data);

        [OperationContract]
        Task<byte[]> GetAllAchievements(byte[] data);
    }
}
