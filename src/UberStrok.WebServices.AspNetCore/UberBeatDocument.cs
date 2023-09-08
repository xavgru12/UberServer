using System.Collections.Generic;
using UberStrok.WebServices.AspNetCore.Core.Db;

public class UberBeatDocument : MongoDocument
{
    public List<string> ExceptionData { get; set; }
}
