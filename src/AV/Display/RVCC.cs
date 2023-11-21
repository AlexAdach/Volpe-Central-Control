using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Devices;

namespace VolpeCCReact.AV.Devices
{
    [Serializable]
    public class RVCC : IDevice, IDisposable
    {
        private RoomViewConnectedDisplay roomViewConnectedDisplay;
        private bool disposedValue;

        public bool Connected => roomViewConnectedDisplay.OnlineFeedback.BoolValue;

        public uint Ipid => roomViewConnectedDisplay.ID;

        public string IPAddress => roomViewConnectedDisplay.IpAddressFeedback.StringValue;

        public string DeviceType => "Room View Connected Display";

        public bool PowerState => roomViewConnectedDisplay.PowerOnFeedback.BoolValue;

        public string Description
        {
            get => roomViewConnectedDisplay.Description;
            set => roomViewConnectedDisplay.Description = value;
        }

        public RVCC(uint ipId, CrestronControlSystem cs)
        {
            roomViewConnectedDisplay = new RoomViewConnectedDisplay(ipId, cs);
        }

        public void Register()
        {
            roomViewConnectedDisplay.Register();
        }

        void IDevice.Power_On()
        {
            roomViewConnectedDisplay.PowerOn();
        }

        void IDevice.Power_Off()
        {
            roomViewConnectedDisplay.PowerOff();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    roomViewConnectedDisplay.UnRegister();
                    roomViewConnectedDisplay.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
/*         ~RVCC()
         {
             // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
             Dispose(disposing: false);
         }*/

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
