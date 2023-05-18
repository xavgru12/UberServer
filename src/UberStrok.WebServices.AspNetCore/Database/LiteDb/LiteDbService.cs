using LiteDB;

namespace UberStrok.WebServices.AspNetCore.Database.LiteDb
{
    public class LiteDbService : IDbService
    {
        private readonly LiteDatabase _db;

        private readonly LiteDbSessionCollection _sessions;
        private readonly LiteDbClanCollection _clans;
        private readonly LiteDbMemberCollection _members;
        private readonly LiteDbClanInvitationCollection _clanInvite;
        public IDbSessionCollection Sessions => _sessions;
        public IDbClanCollection Clans => _clans;
        public IDbMemberCollection Members => _members;
        public IDbClanInvitationCollection ClanInvitations => _clanInvite;

        public LiteDbService()
        {
            _db = new LiteDatabase(new ConnectionString(@"uberstrok.db")
            {
                Connection = ConnectionType.Shared
            });
            _clans = new LiteDbClanCollection(_db);
            _members = new LiteDbMemberCollection(_db);
            _sessions = new LiteDbSessionCollection(_db);
            _clanInvite = new LiteDbClanInvitationCollection(_db);
        }
    }
}
