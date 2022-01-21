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


    //dummys;
    private Vector3 CurrentPosition;
    private Vector3 LastPosition;

    private Vector3 CurrentVelocity;
    private Vector3 LastVelocity;

    private Vector3 Acceleraction;

    private Quaternion LastRotation;
    private Quaternion CurrentRotation;


    private void Update()
    {
        LastPosition = CurrentPosition;
        CurrentPosition = transform.position;

        LastVelocity = CurrentVelocity;
        CurrentVelocity = CurrentPosition - LastPosition;

        Acceleraction = CurrentVelocity - LastVelocity;

        LastRotation = CurrentRotation;
        CurrentRotation = transform.rotation;

    }

    private void UpdateInternal()
    {
        TagGame.Photon.PhotonManager.SendTrackedPoseData(new TagGame.Photon.TrackedPosePacket()
        {
            pos = CurrentPosition,
            velocity = CurrentVelocity,
            acceleration = Acceleraction,

            rot = transform.rotation,
        });
    }
}