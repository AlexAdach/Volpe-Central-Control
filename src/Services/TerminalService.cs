using Crestron.SimplSharp.CrestronSockets;
using System;
using System.Text;
using VolpeCCReact.Logging;

namespace VolpeCCReact.Services.Logging
{
    internal class TerminalService : ServiceBase, ILogger
    {
        private const int _port = 12345;
        private TCPServer _terminalServer;

        private TerminalProcessor _terminalCommands = new TerminalProcessor();

        public TerminalService() : base()
        {
        }

        public void ServerStart()
        {
            _terminalServer = new TCPServer("0.0.0.0", _port, 4096);
            
            _terminalServer.WaitForConnectionAsync(ClientConnected);
        }

        private void ClientConnected(TCPServer server, uint index)
        {
            ToClient("Connection Successful!\n", index);
            ToClient("Volpe Signage Control. Tritech 2023\n");
            
            if(!IsSystemOK())
            {
                ToClient("NOTICE! - Database was not properly initialized! Was the excel file loaded?\n");
            }

            server.ReceiveDataAsync(MessageReceived);
        }

        private void MessageReceived(TCPServer server, uint index, int numBytes)
        {
            if (numBytes <= 0)
            {
                Log("Debug Terminal Server Connection closed");
                _terminalServer.Disconnect();
                _terminalServer.WaitForConnectionAsync(ClientConnected);
            }
            else
            {
                string msg = Encoding.ASCII.GetString(server.IncomingDataBuffer, 0, numBytes);

                //ToClient($"Message Recieved: {msg}");
                ParseMessage(msg.ToLower().Trim());
                server.ReceiveDataAsync(MessageReceived);
            }
        }

        private void ToClient(string message, uint index = 0)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            _terminalServer.SendData(data, data.Length);
        }

        void ILogger.Log(string message) => ToClient(message);

        //Terminal Commands
        private void ParseMessage(string message)
        {
            //message = message.ToLower();
            /*            switch (message)
                        {
                            case "rooms":
                                ShowRoomList();
                                break;
                            case "configurations":
                                PrintConfigurations();
                                break;
                            default:

                                break;
                        }*/
            try
            {
                var response = _terminalCommands.ProcessCommand(message);
                ToClient(response);
            }
            catch (Exception ex)
            {

                ToClient($"Error: {ex}");
            }

        }

        private bool IsSystemOK()
        {
            var database = _mediator.GetService<DatabaseService>(this);

            if(database == null)
            {
                return false;
            }
            else
            {
                return database.DatabaseInitialized;
            }
        }


    }
}
