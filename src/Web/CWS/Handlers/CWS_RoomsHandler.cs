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
    internal sealed class CWS_RoomsHandler : CWSBaseHandler
    {

        protected override void ProcessGet(ref HttpCwsContext context)
        {
            try
            {
                var database = ServiceMediator.Instance.GetService<DatabaseService>(this);
                var json = JsonConvert.SerializeObject(database.Database.Site.Rooms);

                context.Response.StatusCode = 200;

                context.Response.Write(json, true);
            }
            catch (Exception ex)
            {

                //throw new Exception("Error retrieving room list.");
            }
        }
    }
}
