using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace UberStrok.Realtime.Server.Comm.UberBeat
{
    public class UberBeat
    {
        public static string bl1 = "allochook-i386.dll|allochook-x86_64.dll|ced3d10hook.dll|ced3d10hook64.dll|ced3d11hook.dll|ced3d11hook64.dll|ced3d9hook.dll|ced3d9hook64.dll|speedhack-i386.dll|speedhack-x86_64.dll|vehdebug-i386.dll|vehdebug-x86_64.dll";

        public static string bl2 = "ceregreset.exe|Cheat Engine.exe|cheatengine-i386.exe|cheatengine-x86_64.exe|DotNetDataCollector32.exe|DotNetDataCollector64.exe|gtutorial-i386.exe|gtutorial-x86_64.exe|Kernelmoduleunloader.exe|Tutorial-i386.exe|Tutorial-x86_64.exe|Sigma Engine 1.0.exe";

        public static string trusteduber = "\\uberstrike_data\\mono\\mono.dll|\\uberstrike_data\\plugins\\csteamworks.dll|\\steam_api.dll|\\uberstrike_data\\plugins\\uberheartbeat.dll|\\uberstrike_data\\plugins\\uberbeat.dll";

        public void Initialize(string report, CommPeer peer)
        {
            //IL_002b: Unknown result type (might be due to invalid IL or missing references)
            //IL_0032: Invalid comparison between Unknown and I4
            if ((int)peer.Actor.View.AccessLevel == 10)
            {
                return;
            }
            new Thread((ThreadStart)delegate
            {
                HashSet<string> hashSet = new HashSet<string>();
                HashSet<string> hashSet2 = new HashSet<string>();
                HashSet<string> hashSet3 = new HashSet<string>();
                HashSet<string> hashSet4 = new HashSet<string>();
                string[] array = report.Split('|');
                foreach (string text in array)
                {
                    if (text.StartsWith("PP:"))
                    {
                        string item = text.Replace("PP:", "");
                        _ = hashSet.Add(item);
                    }
                    if (text.StartsWith("M:"))
                    {
                        string item2 = text.Replace("M:", "");
                        _ = hashSet3.Add(item2);
                    }
                    if (text.StartsWith("W:"))
                    {
                        string item3 = text.Replace("W:", "");
                        _ = hashSet2.Add(item3);
                    }
                    if (text.StartsWith("P:"))
                    {
                        string item4 = text.Replace("P:", "");
                        _ = hashSet4.Add(item4);
                    }
                }
                peer.Actor.Processes = hashSet;
                peer.Actor.Modules = hashSet3;
                peer.Actor.Windows = hashSet2;
                peer.Actor.Exe = hashSet4;
                string[] exe = new List<string>(peer.Actor.Exe).ToArray();
                string[] modules = new List<string>(peer.Actor.Modules).ToArray();
                string[] window = new List<string>(peer.Actor.Windows).ToArray();
                string[] blacklisted_dll = bl1.Split('|');
                string[] blacklisted_exe = bl2.Split('|');
                _ = CheckBlacklistedExe(exe, blacklisted_exe, peer);
                _ = CheckBlacklistedDll(modules, blacklisted_dll, peer);
                _ = CheckBlacklistedWindow(window, peer);
                CheckInjection(modules, peer);
            }).Start();
        }

        private void CheckInjection(string[] modules, CommPeer peer)
        {
            List<string> list = new List<string>();
            foreach (string item in modules)
            {
                list.Add(item);
            }
            List<string> list2 = list.Except(peer.Actor.TrustedModules).ToList();
            if (list2.Count >= 1)
            {
                string untrusted = string.Join<string>("|", list2);
                peer.Events.Lobby.SendModulesRequest(untrusted);
            }
        }

        private int CheckBlacklistedExe(string[] exe, string[] blacklisted_exe, CommPeer peer)
        {
            foreach (string text in exe)
            {
                foreach (string text2 in blacklisted_exe)
                {
                    if (text2.Contains(text) || text.Contains(text2))
                    {
                        End(0, text, peer);
                    }
                }
            }
            return 0;
        }

        private int CheckBlacklistedDll(string[] modules, string[] blacklisted_dll, CommPeer peer)
        {
            foreach (string text in modules)
            {
                try
                {
                    for (int j = 0; j < blacklisted_dll.Length; j++)
                    {
                        if (blacklisted_dll[j] == text.Substring(text.LastIndexOf('\\') + 1))
                        {
                            End(0, text, peer);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return 0;
        }

        private int CheckBlacklistedWindow(string[] window, CommPeer peer)
        {
            foreach (string text in window)
            {
                string text2 = text.ToLower();
                if (!text2.Contains("google") && !text2.Contains("chrome") && !text2.Contains("mozilla") && !text2.Contains("firefox") && !text2.Contains("microsoft") && !text2.Contains("edge") && !text2.Contains("discord") && !text2.Contains("opera"))
                {
                    if ((text2.Contains("sigma") || text2.Contains("cheat")) && text2.Contains("engine"))
                    {
                        End(0, "Cheat Engine Detected." + text, peer);
                    }
                    if (text2.Contains("ahk") || text2.Contains("autohotkey") || (text2.Contains("auto") && text2.Contains("hot") && text2.Contains("key")))
                    {
                        End(1, "AHK Script Detected." + text, peer);
                    }
                    if (text2.Contains("aimbot") || text2.Contains("nesucks"))
                    {
                        End(1, "Aimbot Detected." + text, peer);
                    }
                    if ((text2.Contains("pixel") || text2.Contains("color") || text2.Contains("motion") || text2.Contains("aim")) && text2.Contains("bot"))
                    {
                        End(1, "Aimbot Detected." + text, peer);
                    }
                    if ((text2.Contains("wall") && text2.Contains("hack")) || text2.Contains("cybit"))
                    {
                        End(1, "Hacktools Detected." + text, peer);
                    }
                    if (text2.Contains("process") && text2.Contains("hacker"))
                    {
                        End(1, "Process Hacker Detected." + text, peer);
                    }
                    if (text2.Contains("inject"))
                    {
                        End(1, "Injection tool Detected." + text, peer);
                    }
                }
            }
            return 0;
        }

        private void End(int code, string log, CommPeer peer)
        {
            string text = log.Substring(0, log.IndexOf('.'));
            string message = $"commub:User with CMID {peer.Actor.Cmid} and name {peer.Actor.Name} has been halted by uberbeat. Reason:\n```diff\n- {log}```";
            LobbyRoom.Instance.SendMessageUDP(message);
            peer.SendError("Uberbeat has forced the game to halt\nReason: " + text);
            peer.Disconnect();
            peer.Dispose();
        }
    }
}
