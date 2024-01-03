using Sexed_Bots.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sexed_Bots
{
    public class Config
    {
        public static string AppID = "bf0942f7-9935-4192-b359-f092fa85bef1";
        public static string AppVersion = "2023.4.2p2-1390-53164fa82e-Release";

        public static string PhotonVersion = "Release_1343";
        public static string NameServerHost = "ns.photonengine.io";
        public static string region = "eu";
        public static List<BotClient> BotClients = new List<BotClient>();
    }
}
