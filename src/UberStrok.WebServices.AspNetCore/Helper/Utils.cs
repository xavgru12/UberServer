using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace UberStrok.WebServices.AspNetCore.Helper
{
    public class Utils
    {
        public static Random Random = new Random();

        private static readonly DateTime m_unix = DateTime.UnixEpoch;

        public static T DeserializeJsonAt<T>(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!File.Exists(path))
            {
                return default;
            }
            string json = File.ReadAllText(path);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static T DeserializeJsonWithNewtonsoftAt<T>(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!File.Exists(path))
            {
                return default;
            }
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SerializeJsonAt(string path, object obj)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            File.WriteAllText(path, json);
        }

        public static void SerializeJsonWithNewtonsoftAt(string path, object obj)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            string json = System.Text.Json.JsonSerializer.Serialize(obj);
            File.WriteAllText(path, json);
        }

        public static int GetTimestamp()
        {
            return (int)DateTime.UtcNow.Subtract(m_unix).TotalSeconds;
        }

        public static int GetUserId(string sessionId)
        {
            return BitConverter.ToInt32(Convert.FromBase64String(sessionId).Take(4).Reverse()
                .ToArray());
        }
    }
}
