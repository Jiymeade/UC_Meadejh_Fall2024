using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;


public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Button _startBttn, _leaveBttn, _readyBttn;
    [SerializeField] private GameObject _panelPrefab;
    [SerializeField] private GameObject _ContentGo;
    [SerializeField] private TMP_Text rdytxt;

    [SerializeField] private NetworkedPlayerData _networkPlayers;

    private List<GameObject> _PlayerPanels = new List<GameObject>();

    private ulong _myLocalClientId;

    private bool _isReady = false;

    private void Start()
    {
        _myLocalClientId = NetworkManager.ServerClientId;

        if (IsServer)
        {
            rdytxt.text = "Waiting for Players";
            _readyBttn.gameObject.SetActive(false);
        }
        else
        {
            rdytxt.text = "Not Ready";
            _readyBttn.gameObject.SetActive(true);
        }
        _networkPlayers._allConnectedPlayers.OnListChanged += NetPlayersChanged;
        _leaveBttn.onClick.AddListener(leaveBttnClick);
        _readyBttn.onClick.AddListener(clientRdyBttnToggle);
    }

    private void clientRdyBttnToggle()
    {
        if(IsServer) {return;}
        _isReady = !_isReady;
        if (_isReady)
        {
            rdytxt.text = "Ready";
        }
        else
        {
            rdytxt.text = "Not Ready";
        }
        RdyBttnToggleServerRpc(_isReady);
    }

    [Rpc(target:SendTo.Server, RequireOwnership = false)]
    private void RdyBttnToggleServerRpc(bool readyStatus, RpcParams rpcParams = default)
    {
        Debug.Log(message: "From Rdy bttn RPC");
        _networkPlayers.UpdateReadyClient(rpcParams.Receive.SenderClientId, readyStatus);
    }

    private void leaveBttnClick()
    {
        if (!IsServer) 
        {
            QuitLobbyServerRpc();
            SceneManager.LoadScene(0);

        }
        else
        {
            foreach (PlayerInfoData playerdata in _networkPlayers._allConnectedPlayers)
            {
                if(playerdata._clientId != _myLocalClientId)
                {
                    KickUserBttn(playerdata._clientId);
                }
            }
            NetworkManager.Shutdown();
            SceneManager.LoadScene(0);
        }
    }

    [Rpc(target:SendTo.Server, RequireOwnership = false)]
    private void QuitLobbyServerRpc(RpcParams rpcParams=default)
    {
        KickUserBttn(rpcParams.Receive.SenderClientId);
    }

    private void NetPlayersChanged(NetworkListEvent<PlayerInfoData> changeevent)
    {
        Debug.Log(message: "Net Players had changed event fired!");
        PopulateLabels();
    }

    [ContextMenu(itemName:"PopulateLabel")]
    private void PopulateLabels()
    {
        ClearPlayerPanel();

        bool allReady = true;

        foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            GameObject newPlayerPanel = Instantiate(_panelPrefab, _ContentGo.transform);
            PlayerLabel _playerLabel = newPlayerPanel.GetComponent<PlayerLabel>();

            _playerLabel.OnKickClicked += KickUserBttn;

            if(IsServer && playerData._clientId != _myLocalClientId)
            {
                _playerLabel.setKickActive(isOn: true);
                _readyBttn.GameObject().SetActive(false);
            }
            else
            {
                _playerLabel.setKickActive(isOn: false);
                
            }

            _playerLabel.SetPlayerLabelName(playerData._clientId);
            _playerLabel.SetReady(playerData._isPlayerReady);
            _playerLabel.setPlayerColor(playerData._colorId);
            _PlayerPanels.Add(newPlayerPanel);

            if (playerData._isPlayerReady == false)
            {
                allReady = false;
            }
        }
        if (IsServer)
        {
            if (allReady)
            {
                if(_networkPlayers._allConnectedPlayers.Count > 1)
                {
                    rdytxt.text = "Ready to start";
                    _startBttn.gameObject.SetActive(true);
                }
                else
                {
                    rdytxt.text = "Empty Lobby";
                }
            }
            else
            {
                _startBttn.gameObject.SetActive(false);
                rdytxt.text = "waiting for ready players";
            }
        }
    }

    private void KickUserBttn(ulong kickTarget)
    {
      if(!IsServer || !IsHost) return;

        foreach (PlayerInfoData playerData in _networkPlayers._allConnectedPlayers)
        {
            if (playerData._clientId == kickTarget)
            {
                KickedClientRpc(RpcTarget.Single(kickTarget, RpcTargetUse.Temp));

                NetworkManager.Singleton.DisconnectClient(kickTarget);
            }
        }
    }


    [Rpc(SendTo.SpecifiedInParams)]
    private void KickedClientRpc(RpcParams RpcParmas)
    {
        SceneManager.LoadScene(0);
    }

    private void ClearPlayerPanel()
    {
        foreach (GameObject panel in _PlayerPanels)
        {
            Destroy(panel);
        }
        _PlayerPanels.Clear();
    }
}
