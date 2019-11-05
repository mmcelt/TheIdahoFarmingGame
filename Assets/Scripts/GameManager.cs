using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPun
{
	#region Public / Serialized Fields

	[SerializeField] GameObject[] playerPrefabs;
	[SerializeField] Transform[] playerSpawnPoints;

	public string myFarmerName;
	public GameObject myFarmer;
	public UIManager uiManager;
	public BoardManager bManager;
	public MyDiceRoll myDiceRoll;
	public HarvestManager hManager;
	public DeckManager dManager;
	public StickerManager sManager;

	public Text _timerText;
	public int _activePlayer;
	public string _gameMode;
	public int _numberOfPlayers;
	public List<Player> _cachedPlayerList;

	#endregion

	#region Private Fields / References

	int farmerIndex;

	#endregion

	#region Properties

	public static GameManager Instance = null;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnSpudsPurchased;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnSpudsPurchased;
	}

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);

		//DontDestroyOnLoad(gameObject);
	}

	void Start() 
	{
		_cachedPlayerList = new List<Player>();

		if (PhotonNetwork.IsConnectedAndReady)
		{
			//instantiate the local player...
			object selectedFarmer;

			if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(IFG.Selected_Farmer, out selectedFarmer))
			{
				int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

				myFarmerName = (string)selectedFarmer;

				//Debug.Log("myFarmer: " + myFarmerName);

				int farmerIndex = GetIndexFromName(myFarmerName);


				Vector3 spawnPosition = playerSpawnPoints[farmerIndex].position;

				InstantiateFarmer(farmerIndex, spawnPosition);

				//if (PhotonNetwork.IsMasterClient)
					StartCoroutine(GameLoopRoutine());
			}
		}
		SetTheGameMode();
	}

	#endregion

	#region Public Methods

	public void IncrementTheActivePlayer()
	{
		_activePlayer++;
		if (_activePlayer == _cachedPlayerList.Count + 1)
			_activePlayer = 1;

		ChangeActivePlayer();
	}

	#endregion

	#region Private Methods

	IEnumerator GameLoopRoutine()
	{
		yield return StartCoroutine(PregameSetupRoutine());
		yield return StartCoroutine(GameSetupRoutine());
	}

	IEnumerator PregameSetupRoutine()
	{
		SetTheGameMode();
		_numberOfPlayers = GetNumberOfPlayers();
		UpdateCachedPlayerList();

		yield return null;
	}

	IEnumerator GameSetupRoutine()
	{
		//give 2 OTB's to each player
		if (PhotonNetwork.IsMasterClient)
		{
			GiveTwoOtbCardsToPlayers();
			//place the initial board stickers
			PlaceInitialBoardStickers();
		}
		//determine who is 1st player
		SelectFirstPlayer();

		//Debug.Log("Active Player: " + _activePlayer);
		yield return null;
	}

	void SetTheGameMode()
	{
		object gameType;
		if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(IFG.Networth_Game, out gameType))
			_gameMode = "Networth Game";
		else
			_gameMode = "Timed Game";

		Debug.Log("Game Mode: " + _gameMode);
	}

	int GetNumberOfPlayers()
	{
		int numberOfPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
		return numberOfPlayers;
	}

	int GetIndexFromName(string farmer)
	{
		//Debug.Log("farmer: " + farmer);

		if (farmer == "Rigby Ron")
			return 0;
		else if (farmer == "Kimberly Kay")
			return 1;
		else if (farmer == "Jerome Jerry")
			return 2;
		else if (farmer == "Ririe Ric")
			return 3;
		else if (farmer == "Menan Mike")
			return 4;
		else if (farmer == "Blackfoot Becky")
			return 5;
		else
		{
			Debug.LogWarning("INVALID FARMER!");
			return 0;
		}
	}

	void InstantiateFarmer(int farmerIndex, Vector3 spawnPosition)
	{
		PhotonNetwork.Instantiate(playerPrefabs[farmerIndex].name, spawnPosition, Quaternion.identity);
	}

	void UpdateCachedPlayerList()
	{
		foreach (Player player in PhotonNetwork.PlayerList)
			_cachedPlayerList.Add(player);
	}

	void GiveTwoOtbCardsToPlayers()
	{
		for(int i=0; i<2; i++)
		{
			foreach (Player player in _cachedPlayerList)
			{
				//target player
				int targetPlayer = player.ActorNumber;

				OTBCard drawnCard = dManager._otbCards[0];
				dManager._otbCards.RemoveAt(0);

				//Raise the return event to the sender...
				//event data: cardNumber/description/summary/totalCost
				object[] sndData = new object[] { drawnCard.cardNumber, drawnCard.description, drawnCard.summary, drawnCard.totalCost };
				//send options
				RaiseEventOptions eventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { targetPlayer },
					CachingOption = EventCaching.DoNotCache
				};
				//send option
				SendOptions sendOptions = new SendOptions
				{
					Reliability = true
				};
				//fire the event...
				PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Get_Initial_Otb_Event_Code, sndData, eventOptions, sendOptions);
			}
		}
	}

	void PlaceInitialBoardStickers()
	{
		//get the farmer name
		//string farmer = (string)player.CustomProperties[IFG.Selected_Farmer];
		//event data
		object[] sndData = new object[] { 1 };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Place_Initial_Stickers_Event_Code, sndData, eventOptions, sendOptions);
	}

	void SelectFirstPlayer()
	{
		_activePlayer = Random.Range(1, _numberOfPlayers + 1);
		ChangeActivePlayer();
	}

	void ChangeActivePlayer()
	{
		//fire the change active player event...
		//event data
		object[] sendData = new object[] { _activePlayer };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Change_Active_Player_Event_Code, sendData, eventOptions, sendOptions);
	}

	void OnSpudsPurchased(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Spud_Bonus_Given_Event_Code)
		{
			IFG.SpudBonusGiven = true;
			//extract the sent data: awarded nickname, awarded farmer
			object[] recData = (object[])eventData.CustomData;
			string awardedPlayer = (string)recData[0];
			string awardedFarmer = (string)recData[1];
			//send msg to display the message to all...
			//event data
			Color fontColor = uiManager.SelectFontColorForFarmer(awardedFarmer);
			Vector3 splitColor = Vector3.zero;
			splitColor = SplitColorToRGB(fontColor);
			string message = awardedPlayer + " has received a $1000 Bonus for being the first to plant Spuds! This is Idaho after all...";
			object[] sndData = new object[] {  IFG.SpudBonusGiven, splitColor, message };
			//event options
			RaiseEventOptions eventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.All,
				CachingOption = EventCaching.DoNotCache
			};
			//send options
			SendOptions sendOptions = new SendOptions() { Reliability = true };
			//fire the event
			PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Spud_Message_Event_Code, sndData, eventOptions, sendOptions);
		}
	}

	Vector3 SplitColorToRGB(Color color)
	{
		float r=0, g=0, b=0;

		if (color == Color.white)
		{
			r = 1f;
			g = 1f;
			b = 1f;
		}
		else if (color == Color.yellow)
		{
			r = 1f;
			g = 0.92f;
			b = 0.016f;
		}
		else if (color == Color.red)
		{
			r = 1f;
			g = 0f;
			b = 0f;
		}
		else if (color == IFG.Purple)
		{
			r = 0.6039f;
			g = 0.1607f;
			b = 0.7411f;
		}
		else if (color == Color.blue)
		{
			r = 0f;
			g = 0f;
			b = 1f;
		}
		else if (color == Color.black)
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}

		Vector3 splitColor = new Vector3(r, g, b);
		return splitColor;
	}

	#endregion
}
