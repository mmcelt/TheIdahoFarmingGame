using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Doozy.Engine.Extensions;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	#region Public / Serialized Fields
	[Header("Login Panel")]
	[SerializeField] GameObject loginPanel;
	[SerializeField] TMP_InputField playerNameInput;
	[SerializeField] Button loginButton;
	[SerializeField] Button quitButton;

	[Header("Connecting Info Panel")]
	[SerializeField] GameObject connectingInfoPanel;

	[Header("Game Options Panel")]
	[SerializeField] GameObject gameOptionsPanel;
	[SerializeField] Button hostButton;

	[Header("Create Game Panel")]
	[SerializeField] GameObject createGamePanel;
	[SerializeField] TMP_InputField gameNameInput;
	[SerializeField] TMP_InputField numberOfPlayersInput;
	[SerializeField] Toggle networthToggle;
	[SerializeField] Toggle timedToggle;
	[SerializeField] TMP_InputField networthInput;
	[SerializeField] TMP_InputField timedInput;
	[SerializeField] Button createGameButton;
	[SerializeField] GameObject messageText;

	[Header("Join Game Panel")]
	[SerializeField] GameObject joinGamePanel;
	[SerializeField] GameObject roomListEntryPrefab;
	[SerializeField] Transform roomListParent;

	[Header("Creating Game Info Panel")]
	[SerializeField] GameObject creatingGameInfoPanel;

	[Header("Farmer Selection Panel")]	//AKA Inside Room Panel
	[SerializeField] GameObject farmerSelectionPanel;
	[SerializeField] Text gameInfoText;
	[SerializeField] GameObject playerListPrefab;
	[SerializeField] GameObject startGameButton;
	//[SerializeField] GameObject[] playerSelectionGameObjects;
	[SerializeField] Transform playerListContent;

	public Dictionary<int, GameObject> playerListGameobjects;

	[SerializeField] string gameTypeString = "?";
	[SerializeField] int gameAmount = 0;
	[SerializeField] string gameAmountString = "0";

	#endregion

	#region Private Fields / References

	Dictionary<string, RoomInfo> cachedRoomlist;
	Dictionary<string, GameObject> roomListGameobjects;

	int numberOfPlayers;
	int networthGameAmount;
	float timedGameAmount;

	#endregion

	#region Properties

	public static NetworkManager Instance;

	#endregion

	#region MonoBehaviour Methods

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
	}

	void Start() 
	{
		cachedRoomlist = new Dictionary<string, RoomInfo>();
		roomListGameobjects = new Dictionary<string, GameObject>();

		ActivatePanel(loginPanel.name);
		PhotonNetwork.AutomaticallySyncScene = true;
		networthToggle.isOn = true;

		//int test = 20;
		//Debug.Log("TEST: " + (test /= 10));
	}
	#endregion

	#region Photon Custom Properties

	ExitGames.Client.Photon.Hashtable _gameTypeProperty = new ExitGames.Client.Photon.Hashtable();

	#endregion

	#region UI Callbacks

	//used to ensure the player's name is not null or empty
	public void OnPlayerNameInputValueChanged()
	{
		if (playerNameInput.text != "")
			loginButton.interactable = true;
		else
			loginButton.interactable = false;
	}

	public void OnLoginButtonClicked()
	{
		string playerName = playerNameInput.text;

		//if (!string.IsNullOrEmpty(playerName))
		//{
			ActivatePanel(connectingInfoPanel.name);

			if (!PhotonNetwork.IsConnected)
			{
				PhotonNetwork.LocalPlayer.NickName = playerName;
				PhotonNetwork.ConnectUsingSettings();
			}
		//}
		//else
		//{
		//	Debug.LogWarning("Player Name is invalid!");
		//}
	}

	public void OnQuitButtonClicked()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}

	public void OnCancelButtonClicked()
	{
		PhotonNetwork.Disconnect();
		ActivatePanel(loginPanel.name);
	}

	public void OnGoBackButtonClicked()
	{
		ActivatePanel(gameOptionsPanel.name);
	}

	public void OnHostButtonClicked()
	{
		ActivatePanel(createGamePanel.name);
	}

	public void OnGameNameInputValueChanged()
	{
		if (gameNameInput.text != "")
		{
			numberOfPlayersInput.gameObject.SetActive(true);
		}
		else
		{
			numberOfPlayersInput.gameObject.SetActive(false);
		}
	}

	public void OnNOPInputValueChanged()
	{
		if (numberOfPlayersInput.text != "")
		{
			networthToggle.gameObject.SetActive(true);
			timedToggle.gameObject.SetActive(true);
			if (networthToggle.isOn)
			{
				networthInput.gameObject.SetActive(true);
				OnGameTypeToggleChanged();
			}

			numberOfPlayers = int.Parse(numberOfPlayersInput.text);
		}
		else
		{
			networthToggle.gameObject.SetActive(false);
			timedToggle.gameObject.SetActive(false);
			if (networthInput.isActiveAndEnabled)
				networthInput.gameObject.SetActive(false);
		}
	}

	public void OnGameTypeToggleChanged()
	{
		if (networthToggle.isOn)
		{
			networthInput.gameObject.SetActive(true);
			timedInput.gameObject.SetActive(false);
		}
		else
		{
			networthInput.gameObject.SetActive(false);
			timedInput.gameObject.SetActive(true);
		}
	}

	public void OnNetworthInputValueChanged()
	{
		if (networthInput.text != "")
		{
			createGameButton.gameObject.SetActive(true);
			networthGameAmount = int.Parse(networthInput.text);
			if (networthGameAmount < 41000)
				networthGameAmount = 41000;
		}
		else
			createGameButton.gameObject.SetActive(false);
	}

	public void OnTimedGameInputValueChanged()
	{
		if (timedInput.text != "")
		{
			createGameButton.gameObject.SetActive(true);
			timedGameAmount = float.Parse(timedInput.text);
		}
		else
			createGameButton.gameObject.SetActive(false);
	}

	public void OnCreateGameButtonClicked()
	{
		string roomName = gameNameInput.text;

		ActivatePanel(creatingGameInfoPanel.name);
		RoomOptions ro = new RoomOptions();
		ro.MaxPlayers = (byte)numberOfPlayers;
		ro.IsOpen = true;
		ro.IsVisible = true;
		PhotonNetwork.CreateRoom(roomName, ro);
	}

	public void OnJoinButtonClicked()
	{
		if (!PhotonNetwork.InLobby)
			PhotonNetwork.JoinLobby();

		ActivatePanel(joinGamePanel.name);
	}

	public void OnBackButtonClicked()
	{
		if (PhotonNetwork.InLobby)
			PhotonNetwork.LeaveLobby();

		ActivatePanel(gameOptionsPanel.name);
	}

	public void OnLeaveGameButtonClicked()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void OnStartGameButtonClicked()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		PhotonNetwork.LoadLevel(1);
	}
	#endregion

	#region Photon Callbacks

	public override void OnConnectedToMaster()
	{
		ActivatePanel(gameOptionsPanel.name);

		Debug.Log("Connected to Master");
	}

	public override void OnCreatedRoom()
	{
		if (networthToggle.isOn)
		{
			//ExitGames.Client.Photon.Hashtable networthProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Networth_Game, networthGameAmount } };
			//PhotonNetwork.CurrentRoom.SetCustomProperties(networthProp);

			_gameTypeProperty[IFG.Networth_Game] = networthGameAmount;
			PhotonNetwork.CurrentRoom.SetCustomProperties(_gameTypeProperty);

			//Debug.Log("SETTING NETWORTH FOR GAME " + networthGameAmount);
		}
		else if (timedToggle.isOn)
		{
			//ExitGames.Client.Photon.Hashtable timedProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Timed_Game, timedGameAmount } };
			//PhotonNetwork.CurrentRoom.SetCustomProperties(timedProp);

			_gameTypeProperty[IFG.Timed_Game] = timedGameAmount;
			PhotonNetwork.CurrentRoom.SetCustomProperties(_gameTypeProperty);

			//Debug.Log("SETTING TIME FOR GAME " + timedGameAmount);
		}
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
		ActivatePanel(gameOptionsPanel.name);
		messageText.SetActive(true);
		StartCoroutine(TryJoiningAgain());
	}

	public override void OnJoinedRoom()
	{
		print(PhotonNetwork.LocalPlayer.NickName + " has joined room: " + PhotonNetwork.CurrentRoom.Name);
		ActivatePanel(farmerSelectionPanel.name);

		//if (PhotonNetwork.LocalPlayer.IsMasterClient)
		//	startGameButton.SetActive(false);
		//else
		//	startGameButton.SetActive(false);

		//update gameInfoText
		UpdateRoomInfoText();

		if (playerListGameobjects == null)
			playerListGameobjects = new Dictionary<int, GameObject>();

		//instantiating playerListGameobjects
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			GameObject playerListGameobject = Instantiate(playerListPrefab, playerListContent);
			playerListGameobject.transform.localScale = Vector3.one;

			//update the playerListGameobject info...
			playerListGameobject.GetComponent<PlayerListEntryInitializer>().Initialize(player.ActorNumber, player.NickName);

			object isPlayerReady;
			if (player.CustomProperties.TryGetValue(IFG.Player_Ready, out isPlayerReady))
			{
				playerListGameobject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
			}
			//turn on the ready button for the local player only...
			if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
				playerListGameobject.transform.Find("Ready Button").gameObject.SetActive(true);
			else
				playerListGameobject.transform.Find("Ready Button").gameObject.SetActive(false);

			//update the playerListGameobjects Dictionary
			playerListGameobjects.Add(player.ActorNumber, playerListGameobject);
		}

		UpdateRoomInfoText();

		startGameButton.SetActive(false);
	}

	public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
	{
		GameObject playerListGameObject;

		if (playerListGameobjects.TryGetValue(target.ActorNumber, out playerListGameObject))
		{
			object isPlayerReady;
			if (changedProps.TryGetValue(IFG.Player_Ready, out isPlayerReady))
			{
				playerListGameObject.GetComponent<PlayerListEntryInitializer>().SetPlayerReady((bool)isPlayerReady);
			}

			object selectedFarmer;
			if (changedProps.TryGetValue(IFG.Selected_Farmer, out selectedFarmer))
			{
				Debug.Log(target.NickName + "'s " + "Selected Farmer is:" + (string)selectedFarmer);
			}
		}

		startGameButton.SetActive(CheckPlayersReady());
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		GameObject playerListGameobject = Instantiate(playerListPrefab, playerListContent);
		playerListGameobject.transform.localScale = Vector3.one;
		playerListGameobject.GetComponent<PlayerListEntryInitializer>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

		playerListGameobjects.Add(newPlayer.ActorNumber, playerListGameobject);

		UpdateRoomInfoText();

		if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
			startGameButton.SetActive(CheckPlayersReady());

		UpdateRoomInfoText();
	}


	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		UpdateRoomInfoText();

		Destroy(playerListGameobjects[otherPlayer.ActorNumber].gameObject);
		playerListGameobjects.Remove(otherPlayer.ActorNumber);

		startGameButton.SetActive(CheckPlayersReady());
	}

	public override void OnLeftRoom()
	{
		if (gameOptionsPanel != null)
			ActivatePanel(gameOptionsPanel.name);

		foreach (GameObject playerListGameobject in playerListGameobjects.Values)
		{
			Destroy(playerListGameobject);
		}
		playerListGameobjects.Clear();
		playerListGameobjects = null;
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		ClearRoomListView();

		foreach (RoomInfo room in roomList)
		{
			//Debug.Log("Room: " + room.Name);

			if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
			{
				//if this room is already in the list, remove the old entry
				if (cachedRoomlist.ContainsKey(room.Name))
				{
					cachedRoomlist.Remove(room.Name);
				}
			}
			else
			{
				//update cachedRoomList
				if (cachedRoomlist.ContainsKey(room.Name))
				{
					cachedRoomlist[room.Name] = room;
				}
				//add the new room to the cachedRoomList
				else
				{
					cachedRoomlist.Add(room.Name, room);
				}
			}
		}

		foreach (RoomInfo room in cachedRoomlist.Values)
		{
			GameObject roomListEntryGameObject = Instantiate(roomListEntryPrefab, roomListParent);
			roomListEntryGameObject.transform.localScale = Vector3.one;
			//fill in the room data...
			roomListEntryGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
			roomListEntryGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount + " / " + room.MaxPlayers;
			//add an OnClick() event handler in code...
			roomListEntryGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(() => OnJoinRoomButtonClicked(room.Name));

			//add the GameObject to the roomListGameobjects Dictionary
			roomListGameobjects.Add(room.Name, roomListEntryGameObject);
		}
	}

	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
		{
			startGameButton.SetActive(CheckPlayersReady());
		}
	}

	public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
	{
		UpdateRoomInfoText();
	}
	#endregion

	#region Public Methods

	public void ActivatePanel(string panelNameToBeActivated)
	{
		loginPanel.SetActive(loginPanel.name.Equals(panelNameToBeActivated));
		connectingInfoPanel.SetActive(connectingInfoPanel.name.Equals(panelNameToBeActivated));
		gameOptionsPanel.SetActive(gameOptionsPanel.name.Equals(panelNameToBeActivated));
		joinGamePanel.SetActive(joinGamePanel.name.Equals(panelNameToBeActivated));
		createGamePanel.SetActive(createGamePanel.name.Equals(panelNameToBeActivated));
		creatingGameInfoPanel.SetActive(creatingGameInfoPanel.name.Equals(panelNameToBeActivated));
		joinGamePanel.SetActive(joinGamePanel.name.Equals(panelNameToBeActivated));
		farmerSelectionPanel.SetActive(farmerSelectionPanel.name.Equals(panelNameToBeActivated));
	}
	#endregion

	#region Private Methods

	public void UpdateRoomInfoText()
	{
		//string gameTypeString = "?";
		//int gameAmount = 0;
		//string gameAmountString = "0";

		if (_gameTypeProperty.ContainsKey(IFG.Networth_Game))
		{
			gameTypeString = "Networth Game";
			gameAmount = (int)_gameTypeProperty[IFG.Networth_Game];
			gameAmountString = gameAmount.ToString("c0");
		}

		if (_gameTypeProperty.ContainsKey(IFG.Timed_Game))
		{
			gameTypeString = "Timed Game";
			gameAmount = (int)(float)_gameTypeProperty[IFG.Timed_Game];
			gameAmountString = gameAmount + " Min";
		}

		//gameInfoText.text = "Game Name: " + PhotonNetwork.CurrentRoom.Name + "  " + "Players/MaxPlayers: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + "   Game Type: " + gameTypeString + " : " + gameAmountString;

		photonView.RPC("UpdateRoomTextRPC", RpcTarget.AllBuffered, gameTypeString,  gameAmountString);
	}

	[PunRPC]
	void UpdateRoomTextRPC(string typeString, string amountString)
	{

		gameInfoText.text = "Game Name: " + PhotonNetwork.CurrentRoom.Name + "  " + "Players/MaxPlayers: " + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + "   Game Type: " + typeString + " : " + amountString;

	}
	IEnumerator TryJoiningAgain()
	{
		yield return new WaitForSeconds(3);
		ActivatePanel(connectingInfoPanel.name);
		PhotonNetwork.JoinRandomRoom();
	}

	//void UpdatePlayerInfo()
	//{
	//	gameInfoText.text = "Game name: " + PhotonNetwork.CurrentRoom.Name + " " + " Players/MaxPlaers: " + PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
	//}

	void ClearRoomListView()
	{
		//destroy the actual gameobjects...
		foreach (var roomListGameobject in roomListGameobjects.Values)
			Destroy(roomListGameobject);

		//clear the Dictionary...
		roomListGameobjects.Clear();
	}

	void OnJoinRoomButtonClicked(string roomName)
	{
		if (PhotonNetwork.InLobby)
			PhotonNetwork.LeaveLobby();

		PhotonNetwork.JoinRoom(roomName);
	}

	bool CheckPlayersReady()
	{
		if (!PhotonNetwork.IsMasterClient)
			return false;

		foreach (Player player in PhotonNetwork.PlayerList)
		{
			object isPlayerReady;
			if (player.CustomProperties.TryGetValue(IFG.Player_Ready, out isPlayerReady))
			{
				if (!(bool)isPlayerReady)
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
			return true;
		else
			return false;

		//return true;
	}
	#endregion
}
