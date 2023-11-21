using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;

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
    }
}
