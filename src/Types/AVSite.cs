using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.Types
{

    public class AVSite
    {
        [JsonProperty("Rooms")]
        public List<Room> Rooms { get; set; }

        public AVSite() 
        { 
            Rooms = new List<Room>();
        }
    }
}
