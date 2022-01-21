using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReceivePosition : MonoBehaviour
{
    private Vector3 TargetPosition;
    private Vector3 PredictionPosition;

    private Quaternion TargetRotation;
    private Quaternion PredictionRotation;

    private bool isReceived = false;

    private float lerpFactor;
    private float PacketSendInterval;
    private float TotalPacketSendInterval = 0f;
    private uint PacketReceiveCount = 0;

    private int LastTimeStamp;
    private float UpdateTime;

    private const float ExtrapolationFactor = 7f;
    private const float InterpolationFactor = 1f / 7f;

    public float updateRate = 0;

    private void OnEnable()
    {
        TagGame.Photon.PhotonManager.OnInitialized += PhotonManager_OnInitialized;
    }

    private void PhotonManager_OnInitialized(TagGame.Photon.PhotonManager photonManager)
    {
        photonManager.OnTrackedPoseMessageReceive += PhotonManager_OnTrackedPoseMessageReceive;
    }

    private void PhotonManager_OnTrackedPoseMessageReceive(TagGame.Photon.TrackedPosePacket packet)
    {
        UpdateTarget(packet.time_stamp, packet.pos, packet.rot, packet.velocity, packet.acceleration);
    }

    private void Update()
    {
        UpdateTransform(Time.deltaTime);
    }

    public void UpdateTransform(in float dt)
    {
        if (!isReceived)
            return;

        var rate = 1f;
        UpdateTime += dt;
        updateRate = (UpdateTime) / (TotalPacketSendInterval / PacketReceiveCount);

        //Update Position
        var lerpPosition = Vector3.Lerp(transform.localPosition, Vector3.LerpUnclamped(TargetPosition, PredictionPosition, ExtrapolationFactor * rate), InterpolationFactor);
        var lerpRotation = Quaternion.Slerp(transform.localRotation, Quaternion.SlerpUnclamped(TargetRotation, PredictionRotation, ExtrapolationFactor * rate), InterpolationFactor);

        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.ClampMagnitude(lerpPosition, 5f), InterpolationFactor);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, lerpRotation, InterpolationFactor);
    }

    public void UpdateTarget(in int timeStamp, in Vector3 nextPosition, in Quaternion nextRotation, in Vector3 velocity, in Vector3 acceleration)
    {
        //when first received
        if (!isReceived)
        {
            transform.localPosition = TargetPosition = nextPosition;
            transform.localRotation = TargetRotation = nextRotation;

            isReceived = true;
            LastTimeStamp = timeStamp;
            return;
        }

        PacketSendInterval = (timeStamp - LastTimeStamp) * 0.001f;
        TotalPacketSendInterval += PacketSendInterval;
        PacketReceiveCount++;

        if (PacketReceiveCount > 100)
        {
            var avg = (TotalPacketSendInterval / PacketReceiveCount);
            TotalPacketSendInterval -= avg;
            PacketReceiveCount--;
        }

        var transportDiff = (PhotonNetwork.ServerTimestamp - timeStamp) * 0.001f;

        UpdateTime = 0f;
        LastTimeStamp = timeStamp;

        //continuous received
        lerpFactor = Mathf.Lerp(lerpFactor, (velocity + (acceleration * (transportDiff + PacketSendInterval))).magnitude, 0.4f);

        PredictionPosition = nextPosition + (velocity + (acceleration * (transportDiff + PacketSendInterval))) * (transportDiff + PacketSendInterval);
        PredictionRotation = nextRotation * (nextRotation * Quaternion.Inverse(TargetRotation));

        TargetPosition = nextPosition;
        TargetRotation = nextRotation;
    }


    private void OnDisable()
    {
        if (TagGame.Photon.PhotonManager.TryGetInstance(out var instance))
        {
            instance.OnTrackedPoseMessageReceive -= PhotonManager_OnTrackedPoseMessageReceive;
        }
        TagGame.Photon.PhotonManager.OnInitialized -= PhotonManager_OnInitialized;
    }
}
