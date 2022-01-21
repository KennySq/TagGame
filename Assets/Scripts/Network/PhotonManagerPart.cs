using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun;
using System;
using System.Linq;
using Utils;
using TagGame.Photon;

namespace TagGame.Photon
{
    [Serializable]
    public class Packet
    {
        public long UUID;
        public int time_stamp;

        public Packet()
        {
            UUID = PlayerUUID.UUID;
            time_stamp = PhotonManager.GetTimeStamp;
        }

        public virtual Packet Serialize()
        {
            return this;
        }

        public virtual Packet DeSerialize()
        {
            return this;
        }
    }


    [Serializable]
    public class TrackedPosePacket : Packet
    {
        public Vector3 pos;
        public Vector3 velocity;
        public Vector3 acceleration;

        public Quaternion rot;
        public Vector3 angularVelocity;
    }



    public partial class PhotonManager : PhotonSingleton<PhotonManager>
    {

        public event Action<TrackedPosePacket> OnTrackedPoseMessageReceive;

        private const RpcTarget SendTarget = RpcTarget.Others;

        public static float RoundTripTime { get => PhotonNetwork.NetworkingClient.LoadBalancingPeer.RoundTripTime; }


        public static int GetTimeStamp
        {
            get
            {
                return PhotonNetwork.ServerTimestamp;
            }
        }

        public static void FetchTime()
        {
            PhotonNetwork.FetchServerTimestamp();
        }


        //implement serialization here
        private static string SerializeData<T>(in T packet) where T : Packet
        {
            return JsonUtility.ToJson(packet.Serialize());
        }

        private static T DeserializeData<T>(in string data) where T : Packet
        {
            var packet = JsonUtility.FromJson<T>(data);
            return packet.DeSerialize() as T;
        }

        private static bool isValid
        {
            get =>
                isInitialized &&
                PhotonNetwork.IsConnected &&
                Instance.photonView != null &&
                Instance.photonView.isActiveAndEnabled &&
                PhotonNetwork.InRoom &&
                PhotonNetwork.CurrentRoom.PlayerCount > 1;
        }


        //RPC Send/Receive
        //Define Static class for connection test
        public static void SendTrackedPoseData(in TrackedPosePacket data)
        {
            if (!isValid)
                return;

            Instance.photonView.RPC("ReceiveTrackedPoseData", SendTarget, SerializeData(data));
        }

        [PunRPC]
        public void ReceiveTrackedPoseData(string data)
        {
            var packet = DeserializeData<TrackedPosePacket>(data);
            OnTrackedPoseMessageReceive?.Invoke(packet);
        }

    }


}