using Newtonsoft.Json;
using System;
using System.IO;
using VolpeCCReact.Services;
using VolpeCCReact.Types;

namespace VolpeCCReact.IO
{
    internal class JsonReadWrite : ServiceBase
    {
        public ProgramDatabase GenerateDatabaseFromJson()
        {
            ProgramDatabase newDatabase = new ProgramDatabase();
            try
            {
                // Read the JSON file
                string jsonString = File.ReadAllText(Config.ExcelFilePath);

                newDatabase = JsonConvert.DeserializeObject<ProgramDatabase>(jsonString);
                
            }
            catch(Exception ex)
            {
                Error($"Error Loading Json Data: {ex.Message}");
            }
            
            return newDatabase;
        }

        public void SaveDatabaseToFile(ProgramDatabase data)
        {
            try
            {
                data.LastModifiedTime = DateTime.Now;

                string dataString = JsonConvert.SerializeObject(data);

                File.WriteAllText(Config.JsonFilePath, dataString);
            }
            catch (Exception)
            {
                Error("Error saving Json File.");
                throw;
            }


        }
    }
}
