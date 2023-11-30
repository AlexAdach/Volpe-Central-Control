using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;
using VolpeCCReact.src.Web.CWS;

namespace VolpeCCReact.Web.CWS.Handlers
{
    internal sealed class CWS_RoomIdHandler : CWSBaseHandler
    {
        protected override void ProcessGet(ref HttpCwsContext context)
        {
            if (context.Request.RouteData.Values.ContainsKey("ID"))
            {
                var id = context.Request.RouteData.Values["ID"].ToString();

                var database = ServiceMediator.Instance.GetService<DatabaseService>(this);

                var json = JsonConvert.SerializeObject(database.GetRoom(id));

                context.Response.StatusCode = 200;

                context.Response.Write(json, true);
            }
        }

        protected override void ProcessPost(ref HttpCwsContext context)
        {
            try
            {
                using (var reader = new Crestron.SimplSharp.CrestronIO.StreamReader(context.Request.InputStream))
                {
                    var bodyString = reader.ReadToEnd();
                    var roomRequest = JsonConvert.DeserializeObject<RoomPostRequest>(bodyString);

                    if(roomRequest != null)
                    {
                        if(roomRequest.Startup != String.Empty)
                        {
                            Log($"{roomRequest.Startup}");
                        }
                        else if(roomRequest.Shutdown != String.Empty)
                        {
                            Log($"{roomRequest.Shutdown}");
                        }

                    }


                }
            }
            catch (Exception ex)
            {

                Error($"Error parsing put request: {ex.Message}");
            }

            context.Response.StatusCode = 200;



        }
    }
}
