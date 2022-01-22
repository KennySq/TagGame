using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestReceivePosition : MonoBehaviour
{

    [SerializeField]
    private Rigidbody rigid;


    private Player3D player;

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

    public void Initialize(in Player3D player)
    {
        this.player = player;
    }

    private void OnEnable()
    {
        TagGame.Photon.PhotonManager.OnInitialized += PhotonManager_OnInitialized;
    }

    private void PhotonManager_OnInitialized(TagGame.Photon.PhotonManager photonManager)
    {
        photonManager.OnTrackedPoseMessageReceive += PhotonManager_OnTrackedPoseMessageReceive;
        photonManager.OnAnimationMessageReceive += PhotonManager_OnAnimationMessageReceive;
    }

    private void PhotonManager_OnAnimationMessageReceive(TagGame.Photon.AnimationPacket obj)
    {
        player.InvokeAnimationTrigger(obj.index);
    }

    private void PhotonManager_OnTrackedPoseMessageReceive(TagGame.Photon.TrackedPosePacket packet)
    {
        UpdateTarget(packet.time_stamp, packet.pos, packet.rot, packet.velocity, packet.acceleration, packet.useSnap == 1);
    }

    private void Update()
    {
        UpdateTransform(Time.deltaTime);
    }

    public void UpdateTransform(in float dt)
    {
        if (!isReceived)
            return;

        UpdateTime += dt;
        updateRate = (UpdateTime) / (TotalPacketSendInterval / PacketReceiveCount);

        //Update Position
        var lerpPosition = Vector3.LerpUnclamped(TargetPosition, PredictionPosition, ExtrapolationFactor);
        //var lerpRotation = Quaternion.SlerpUnclamped(TargetRotation, PredictionRotation, ExtrapolationFactor);

        var nextPosition = Vector3.Lerp(rigid.position, lerpPosition, InterpolationFactor);

        rigid.position = nextPosition;
        rigid.rotation = Quaternion.Slerp(rigid.rotation, PredictionRotation, InterpolationFactor);
    }

    public void UpdateTarget(in int timeStamp, in Vector3 nextPosition, in Quaternion nextRotation, in Vector3 velocity, in Vector3 acceleration, in bool useSnap)
    {
        //when first received
        if (!isReceived || useSnap)
        {
            rigid.position = TargetPosition = nextPosition;
            rigid.rotation = TargetRotation = nextRotation;

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

        rigid.velocity = velocity;

        //continuous received
        lerpFactor = Mathf.Lerp(lerpFactor, (velocity /*+ (acceleration * (transportDiff + PacketSendInterval))*/).magnitude, 0.4f);

        PredictionPosition = nextPosition + (velocity /*+ (acceleration * (transportDiff + PacketSendInterval))*/) * (transportDiff + PacketSendInterval);
        PredictionRotation = nextRotation * (nextRotation * Quaternion.Inverse(TargetRotation));

        TargetPosition = nextPosition;
        TargetRotation = nextRotation;
    }


    private void OnDisable()
    {
        if (TagGame.Photon.PhotonManager.TryGetInstance(out var instance))
        {
            instance.OnAnimationMessageReceive -= PhotonManager_OnAnimationMessageReceive;
            instance.OnTrackedPoseMessageReceive -= PhotonManager_OnTrackedPoseMessageReceive;
        }
        TagGame.Photon.PhotonManager.OnInitialized -= PhotonManager_OnInitialized;
    }
}
