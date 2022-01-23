using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using TagGame.Photon;
using Photon.Pun;
using System.Linq;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField]
    private List<Rigidbody> Platforms;

    public List<Collider> colliders;

    [SerializeField]
    private float upwardSpeed;

    [SerializeField]
    private float threshold;

    private void OnEnable()
    {
        PhotonManager.Instance.OnPlatformPacketReceive += Instance_OnPlatformPacketReceive;
    }

    private void Instance_OnPlatformPacketReceive(PlatformPacket obj)
    {
        var transportDiff = (PhotonNetwork.ServerTimestamp - obj.time_stamp) * 0.001f;

        for (int i = 0; i < Platforms.Count; ++i)
        {
            Platforms[i].position = new Vector3(0, 0, obj.heights[i] + (upwardSpeed * Time.fixedDeltaTime * transportDiff));
        }
    }

    private IEnumerator Start()
    {
        var waitForSendRate = new WaitForSecondsRealtime(2f);

        while (isActiveAndEnabled)
        {
            UpdateInternal();

            yield return waitForSendRate;
        }
    }

    private void UpdateInternal()
    {
        var list = Platforms.Select((rig) => rig.position.z);
        PhotonManager.SendPlatformPacketData(new PlatformPacket()
        {
            heights = list.ToList()
        });
    }

    private void FixedUpdate()
    {
        foreach (var platform in Platforms)
        {
            platform.MovePosition(platform.position + Vector3.forward * upwardSpeed * Time.fixedDeltaTime);

            if (platform.position.z > threshold)
            {
                platform.position = new Vector3(platform.position.x, platform.position.y, -platform.position.z);
            }
        }
    }

    private void OnDisable()
    {
        PhotonManager.Instance.OnPlatformPacketReceive += Instance_OnPlatformPacketReceive;
    }
}
