using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.Logging
{
    internal class LoggingObject
    {
        private object _sender;
        private string _value;
        private LogLevelType _level;

        public LogLevelType Level => _level;

        public string Message => $"[{_level}][{_sender.GetType().Name}] {_value} \n";

        public LoggingObject(object sender, string value, LogLevelType level) 
        { 
            _sender = sender;
            _value = value;
            _level = level;
        }        
    }
}
