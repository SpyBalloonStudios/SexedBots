using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Realtime;
using Sexed_Bots.Wrapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Sexed_Bots.Client
{
    public class BotClient
    {
        public PhotonClient PhotonClient_ { get; set; }
        public HttpClient HttpClient { get; set; }
        public string FakeMacId_ { get; set; }
        public string AuthCookie_ { get; set; }
        public string UserID_ { get; set; }
        public string DisplayName_ { get; set; }
        public string CurrentWorldInstance_ { get; set; }
        public BotClient(string AuthCookie)
        {
            GenerateFakeMac();
            SetupClient(AuthCookie);
            AuthCookie_ = AuthCookie;
            DisplayName_ = VRCWrapper.Get("auth/user", this).Result["displayName"].ToString();
            UserID_ = VRCWrapper.Get("auth/user", this).Result["id"].ToString();

            System.Timers.Timer VisitPing = new System.Timers.Timer(30000)
            {
                AutoReset = true,
                Enabled = true
            };
            VisitPing.Elapsed += async delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                if (string.IsNullOrEmpty(CurrentWorldInstance_)) return;
                HttpRequestMessage visitPayload = new HttpRequestMessage(HttpMethod.Put, "https://api.vrchat.cloud/api/1/visits?apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26&organization=vrchat");
                string visitBody = JsonConvert.SerializeObject(new { userId = UserID_, worldId = CurrentWorldInstance_ });
                visitPayload.Content = new StringContent(visitBody, Encoding.UTF8, "application/json");
                if (visitPayload != null)
                {
                    try
                    {
                        HttpResponseMessage visitResp = await HttpClient.SendAsync(visitPayload);
                        string visitRespBody = await visitResp.Content.ReadAsStringAsync();
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
            };

            Console.WriteLine($"\n| Created BotClient\n| {DisplayName_}\n| ({UserID_})\n");

            PhotonClient_ = new PhotonClient(DisplayName_, AuthCookie_, UserID_, Config.region, FakeMacId_);
        }
        public void GenerateFakeMac()
        {
            byte[] bytes = new byte[20];
            new Random(Environment.TickCount).NextBytes(bytes);
            FakeMacId_ = string.Join("", bytes.Select(x => x.ToString("x2")));
        }
        public void SetupClient(string Auth)
        {
            var httpClientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            HttpClient = new HttpClient(httpClientHandler);
            HttpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            HttpClient.DefaultRequestHeaders.Add("X-MacAddress", FakeMacId_);
            HttpClient.DefaultRequestHeaders.Add("X-Client-Version", Config.AppVersion);
            HttpClient.DefaultRequestHeaders.Add("X-Platform", "standalonewindows");
            HttpClient.DefaultRequestHeaders.Add("Origin", "vrchat.com");
            HttpClient.DefaultRequestHeaders.Add("Host", "api.vrchat.cloud");
            HttpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive, TE");
            HttpClient.DefaultRequestHeaders.Add("TE", "identity");
            HttpClient.DefaultRequestHeaders.Add("User-Agent", "VRC.Core.BestHTTP");
            HttpClient.DefaultRequestHeaders.Add("Cookie", $"apiKey=JlE5Jldo5Jibnk5O5hTx6XVqsJu4WJ26; auth={Auth}; twoFactorAuth=");
        }
        public void ChangeAvatar(string AvatarID)
        {

        }
        public void JoinRoom(string RoomID)
        {
            CurrentWorldInstance_ = RoomID;

            WorldVisitPayload visitPayload = new WorldVisitPayload(this, RoomID);

            if (visitPayload == null)
            {
                Console.WriteLine($"Failed to get VisitPayload. Exiting\n");
                return;
            }
            Console.WriteLine($"\n| VisitPayload for [{DisplayName_}]\n| Capacity {visitPayload.Capacity_}\n| PhotonVersion {visitPayload.RoomVersion_}\n");

            EnterRoomParams enterRoomParams = new EnterRoomParams
            {
                CreateIfNotExists = true,
                RejoinOnly = false,
                RoomName = RoomID,
                RoomOptions = new RoomOptions
                {
                    IsOpen = true,
                    IsVisible = false,
                    MaxPlayers = Convert.ToByte(visitPayload.Capacity_),
                    CustomRoomProperties = new Hashtable
                    {
                        { (byte)3, visitPayload.RoomVersion_},
                        { (byte)2, visitPayload.RoomToken_}
                    },
                    EmptyRoomTtl = 0,
                    PublishUserId = false
                }
            };
            PhotonClient_.OpJoinRoom(enterRoomParams);
        }
        
    }
}
