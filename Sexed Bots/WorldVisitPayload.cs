using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sexed_Bots.Client;
using Sexed_Bots.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sexed_Bots
{
    public class WorldVisitPayload
    {
        public BotClient BClient_ { get; set; }
        public string WorldID_ { get; set; }
        public string RoomToken_ { get; set; }
        public int RoomVersion_ { get; set; }
        public int Capacity_ { get; set; }
        public bool CanModerateInstance_ { get; set; }
        public WorldVisitPayload(BotClient BClient, string WorldID)
        {
            Console.WriteLine("public WorldVisitPayload(BotClient BClient, string WorldID)");

            WorldID_ = WorldID;
            BClient_ = BClient;

            // check if the return value of travel request is true
            if (!TravelRequest().Result)
            {
                Console.WriteLine("Failed to send travel request. Exiting");
                return;
            }

            VisitPayload();

            Capacity_ = GetCapacity();
            
            
            JObject PhotonWorld = TravelToken().Result;
            RoomToken_ = PhotonWorld["token"].ToString();
            RoomVersion_ = Convert.ToInt32(PhotonWorld["version"].ToString());
            Console.WriteLine($"| PhotonWorld for {BClient.DisplayName_}\n| Version {RoomVersion_}\n| CanModerateInstance {PhotonWorld["canModerateInstance"].ToString()}\n");

        }
        public async Task VisitPayload()
        {
            Console.WriteLine("public async Task VisitPayload()");

            if (string.IsNullOrEmpty(WorldID_)) return;
            HttpRequestMessage visitPayload = new HttpRequestMessage(HttpMethod.Put, "https://api.vrchat.cloud/api/1/visits?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
            string visitBody = JsonConvert.SerializeObject(new { userId = BClient_.UserID_, worldId = WorldID_ });
            visitPayload.Content = new StringContent(visitBody, Encoding.UTF8, "application/json");
            if (visitPayload != null)
            {
                try
                {
                    HttpResponseMessage visitResp = await BClient_.HttpClient.SendAsync(visitPayload);
                    string visitRespBody = await visitResp.Content.ReadAsStringAsync();
                    Console.WriteLine("tetsas");
                    JObject visitObjResp = JObject.Parse(visitRespBody);
                    if (visitObjResp.ContainsKey("success"))
                    {
                        Console.WriteLine(visitObjResp["success"]?["message"]);
                    }
                }
                catch
                {
                    Console.WriteLine("Failed to Send Visit Payload [1]");
                }
            }
            else
            {
                Console.WriteLine("Failed to Send Visit Payload [2]");
            }
        }
        public int GetCapacity()
        {
            Console.WriteLine("public int GetCapacity()");

            return Convert.ToInt32((Task.Run(async () => await VRCWrapper.Get($"worlds/{WorldID_.Split(':')[0]}", BClient_)).Result["capacity"].ToString()));
        }
        public async Task<bool> TravelRequest()
        {
            Console.WriteLine("public async Task<bool> TravelRequest()");

            var url = $"https://api.vrchat.cloud/api/1/travel/{WorldID_}/request?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";

            HttpResponseMessage TravelResponseMessage = await BClient_.HttpClient.PostAsync(url, null);

            if (TravelResponseMessage.IsSuccessStatusCode)
            {
                // Write the response to the console
                string TravelBody = await TravelResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine(TravelBody);
            } else {
                Console.WriteLine($"Response status code: {(int)TravelResponseMessage.StatusCode} ({TravelResponseMessage.StatusCode})");
                string errorContent = await TravelResponseMessage.Content.ReadAsStringAsync();
                Console.WriteLine($"Error content: {errorContent}");
                return false;
            }
            return true;
        }
        public async Task<JObject> TravelToken()
        {

            Console.WriteLine("public async Task<JObject> TravelToken()");

            string url = $"https://api.vrchat.cloud/api/1/travel/{WorldID_}/token?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat";
            HttpResponseMessage WorldResponse = await BClient_.HttpClient.GetAsync(url);
            string WorldBody;

            if (WorldResponse.IsSuccessStatusCode)
            { 
                // Write the response to the console
                WorldBody = await WorldResponse.Content.ReadAsStringAsync();
                Console.WriteLine(WorldBody);
            } else {
                Console.WriteLine($"Response status code: {(int)WorldResponse.StatusCode} ({WorldResponse.StatusCode})");
                string errorContent = await WorldResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Error content: {errorContent}");
            }

            WorldResponse.EnsureSuccessStatusCode();
            WorldBody = await WorldResponse.Content.ReadAsStringAsync();
            return JObject.Parse(WorldBody);
        }
    }
}
