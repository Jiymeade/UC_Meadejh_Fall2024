using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button serverBttn, clientBttn, hostBttn;
    void Start()
    {
        hostBttn.onClick.AddListener(HostClick);
        clientBttn.onClick.AddListener(ClientClick);
        serverBttn.onClick.AddListener(ServerClick);
    }

    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
        this.gameObject.SetActive(false);
    }
    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
        this.gameObject.SetActive(false);
    }
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        this.gameObject.SetActive(false);
    }
}

