using Crestron.SimplSharpPro;
using System;
using System.IO;
using System.Linq;
using VolpeCCReact.AV;
using VolpeCCReact.Devices;
using VolpeCCReact.IO;
using VolpeCCReact.src.AV.Display;
using VolpeCCReact.Types;

namespace VolpeCCReact.Services
{
    public class DatabaseService : ServiceBase
    {
        private CrestronControlSystem cs;

        public bool DatabaseInitialized { get; set; }
        public ProgramDatabase Database { get; set; }


        public bool ExcelFileFound { get; set; }
        public bool JsonFileFound { get; set; }

        public DatabaseService(CrestronControlSystem cs)
        {
            this.cs = cs;
        }


        /// <summary>
        /// Same method with parameters in case it needs to be called with Crestron invoke. 
        /// </summary>
        /// <param name="obj"></param>
        public void Initialize(object obj = null) => Initialize();
        
        /// <summary>
        /// Database Initialization called on program start.
        /// </summary>
        public void Initialize()
        {
            ExcelFileFound = File.Exists(Config.ExcelFilePath);
            //JsonFileFound = File.Exists(Config.JsonFilePath);
            JsonFileFound = false; //Just for now.


            if(DatabaseInitialized) //If the database was alerady initialized need to dispose of all of the devices. 
            {
                DisposeDevices();
            }


            if(JsonFileFound)
            {
                Log("Loading Database from Json File.");
                Json_GenerateDatabase();

            }
            else if(!JsonFileFound &&  ExcelFileFound) 
            {
                Log("No Json file found. Generating Database from Excel.");
                Excel_GenerateDatabase();

                Json_SaveDatabase();
            }
            else
            {
                Error("No Database source found!");
            }

            if(DatabaseInitialized)
            {
                InitializeDevices();
                Log("Database successfully initialized.");
            }
        }

        private void Excel_GenerateDatabase()
        {
            try
            {
                using (ExcelReader excelReader = new ExcelReader())
                {
                    Database = new ProgramDatabase();

                    Log("Generating Database from Excel");

                    Database.Site = excelReader.ReadExcelFile();

                    Database.CreationDate = DateTime.Now;

                    DatabaseInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Error($"Excel reader error. {ex.Message}");
                throw;
            }
        }

        private void Json_GenerateDatabase()
        {
            try
            {
                using(JsonReadWrite jsonReader = new JsonReadWrite())
                {
                    Log("Loading Database from Json.");

                    Database = jsonReader.GenerateDatabaseFromJson();

                    DatabaseInitialized = true;
                }
            }
            catch (Exception ex)
            {
                Error($"Json reader error. {ex.Message}");
                throw;
            }
        }

        private void Json_SaveDatabase()
        {
            using (JsonReadWrite jsonReader = new JsonReadWrite())
            {
                Log("Saving database to Json");

                jsonReader.SaveDatabaseToFile(Database);

            }

        }


        /// <summary>
        /// Runs through the deveice database and creates the necessary Crestron Devices.
        /// </summary>
        private void InitializeDevices()
        {
            foreach(var room in Database.Site.Rooms)
            {
                for(var i = 0; i < room.Devices.Count; i++)
                {
                    try
                    {
                        room.Devices[i].CrestronDevice = DeviceFactory.Create(room.Devices[i], cs, room);
                        room.Devices[i].CrestronDevice.OnlineStatusChangedHandler += OnDeviceOnlineStatusChanged;

                        if (room.Devices[i].CrestronDevice is ISerialDevice com)
                        {
                            com.SerialTxMessageHandler += DebugSerialTXMessage;
                            com.SerialRxMessageHandler += DebugSerialRXMessage;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error($"Error initializing room {room.Number} device {i + 1}. {ex.Message}");
                    }
                }
            }
        }

        #region Dispose
        /// <summary>
        /// Disposes all the objects in the database
        /// </summary>
        private void DisposeDevices()
        {
            foreach (var room in Database.Site.Rooms)
            {
                for (var i = 0; i < room.Devices.Count; i++)
                {
                    if (room.Devices[i].CrestronDevice != null)
                    {
                        try
                        {
                            room.Devices[i].CrestronDevice.OnlineStatusChangedHandler -= OnDeviceOnlineStatusChanged;

                            if (room.Devices[i].CrestronDevice is ISerialDevice com)
                            {
                                com.SerialTxMessageHandler -= DebugSerialTXMessage;
                                com.SerialRxMessageHandler -= DebugSerialRXMessage;
                            }

                            room.Devices[i].CrestronDevice.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Error($"Error disposing room {room.Number} device {i + 1}. {ex.Message}");
                        }
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            DisposeDevices();

            base.Dispose(disposing);
        }
        #endregion Dispose

        /// <summary>
        /// Returns a room object given an ID.
        /// </summary>
        /// <param name="targetId">GUID of room.</param>
        /// <returns>Room object, otherwise null.</returns>
        public Room GetRoom(string targetId)
        {
            Room targetRoom = Database.Site.Rooms.FirstOrDefault(obj => obj.Guid.ToString() == targetId);

            if(targetRoom == null )
            {
                Error($"Room not found in database. ID: {targetId}");
            }
            return targetRoom;  
        }
        /// <summary>
        /// Gets the first device in a room (Debugging).
        /// </summary>
        /// <param name="number">Room Number</param>
        /// <returns>Crestron Device in room's device list at index 0</returns>
        public IDevice GetDevicebyRoomNumber(string number)
        {

            Room targetRoom = Database.Site.Rooms.FirstOrDefault(obj => obj.Number == number);

            return targetRoom.Devices[0].CrestronDevice;

        }
        /// <summary>
        /// Searches the database for a device object based on ID.
        /// </summary>
        /// <param name="targetId">GUID of the device</param>
        /// <returns>Device if found, otherwise null</returns>
        public Device GetDevicebyID(string targetId)
        {
            try
            {
                return Database.Site.Rooms.SelectMany(room => room.Devices).FirstOrDefault(device => device.Guid.ToString() == targetId);
            }
            catch (Exception ex)
            {
                Error($"Cannot find device that matches id: {targetId}");

            }

            return null;
        }

        private void DebugSerialTXMessage(object sender, SerialMessageEventArgs e)
        {
            Log($"Room: {e.Room} - Tx: {e.Message}");
        }

        private void DebugSerialRXMessage(object sender, SerialMessageEventArgs e)
        {
            Log($"Room: {e.Room} - Rx: {e.Message}");
        }

        private void OnDeviceOnlineStatusChanged(object sender, CrestronDeviceOnlineEventArgs e)
        {
            string status = (e.IsOnline) ? "Online" : "Offline";
            Log($"Room: {e.Room} {e.DeviceType} ID: {e.ID.ToString("X2")}({e.ID}), {status}");
        }

        private void OnSerialMessageEvent(object sender, SerialMessageEventArgs e)
        {
            Log($"Message: {e.Message}");
        }
    }
}
