/******************************************************************************
* Filename    = RestClient.cs
*
* Author      = Shubh Pareek
*
* Roll Number = 112001039
*
* Product     = Messenger 
* 
* Project     = Dashboard 
*
* Description =  for implementing storing information locally .
* * *****************************************************************************/
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
    /// <summary>
    /// Provides functionality for local data storage and retrieval.
    /// </summary>
    public static class LocalSave
    {
        /// <summary>
        /// The path to the saved information file.
        /// </summary>
        private static readonly string s_path = "./savedInfo";

        /// <summary>
        /// Static constructor to initialize the file path based on the application data folder.
        /// </summary>
        static LocalSave()
        {
            // Get the local application data folder
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            // Combine the local application data folder with the application-specific folder ("Messenger")
            string appDataFolder = Path.Combine(localAppData, "Messenger");

            // Create the application data folder if it doesn't exist
            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            // Set the file path to the session information file within the application data folder
            s_path = Path.Combine(appDataFolder, "sessionInfo.txt");
        }

        /// <summary>
        /// Adds an entity to the list, saves the updated list to the file.
        /// </summary>
        /// <param name="entity">The entity to be added.</param>
        public static void AddEntity(EntityInfoWrapper entity)
        {
            // Read the existing entities from the file
            List<EntityInfoWrapper> res = ReadFromFile();

            // Add the new entity to the list
            res.Add(entity);

            // Save the updated list back to the file
            SaveToFile(res);
        }

        /// <summary>
        /// Reads entities from the file and returns a list.
        /// </summary>
        /// <returns>A list of EntityInfoWrapper objects.</returns>
        public static List<EntityInfoWrapper> ReadFromFile()
        {
            try
            {
                // Log a trace message indicating the file being read
                Trace.WriteLine("Reading from" + s_path);

                // Read the contents of the file using a StreamReader
                using StreamReader reader = new(s_path);
                string jsonString = reader.ReadToEnd();

                // Deserialize the JSON string into a list of EntityInfoWrapper objects
                var serializer = new DataContractJsonSerializer(typeof(List<EntityInfoWrapper>));
                using MemoryStream stream = new(System.Text.Encoding.UTF8.GetBytes(jsonString));
                return (List<EntityInfoWrapper>)serializer.ReadObject(stream);
            }
            catch (FileNotFoundException)
            {
                // Log a trace message if the file is not found and return an empty list
                Trace.WriteLine("File not found. Returning an empty list.");
                return new List<EntityInfoWrapper>();
            }
        }

        /// <summary>
        /// Saves a list of entities to the file in JSON format.
        /// </summary>
        /// <param name="entities">The list of entities to be saved.</param>
        public static void SaveToFile(List<EntityInfoWrapper> entities)
        {
            // Create a new JSON serializer for the EntityInfoWrapper type
            var serializer = new DataContractJsonSerializer(typeof(List<EntityInfoWrapper>));

            // Create a MemoryStream to store the serialized data
            using MemoryStream stream = new();

            // Serialize the list of entities to the MemoryStream
            serializer.WriteObject(stream, entities);

            // Write the serialized data to the file
            File.WriteAllText(s_path, System.Text.Encoding.UTF8.GetString(stream.ToArray()));
        }
    }
}


