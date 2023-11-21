using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ExcelDataReader.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Devices;

namespace VolpeCCReact.AV.Devices
{
    internal class CenCom102Wrapper : IDevice, IDisposable
    {
        private const string C_POWER_ON = "\xAA\x11\xFE\x01\x01\x11";
        private const string C_POWER_OFF = "\xAA\x11\xFE\x01\x00\x10";

        private CenIoCom102 _cen102;

        private const int COMPORTINDEX = 1;
        private bool powerState;
        private bool disposedValue;

        public bool Connected => _cen102.IsOnline;

        public uint Ipid => _cen102.ID;

        public string IPAddress => "IP Address";

        public string DeviceType => "Crestron CEN-COM-102";

        public bool PowerState => powerState;

        public string Description
        {
            get => _cen102.Description;
            set => _cen102.Description = value;
        } 
        

        internal CenCom102Wrapper(uint ipId, CrestronControlSystem cs)
        {
            _cen102 = new CenIoCom102(ipId, cs);            
            
            _cen102.ComPorts[COMPORTINDEX].SerialDataReceived += OnMessageReceived;
        }

        public void Register()
        {
            _cen102.Register();
        }

        private void OnMessageReceived(ComPort ReceivingComPort, ComPortSerialDataEventArgs args)
        {
            
        }

        public void Power_On()
        {
            _cen102.ComPorts[COMPORTINDEX].Send(C_POWER_ON);
        }

        public void Power_Off()
        {
            _cen102.ComPorts[COMPORTINDEX].Send(C_POWER_OFF);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cen102.UnRegister();
                    _cen102.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
/*         ~CenCom102Wrapper()
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
