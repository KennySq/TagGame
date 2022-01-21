using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameManager : MonoBehaviour
{
    public string IPAddress;
    public Button ConnectButton;

    private NetworkManager mNetworkManager;

    private void Awake()
    {
        mNetworkManager = GetComponent<NetworkManager>();


    }

    void Start()
    {
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    public void Host()
    {
        bool bResult = mNetworkManager.StartHost();

        if(bResult == false)
        {
            Debug.LogError("Failed to hosting session.");
        }
    }

}
