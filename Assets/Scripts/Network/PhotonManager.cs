using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;
using System;
using System.Linq;
using Utils;

namespace TagGame.Photon
{
    public class PhotonSingleton<T> : MonoBehaviourPunCallbacks, IPunObservable where T : PhotonSingleton<T>
    {
        protected static T _instance;

        private static object _lock = new object();

        public static T Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                        "' already destroyed on application quit." +
                        " Won't create again - returning null.");
                    return null;
                }

                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = (T)FindObjectOfType(typeof(T));

                        if (FindObjectsOfType(typeof(T)).Length > 1)
                        {
                            Debug.LogError("[Singleton] Something went really wrong " +
                                " - there should never be more than 1 singleton!" +
                                " Reopening the scene might fix it.");
                            return _instance;
                        }

                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject();
                            _instance = singleton.AddComponent<T>();
                            singleton.name = "(singleton) " + typeof(T).ToString();

                            DontDestroyOnLoad(singleton);
                            Debug.Log("[Singleton] An instance of " + typeof(T) +
                                " is needed in the scene, so '" + singleton +
                                "' was created with DontDestroyOnLoad.");
                        }
                        else
                        {
                            Debug.Log("[Singleton] Using instance already created: " +
                                _instance.gameObject.name);
                        }
                    }

                    return _instance;
                }
            }
        }

        private static event Action<T> onInitialized;
        public static event Action<T> OnInitialized
        {
            add
            {
                if (_instance != null)
                {
                    value?.Invoke(_instance);
                    return;
                }

                onInitialized += value;
            }
            remove
            {
                onInitialized -= value;
            }
        }
        public static bool isInitialized { get; protected set; }
        private static bool applicationIsQuitting = false;
        /// <summary>
        /// When Unity quits, it destroys objects in a random order.
        /// In principle, a Singleton is only destroyed when application quits.
        /// If any script calls Instance after it have been destroyed, 
        ///   it will create a buggy ghost object that will stay on the Editor scene
        ///   even after stopping playing the Application. Really bad!
        /// So, this was made to be sure we're not creating that buggy ghost object.
        /// </summary>
        public virtual void OnDestroy()
        {
            if (_instance == this)
            {
                applicationIsQuitting = true;
            }
        }

        public virtual void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);

                //invoke added handler
                isInitialized = true;

                onInitialized?.Invoke(_instance);
                onInitialized = null;

            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }

        public static bool TryGetInstance(out T instance)
        {
            instance = _instance;
            return instance != null;
        }
    }

    public partial class PhotonManager : PhotonSingleton<PhotonManager>
    {
        public Notifier<int> ConnectedPlayerCount { get; private set; } = new Notifier<int>();
        public NotifierClass<string> ConnectionState { get; private set; } = new NotifierClass<string>();


        public readonly NotifierClass<string> CurrenetRoomName = new NotifierClass<string>();
        public readonly NotifierClass<string> LastRoomName = new NotifierClass<string>();

        public event Action<long> OnPlayerDisconnected;

        private void Start()
        {
            PhotonNetwork.SerializationRate = 10;
            PhotonNetwork.SendRate = 20;

            var photonView = PhotonView.Get(this);
            photonView = gameObject.AddComponent<PhotonView>();
            photonView.ObservedComponents = new List<Component>(0);
            photonView.ObservedComponents.Add(this);
            photonView.ViewID = 1;
            //PhotonNetwork.AddCallbackTarget(this);

            PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 1f;

            ConnectionState.OnDataChanged += ConnectionState_OnDataChanged;
        }


        private void ConnectionState_OnDataChanged(string obj)
        {
            Debug.Log("ConnectionState : " + obj);
        }


        public void StartConnect()
        {
            PhotonNetwork.LocalPlayer.NickName = Guid.NewGuid().ToString();
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomList.RemoveAll((info) => info.Name.Equals(LastRoomName.CurrentData));

            foreach (var room in roomList)
            {
                if (room.PlayerCount == 0)
                    continue;
                if (room.PlayerCount == room.MaxPlayers)
                    continue;
                if (!room.IsOpen)
                    continue;
                if (!room.IsVisible)
                    continue;

                PhotonNetwork.JoinRoom(room.Name);
                return;
            }


            CreateRoom();
            LastRoomName.CurrentData = string.Empty;
        }

        private void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions
            {
                PublishUserId = true,
                MaxPlayers = 2
            };

            PhotonNetwork.CreateRoom("Lobby" + Time.time, roomOptions);
        }

        public void StartDisconnect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        private void CheckMatchableCondition()
        {
            int playerCount = PhotonNetwork.CurrentRoom.Players.Count;

            foreach (var player in PhotonNetwork.CurrentRoom.Players.OrderBy(i => i.Value.ActorNumber))
            {
                //add playerinfo here
            }


            switch (playerCount)
            {
                case 1:
                {
                    break;
                }

                case 2:
                {
                    break;
                }

                default:
                    break;
            }

            ConnectedPlayerCount.CurrentData = playerCount;
        }

        //callbacks
        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            ConnectionState.CurrentData = "StartConnect";
        }

        public override void OnJoinedLobby()
        {
            ConnectionState.CurrentData = "StartConnect";
            CurrenetRoomName.CurrentData = string.Empty;
        }

        public override void OnJoinedRoom()
        {
            CurrenetRoomName.CurrentData = PhotonNetwork.CurrentRoom.Name;
            LastRoomName.CurrentData = PhotonNetwork.CurrentRoom.Name;
            ConnectionState.CurrentData = "JoinedRoom";
            CheckMatchableCondition();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            CheckMatchableCondition();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckMatchableCondition();
            if (long.TryParse(otherPlayer.NickName, out var uuid))
            {
                OnPlayerDisconnected?.Invoke(uuid);
            }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            CheckMatchableCondition();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            CurrenetRoomName.CurrentData = string.Empty;
            PhotonNetwork.JoinLobby();
        }


        public bool isMasterClient
        {
            get => PhotonNetwork.LocalPlayer.IsMasterClient;
        }
    }
}