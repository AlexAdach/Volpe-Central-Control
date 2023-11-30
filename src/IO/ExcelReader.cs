using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using ExcelDataReader;
using VolpeCCReact.Services;
using VolpeCCReact.Types;

namespace VolpeCCReact.IO
{
    public class ExcelReader : ServiceBase
    {

        private Dictionary<string, int> ColumnIndices = new Dictionary<string, int>
        {
            { "Room_Number", -1 },
            { "Room_Floor", -1 },
            { "Room_Code", -1 },
            { "Room_Name", -1 },
            { "Room_Type", -1 },
            { "Room_Location", -1 },
            { "Device_Number", -1 },
            { "Device_Type", -1 },
            { "Device_IPID", -1 },
            { "Device_Model", -1 },
            { "Device_Manu", -1 },
            { "Device_Conn", -1 },
            { "Device_Location", -1 }
        };

        public bool FileFound()
        {
            return File.Exists(Config.ExcelFilePath);
        }

        public AVSite ReadExcelFile()
        {
            //Read the data from the excel and return the datatable of Data.
            DataTable sheetData;
            try
            {
                string path = Config.ExcelFilePath;

                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        sheetData = result.Tables[0];
                    }
                }

                //Get the index values for the various colums;
                GetColumnIndices(sheetData);

                return GenerateDataBase(sheetData);

            }
            catch (System.Exception ex)
            {
                Error("Error Reading Excel File");
                throw ex;
            }
        }

        private void GetColumnIndices(DataTable sheetData)
        {
            for (var c = 0; c < sheetData.Columns.Count; c++)
            {
                string columnText = sheetData.Rows[0][c].ToString();

                if (ColumnIndices.ContainsKey(columnText))
                    ColumnIndices[columnText] = c;

            }
        }

        private AVSite GenerateDataBase(DataTable sheetData)
        {
            int row = 0;

            AVSite newSite = new AVSite();

            //Loop through all the rows.
            while(row < sheetData.Rows.Count)
            {

                if (CheckRowPrefix(row, "R", sheetData))
                {
                    Room newRoom = new Room();

                    newRoom.Guid = Guid.NewGuid();
                    newRoom.Number = GetValueByKey(row, "Room_Number", sheetData);
                    newRoom.Floor = GetValueByKey(row, "Room_Floor", sheetData);
                    newRoom.RoomCode = GetValueByKey(row, "Room_Code", sheetData);
                    newRoom.RoomName = GetValueByKey(row, "Room_Name", sheetData);
                    newRoom.RoomType = GetValueByKey(row, "Room_Type", sheetData);
                    newRoom.RoomLocation = GetValueByKey(row, "Room_Location", sheetData);



                    newRoom.Devices = new List<Device>();

                    while (CheckRowPrefix(row + 1, "D", sheetData)) //is the next row a device row?
                    {
                        row++; //Ok it is a device, go through the next row.

                        Device newDevice = new Device();

                        newDevice.Guid = Guid.NewGuid();
                        newDevice.Type = GetValueByKey(row, "Device_Type", sheetData);
                        newDevice.Name = GetValueByKey(row, "Device_Name", sheetData);
                        newDevice.Model = GetValueByKey(row, "Device_Model", sheetData);
                        newDevice.Manufacturer = GetValueByKey(row, "Device_Manu", sheetData);
                        newDevice.IPID = GetValueByKey(row, "Device_IPID", sheetData);
                        newDevice.ConnectionType = GetValueByKey(row, "Device_Conn", sheetData);

                        newRoom.Devices.Add(newDevice);
                        //The while check executes, checks the next row for Device, if not continue
                    }

                    //Next row must be new room or empty, add the room to the Site.
                    NormalizeRoomValues(ref newRoom);
                    newSite.Rooms.Add(newRoom);

                    row++;
                }
                else
                    row++;
            }
            return newSite;
        }

        //Checks to see if that row actually exists, and if the prefix matches the parameter.
        private bool CheckRowPrefix(int row, string prefix, DataTable sheetData)
        {
            if (sheetData.Rows.Count - 1 < row)
                return false;
            else if (sheetData.Rows[row][0].ToString() == prefix)
                return true;
            else
                return false;
        }
        private string GetValueByKey(int row, string key, DataTable sheetData)
        {
            //Just another sanity check in case there's a typo inthe code.
            if (!ColumnIndices.ContainsKey(key))
                return string.Empty;

            if (ColumnIndices[key] >= 0)
            {
                return sheetData.Rows[row][ColumnIndices[key]].ToString();
            }
            else
                return string.Empty;
            
        }

        private void NormalizeRoomValues(ref Room room)
        {
            if(room.RoomName == "") //Add room name if none listed. 
            {
                if(room.RoomType != "")
                {
                    room.RoomName = room.RoomType + " Room " + room.Number;
                }
                else
                {
                    room.RoomName = "Room " + room.Number;
                }
                
            }

            var currentTime = DateTime.Now;

            room.StartupTime = new TimeSpan(6, 0, 0);
            room.ShutdownTime = new TimeSpan(20, 0, 0);

            //room.STime_Power_On = currentTime.Add(new TimeSpan(6,0,0));
            //room.STime_Power_Off = currentTime.Add(new TimeSpan(20, 0, 0));


        }
    }


}
