using System;
using System.Collections.Generic;
using System.Text;

namespace KZOMNAV.Models
{
    static class UserAgentController
    {
        public static string[] UserAgenteChrome = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36" };

        public static string[] UserAgentBrave = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36" };

        public static string[] UserAgentEdge = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/94.0.4606.81 Safari/537.36 Edg/94.0.992.47" };

        public static string[] UserAgentFireFox = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0" };

        public static string[] UserAgentOpera = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0" };

        public static string[] UserAgentWaterFox = { "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:93.0) Gecko/20100101 Firefox/93.0" };

        private static Random rand = new Random();

        public static string GetUserChrome()
        {
            return UserAgenteChrome[rand.Next(0, (UserAgenteChrome.Length - 1))];
        }

        public static string GetUserBrave()
        {
            return UserAgentBrave[rand.Next(0, (UserAgentBrave.Length - 1))];
        }

        public static string GetUserEdge()
        {
            return UserAgentEdge[rand.Next(0, (UserAgentBrave.Length - 1))];
        }

        public static string GetUserFireFox()
        {
            return UserAgentFireFox[rand.Next(0, (UserAgentBrave.Length - 1))];
        }

        public static string GetUserOpera()
        {
            return UserAgentOpera[rand.Next(0, (UserAgentBrave.Length - 1))];
        }

        public static string GetUserWaterFox()
        {
            return UserAgentWaterFox[rand.Next(0, (UserAgentBrave.Length - 1))];
        }
    }
}
