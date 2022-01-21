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
        mLevel = GetComponent<Level>();
    }

    void Start()
    {
        // 임시 코드.
        // 포톤 적용후엔 local-player로 초기화.
        mLocalPlayer = GameObject.Find("Player3D");

        Actor localActor = mLocalPlayer.GetComponent<Actor>();
        localActor.CurrentLevel = mLevel;
        mLevel.LocalPlayer = mLocalPlayer;
    }


    void Update()
    {

    }
}
