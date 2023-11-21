using Crestron.SimplSharpPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VolpeCCReact.AV.Devices;
using VolpeCCReact.Devices;
using VolpeCCReact.Services;
using VolpeCCReact.Types;

namespace VolpeCCReact.AV
{
    internal static class DeviceFactory
    {

        internal static IDevice Create(Device device, CrestronControlSystem cs, Room room)
        {
            //Check to see if device is null
            if (device == null)
                throw new ArgumentNullException("Device");

            //Check for valid IP ID.
            if(!IsValidIPID(device.IPID))
            {
                throw new ArgumentException("IP ID string {0} is not valid.");
            }

            
            //if valid create an IP Id.
            uint id = Convert.ToUInt32(device.IPID, 16);

            IDevice newDevice;
            if(device.ConnectionType == "RVCC" || device.ConnectionType == "RCVV")
            {
                newDevice = new RVCC(id, cs);
                newDevice.Description = room.Number;
                newDevice.Register();
                return newDevice;
            }
            else if(device.ConnectionType == "COM102")
            {
                newDevice = new CenCom102Wrapper(id, cs);
                newDevice.Description = room.Number;
                newDevice.Register();
                return newDevice;
            }

            return null;

        }

        internal static bool IsValidIPID(string hexString)
        {
            string hexPattern = @"^0x[0-9A-Fa-f]+$";

            // Check if the string matches the pattern
            if (Regex.IsMatch(hexString, hexPattern))
            {
                // Additional checks if needed (e.g., length, range)
                return true;
            }

            return false;
        }



    }
}
