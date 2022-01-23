using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
using TagGame.Photon;


public class Portal : MonoBehaviour
{
    [SerializeField]
    private List<Player3D> Players;

    [Serializable]
    public class PortalPair
    {
        public Collider Trigger;
        public Transform Destination;
    }

    [SerializeField]
    private List<PortalPair> portals;


    private void FixedUpdate()
    {
        foreach (var portal in portals)
        {
            foreach (var player in Players)
            {
                if (portal.Trigger.ComputePenetration(player.Capsule3D, out var dir, out var dis))
                {
                    Teleport(player, portal);
                }
            }
        }
    }

    private void Teleport(in Player3D player, in PortalPair portal)
    {
        if (!player.IsLocalPlayer)
            return;

        var diff = player.RigidGameObject.transform.position - portal.Trigger.transform.position;
        player.Rigidbody3D.MovePosition(portal.Destination.position + diff);

        PhotonManager.SendTrackedPoseData(new TrackedPosePacket()
        {
            pos = player.Rigidbody3D.position,
            velocity = player.Rigidbody3D.velocity,
            //acceleration = Acceleraction,

            rot = transform.rotation,
            useSnap = 1
        });
    }

}
