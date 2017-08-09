using Browsergame.Game;
using Browsergame.Game.Engine;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Browsergame.Services {
    static class Security {
        public static string getToken(string name, string pw) {
            return hash(name + pw);
        }

        public static string hash(string str) {
            byte[] tmpSource = null;
            byte[] tmpHash = null;

            str += "cQCXfXnOenPeaWBSuIvBiVBVsjZ6tZrozWNBEeJDjHbjHmasdqwme8RuYHZuRZ6joI";
            //Create a byte array from source data.
            tmpSource = ASCIIEncoding.ASCII.GetBytes(str);
            //Compute hash based on source data.
            tmpHash = new SHA512CryptoServiceProvider().ComputeHash(tmpSource);

            int i = 0;
            StringBuilder sOutput = new StringBuilder(tmpHash.Length);
            for (i = 0; i <= tmpHash.Length - 1; i++) {
                sOutput.Append(tmpHash[i].ToString("X2"));
            }
            return sOutput.ToString();
        }
        public static Boolean authenticateRequest(IOwinRequest Request) {
            string path = Request.Path.ToString();
            if (path == "/" || path == "/favicon.ico" || path.StartsWith("/static") || path.StartsWith("/login")) return true;

            var token = Request.Cookies.FirstOrDefault(c => c.Key == "token").Value;
            return isTokenOK(token, path);
        }
        public static bool isTokenOK(string token, string requestPathForLogging)
        {
            if (token == null)
            {
                Logger.log(5, Category.Security, Severity.Warn, String.Format("Token missing requesting {0}", requestPathForLogging));
                return false;
            }
            State state = StateEngine.getState();
            if (state.getPlayer(token) == null)
            {
                Logger.log(5, Category.Security, Severity.Warn, String.Format("Token wrong requesting {0}", requestPathForLogging));
                return false;
            }
            //Token exists
            return true;
        }
    }
}
