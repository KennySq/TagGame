using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���� �Ŵ���
// ��Ʈ��ũ, �÷��̾� �Ŵ�¡
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
