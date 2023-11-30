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

        private void ToClient(byte[] data)
        {
            _terminalServer.SendData(data, data.Length);
        }

        private void ToClient(string message, uint index = 0)
        {
            byte[] data = Encoding.GetEncoding(28591).GetBytes(message);
            _terminalServer.SendData(data, data.Length);
        }

        void ILogger.Log(string message) => ToClient(message);
        void ILogger.LogBytes(byte[] bytes) => ToClient(bytes);

        /// <summary>
        /// Takes the received message and sends it to it's corrosponding terminal processor. 
        /// </summary>
        /// <param name="message"></param>
        private void ParseMessage(string message)
        {
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

        /// <summary>
        /// Checks to see if the system Database had been initialized to warn the end user. 
        /// </summary>
        /// <returns></returns>
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

        #region Dispose
        public override void Dispose()
        {
            Dispose(true);
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        ~TerminalService()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ToClient("Program is shutting Down.");
                _terminalServer.DisconnectAll();
                _terminalServer.Stop();
                base.Dispose(disposing);
            }
        }
        #endregion Dispose




    }
}
