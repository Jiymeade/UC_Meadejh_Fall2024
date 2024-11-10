using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _chat;           
    [SerializeField] private Button _sendBttn;               
    [SerializeField] private TMP_InputField ChatInputField;    

    private string chatHistory = "";

    private void Start()
    {

        _chat.gameObject.SetActive(false);


        if (IsServer)
        {
            _sendBttn.gameObject.SetActive(false); 
        }
        else
        {
            _sendBttn.onClick.AddListener(OnSendButtonPressed); 
        }
    }

    private void OnSendButtonPressed()
    {
        _chat.gameObject.SetActive(true);
        string message = ChatInputField.text;
        if (!string.IsNullOrEmpty(message))
        {
            SendMessageToServerRpc(message);
            ChatInputField.text = ""; 
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMessageToServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        string formattedMessage = $"Player {senderClientId}: {message}";

        UpdateChatHistoryClientRpc(formattedMessage); 
    }

    [ClientRpc]
    private void UpdateChatHistoryClientRpc(string formattedMessage)
    {
        chatHistory += formattedMessage + "\n";
        _chat.text = chatHistory; 
    }
}
