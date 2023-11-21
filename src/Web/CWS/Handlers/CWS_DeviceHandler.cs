using Crestron.SimplSharp.Net;
using Crestron.SimplSharp.WebScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;
using VolpeCCReact.Web.CWS;

namespace VolpeCCReact.src.Web.CWS.Handlers
{
    internal sealed class CWS_DeviceHandler : CWSBaseHandler
    {
        protected override void ProcessPost(ref HttpCwsContext context)
        {
            Log(context.Request.Path);
            if (context.Request.RouteData.Values.ContainsKey("ID"))
            {
                if(context.Request.RouteData.Values.ContainsKey("POWER"))
                {
                    var id = (string)context.Request.RouteData.Values["ID"];
                    var power = (string)context.Request.RouteData.Values["POWER"];

                    var database = ServiceMediator.Instance.GetService<DatabaseService>(this);

                    var device = database.GetDevicebyID(id);

                    if(device != null)
                    {
                        if (device.CrestronDevice != null)
                        {
                           
                            if (power == "on")
                            {
                                Log($"Client set display id {id} power on");
                                device.CrestronDevice.Power_On();
                            }
                            else
                            {
                                Log($"Client set display id {id} power off");
                                device.CrestronDevice.Power_Off();
                            }
                        }
                        else
                        {
                            Error($"Device {id} associated crestron device is null. Double check database for connection typos");
                        }
                    }
                }
            }
        }

    }
}
