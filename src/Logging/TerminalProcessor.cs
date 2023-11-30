using Newtonsoft.Json;
using System.Text;
using VolpeCCReact.src.Services;
using VolpeCCReact.Web;

namespace VolpeCCReact.Services
{
    public class TerminalProcessor
    {
        public RootCommands RootCommands { get; private set; }
        public DatabaseCommands DatabaseCommands { get; private set; } 
        public ServiceCommands ServiceCommands { get; private set; }
        public DeviceCommands DeviceCommands { get; private set; }

        public TerminalProcessor()
        {
           
            DatabaseCommands = new DatabaseCommands(this);
            ServiceCommands = new ServiceCommands(this);
            RootCommands = new RootCommands(this);
            DeviceCommands = new DeviceCommands(this);
        }

        public string ProcessCommand(string command)
        {

            command = command.ToLower().Trim();

            string response;

            if (command.StartsWith(DatabaseCommands.TypePrefix))
                response = DatabaseCommands.ProcessCommand(command);
            else if (command.StartsWith(ServiceCommands.TypePrefix))
                response = ServiceCommands.ProcessCommand(command);
            else if(command.StartsWith(DeviceCommands.TypePrefix))
                response = DeviceCommands.ProcessCommand(command);  
            else
                response = RootCommands.ProcessCommand(command);


            return response;

        }
    }

    #region CommandClasses
    public abstract class TerminalCommands
    {
        protected const string NotFound = "Command not recognized.";

        protected ServiceMediator _mediator => ServiceMediator.Instance;

        protected TerminalProcessor _terminalProcessor;

        //protected readonly string prefix;

        protected StringBuilder stringBuilder;

        public TerminalCommands(TerminalProcessor terminalProcessor)
        {
            _terminalProcessor = terminalProcessor;
            stringBuilder = new StringBuilder();
        }

        public abstract string ProcessCommand(string command);
        
    }

    public class RootCommands : TerminalCommands
    {
        public static readonly string TypePrefix = "root";

        public RootCommands(TerminalProcessor terminalProcessor) : base(terminalProcessor) { }

        public override string ProcessCommand(string command)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (command == "errors")
            {
                var errorList = _mediator.Errors;

                if (errorList.Count == 0)
                    stringBuilder.Append("No Errors Saved :)");
                else
                {
                    foreach (var error in errorList)
                    {
                        stringBuilder.Append(error).AppendLine();
                    }

                }
            }
            else if (command == "logs")
            {
                var logList = _mediator.Logs;

                if (logList.Count == 0)
                    stringBuilder.Append("No Logs recorded?");
                else
                {
                    foreach(var log in logList)
                    {
                        stringBuilder.Append(log).AppendLine();
                    }
                }

            }

            else if (command == "config")
            {
                stringBuilder.Append("Excel File Path: ").Append(Config.ExcelFilePath).AppendLine();
                stringBuilder.Append("Terminal Port: ").Append(Config.TerminalPort).AppendLine();
                stringBuilder.Append("Processor IP: ").Append(Config.ProcessorIP).AppendLine();
            }
            else if (command == string.Empty)
                return string.Empty;
            else
                stringBuilder.Append(NotFound).AppendLine();
            

