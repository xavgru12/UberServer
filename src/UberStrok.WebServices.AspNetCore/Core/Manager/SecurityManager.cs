using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UberStrok.WebServices.AspNetCore.Core.Db;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Db.Tables;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class SecurityManager
    {
        private readonly HashSet<int> sm_cmidBans;

        private readonly HashSet<string> sm_ipBans;

        private readonly HashSet<string> sm_hwdBans;

        private readonly MongoDatabase<SecurityDocument> sm_database;

        private readonly FilterDefinition<SecurityDocument> sm_filter = Builders<SecurityDocument>.Filter.Eq((SecurityDocument f) => f.Type, "Security");

        public SecurityManager(SecurityTable table)
        {
            sm_database = table.Table;
            SecurityDocument document = sm_database.Collection.Find(sm_filter).FirstOrDefault();
            if (document != null)
            {
                sm_cmidBans = document.CmidBans;
                sm_ipBans = document.IpBans;
                sm_hwdBans = document.HwdBans;
            }
            else
            {
                sm_cmidBans = new HashSet<int>();
                sm_ipBans = new HashSet<string>();
                sm_hwdBans = new HashSet<string>();
            }
        }

        public bool IsCmidBanned(int cmid)
        {
            return sm_cmidBans.Contains(cmid);
        }

        public void BanCmid(int cmid)
        {
            if (!sm_cmidBans.Contains(cmid))
            {
                _ = sm_cmidBans.Add(cmid);
                _ = sm_database.Collection.UpdateOneAsync(sm_filter, Builders<SecurityDocument>.Update.AddToSet((SecurityDocument document) => document.CmidBans, cmid), new UpdateOptions
                {
                    IsUpsert = true
                });
            }
        }

        public void UnbanCmid(int cmid)
        {
            if (sm_cmidBans.Contains(cmid))
            {
                _ = sm_cmidBans.Remove(cmid);
                _ = sm_database.Collection.UpdateOneAsync(sm_filter, Builders<SecurityDocument>.Update.Pull((SecurityDocument document) => document.CmidBans, cmid), new UpdateOptions
                {
                    IsUpsert = true
                });
            }
        }

        public bool IsHwdBanned(string hwd)
        {
            return hwd == null ? throw new ArgumentNullException("hwd") : sm_hwdBans.Contains(hwd);
        }

        public void BanHwd(string hwd)
        {
            if (!sm_hwdBans.Contains(hwd))
            {
                _ = sm_hwdBans.Add(hwd);
                _ = sm_database.Collection.UpdateOneAsync(sm_filter, Builders<SecurityDocument>.Update.AddToSet((SecurityDocument document) => document.HwdBans, hwd), new UpdateOptions
                {
                    IsUpsert = true
                });
            }
        }

        public bool IsIpBanned(string ip)
        {
            return ip == null ? throw new ArgumentNullException("ip") : sm_ipBans.Contains(ip);
        }

        public void BanIp(string ip)
        {
            if (!sm_ipBans.Contains(ip))
            {
                _ = sm_ipBans.Add(ip);
                _ = sm_database.Collection.UpdateOneAsync(sm_filter, Builders<SecurityDocument>.Update.AddToSet((SecurityDocument document) => document.IpBans, ip), new UpdateOptions
                {
                    IsUpsert = true
                });
            }
        }
    }
}
