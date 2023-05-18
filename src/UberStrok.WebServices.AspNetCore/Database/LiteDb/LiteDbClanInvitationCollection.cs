using LiteDB;
using System;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Models;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public class LiteDbClanInvitationCollection : LiteDbCollection<GroupInvitation>, IDbClanInvitationCollection
    {
        public LiteDbClanInvitationCollection(LiteDatabase db) : base(db)
        {
            // Space
        }

        public override Task DeleteAsync(GroupInvitation document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            Collection.Delete(document.Id);
            return Task.CompletedTask;
        }

        public Task<bool> IsInvited(int clanId, int invitee)
        {
            return Task.Run(() => Collection.FindOne(x => x.View.GroupId == clanId && x.View.InviteeCmid == invitee) != null);
        }
    }
}
