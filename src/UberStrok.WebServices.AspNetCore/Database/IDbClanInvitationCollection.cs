using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database
{
    public interface IDbClanInvitationCollection : IDbCollection<GroupInvitation>
    {
        // Space
        Task<bool> IsInvited(int clanId, int invitee);
    }
}
