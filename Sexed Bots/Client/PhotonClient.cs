using ExitGames.Client.Photon;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sexed_Bots.Client
{
    public class PhotonClient : LoadBalancingClient, IConnectionCallbacks, IInRoomCallbacks, IMatchmakingCallbacks, IPhotonPeerListener
    {
        private string _botName { get; set; }
        public PhotonClient(string BotName, string auth, string userid, string region, string FakeMacId)
        {
            _botName = BotName;
            Debug($"Setting up PhotonClient for {_botName}");
            AppId = Config.AppID;
            AppVersion = Config.PhotonVersion;
            NameServerHost = Config.NameServerHost;

            System.Timers.Timer PhotonLoop = new(50)
            {
                Enabled = true,
                AutoReset = true,
            };
            PhotonLoop.Elapsed += delegate (object sender, System.Timers.ElapsedEventArgs e)
            {
                Service();
            };

            CustomTypes.Register(this);
            AuthValues = new AuthenticationValues
            {
                AuthType = CustomAuthenticationType.Custom,
            };
            AuthValues.AddAuthParameter("token", auth);
            AuthValues.AddAuthParameter("user", userid);
            AuthValues.AddAuthParameter("hwid", FakeMacId);
            AuthValues.AddAuthParameter("platform", "android");
            AuthValues.AddAuthParameter("unityVersion", AppVersion.Split('-')[0]);

            AddCallbackTarget(this);

            if (!ConnectToRegionMaster(region)) Debug($"| Failed to connect to Photon\n| Region {region}\n");
        }
        public void Debug(string message)
        {
            Console.WriteLine($"[{_botName}] {message}");
        }
        public void OnConnected()
        {
            Console.WriteLine($"\n| OnConnected\n| {_botName}\n");
        }

        public void OnConnectedToMaster()
        {
            LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { "groupOnNameplate", ""},
                { "isInvisible", false},
                { "showGroupBadgeToOthers", false},
                { "steamUserID", "0"},
                { "inVRMode", true},
                { "modTag", null},
                { "showSocialRank", true},
            });
            Console.WriteLine($"\n| OnConnectedtoMasterPayload for {_botName}\n| IP {MasterServerAddress}\n| groupOnNameplate => NULL\n| isInvisible => false\n| showGroupBadgeToOthers => false\n| steamUserID => 0\n| inVRMode => true\n| modTag => NULL\n| showSocialRank => true\n");
        }

        public void OnCreatedRoom()
        {
            Debug("OnCreatedRoom");
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            Console.WriteLine($"\n| OnCreateRoomFailed for {_botName}:\n| ReturnCode ==> {returnCode}\n| Message ==> {message}\n");
        }
        public void OnDisconnected(DisconnectCause cause)
        {
            Console.WriteLine($"\n| OnDisconnected for {_botName}\n| Cause ==> {cause}\n");
        }
        public bool IsInstantiated = false;
        public void OnJoinedRoom()
        {
            Console.WriteLine("| Successfully Joined Room");
            Dictionary<string, object> user = new Dictionary<string, object>();
            user["displayName"] = _botName;
            LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { "user", user}
            });
            user["displayName"] = _botName;
            LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { "user", user}
            });
            Debug($"| OnJoinedRoom");
            Debug($"| Connected to Room  ==> {CurrentRoom.Name}");
            Debug($"| Player Count ==> {CurrentRoom.PlayerCount}");
            Debug($"| Cloud Region ==> {CloudRegion}");
            string[][] bytes = Serialization.FromByteArray<string[][]>(Convert.FromBase64String("AAEAAAD/////AQAAAAAAAAAHAQAAAAEBAAAAAgAAAAYJAgAAAAkDAAAAEQIAAAAAAAAAEQMAAAAAAAAACw=="));
            OpRaiseEvent(33, new Dictionary<byte, object>
                {
                    { 0, (byte)20 },
                    { 3, bytes},
                }, new RaiseEventOptions()
                {
                    CachingOption = EventCaching.DoNotCache,
                    Receivers = ReceiverGroup.Others,
                }, new SendOptions()
                {
                    DeliveryMode = DeliveryMode.Reliable,
                    Reliability = true,
                    Channel = 0,
                });
            int[] viewids = new int[4]
                {
                    int.Parse(LocalPlayer.ActorNumber + "00001"),
                    int.Parse(LocalPlayer.ActorNumber + "00002"),
                    int.Parse(LocalPlayer.ActorNumber + "00003"),
                    int.Parse(LocalPlayer.ActorNumber + "00004")
                };
            Hashtable InstantiateHashtable = new Hashtable
            {
                [(byte)0] = "VRCPlayer",
                [(byte)1] = new Vector3(0f, 0f, 0f),
                [(byte)2] = new Quaternion(0f, 0f, 0f, 1f),
                [(byte)4] = viewids,
                [(byte)6] = LoadBalancingPeer.ServerTimeInMilliSeconds,
                [(byte)7] = viewids[0]
            };
            OpRaiseEvent(202, InstantiateHashtable, RaiseEventOptions.Default, SendOptions.SendReliable);
            IsInstantiated = true;
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
            Console.WriteLine($"\n| OnJoinRandomFailed for {_botName}\n| ReturnCode ==> {returnCode}\n| Message ==> {message}\n");
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
            Console.WriteLine($"\n| OnJoinRoomFailed for {_botName}\n| ReturnCode ==> {returnCode}\n| Message ==>  {message}\n");
        }

        public void OnLeftRoom()
        {
            Debug("OnLeftRoom");
        }

        public void OnMasterClientSwitched(Player newMasterClient)
        {
            //Debug($"OnMasterClientSwitched  ==> {newMasterClient.NickName}");
        }

        public void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug($"\n| OnPlayerEnteredRoom ==> {newPlayer.GetDisplayName()}\n");
        }

        public void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug($"\n| OnPlayerLeftRoom ==> {otherPlayer.GetDisplayName()}\n");
        }

        public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            //Debug($"OnPlayerPropertiesUpdate ==> {targetPlayer.NickName}");
        }
        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            throw new NotImplementedException();
        }
        public void OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
        }

        public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
            
        }

        public void OnCustomAuthenticationFailed(string debugMessage)
        {
           
        }
    }
}
