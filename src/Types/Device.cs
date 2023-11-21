using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Devices;

namespace VolpeCCReact.Types
{
    public class Device
    {
        public Guid Guid { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public string IPID { get; set; }

        public string IPAddress { get; set; }

        public string ConnectionType { get; set; }

        public IDevice CrestronDevice { get; set;  }



    }
}