            return stringBuilder.ToString();    

        }

    }

    public class DatabaseCommands : TerminalCommands
    {
        public static readonly string TypePrefix = "database";

        public DatabaseCommands(TerminalProcessor terminalProcessor) : base(terminalProcessor) { }

        public string Help = TypePrefix + "\n" + string.Join("\n", _helpCommands);

        private static string[] _helpCommands = new string[]
        { "Json - Returns a Json representation of the current database values",
            "Source - Shows the loaded database source",
        "Room - Room specific commands "};

        public override string ProcessCommand(string command)
        {
            command = command.Replace(TypePrefix, "").Trim();
            
            switch (command)
            {
                case "json":
                    return SiteJson();
                case "reload":
                    return ReloadDatabase();
                case "":
                    return Help;
                default:
                    return TypePrefix + NotFound;
            }
        }

        private string ReloadDatabase()
        {
            var database = _mediator.GetService<DatabaseService>(this);

            if (database == null)
            {
                return "Site Database reference could not be found!";
            }

            database.Initialize();
            return "Reloading Database\n";
        }

        private string SiteJson()
        {
            var site = _mediator.GetService<DatabaseService>(this).Database.Site;

            if (site == null)
            {
                return "Site Database reference could not be found!";
            }

            return JsonConvert.SerializeObject(site, Formatting.Indented);
        }


    }

    public class ServiceCommands : TerminalCommands
    {
        public static readonly string TypePrefix = "service";

        public ServiceCommands(TerminalProcessor terminalProcessor) : base(terminalProcessor) { }

        public string Help = TypePrefix + "\n" + string.Join("\n", _helpCommands);

        private static string[] _helpCommands = new string[]
        { "List - Shows a List of all services",
        "Frontend - Show Frontend server info"};

        public override string ProcessCommand(string command)
        {
            command = command.Replace(TypePrefix, "").Trim();
            switch (command)
            {
                case "list":
                    return ListServices();
                case "timerlog":
                    return DisplayTimerLog();
                case "":
                    return Help;
                default:
                    return TypePrefix + NotFound;

            }
        }

        private string ListServices()
        {
            stringBuilder.Append("Program Services:").AppendLine();
            foreach (var service in _mediator.Services)
            {
                
                stringBuilder.Append("Name: ").Append(service.GetType().Name).AppendLine();
            }

            return stringBuilder.ToString();
        }
        
        private string DisplayTimerLog()
        {
            var timerserver = ServiceMediator.Instance.GetService<TimerService>(this);

            if (timerserver == null)
            {
                return "TimerServer is null.";
            }
                foreach (var log in timerserver.logEntries)
                {
                    stringBuilder.AppendLine(log);
                }
            
            return stringBuilder.ToString();
            
        }
    }

    public class DeviceCommands : TerminalCommands
    {
        public static readonly string TypePrefix = "device";

        public DeviceCommands(TerminalProcessor terminalProcessor) : base(terminalProcessor) { }

        public string Help = TypePrefix + "\n" + string.Join("\n", _helpCommands);

        private static string[] _helpCommands = new string[]
        { "List - Shows a List of all services",
        "Frontend - Show Frontend server info"};

        public override string ProcessCommand(string command)
        {
            command = command.Replace(TypePrefix, "").Trim();
            switch (command)
            {
                case "on":
                    return PowerOn();
                case "off":
                    return PowerOff();
                case "timers":
                    return ShowTimers();
                case "":
                    return Help;
                default:
                    return TypePrefix + NotFound;

            }
        }

        private string ShowTimers()
        {
            var database = _mediator.GetService<DatabaseService>(this);

            StringBuilder sb = new StringBuilder();

            foreach(var room in database.Database.Site.Rooms)
            {
                //var start = room.STime_Power_On.TimeOfDay.ToString();
                //var stop  = room.STime_Power_Off.TimeOfDay.ToString();

                var start = room.StartupTime.ToString();
                var stop = room.ShutdownTime.ToString();

                sb.Append(room.RoomName).Append(" Start  - ").Append(start).AppendLine();
                sb.Append(room.RoomName).Append(" Stop  - ").Append(stop).AppendLine();

            }

            return sb.ToString();
        }

        private string PowerOn()
        {


        var database = _mediator.GetService<DatabaseService>(this);

        var device = database.GetDevicebyRoomNumber("202");

            device.Power_On();
            return "Powering On";
        }

        private string PowerOff()
        {
            var database = _mediator.GetService<DatabaseService>(this);

            var device = database.GetDevicebyRoomNumber("202");

            device.Power_Off();

            return "Powering Off";
        }
    }
    #endregion CommandClasses

}
