using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonChatManager : MonoBehaviour, IChatClientListener
{
	#region Setup

	public static PhotonChatManager Instance;

	[SerializeField] GameObject joinChatButton;
	ChatClient chatClient;
	bool isConnected;
	[SerializeField] string username;
	[SerializeField] bool chatPanelActive;

	//public void UsernameOnValueChange(string valueIn)
	//{
	//	username = valueIn;
	//}

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
	}

	public void ChatConnectOnClick()
	{
		isConnected = true;
		chatClient = new ChatClient(this);
		chatClient.ChatRegion = "US";
		chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(username));
		Debug.Log("Connecting");
	}

	#endregion Setup

	#region General

	[SerializeField] GameObject chatPanel;
	string privateReceiver = "";
	string currentChat;
	[SerializeField] InputField chatField;
	[SerializeField] Text chatDisplay;

	void Start()
	{
		chatField.text = "";
		chatPanelActive = false;

		username = PhotonNetwork.LocalPlayer.NickName;
		ChatConnectOnClick();
	}

	void Update()
	{
		if (isConnected)
		{
			chatClient.Service();
		}

		if (chatField.text != "" && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
		{
			SubmitPublicChatOnClick();
			SubmitPrivateChatOnClick();
		}

	}

	public void ToggleChatWindow()
	{
		chatPanelActive = !chatPanelActive;
		chatPanel.SetActive(chatPanelActive);
	}

	#endregion General

	#region PublicChat

	public void SubmitPublicChatOnClick()
	{
		if (privateReceiver == "")
		{
			chatClient.PublishMessage("RegionChannel", currentChat);
			chatField.text = "";
			currentChat = "";
		}
	}

	public void TypeChatOnValueChange(string valueIn)
	{
		currentChat = valueIn;
	}

	#endregion PublicChat

	#region PrivateChat

	public void ReceiverOnValueChange(string valueIn)
	{
		privateReceiver = valueIn;
	}

	public void SubmitPrivateChatOnClick()
	{
		if (privateReceiver != "")
		{
			chatClient.SendPrivateMessage(privateReceiver, currentChat);
			chatField.text = "";
			currentChat = "";
		}
	}

	#endregion PrivateChat

	#region Callbacks

	public void DebugReturn(DebugLevel level, string message)
	{
		//throw new System.NotImplementedException();
	}

	public void OnChatStateChange(ChatState state)
	{
		if (state == ChatState.Uninitialized)
		{
			isConnected = false;
			joinChatButton.SetActive(true);
			chatPanel.SetActive(false);
		}

		//throw new System.NotImplementedException();
		//Debug.Log("Connected");
		//isConnected = true;
		//joinChatButton.SetActive(false);
	}

	public void OnConnected()
	{
		Debug.Log("Connected");
		joinChatButton.SetActive(false);
		chatClient.Subscribe(new string[] { "RegionChannel" });
	}

	public void OnDisconnected()
	{
		isConnected = false;
		joinChatButton.SetActive(true);
		chatPanel.SetActive(false);
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (!chatPanel.activeSelf)
		{
			chatPanel.SetActive(true);
			chatPanelActive = true;
		}

		string msgs = "";
		for (int i = 0; i < senders.Length; i++)
		{
			msgs = string.Format("{0}: {1}", senders[i], messages[i]);

			chatDisplay.text += "\n" + msgs;

			Debug.Log(msgs);
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		string msgs = "";

		msgs = string.Format("(Private) {0}: {1}", sender, message);

		chatDisplay.text += "\n " + msgs;

		Debug.Log(msgs);

	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		throw new System.NotImplementedException();
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		//chatPanel.SetActive(true);
		Debug.Log("Subscribed to: " + channels[0]);
	}

	public void OnUnsubscribed(string[] channels)
	{
		throw new System.NotImplementedException();
	}

	public void OnUserSubscribed(string channel, string user)
	{
		throw new System.NotImplementedException();
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
		throw new System.NotImplementedException();
	}

	#endregion Callbacks
}
