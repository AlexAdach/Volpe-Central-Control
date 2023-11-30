using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronConnected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Devices;
using VolpeCCReact.src.AV.Display;

namespace VolpeCCReact.AV.Devices
{
    [Serializable]
    public class RVCC : IDevice, IDisposable
    {
        private RoomViewConnectedDisplay roomViewConnectedDisplay;

        private int _onlineStatusDebounce;

        public bool Connected => roomViewConnectedDisplay.IsOnline;

        public uint Ipid => roomViewConnectedDisplay.ID;

        public string IPAddress => roomViewConnectedDisplay.IpAddressFeedback.StringValue;

        public string DeviceType => "Room View Connected Display";

        public bool PowerState => roomViewConnectedDisplay.PowerOnFeedback.BoolValue;

        public string Description
        {
            get => roomViewConnectedDisplay.Description;
            set => roomViewConnectedDisplay.Description = value;
        }

        public event EventHandler<CrestronDeviceOnlineEventArgs> OnlineStatusChangedHandler;

        public RVCC(uint ipId, CrestronControlSystem cs)
        {
            roomViewConnectedDisplay = new RoomViewConnectedDisplay(ipId, cs);
            roomViewConnectedDisplay.OnlineStatusChange += OnOnlineStatusChanged;
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

        public void OnOnlineStatusChanged(object sender, OnlineOfflineEventArgs args)
        {
            if (args.DeviceOnLine)
            {
                OnlineStatusChangedHandler?.Invoke(this, new CrestronDeviceOnlineEventArgs(args.DeviceOnLine, Ipid, Description, DeviceType));
                _onlineStatusDebounce = 2;
            }
            else if(!args.DeviceOnLine && _onlineStatusDebounce > 0)
            {
                _onlineStatusDebounce--;
            }
            else if(!args.DeviceOnLine && _onlineStatusDebounce <= 0)
            {
                OnlineStatusChangedHandler?.Invoke(this, new CrestronDeviceOnlineEventArgs(args.DeviceOnLine, Ipid, Description, DeviceType));
            }

            
        }

        protected virtual void Dispose(bool disposing)
        {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    roomViewConnectedDisplay.UnRegister();
                    roomViewConnectedDisplay.Dispose();
                }
            
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~RVCC()
        {
            Dispose(false);
        }
    }
}
