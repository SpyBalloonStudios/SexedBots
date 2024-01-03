using Sexed_Bots.Client;
using Sexed_Bots.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sexed_Bots
{
    public class Program
    {
        public static void LoadBots()
        {
            VRCWrapper.AddBot("authcookie_61f9bd97-bcf7-46b1-86a7-1336fff7a0ce");


            if(Config.BotClients.Count == 1)
                Console.Title = $"Sexed Bots | {Config.BotClients.Count} Bot";

            else
                Console.Title = $"Sexed Bots | {Config.BotClients.Count} Bots";
        }
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += Exit;
            Console.Title = $"Sexed Bots";
            Console.WriteLine($"Sexed Bots");
            Console.WriteLine($"Version: {Config.AppVersion}");
            Console.WriteLine($"AppID: {Config.AppID}");
            Console.WriteLine($"Photon Version: {Config.PhotonVersion}");
            Console.WriteLine($"Name Server Host: {Config.NameServerHost}");
            Console.WriteLine($"");

            LoadBots();


            /*Console.ReadLine();
            Console.WriteLine("WorldID");
            Console.Write("> ");
            string worldID = Console.ReadLine();*/

            string worldID = "wrld_a6e75419-0f76-402b-966e-3dc8b79a6b30:33288~region(eu)";

            foreach (var client in Config.BotClients)
            {
                client.JoinRoom(worldID);
            }
            Console.ReadLine();
        }

        private static void Exit(object? sender, EventArgs e)
        {
            foreach(var client in Config.BotClients)
            {
                client.PhotonClient_.OpLeaveRoom(true);
            }
        }
    }
}
