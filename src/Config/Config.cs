using Crestron.SimplSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact
{
    public class Config
    {
        public static readonly string JsonFilePath = Crestron.SimplSharp.CrestronIO.Directory.GetApplicationRootDirectory() + "//html//Database.json";
        public static readonly string ExcelFilePath = Crestron.SimplSharp.CrestronIO.Directory.GetApplicationRootDirectory() + "//html//Volpe Central Control Master.xlsx";
        public static readonly string ReactFilePath = Crestron.SimplSharp.CrestronIO.Directory.GetApplicationRootDirectory() + "//html//client";
        public static readonly int TerminalPort = 12345;
        public static string ProcessorIP = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, 0);
        public static readonly int FrontEndPort = 4321;
    }
}
