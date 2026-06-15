using KardiaApi.Logic.DataLogic;
using KardiaApi.Logic.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KardiaApi.Logic.BusinessLogic
{
    public class BllForKardiaApi
    {
        DllForKardiaApi DllforKardia;
        public BllForKardiaApi(string appConnectionString)
        {
            DllforKardia = new DllForKardiaApi(appConnectionString);
        }

        public string GetURL(string mrnID)
        {
            string returnString = string.Empty;
            char separator = '-';
            string[] authorsList = mrnID.Split(separator);
            string enviromentId = authorsList[1];
            int protocolID = Convert.ToInt32(authorsList[0]);
            DataTable data = DllforKardia.GetUrl(enviromentId, protocolID);
            if (data != null)
            {
                returnString = data.Rows[0]["URL"].ToString();
            }
            return returnString;
        }
    }
}
