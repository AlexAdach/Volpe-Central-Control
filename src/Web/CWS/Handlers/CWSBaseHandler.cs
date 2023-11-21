using Crestron.SimplSharp.WebScripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;

namespace VolpeCCReact.Web.CWS
{
    internal abstract class CWSBaseHandler : ServiceBase, IHttpCwsHandler
    {
        public void ProcessRequest(HttpCwsContext context)
        {
            Log(context.Request.PathInfo);
            
            AddDefaultHeaders(ref context);


            var method = context.Request.HttpMethod;

            switch(method)
            {
                case "GET":
                    ProcessGet(ref context); 
                    break;
                case "POST":
                    ProcessPost(ref context);
                    break;
                case "PUT":
                    ProcessPut(ref context);
                    break;
                default:
                    context.Response.StatusCode = 501;  // Not implemented
                    context.Response.Write(
                        String.Format("{0} method not implemented!", method), true);
                break;
            }
        }

        private void AddDefaultHeaders(ref HttpCwsContext context)
        {
            context.Response.AppendHeader("Transfer-Encoding", "chunked");
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*"); // Replace '*' with your specific allowed origin(s)
            context.Response.AppendHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
            context.Response.AppendHeader("Access-Control-Allow-Headers", "Content-Type, Authorization");

            context.Response.ContentType = "application/json";
        }

        protected virtual void ProcessGet(ref HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
        }

        protected virtual void ProcessPost(ref HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
        }

        protected virtual void ProcessPut(ref HttpCwsContext context)
        {
            context.Response.StatusCode = 501;
        }

    }
}
