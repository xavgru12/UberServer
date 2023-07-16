using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UberStrok.WebServices.AspNetCore.Core.Db.Items;
using UberStrok.WebServices.AspNetCore.Core.Discord;

namespace UberStrok.WebServices.AspNetCore.Core.Manager
{
    public class UberBeatManager
    {
        public List<string> ExceptionData { get; set; } = new List<string>();

        private readonly UserManager userManager;

        public UberBeatManager(UserManager userManager)
        {
            this.userManager = userManager;
        }

        public void Update(UserDocument document, string hwid)
        {
            try
            {
                UberBeat uberBeat = ParseHWIDToObject(hwid);
                if (uberBeat != null)
                {
                    document.HDD.UnionWith(uberBeat.HDD);
                    document.BIOS.UnionWith(uberBeat.BIOS);
                    document.MOTHERBOARD.UnionWith(uberBeat.MOTHERBOARD);
                    document.MAC.UnionWith(uberBeat.MAC);
                    document.UNITY.UnionWith(uberBeat.UNITY);
                    _ = userManager.Save(document);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public string Mute(int cmid, int duration = 0)
        {
            string result2;
            try
            {
                UserDocument result = userManager.GetUser(cmid).Result;
                if (duration > 0)
                {
                    result.UBMute = DateTime.UtcNow.AddMinutes(duration).ToString();
                    _ = userManager.Save(result);
                    result2 = string.Format("{0} has been muted for {1} minutes.", result.Profile.Name, duration.ToString());
                }
                else
                {
                    result.UBMute = "-1";
                    _ = userManager.Save(result);
                    result2 = string.Format("{0} has been muted permanently.", result.Profile.Name);
                }
            }
            catch (Exception ex)
            {
                result2 = ex.ToString();
            }
            return result2;
        }

        public string Unmute(int cmid)
        {
            string result2;
            try
            {
                UserDocument result = userManager.GetUser(cmid).Result;
                if (result == null)
                {
                    result2 = string.Format("User with cmid {0} does not exist.", cmid.ToString());
                }
                else if (result.UBMute == null)
                {
                    result2 = string.Format("User with cmid {0} and name {1} is not muted.", cmid.ToString(), result.Profile.Name);
                }
                else
                {
                    result.UBBan = null;
                    _ = userManager.Save(result);
                    result2 = string.Format("User with cmid {0} and name {1} has been unmuted.", cmid.ToString(), result.Profile.Name);
                }
            }
            catch (Exception ex)
            {
                result2 = ex.ToString();
            }
            return result2;
        }
        public int MuteDuration(UserDocument member, string hwid)
        {
            List<int> list = new List<int>();
            try
            {
                if (member != null)
                {
                    if (!string.IsNullOrEmpty(member.UBMute))
                    {
                        if (member.UBMute == "-1")
                        {
                            return -1;
                        }
                        DateTime dateTime = DateTime.Parse(member.UBMute);
                        if (dateTime > DateTime.UtcNow)
                        {
                            list.Add((int)(dateTime - DateTime.UtcNow).TotalMinutes + 1);
                        }
                        else if (dateTime <= DateTime.UtcNow)
                        {
                            member.UBMute = null;
                            _ = userManager.Save(member);
                        }
                    }
                    foreach (int cmid in AltCmids(member.Profile.Cmid))
                    {
                        list.Add(getDuration(cmid, true));
                    }
                }
                foreach (int cmid2 in AltCmids(hwid))
                {
                    list.Add(getDuration(cmid2, true));
                }
                return list.Contains(-1) ? -1 : list.Max();
            }
            catch
            {
                if (list.Count > 0)
                {
                    return list.Max();
                }
            }
            return 0;
        }

        public string Ban(int cmid, int duration = 0)
        {
            string result2;
            try
            {
                UserDocument result = userManager.GetUser(cmid).Result;
                if (duration > 0)
                {
                    result.UBBan = DateTime.UtcNow.AddMinutes(duration).ToString();
                    _ = userManager.Save(result);
                    result2 = string.Format("{0} has been banned for {1} minutes.", result.Profile.Name, duration.ToString());
                }
                else
                {
                    result.UBBan = "-1";
                    _ = userManager.Save(result);
                    result2 = string.Format("{0} has been banned permanently.", result.Profile.Name);
                }
            }
            catch (Exception ex)
            {
                result2 = ex.ToString();
            }
            return result2;
        }

        public string Unban(int cmid)
        {
            string result2;
            try
            {
                UserDocument result = userManager.GetUser(cmid).Result;
                if (result == null)
                {
                    result2 = string.Format("User with cmid {0} does not exist.", cmid.ToString());
                }
                else if (result.UBBan == null)
                {
                    result2 = string.Format("User with cmid {0} and name {1} is not banned.", cmid.ToString(), result.Profile.Name);
                }
                else
                {
                    result.UBBan = null;
                    _ = userManager.Save(result);
                    result2 = string.Format("User with cmid {0} and name {1} has been unbanned.", cmid.ToString(), result.Profile.Name);
                }
            }
            catch (Exception ex)
            {
                result2 = ex.ToString();
            }
            return result2;
        }

        // Token: 0x0600013F RID: 319 RVA: 0x00006E14 File Offset: 0x00005014
        public int BanDuration(UserDocument member, string hwid)
        {
            List<int> list = new List<int>();
            try
            {
                if (!string.IsNullOrEmpty(member.UBBan))
                {
                    int ubbanRemainingMinutes = UberBeatManager.GetUBBanRemainingMinutes(member.UBBan);
                    if (ubbanRemainingMinutes == -1)
                    {
                        return -1;
                    }
                    if (ubbanRemainingMinutes == 0)
                    {
                        member.UBBan = null;
                        _ = userManager.Save(member);
                    }
                }
                foreach (int cmid in AltCmids(member.Profile.Cmid))
                {
                    list.Add(getDuration(cmid, false));
                }
                foreach (int cmid2 in AltCmids(hwid))
                {
                    list.Add(getDuration(cmid2, false));
                }
                return list.Contains(-1) ? -1 : list.Max();
            }
            catch
            {
                if (list.Count > 0)
                {
                    return list.Max();
                }
            }
            return 0;
        }

        public static int GetUBBanRemainingMinutes(string ubban)
        {
            return ubban == "-1" ? -1 : (int)Math.Max(DateTime.Parse(ubban).Subtract(DateTime.UtcNow).TotalMinutes + 1.0, 0.0);
        }

        public int getDuration(int cmid, bool muteduration = false)
        {
            UserDocument result = userManager.GetUser(cmid).Result;
            string text = muteduration ? result.UBMute : result.UBBan;
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            int result2;
            try
            {
                DateTime dateTime = DateTime.Parse(text);
                if (dateTime > DateTime.UtcNow)
                {
                    result2 = (int)(dateTime - DateTime.UtcNow).TotalMinutes + 1;
                }
                else
                {
                    if (muteduration)
                    {
                        result.UBMute = null;
                    }
                    else
                    {
                        result.UBBan = null;
                    }
                    _ = userManager.Save(result);
                    result2 = 0;
                }
            }
            catch
            {
                result2 = 0;
            }
            return result2;
        }

        private void FilterExceptionData(ref UberBeat userdata)
        {
            HashSet<string> other = new HashSet<string>(ExceptionData);
            userdata.HDD.ExceptWith(other);
            userdata.BIOS.ExceptWith(other);
            userdata.MOTHERBOARD.ExceptWith(other);
            userdata.UNITY.ExceptWith(other);
            userdata.MAC.ExceptWith(other);
        }

        public static UberBeat ParseHWIDToObject(string data)
        {
            UberBeat uberBeat = new UberBeat();
            try
            {
                string[] array = data.Split("|", StringSplitOptions.None).Distinct<string>().ToArray<string>();
                foreach (string text in array)
                {
                    if (text.Contains("MAC:"))
                    {
                        string item = text.Replace("MAC:", "");
                        _ = uberBeat.MAC.Add(item);
                    }
                    if (text.Contains("HDD:"))
                    {
                        string item2 = text.Replace("HDD:", "");
                        _ = uberBeat.HDD.Add(item2);
                    }
                    if (text.Contains("BIOS:"))
                    {
                        string item3 = text.Replace("BIOS:", "");
                        _ = uberBeat.BIOS.Add(item3);
                    }
                    if (text.Contains("MOTHERBOARD:"))
                    {
                        string item4 = text.Replace("MOTHERBOARD:", "");
                        _ = uberBeat.MOTHERBOARD.Add(item4);
                    }
                    if (text.Contains("UNITY:"))
                    {
                        string item5 = text.Replace("UNITY:", "");
                        _ = uberBeat.UNITY.Add(item5);
                    }
                }
                return uberBeat;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return uberBeat;
        }
        public List<string> GetHWID(int cmid)
        {
            UserDocument result = userManager.GetUser(cmid).Result;
            List<string> list = new List<string>();
            if (result == null)
            {
                list.Add("Invalid CMID");
                return list;
            }
            foreach (string str in result.Names)
            {
                list.Add("NAME:" + str);
            }
            list.Add("STEAM ID:" + result.SteamId);
            foreach (string str2 in result.HDD)
            {
                list.Add("HDD:" + str2);
            }
            foreach (string str3 in result.MAC)
            {
                list.Add("MAC:" + str3);
            }
            foreach (string str4 in result.MOTHERBOARD)
            {
                list.Add("MOTHERBOARD:" + str4);
            }
            foreach (string str5 in result.BIOS)
            {
                list.Add("BIOS:" + str5);
            }
            foreach (string str6 in result.UNITY)
            {
                list.Add("UNITY:" + str6);
            }
            return list;
        }
        internal List<int> AltCmids(string hwid)
        {
            return FindMatchingCmid(ParseHWIDToObject(hwid));
        }
        public List<int> AltCmids(int cmid)
        {
            UserDocument doc = userManager.GetUser(cmid).GetAwaiter().GetResult();
            UberBeat ub = new UberBeat
            {
                HDD = doc.HDD,
                BIOS = doc.BIOS,
                MAC = doc.MAC,
                MOTHERBOARD = doc.MOTHERBOARD
            };
            return FindMatchingCmid(ub);
        }

        private List<int> FindMatchingCmid(UberBeat ub)
        {
            List<int> alts = new List<int>();
            FilterExceptionData(ref ub);
            HashSet<int>[] result = Task.WhenAll(userManager.MatchHWID("bios", ub.BIOS),
                userManager.MatchHWID("mac", ub.MAC),
                userManager.MatchHWID("mac", ub.MAC),
                userManager.MatchHWID("hdd", ub.HDD),
                userManager.MatchHWID("motherboard", ub.MOTHERBOARD)).GetAwaiter().GetResult();
            HashSet<int> bios = result[0];
            HashSet<int> mac = result[1];
            HashSet<int> hdd = result[2];
            HashSet<int> motherboard = result[3];
            List<int> overallAlts = new List<int>();
            overallAlts.AddRange(bios); overallAlts.AddRange(mac); overallAlts.AddRange(hdd); overallAlts.AddRange(motherboard);
            IEnumerable<IGrouping<int, int>> keypair = overallAlts.GroupBy(i => i);
            foreach (IGrouping<int, int> pair in keypair)
            {
                if (pair.Count() > 1 && !alts.Contains(pair.Key))
                {
                    alts.Add(pair.Key);
                }
            }
            return alts;
        }

        // Token: 0x06000147 RID: 327 RVA: 0x00007750 File Offset: 0x00005950
        public List<string> Search(string name)
        {
            List<string> result2;
            try
            {
                List<string> list = new List<string>();
                List<UserDocument> result = userManager.FindUser(name).Result;
                if (result.Count < 1)
                {
                    list.Add("Cant find name with such characters");
                    result2 = list;
                }
                else
                {
                    foreach (UserDocument userDocument in result)
                    {
                        string arg = (userDocument.Names.Count > 0) ? string.Join(Environment.NewLine, userDocument.Names) : userDocument.Profile.Name;
                        list.Add(string.Format("CMID: {0} Name: {1}", userDocument.Profile.Cmid, arg));
                    }
                    result2 = list;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                result2 = null;
            }
            return result2;
        }

        // Token: 0x06000148 RID: 328 RVA: 0x0000783C File Offset: 0x00005A3C
        public List<string> Alts(int cmid)
        {
            List<string> list = new List<string>();
            foreach (int id in AltCmids(cmid))
            {
                UserDocument result = userManager.GetUser(id).Result;
                if (result != null)
                {
                    list.Add(string.Format("CMID: {0} Name: {1}", result.Profile.Cmid, string.Join(Environment.NewLine, result.Names)));
                }
            }
            return list;
        }

        // Token: 0x06000149 RID: 329 RVA: 0x000078D0 File Offset: 0x00005AD0
        public List<string> Leaderboard(int limit, string key)
        {
            List<string> result2;
            try
            {
                List<UserDocument> result = userManager.Leaderboard(limit, key).Result;
                List<string> list = new List<string>();
                int num = 1;
                foreach (UserDocument userDocument in result)
                {
                    string text = null;
                    string text2 = getKda(userDocument.Kills, userDocument.Deaths).ToString();
                    if (!string.IsNullOrEmpty(userDocument.Profile.GroupTag))
                    {
                        text = "[" + userDocument.Profile.GroupTag + "] ";
                    }
                    list.Add(string.Format("[{0}]    > {1}{2}\n           Kills {3} - Level {4} - {5} XP - KDR {6}\n", new object[]
                    {
                        num,
                        text,
                        userDocument.Profile.Name,
                        userDocument.Kills,
                        userDocument.Statistics.Level,
                        userDocument.Statistics.Xp,
                        text2
                    }));
                    num++;
                }
                result2 = list;
            }
            catch
            {
                result2 = new string[]
                {
                    "Error",
                }.ToList<string>();
            }
            return result2;
        }

        // Token: 0x0600014A RID: 330 RVA: 0x00007A40 File Offset: 0x00005C40
        public List<string> bannedUsers()
        {
            List<string> result2;
            try
            {
                List<UserDocument> result = userManager.bannedUsers().Result;
                List<string> list = new List<string>();
                foreach (UserDocument userDocument in result)
                {
                    string arg = null;
                    if (!string.IsNullOrEmpty(userDocument.Profile.GroupTag))
                    {
                        arg = "{" + userDocument.Profile.GroupTag + "} ";
                    }
                    list.Add(string.Format("[{0}]  {1}{2}\n", userDocument.Profile.Cmid, arg, userDocument.Profile.Name));
                }
                result2 = list;
            }
            catch
            {
                result2 = new string[]
                {
                    "Error"
                }.ToList<string>();
            }
            return result2;
        }

        // Token: 0x0600014B RID: 331 RVA: 0x00007B24 File Offset: 0x00005D24
        private double getKda(double kills, double deaths)
        {
            if (deaths == 0.0)
            {
                deaths = 1.0;
            }
            double num = kills / deaths;
            return Math.Truncate(100.0 * num) / 100.0;
        }
    }
}
