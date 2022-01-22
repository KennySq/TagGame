using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using TagGame.Photon;

// 게임 매니저
// 네트워크, 플레이어 매니징
public class GameManager : MonoBehaviour
{
    public Button ConnectButton;
    private Level mLevel;

    [SerializeField]
    private List<GameObject> mPlayers;

    private GameObject mLocalPlayer;

    private PhotonManager mNetManager;

    private void Awake()
    {
        if (Actor.LocalPlayer.CurrentData == null)
        {
            Actor.LocalPlayer.OnDataChangedOnce += OnLocalPlayerInitlaized;
        }
        else
        {
            OnLocalPlayerInitlaized(Actor.LocalPlayer.CurrentData);
        }
    }

    private void OnLocalPlayerInitlaized(Actor obj)
    {
        mLocalPlayer = obj.gameObject;
        Actor localActor = obj;

        mLevel.LocalPlayer = mLocalPlayer;
        mLevel.MainCamera = localActor.MainCamera;
        localActor.MainCamera.gameObject.SetActive(true);
    }

}
