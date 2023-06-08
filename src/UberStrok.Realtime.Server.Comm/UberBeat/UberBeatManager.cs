using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UberStrok.Realtime.Server.Comm.UberBeat
{
    public class UberBeatManager
    {
        private class UserData
        {
            public int Cmid;

            public HashSet<string> Name = new HashSet<string>();

            public HashSet<string> HDD = new HashSet<string>();

            public HashSet<string> BIOS = new HashSet<string>();

            public HashSet<string> MAC = new HashSet<string>();

            public HashSet<string> MOTHERBOARD = new HashSet<string>();

            public HashSet<string> UNITY = new HashSet<string>();

            public bool isBanned;

            public string Date;
        }

        public string Database = "C:\\UberBeatDB\\Database.json";

        public string HackReport = "C:\\UberBeatDB\\Reports.txt";

        public string ExceptionData = "C:\\UberBeatDB\\ExceptionData.txt";

        public string MuteData = "C:\\UberBeatDB\\Mute.txt";

        public UberBeatManager()
        {
            if (!File.Exists(Database))
            {
                File.WriteAllText(Database, "");
            }
            if (!File.Exists(HackReport))
            {
                File.WriteAllText(HackReport, "");
            }
            if (!File.Exists(ExceptionData))
            {
                File.WriteAllText(ExceptionData, "");
            }
            if (!File.Exists(MuteData))
            {
                File.WriteAllText(MuteData, "");
            }
        }

        public bool CreateBan(int cmid, int date = -1)
        {
            bool result = false;
            string[] source = File.ReadAllLines(Database);
            source = source.Distinct().ToArray();
            for (int i = 0; i < source.Length; i++)
            {
                UserData userData = JsonConvert.DeserializeObject<UserData>(source[i]);
                if (userData.Cmid == cmid)
                {
                    result = true;
                    if (date != -1)
                    {
                        userData.Date = DateTime.UtcNow.AddMinutes(date).ToString();
                    }
                    else
                    {
                        userData.isBanned = true;
                    }
                    string text = JsonConvert.SerializeObject(userData);
                    source[i] = text;
                    File.WriteAllLines(Database, source);
                    break;
                }
            }
            return result;
        }

        public List<string> GetHWID(int cmid)
        {
            List<string> list = new List<string>();
            UserData uberBeatUser = GetUberBeatUser(cmid);
            foreach (string item in uberBeatUser.Name)
            {
                list.Add("NAME:" + item);
            }
            foreach (string item2 in uberBeatUser.HDD)
            {
                list.Add("HDD:" + item2);
            }
            foreach (string item3 in uberBeatUser.MAC)
            {
                list.Add("MAC:" + item3);
            }
            foreach (string item4 in uberBeatUser.MOTHERBOARD)
            {
                list.Add("MOTHERBOARD:" + item4);
            }
            foreach (string bIO in uberBeatUser.BIOS)
            {
                list.Add("BIOS:" + bIO);
            }
            foreach (string item5 in uberBeatUser.UNITY)
            {
                list.Add("UNITY:" + item5);
            }
            return list;
        }

        public bool Unban(int cmid)
        {
            bool result = false;
            HashSet<int> hashSet = FindAltCmid(cmid);
            string[] source = File.ReadAllLines(Database);
            source = source.Distinct().ToArray();
            foreach (int item in hashSet)
            {
                for (int i = 0; i < source.Length; i++)
                {
                    UserData userData = JsonConvert.DeserializeObject<UserData>(source[i]);
                    if (userData.Cmid == item)
                    {
                        userData.isBanned = false;
                        userData.Date = null;
                        string text = JsonConvert.SerializeObject(userData);
                        source[i] = text;
                        File.WriteAllLines(Database, source);
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private UserData FilterExceptionData(UserData userdata)
        {
            HashSet<string> second = new HashSet<string>(File.ReadAllLines(ExceptionData));
            return new UserData
            {
                Cmid = userdata.Cmid,
                Name = userdata.Name,
                Date = userdata.Date,
                isBanned = userdata.isBanned,
                HDD = new HashSet<string>(from i in userdata.HDD.Except(second)
                                          where !string.IsNullOrEmpty(i)
                                          select i),
                BIOS = new HashSet<string>(from i in userdata.BIOS.Except(second)
                                           where !string.IsNullOrEmpty(i)
                                           select i),
                MOTHERBOARD = new HashSet<string>(from i in userdata.MOTHERBOARD.Except(second)
                                                  where !string.IsNullOrEmpty(i)
                                                  select i),
                UNITY = new HashSet<string>(from i in userdata.UNITY.Except(second)
                                            where !string.IsNullOrEmpty(i)
                                            select i),
                MAC = new HashSet<string>(from i in userdata.MAC.Except(second)
                                          where !string.IsNullOrEmpty(i)
                                          select i)
            };
        }

        public HashSet<int> FindAltCmid(int cmid)
        {
            HashSet<int> hashSet = new HashSet<int>();
            UserData uberBeatUser = GetUberBeatUser(cmid);
            if (uberBeatUser == null)
            {
                _ = hashSet.Add(0);
                return hashSet;
            }
            uberBeatUser = FilterExceptionData(uberBeatUser);
            try
            {
                string[] array = File.ReadAllLines(Database);
                for (int i = 0; i < array.Length; i++)
                {
                    UserData userdata = JsonConvert.DeserializeObject<UserData>(array[i]);
                    userdata = FilterExceptionData(userdata);
                    userdata.HDD.IntersectWith(uberBeatUser.HDD);
                    userdata.BIOS.IntersectWith(uberBeatUser.BIOS);
                    userdata.MOTHERBOARD.IntersectWith(uberBeatUser.MOTHERBOARD);
                    userdata.MAC.IntersectWith(uberBeatUser.MAC);
                    userdata.UNITY.IntersectWith(uberBeatUser.UNITY);
                    if (0 + (userdata.UNITY.Count * 3) + userdata.HDD.Count + userdata.BIOS.Count + userdata.MOTHERBOARD.Count + userdata.MAC.Count >= 3)
                    {
                        _ = hashSet.Add(userdata.Cmid);
                    }
                }
                return hashSet;
            }
            catch
            {
                _ = hashSet.Add(0);
                return hashSet;
            }
        }

        public HashSet<string> FindAlt(int cmid)
        {
            HashSet<string> hashSet = new HashSet<string>();
            UserData uberBeatUser = GetUberBeatUser(cmid);
            uberBeatUser = FilterExceptionData(uberBeatUser);
            try
            {
                string[] array = File.ReadAllLines(Database);
                for (int i = 0; i < array.Length; i++)
                {
                    UserData userdata = JsonConvert.DeserializeObject<UserData>(array[i]);
                    userdata = FilterExceptionData(userdata);
                    userdata.HDD.IntersectWith(uberBeatUser.HDD);
                    userdata.BIOS.IntersectWith(uberBeatUser.BIOS);
                    userdata.MOTHERBOARD.IntersectWith(uberBeatUser.MOTHERBOARD);
                    userdata.MAC.IntersectWith(uberBeatUser.MAC);
                    userdata.UNITY.IntersectWith(uberBeatUser.UNITY);
                    if (0 + (userdata.UNITY.Count * 3) + userdata.HDD.Count + userdata.BIOS.Count + userdata.MOTHERBOARD.Count + userdata.MAC.Count < 3)
                    {
                        continue;
                    }
                    string text = "";
                    if (userdata.Name.Count > 0)
                    {
                        foreach (string item in userdata.Name)
                        {
                            text = (!string.IsNullOrEmpty(text)) ? (text + "\n" + item) : item;
                        }
                    }
                    else
                    {
                        text = "Player";
                    }
                    _ = hashSet.Add("Cmid: " + userdata.Cmid + "Name: " + text);
                }
                return hashSet;
            }
            catch
            {
                _ = hashSet.Add("Error");
                return hashSet;
            }
        }

        public HashSet<string> FindAccounts(string query)
        {
            HashSet<string> hashSet = new HashSet<string>();
            try
            {
                string[] array = File.ReadAllLines(Database);
                for (int i = 0; i < array.Length; i++)
                {
                    UserData userData = JsonConvert.DeserializeObject<UserData>(array[i]);
                    foreach (string item in userData.Name)
                    {
                        if (item.ToLower().Contains(query.ToLower()))
                        {
                            string text = string.Join("|", userData.Name.ToArray());
                            _ = hashSet.Add(userData.Cmid + " : " + text);
                            break;
                        }
                    }
                }
                return hashSet;
            }
            catch
            {
                _ = hashSet.Add("Error");
                return hashSet;
            }
        }

        private UserData GetUberBeatUser(int cmid)
        {
            string[] array = File.ReadAllLines(Database);
            for (int i = 0; i < array.Length; i++)
            {
                UserData userData = JsonConvert.DeserializeObject<UserData>(array[i]);
                if (userData.Cmid == cmid)
                {
                    return userData;
                }
            }
            return null;
        }

        public void Disconnect(CommPeer peer, string message = "Your Computer has been Banned")
        {
            peer.SendError(message);
            peer.Disconnect();
            peer.Dispose();
        }

        public void SendModulesSignatureRequest(CommPeer peer, string untrusted)
        {
            peer.Events.Lobby.SendModulesRequest(untrusted);
        }
    }

}
