using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSendPosition : MonoBehaviour
{
    private IEnumerator Start()
    {
        var waitForSendRate = new WaitForSecondsRealtime(1f / PhotonNetwork.SendRate);

        while (isActiveAndEnabled)
        {
            UpdateInternal();

            yield return waitForSendRate;
        }
    }

    [SerializeField]
    private Rigidbody rigid;

    //dummys;
    private Vector3 CurrentVelocity;
    private Vector3 LastVelocity;

    private Vector3 Acceleraction;

    private Quaternion LastRotation;
    private Quaternion CurrentRotation;


    private void FixedUpdate()
    {
        LastVelocity = CurrentVelocity;
        CurrentVelocity = rigid.velocity;

        Acceleraction = CurrentVelocity - LastVelocity;

        LastRotation = CurrentRotation;
        CurrentRotation = transform.rotation;
    }

    private void UpdateInternal()
    {
        TagGame.Photon.PhotonManager.SendTrackedPoseData(new TagGame.Photon.TrackedPosePacket()
        {
            pos = rigid.position,
            velocity = rigid.velocity,
            acceleration = Acceleraction,

            rot = transform.rotation,
        });
    }
}