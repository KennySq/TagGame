using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviour
{
    private void OnEnable()
    {
        TagGame.Photon.PhotonManager.Instance.StartConnect();
    }

    private void OnDisable()
    {
        TagGame.Photon.PhotonManager.Instance.StartDisconnect();
    }
}