﻿using Newtonsoft.Json;
using System;
using System.IO;

namespace UberStrok.WebServices
{
    public static class Utils
    {
        public static readonly Random Random = new Random();

        public static T DeserializeJsonAt<T>(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path))
                return default(T);

            var json = File.ReadAllText(path);
            var obj = JsonConvert.DeserializeObject<T>(json);
            return obj;
        }

        public static void SerializeJsonAt(string path, object obj)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var json = JsonConvert.SerializeObject(obj);
            File.WriteAllText(path, json);
        }
    }
}
