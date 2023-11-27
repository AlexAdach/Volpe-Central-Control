using Crestron.SimplSharpPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.src.AV.Display
{
    public class CrestronDeviceOnlineEventArgs : EventArgs
    {
        private bool _isOnline;
        private uint _iD;
        private string _room;
        private string _deviceType;

        public bool IsOnline {  get { return _isOnline; } }
        public uint ID { get { return _iD; } }
        public string Room { get { return _room; } }
        public string DeviceType { get { return _deviceType; } }

        public CrestronDeviceOnlineEventArgs(bool isOnline, uint id, string room, string deviceType)
        {
            _isOnline = isOnline;
            _iD = id;
            _room = room;
            _deviceType = deviceType;
        }
    }
}
