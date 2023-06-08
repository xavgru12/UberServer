using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using UberStrok.WebServices.AspNetCore.Core.Db;
using UberStrok.WebServices.AspNetCore.Core.Db.Tables;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class ServerManager
    {
        private readonly MongoDatabase<UberBeatDocument> sm_database;
        private UberBeatDocument _document;
        public UberBeatDocument Document
        {
            get
            {
                _document = sm_database.Collection.Find(_ => true).FirstOrDefault();
                return _document;
            }
        }

        public ServerManager(UberBeatTable table)
        {
            sm_database = table.Table;
            _document = sm_database.Collection.Find(_ => true).FirstOrDefault();
            _document ??= CreateEmpty();
        }

        public void Append(string exceptiondata)
        {
            _document.ExceptionData.Add(exceptiondata);
            _ = sm_database.Collection.ReplaceOne((UberBeatDocument f) => f.Id == _document.Id, _document, (ReplaceOptions)null, default);
        }

        public void Remove(string exceptiondata)
        {
            _ = _document.ExceptionData.Remove(exceptiondata);
            _ = sm_database.Collection.ReplaceOne((UberBeatDocument f) => f.Id == _document.Id, _document, (ReplaceOptions)null, default);
        }

        public string GetExceptionData()
        {
            return string.Join(Environment.NewLine, _document.ExceptionData);
        }

        public UberBeatDocument CreateEmpty()
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Note Dont forget to add Exception Data to DB\n");
            Console.ResetColor();
            UberBeatDocument doc = new UberBeatDocument
            {
                ExceptionData = new List<string> { "Default string" }
            };
            sm_database.Collection.InsertOne(doc);
            return doc;
        }
    }
}
