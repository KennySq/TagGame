using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 게임 매니저
// 네트워크, 플레이어 매니징
public class GameManager : MonoBehaviour
{
    public Button ConnectButton;
    private Level mLevel;

    [SerializeField]
    private List<GameObject> mPlayers;

    private GameObject mLocalPlayer;

    private void Awake()
    {
        mLevel = GetComponent<Level>();

    }

    void Start()
    {
    }

    void Update()
    {

    }
}
