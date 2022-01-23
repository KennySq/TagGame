using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSendPosition : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigid;
    public Actor Owner { get; private set; }

    //dummys;
    private Vector3 CurrentVelocity;
    private Vector3 LastVelocity;

    private Vector3 Acceleraction;

    private Quaternion LastRotation;
    private Quaternion CurrentRotation;

    public void Initialize(in Actor actor)
    {
        this.enabled = true;

        Owner = actor;

        Owner.OnTeleport += Owner_OnTeleport;
    }

    private IEnumerator Start()
    {
        var waitForSendRate = new WaitForSecondsRealtime(1f / PhotonNetwork.SendRate);

        while (isActiveAndEnabled)
        {
            UpdateInternal();

            yield return waitForSendRate;
        }
    }

    private void Owner_OnTeleport()
    {
        UpdateInternal(true);
    }

    private void FixedUpdate()
    {
        LastVelocity = CurrentVelocity;
        CurrentVelocity = rigid.velocity;

        Acceleraction = CurrentVelocity - LastVelocity;

        LastRotation = CurrentRotation;
        CurrentRotation = transform.rotation;
    }

    private void UpdateInternal(bool isSnap = false)
    {
        TagGame.Photon.PhotonManager.SendTrackedPoseData(new TagGame.Photon.TrackedPosePacket()
        {
            pos = rigid.position,
            velocity = rigid.velocity,
            //acceleration = Acceleraction,

            rot = transform.rotation,
            useSnap = isSnap ? 1 : 0
        });
    }
}