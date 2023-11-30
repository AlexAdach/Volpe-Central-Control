using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.Types
{
    public class Room
    {
        public Guid Guid { get; set; }
        
        public string Number { get; set; }
        
        public string Floor { get; set; }
        
        public string RoomCode { get; set; }

        public string RoomName { get; set; }

        public string RoomType { get; set; }

        public string RoomLocation { get; set; }

        public bool RoomTimerActive { get; set; }

        public TimeSpan StartupTime { get; set; }

        public TimeSpan ShutdownTime { get; set; }

        public List<Device> Devices { get; set; }


    }
}
