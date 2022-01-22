using System.Collections;
using UnityEngine;

using Utils;
using TagGame.Photon;



public class PlayerStatus
{
    public readonly Notifier<int> HP = new Notifier<int>();
}

public class LocalPlayerData : Singleton<LocalPlayerData>
{
    public readonly Notifier<int> characterIndex = new Notifier<int>();
    public PlayerStatus status = new PlayerStatus();

    public void SendPlayerStatus()
    {
        var packet = new PlayerStatusPacket();
        packet.HP = status.HP.CurrentData;

        PhotonManager.SendPlayerStatusPacketData(packet);
    }

}

public class RemotePlayerData : Singleton<RemotePlayerData>
{
    public readonly Notifier<int> characterIndex = new Notifier<int>();

    public PlayerStatus status = new PlayerStatus();


    public void Initialize()
    {
        PhotonManager.OnInitialized += PhotonManager_OnInitialized;
    }

    private void PhotonManager_OnInitialized(PhotonManager photonManager)
    {
        photonManager.OnChacterInitializeMessageReceive += PhotonManager_OnChacterInitializePacketReceive;
        photonManager.OnPlayerStatusMessageReceive += PhotonManager_OnPlayerStatusMessageReceive;
    }

    private void PhotonManager_OnPlayerStatusMessageReceive(PlayerStatusPacket obj)
    {
        status.HP.CurrentData = obj.HP;
    }

    private void PhotonManager_OnChacterInitializePacketReceive(ChacterInitializePacket obj)
    {
        this.characterIndex.CurrentData = obj.index;
        LocalPlayerData.Instance.characterIndex.CurrentData = obj.index == 1 ? 0 : 1;

        Debug.Log($"remote : {characterIndex.CurrentData}, local : {LocalPlayerData.Instance.characterIndex.CurrentData}");
    }

    public void Release()
    {
        if (PhotonManager.TryGetInstance(out var instance))
        {
            instance.OnPlayerStatusMessageReceive -= PhotonManager_OnPlayerStatusMessageReceive;
            instance.OnChacterInitializeMessageReceive -= PhotonManager_OnChacterInitializePacketReceive;
        }
        else
        {
            PhotonManager.OnInitialized -= PhotonManager_OnInitialized;
        }
    }
}