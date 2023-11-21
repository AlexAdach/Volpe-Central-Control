using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.Devices
{
    public interface IDevice
    {
        bool Connected { get; }
        uint Ipid { get; }
        string IPAddress { get; }

        string DeviceType { get; }

        bool PowerState { get; }

        [JsonIgnore]
        string Description { get; set; }

        void Power_On();
        void Power_Off();

        void Register();
        void Dispose();

    }
}
