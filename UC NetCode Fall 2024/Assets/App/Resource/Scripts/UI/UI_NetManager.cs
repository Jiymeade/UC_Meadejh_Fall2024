using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System;

public class UI_NetManager : NetworkBehaviour
{
    [SerializeField] private Button serverBttn, clientBttn, hostBttn, startBttn;
    [SerializeField] private GameObject _connetionBttnGroup, _socialPanel;

    [SerializeField] private SpawnController _mySpawnController;
    void Start()
    {
        startBttn.gameObject.SetActive(false);
        if(hostBttn != null) hostBttn.onClick.AddListener(HostClick);
        if (clientBttn != null) clientBttn.onClick.AddListener(ClientClick);
        if (serverBttn != null) serverBttn.onClick.AddListener(ServerClick);
        if (startBttn != null) startBttn.onClick.AddListener(StartClick);
    }

    private void StartClick()
    {
        if (IsServer)
        {
            _mySpawnController.SpawnAllPlayers();
            HideGuiRpc();
        }
        
    }

    [Rpc(target:SendTo.Everyone)]
    private void HideGuiRpc()
    {
        _socialPanel.SetActive(false);
    }

    private void ServerClick()
    {
        NetworkManager.Singleton.StartServer();
        _connetionBttnGroup.SetActive(false);
        startBttn.gameObject.SetActive(true);
    }
    private void ClientClick()
    {
        NetworkManager.Singleton.StartClient();
        _connetionBttnGroup.SetActive(false);
        
    }
    private void HostClick()
    {
        NetworkManager.Singleton.StartHost();
        _connetionBttnGroup.SetActive(false);
        startBttn.gameObject.SetActive(true);
    }
}

