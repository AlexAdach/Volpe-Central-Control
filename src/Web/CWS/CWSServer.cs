using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VolpeCCReact.Services;
using VolpeCCReact.src.Web.CWS.Handlers;
using VolpeCCReact.Web.CWS.Handlers;

namespace VolpeCCReact.Web.CWS
{
    internal class CWSService : ServiceBase
    {
        private HttpCwsServer api;
        //private IHttpCwsHandler routeHandler;

        internal CWSService()
        {

        }

        public void Initialize(object obj = null)
        {
            api = new HttpCwsServer("/CentralControl");
            api.HttpRequestHandler = new CWS_FrontendHandler();


            var rooms = new HttpCwsRoute("api/rooms");
            rooms.RouteHandler = new CWS_RoomsHandler();
            api.AddRoute(rooms);

            var roomId = new HttpCwsRoute("api/rooms/{ID}");
            roomId.RouteHandler = new CWS_RoomIdHandler();
            api.AddRoute(roomId);

            var device = new HttpCwsRoute("api/device/{ID}/{POWER}");
            device.RouteHandler = new CWS_DeviceHandler();
            api.AddRoute(device);

            api.ReceivedRequestEvent += Api_ReceivedRequestEvent;
            api.Register();
        }

        private void Api_ReceivedRequestEvent(object sender, HttpCwsRequestEventArgs args)
        {
            Log($"{args.Context.Request.HttpMethod} request received from client");
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
                if (api != null)
                {
                    api.Unregister();
                    api.Dispose();
                }
            }
        }

        ~CWSService()
        {
            Dispose(false);
        }
        #endregion Dispose

    }
}
