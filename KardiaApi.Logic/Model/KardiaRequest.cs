using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KardiaApi.Logic.Model
{
    public class KardiaRequest
    {
        public int protocolID { get; set; }
        public int usubJID { get; set; }
        public string heartRate { get; set; }
    }
}
