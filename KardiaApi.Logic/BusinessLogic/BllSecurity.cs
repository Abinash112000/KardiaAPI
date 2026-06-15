using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KardiaApi.Logic.BusinessLogic
{
    public class BllSecurity
    {
        public static string EncryptPassword(string data, string key)
        {
            string retrunstring = string.Empty;
            try
            {
                // byte[] bytes = Encoding.ASCII.GetBytes(data);
                retrunstring = RSA.Encrypt(data, key);
            }
            catch (Exception ex)
            {

            }
            return retrunstring;

        }
        public static string DecryptPassword(string data, string key)
        {
            string retrunstring = string.Empty;

            try
            {
                // byte[] bytes = Encoding.ASCII.GetBytes(data);
                retrunstring = RSA.Decrypt(data, key);
            }
            catch (Exception ex)
            {

            }
            return retrunstring;

        }
    }
}
