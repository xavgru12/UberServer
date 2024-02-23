namespace UberStrok.Core.Common
{
    public enum MemberRegistrationResult
    {
        // Token: 0x040000A1 RID: 161
        Ok,
        // Token: 0x040000A2 RID: 162
        InvalidEmail,
        // Token: 0x040000A3 RID: 163
        InvalidName,
        // Token: 0x040000A4 RID: 164
        InvalidPassword,
        // Token: 0x040000A5 RID: 165
        DuplicateEmail,
        // Token: 0x040000A6 RID: 166
        DuplicateName,
        // Token: 0x040000A7 RID: 167
        DuplicateEmailName,
        // Token: 0x040000A8 RID: 168
        InvalidData,
        // Token: 0x040000A9 RID: 169
        InvalidHandle,
        // Token: 0x040000AA RID: 170
        DuplicateHandle,
        // Token: 0x040000AB RID: 171
        InvalidEsns,
        // Token: 0x040000AC RID: 172
        MemberNotFound,
        // Token: 0x040000AD RID: 173
        OffensiveName,
        // Token: 0x040000AE RID: 174
        IsIpBanned,
        // Token: 0x040000AF RID: 175
        EmailAlreadyLinkedToActualEsns,
        // Token: 0x040000B0 RID: 176
        Error_MemberNotCreated
    }
}
