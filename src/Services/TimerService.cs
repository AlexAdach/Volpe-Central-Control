using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;

namespace VolpeCCReact.src.Services
{
    internal class TimerService : ServiceBase
    {
        private CTimer _timer;

        public Queue<string> logEntries = new Queue<string>();
        private const int CONST_MAXLOGENTRIES = 140;

        internal TimerService()
        {
            _timer = new CTimer(CheckTimes, null, 0, 60000);
        }

        private void CheckTimes(object state)
        {
            var database = ServiceMediator.Instance.GetService<DatabaseService>(this);
            var currentTime = DateTime.Now.TimeOfDay;

            
            if (database == null)
                return;

            Log($"Checking Times - {currentTime}");

            try
            {
                foreach (var room in database.Database.Site.Rooms)
                {
                    if (currentTime > room.StartupTime && currentTime < room.ShutdownTime)
                    {
                        for (var i = 0; i < room.Devices.Count; i++)
                        {
                            if (room.Devices[i].CrestronDevice == null)
                            {
                                //RecordTimerEvent($"{currentTime} - Room {room.RoomName} Device {i} Power On Failed. Device is Null.");
                                continue;
                            }

                            if (room.Devices[i].CrestronDevice.Connected && !room.Devices[i].CrestronDevice.PowerState)
                            {
                                room.Devices[i].CrestronDevice.Power_On();

                                RecordTimerEvent($"{currentTime} - Room {room.RoomName} Device {i} Powered On");
                            }
                            else
                            {
                                //RecordTimerEvent($"{currentTime} - Room {room.RoomName} Device {i} Power on Failed. Device is not connected");
                            }
                        }
                    }
                    else if (currentTime > room.ShutdownTime)
                    {
                        for (var i = 0; i < room.Devices.Count; i++)
                        {
                            if (room.Devices[i].CrestronDevice == null)
                            {
                                //RecordTimerEvent($"{currentTime} - Room {room.RoomName} Device {i} Power Off Failed. Device is Null.");
                                continue;
                            }

                            if (room.Devices[i].CrestronDevice.Connected && room.Devices[i].CrestronDevice.PowerState)
                            {
                                room.Devices[i].CrestronDevice.Power_Off();
                                RecordTimerEvent($"{currentTime} - Room {room.RoomName} Device {i} Powered Off");
                            }
                            else
                            {
                                //RecordTimerEvent($"{currentTime} - Room {room.RoomName} Device {i} Power off Failed. Device is not connected");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Error($"Error while running timer check {ex.Message}");
            }

            
            
        }

        #region Dispose
        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timer.Dispose();
            }
        }

        ~TimerService()
        {
            Dispose(false);
        }
        #endregion Dispose

        private void RecordTimerEvent(string message)
        {
            logEntries.Enqueue(message);
            
            Log(message );

            if(logEntries.Count > CONST_MAXLOGENTRIES)
            {
                logEntries.Dequeue();
            }


        }
    }
}
