/// <credits>
/// <author>
/// <name>Shubh Pareek</name>
/// <rollnumber>112001039</rollnumber>
/// </author>
/// </credits>
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MessengerCloud;
using System.Diagnostics;

namespace MessengerDashboard
{
    public static class LocalSave
    {
        private static readonly string s_path = "./savedInfo";

        static LocalSave()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDataFolder = Path.Combine(localAppData, "Messenger");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            s_path = Path.Combine(appDataFolder, "sessionInfo.txt");
        }

        public static void AddEntity(EntityInfoWrapper entity)
        {
            List<EntityInfoWrapper> res = ReadFromFile();
            res.Add(entity);
            SaveToFile(res);
        }

        public static List<EntityInfoWrapper> ReadFromFile()
        {
            try
            {
                Trace.WriteLine("Reading from" + s_path);
                using StreamReader reader = new(s_path);
                string jsonString = reader.ReadToEnd();
                var serializer = new DataContractJsonSerializer(typeof(List<EntityInfoWrapper>));
                using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes(jsonString));
                return (List<EntityInfoWrapper>)serializer.ReadObject(stream);
            }
            catch (FileNotFoundException)
            {
                Trace.WriteLine("File not found. Returning an empty list.");
                return new List<EntityInfoWrapper>();
            }
        }

        public static void SaveToFile(List<EntityInfoWrapper> entities)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<EntityInfoWrapper>));
            using MemoryStream stream = new();
            serializer.WriteObject(stream, entities);
            File.WriteAllText(s_path, System.Text.Encoding.UTF8.GetString(stream.ToArray()));
        }
    }
}


