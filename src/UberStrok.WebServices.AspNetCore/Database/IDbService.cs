namespace UberStrok.WebServices.AspNetCore.Database
{
    public interface IDbService
    {
        IDbSessionCollection Sessions { get; }
        IDbClanCollection Clans { get; }
        IDbMemberCollection Members { get; }
        IDbClanInvitationCollection ClanInvitations { get; }
    }
}
