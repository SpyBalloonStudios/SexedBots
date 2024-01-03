using Newtonsoft.Json.Linq;
using Sexed_Bots.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sexed_Bots.Wrapper
{
    public class VRCWrapper
    {
        public static void AddBot(string authcookie)
        {
            BotClient client = new BotClient(authcookie);
            Config.BotClients.Add(client);
        }
        public static async Task<JObject> Get(string endpoint, BotClient client)
        {
            string body = await client.HttpClient.GetStringAsync($"https://api.vrchat.cloud/api/1/{endpoint}");
            // 400 | Using a web-derived token in client
            return JObject.Parse(body);
        }
    }
}
