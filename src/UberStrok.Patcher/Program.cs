﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace UberStrok.Patcher
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            const int UBERSTRIKE_STEAMAPP_ID = 291210;
            Console.WriteLine("If you haven't installed UberStrike on Steam yet, Please click the link below and wait for installation completed! Else just press enter and start patching!");
            Console.WriteLine("https://steamdb.info/app/291210/");
            Console.ReadLine();
            var sw = Stopwatch.StartNew();
            Console.WriteLine(" Searching for Steam installation...");

            var steamPath = default(string);
            try { steamPath = Steam.Path; }
            catch (DirectoryNotFoundException)
            {
                Console.Error.WriteLine(" Unable to find Steam installation.");
                return 1;
            }

            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Path -> " + steamPath);
            Console.WriteLine(" Games ->");
            foreach (var keyValue in Steam.Apps)
            {
                var app = keyValue.Value;
                int id = app.Id;
                string name = app.Name;

                Console.WriteLine(id.ToString().PadLeft(" Games ->".Length) + " -> " + name);
            }
            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Searching for UberStrike installation...");

            var uberStrikeApp = default(SteamApp);
            if (!Steam.Apps.TryGetValue(UBERSTRIKE_STEAMAPP_ID, out uberStrikeApp))
            {
                Console.Error.WriteLine(" Unable to find UberStrike manifest.");
            }
            var uberStrikePath = default(string);
            try
            {
                uberStrikePath = uberStrikeApp.Path;
            }
            catch
            {
                Console.Error.WriteLine(" Unable to parse UberStrike manifest.");
                uberStrikePath = OpenFile();
            }

            if (!Directory.Exists(uberStrikePath))
            {
                Console.Error.WriteLine(" Unable to find UberStrike installation directory.");
                
            }

            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Path -> " + uberStrikePath);

            var uberStrike = new UberStrike(uberStrikePath);

            Console.WriteLine(" Backups ->");

            var dlls = Directory.GetFiles(uberStrike.ManagedPath, "*.dll");
            Directory.CreateDirectory(Path.Combine(uberStrike.ManagedPath, "backup"));
            foreach (var dll in dlls)
            {
                var fileName = Path.GetFileName(dll);
                var dst = Path.Combine(uberStrike.ManagedPath, "backup", fileName);

                File.Copy(dll, dst, true);

                Console.WriteLine(new string(' ', " Games ->".Length) + fileName + " -> " + "backup/" + fileName);
            }

            Console.WriteLine(" -----------------------------------");

            var patches = new Patch[]
            {
                //new QuickSwitchPatch(),
                new WebServicesPatch(),
                //new HostPatch(),
                //new MouseSensitivePatch(),
                //new CreditPatch()
            };

            Console.WriteLine(" Patches ->");
            foreach (var patch in patches)
            {
                var name = patch.GetType().Name;
                Console.Write(new string(' ', " Games ->".Length) + "applying " + name + " -> ");

                try
                {
                    patch.Apply(uberStrike);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("done");
                    Console.ResetColor();
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("failed");
                    Console.ResetColor();
                }
            }

            Console.WriteLine(" Writing new assemblies...");
            try { uberStrike.Save("patched"); }
            catch { Console.Error.WriteLine("Failed to write."); }
            if(!File.Exists(uberStrikePath + "\\UberStrike_Data\\.uberstrok"))
            {
                File.WriteAllText(uberStrikePath + "\\UberStrike_Data\\.uberstrok", "http://uberstrike/2.0/");
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("done");
            Console.WriteLine(" -----------------------------------");
            Console.WriteLine($" Finished in {sw.Elapsed.TotalMilliseconds} ms");
            Console.ReadLine();
            return 0;
        }
        public static string GenerateName(int len)
        {
            Random r = new Random();
            char[] consonants = { 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'l', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x' };
            char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
            StringBuilder Name = new StringBuilder();
            int b = 0; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name.Append(consonants[r.Next(consonants.Length)]);
                b++;
                Name.Append(vowels[r.Next(vowels.Length)]);
                b++;
            }
            return Name.ToString();
        }
        private static string OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            return ofd.FileName;
        }
    }
}
