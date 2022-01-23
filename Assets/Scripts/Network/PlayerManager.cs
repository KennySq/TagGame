using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Utils;
using TagGame.Photon;

[System.Obsolete]
public class PlayerManager : MonoSingleton<PlayerManager>
{

    //private class LongComparer : IEqualityComparer<long>
    //{
    //    public bool Equals(long x, long y)
    //    {
    //        return x == y;
    //    }

    //    public int GetHashCode(long obj)
    //    {
    //        return (int)obj;
    //    }
    //}

    //[SerializeField]
    //private GameObject RemotePlayerOrigin;

    //public event Action<long, long> OnPlayerDeath;


    //protected override void Awake()
    //{
    //    base.Awake();
    //    if (_instance != this)
    //    {
    //        DestroyImmediate(gameObject);
    //        return;
    //    }
    //    UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    //}

    //private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    //{
    //    //이건 뭔코드지? 
    //    foreach (var player in playerDict.Values)
    //    {
    //        if (player != null)
    //            player.Destroy();
    //    }

    //    playerDict.Clear();
    //}

    //private Dictionary<long, RemotePlayer> playerDict = new Dictionary<long, RemotePlayer>(new LongComparer());

    //public int GetRemotePlayerCount { get => playerDict.Count; }

    //public bool TryGetRemotePlayer(in long uuid, out RemotePlayer player)
    //{
    //    return playerDict.TryGetValue(uuid, out player);
    //}



    //private void OnEnable()
    //{
    //    PhotonManager.OnInitialized += (instance) =>
    //    {
    //        instance.OnTrackedPoseMessageReceive += Instance_OnTrackedPoseMessageReceive;
    //        instance.OnPlayerDisconnected += Instance_OnPlayerDisconnected;
    //    };
    //}

    //private void Instance_OnPlayerDisconnected(long uuid)
    //{
    //    if (playerDict.TryGetValue(uuid, out var player))
    //    {
    //        player.Destroy();
    //    }

    //    playerDict.Remove(uuid);
    //    OnPlayerDeath?.Invoke(uuid, -1);
    //}

    //private void Instance_OnTrackedPoseMessageReceive(TrackedPosePacket obj)
    //{
    //    if (!playerDict.TryGetValue(obj.UUID, out var player))
    //    {
    //        var instance = Instantiate(RemotePlayerOrigin);

    //        player = instance.GetComponent<RemotePlayer>();
    //        player.Initialize(obj);
    //        player.OnDeath += Player_OnDeath;
    //        player.OnDestroyed += Player_OnDestroyed;

    //        playerDict[obj.UUID] = player;
    //    }

    //    player.UpdateTransform(obj);
    //}

    //private void Player_OnDestroyed(long uuid)
    //{
    //    playerDict.Remove(uuid);
    //}

    //public bool GetNextPlayer(in RemotePlayer current, out RemotePlayer next)
    //{
    //    next = null;

    //    var list = playerDict.Values.Where(player => player.uuid != PlayerUUID.UUID).ToList();
    //    if (list == null || list.Count <= 0)
    //        return false;

    //    var idx = list.IndexOf(current);
    //    if (idx < 0)
    //        return false;

    //    var nextIdx = (idx + 1) % list.Count;
    //    next = list[nextIdx];

    //    return true;
    //}

    //public bool GetPrevPlayer(in RemotePlayer current, out RemotePlayer prev)
    //{
    //    prev = null;

    //    var list = playerDict.Values.Where(player => player.uuid != PlayerUUID.UUID).ToList();
    //    if (list == null || list.Count <= 0)
    //        return false;

    //    var idx = list.IndexOf(current);
    //    if (idx < 0)
    //        return false;

    //    var prevIdx = (list.Count + (idx - 1)) % list.Count;
    //    prev = list[prevIdx];

    //    return true;
    //}

    //public List<RemotePlayer> GetPlayerList()
    //{
    //    var list = playerDict.Values.Where(player => player.uuid != PlayerUUID.UUID).ToList();
    //    return list;
    //}

    //private void Player_OnDeath(long uuid, long attacker_uuid)
    //{
    //    if (playerDict.TryGetValue(uuid, out var player))
    //    {
    //        player.OnDeath -= Player_OnDeath;
    //        player.Destroy();

    //        playerDict.Remove(uuid);
    //        OnPlayerDeath?.Invoke(uuid, attacker_uuid);
    //        if (playerDict.Count == 0)
    //        {
    //            GameManager.instance.ShowResult();
    //        }
    //    }
    //}

    //private void OnDisable()
    //{
    //    PhotonManager.OnInitialized += (instance) =>
    //    {
    //        instance.OnPlayerDisconnected -= Instance_OnPlayerDisconnected;
    //        instance.OnTrackedPoseMessageReceive -= Instance_OnTrackedPoseMessageReceive;
    //    };
    //}
}