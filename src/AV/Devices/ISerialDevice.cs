using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.src.AV.Display;

namespace VolpeCCReact.src.AV.Display
{
    internal interface ISerialDevice
    {
        event EventHandler<SerialMessageEventArgs> SerialTxMessageHandler;
        event EventHandler<SerialMessageEventArgs> SerialRxMessageHandler;
    }
}
