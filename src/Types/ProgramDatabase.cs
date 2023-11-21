using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolpeCCReact.Types
{
    public class ProgramDatabase
    {
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedTime { get; set; }

        public bool LoadFromExcel { get; set; }

        public AVSite Site {  get; set; }



    }
}
