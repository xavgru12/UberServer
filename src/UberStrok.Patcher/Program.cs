using System;
using System.Diagnostics;
using System.IO;
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
            Console.WriteLine(" Thanks to Anonymous from UberKill providing us MODs");
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

            SteamApp uberStrikeApp;
            if (!Steam.Apps.TryGetValue(UBERSTRIKE_STEAMAPP_ID, out uberStrikeApp))
            {
                Console.Error.WriteLine(" Unable to find UberStrike manifest.");
            }
            string uberStrikePath;
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
            var defaultColor = Console.ForegroundColor;
            Console.WriteLine(" -----------------------------------");
            Console.WriteLine(" Path -> " + uberStrikePath);
            Console.WriteLine(" -----------------------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Input the url of Uberstrok server. Ignore this and direct press \"Enter\" if you already set this before (the .uberstrok file)");
            Console.ForegroundColor = defaultColor;
            var url = Console.ReadLine();
            if (string.IsNullOrEmpty(url))
            {
                url = "http://localhost:999";
                if (File.Exists(uberStrikePath + "\\UberStrike_Data\\.uberstrok"))
                {
                    url = File.ReadAllText(uberStrikePath + "\\UberStrike_Data\\.uberstrok");
                }
            }
            if (!url.StartsWith("http://"))
            {
                url = "http://" + url;
            }
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            File.WriteAllText(uberStrikePath + "\\UberStrike_Data\\.uberstrok", url);
            Console.WriteLine("Binded uberstrike to service...");
            Console.WriteLine(" -----------------------------------");
            var uberStrike = new UberStrike(uberStrikePath);

            Console.WriteLine(" Backups ->");

            var dlls = Directory.GetFiles(uberStrike.ManagedPath, "*.dll");
            Directory.CreateDirectory(Path.Combine(uberStrike.ManagedPath, "backup"));
            foreach (var dll in dlls)
            {
                var fileName = Path.GetFileName(dll);
                var dst = Path.Combine(uberStrike.ManagedPath, "backup", fileName);
                try
                {
                    //don't overwrite if exist
                    File.Copy(dll, dst);
                }
                catch
                {

                }
                Console.WriteLine(new string(' ', " Games ->".Length) + fileName + " -> " + "backup/" + fileName);
            }

            Console.WriteLine(" -----------------------------------");

            var patches = new Patch[]
            {
                new QuickSwitchPatch(),
                new WebServicesPatch(),
                new HostPatch(),
                new MouseSensitivePatch(),
                new CreditPatch(),
                new UIPatch()
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
            uberStrike.Dispose();
            Console.WriteLine(" Copying new assemblies...");
            try
            {
                foreach (var file in Directory.GetFiles(uberStrike.ManagedPath, "*.dll"))
                {
                    if (file.EndsWith("Assembly-CSharp.dll"))
                    {
                        File.Copy("patched\\Assembly-CSharp.dll", file, true);
                    }
                    else if (file.EndsWith("Assembly-CSharp-firstpass.dll"))
                    {
                        File.Copy("patched\\Assembly-CSharp-firstpass.dll", file, true);
                    }
                    else if (file.EndsWith("UnityEngine.dll"))
                    {
                        File.Copy("patched\\UnityEngine.dll", file, true);
                    }
                }
                Directory.Delete("patched", true);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Failed to copy, please copy manually. Error: " + ex.Message);
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
