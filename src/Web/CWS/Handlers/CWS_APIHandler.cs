using Crestron.SimplSharp.WebScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;

namespace VolpeCCReact.Web.CWS.Handlers
{
    internal class CWS_APIHandler : ServiceBase
    {
        public void ProcessRequest(HttpCwsContext context)
        {
            context.Response.StatusCode = 501;

            Error($"Unimplemented api request received {context.Request.Url}");
        }

    }
}
