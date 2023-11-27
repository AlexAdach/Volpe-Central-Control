using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.src.AV.Display
{
    public class SerialMessageEventArgs : EventArgs
    {
        private string _room;
        private uint _id;
        private string _message;

        public string Room { get { return _room; } }
        public uint Id { get { return _id; } }
        public string Message { get { return _message; } }

        public SerialMessageEventArgs(string room, uint id, string message)
        {
            _room = room;
            _id = id;
            _message = message;
        }
    }
}
