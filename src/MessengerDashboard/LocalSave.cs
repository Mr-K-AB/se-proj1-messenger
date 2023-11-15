using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MessengerCloud;

namespace MessengerDashboard
{
    public class LocalSave
    {
        private readonly string _path = "./savedInfo";

        public LocalSave()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appDataFolder = Path.Combine(localAppData, "Messenger");

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            _path = Path.Combine(appDataFolder, "sessionInfo.txt");

        }

        public void AddEntity(EntityInfoWrapper entity)
        {
            List<EntityInfoWrapper> res = ReadFromFile();
            res.Add(entity);
            SaveToFile(res);
        }

        public List<EntityInfoWrapper> ReadFromFile()
        {
            try
            {
                using StreamReader reader = new(_path);
                string jsonString = reader.ReadToEnd();
                var serializer = new DataContractJsonSerializer(typeof(List<EntityInfoWrapper>));
                using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes(jsonString));
                return (List<EntityInfoWrapper>)serializer.ReadObject(stream);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found. Returning an empty list.");
                return new List<EntityInfoWrapper>();
            }
        }

        public void SaveToFile(List<EntityInfoWrapper> entities)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<EntityInfoWrapper>));
            using MemoryStream stream = new();
            serializer.WriteObject(stream, entities);
            File.WriteAllText(_path, System.Text.Encoding.UTF8.GetString(stream.ToArray()));
        }


    }
}


