using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.GeneralIO;
using ExcelDataReader.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Devices;
using VolpeCCReact.src.AV.Display;
using static Crestron.SimplSharpPro.ComPort;

namespace VolpeCCReact.AV.Devices
{
    internal class CenCom102Wrapper : IDevice, ISerialDevice, IDisposable
    {
        private const string C_POWER_ON = "\xAA\x11\xFE\x01\x01\x11";
        private const string C_POWER_OFF = "\xAA\x11\xFE\x01\x00\x10";

        private CenIoCom102 _cen102;

        private const int COMPORTINDEX = 1;
        private bool powerState;

        public bool Connected => _cen102.IsOnline;

        public uint Ipid => _cen102.ID;

        public string IPAddress => "IP Address";

        public string DeviceType => "Crestron CEN-COM-102";

        public bool PowerState => powerState;

        private static ComPortSpec SamsungComPortSpec => new ComPortSpec
        {
            BaudRate = eComBaudRates.ComspecBaudRate9600,
            Parity = eComParityType.ComspecParityNone,
            DataBits = eComDataBits.ComspecDataBits8,
            StopBits = eComStopBits.ComspecStopBits1,
            HardwareHandShake = eComHardwareHandshakeType.ComspecHardwareHandshakeNone,
            SoftwareHandshake = eComSoftwareHandshakeType.ComspecSoftwareHandshakeNone,
            Protocol = eComProtocolType.ComspecProtocolRS232,
        };

        public event EventHandler<CrestronDeviceOnlineEventArgs> OnlineStatusChangedHandler;
        public event EventHandler<SerialMessageEventArgs> SerialTxMessageHandler;
        public event EventHandler<SerialMessageEventArgs> SerialRxMessageHandler;

        public string Description
        {
            get => _cen102.Description;
            set => _cen102.Description = value;
        } 
        

        internal CenCom102Wrapper(uint ipId, CrestronControlSystem cs)
        {
            _cen102 = new CenIoCom102(ipId, cs);

            _cen102.OnlineStatusChange += OnOnlineStatusChanged;
            
            _cen102.ComPorts[COMPORTINDEX].SerialDataReceived += OnMessageReceived;
            
        }

        public void Register()
        {
            _cen102.Register();
            _cen102.ComPorts[COMPORTINDEX].Register();
            _cen102.ComPorts[COMPORTINDEX].SetComPortSpec(SamsungComPortSpec);
        }

        private void OnMessageReceived(ComPort ReceivingComPort, ComPortSerialDataEventArgs args)
        {
            var bytes = StringToByteArray(args.SerialData);
            var data = ByteArrayToString(bytes);
            SerialRxMessageHandler?.Invoke(this, new SerialMessageEventArgs(Description, Ipid, data));
        }

        public void Power_On()
        {
            var bytes = StringToByteArray(C_POWER_ON);
            var data = ByteArrayToString(bytes);
            _cen102.ComPorts[COMPORTINDEX].Send(C_POWER_ON);
            powerState = true; //Fake Feedback.
            SerialTxMessageHandler?.Invoke(this, new SerialMessageEventArgs(Description, Ipid, data));
        }

        public void Power_Off()
        {
            var bytes = StringToByteArray(C_POWER_OFF);
            var data = ByteArrayToString(bytes);
            _cen102.ComPorts[COMPORTINDEX].Send(C_POWER_OFF);
            powerState = false;
            SerialTxMessageHandler?.Invoke(this, new SerialMessageEventArgs(Description, Ipid, data));
        }

        
        
        public void OnOnlineStatusChanged(object sender, OnlineOfflineEventArgs args)
        {
            OnlineStatusChangedHandler?.Invoke(this, new CrestronDeviceOnlineEventArgs(args.DeviceOnLine, Ipid, Description, DeviceType));
        }

        private static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] byteArray = new byte[length];
            for (int i = 0; i < length; i++)
            {
                byteArray[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return byteArray;
        }

        private static string ByteArrayToString(byte[] bytes)
        {
            return string.Concat(bytes.Select(x => $"{x:X2}"));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _cen102.ComPorts[COMPORTINDEX].UnRegister();
                    _cen102.UnRegister();
                    _cen102.Dispose();
                }            
        }
        ~CenCom102Wrapper()
        {
            Dispose(false);
        }
    }
}
