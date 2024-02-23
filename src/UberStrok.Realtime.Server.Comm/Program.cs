using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace UberStrok.WebServices
{
    public static class Program
    {
        /*
            Login sequence:
            1. AuthenticateApplication
            2. LoginSteam -> may vary
            3. GetContactsByGroups
            4. GetConfigurationData
            5. GetMaps
            6. GetShop
            7. GetInventory
            8. GetLoadout
            9. GetMember
         */

        public static WebServiceManager Manager { get; set; }

        public static int Main(string[] args)
        {
            string input;
            Console.WriteLine("Enter server ip address");
            input = Console.ReadLine();
            bool added = false;
            var host = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");
            var lines = File.ReadAllLines(host).ToList();
            for (int x = 0; x < lines.Count; x++)
            {
                if (lines[x].Contains("uberstrike"))
                {
                    //remove it
                    lines[x] = input + " uberstrike";
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                lines.Add(input + " uberstrike");
            }
            File.WriteAllLines(host, lines);
            try
            {
                
                Manager = new WebServiceManager();
                Manager.Start();
            }
            catch (Exception ex)
            {
                WebServiceManager.Log.Fatal($"Reason: {ex.Message}");

                Console.WriteLine(ex);
                Console.WriteLine();
                Console.WriteLine("Unable to start web services. Press any key to exit...");
                Console.ReadKey();
                return 1;
            }
            if (File.Exists("UberStrike.exe"))
            {
                Process.Start("UberStrike.exe");
            }
            Thread.Sleep(Timeout.Infinite);
            return 0;
        }
    }
}
