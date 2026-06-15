using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KardiaApi.Logic.Model
{
    public class Patients
    {
        public string id { get; set; }
        public string mrn { get; set; }
        public string dob { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int? sex { get; set; }
        public string phone { get; set; }
        public string connectionTemplateId { get; set; }
    }
}