using Crestron.SimplSharpPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.AV.Devices
{
    internal abstract class BaseDevice
    {
        public abstract void CreateDevice(uint iPID, CrestronControlSystem cs);



    }
}
