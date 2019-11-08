using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPun
{
	#region Public / Serialized Fields

	[Header("Money Stuff")]
	public int _pCash;
	public int _pNotes;
	public int _pNetworth;

	[Header("Commodities")]
	public int _pHay;
	public int _pFruit;
	public int _pGrain;
	public int _pFarmCows;
	public int _pRangeCows;
	public int _pSpuds;
	public bool _pHayDoubled;
	public int _pHayDoubledCounter;
	public bool _pCherriesCutInHalf;
	public bool _pWheatCutInHalf;
	public bool _pCowsIncreased;
	public bool _pSpudsDoubled;
	public bool _pCornDoubled;

	[Header("Harvest Trackers")]
	public bool _firstHay;
	public bool _cherries;
	public bool _secondHay;
	public bool _wheat;
	public bool _thirdHay;
	public bool _livestock;
	public bool _fourthHay;
	public bool _spuds;
	public bool _apples;
	public bool _corn;

	[Header("Equipment")]
	public bool _pTractor;
	public bool _pHarvester;

	[Header("Ranges")]
	bool _oxfordOwned;
	bool _targheeOwned;
	bool _lostRiverOwned;
	bool _lemhiOwned;

	[Header("Misc Stuff")]
	public bool _pWagesGarnished;
	public bool _pNoWages;
	public bool _isMyTurn;

	public List<OTBCard> _myOtbs = new List<OTBCard>();
	public int _myOtbCount;

	#endregion

	#region Private Fields / References


	UIManager _uiManager;
	PlayerMove _pMove;
	MyDiceRoll _diceRoll;
	RemotePlayerUpdater _rpUpdater;

	Button _customHireOkButton;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Awake()
	{
	}

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnReceiveInitialOtbCards;
		PhotonNetwork.NetworkingClient.EventReceived += OnOtbCardReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnFfCardReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnCustomHireHarvester;
		PhotonNetwork.NetworkingClient.EventReceived += OnChangingActivePlayer;
		PhotonNetwork.NetworkingClient.EventReceived += OnTetonDamEventReceived;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnReceiveInitialOtbCards;
		PhotonNetwork.NetworkingClient.EventReceived -= OnOtbCardReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnFfCardReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnCustomHireHarvester;
		PhotonNetwork.NetworkingClient.EventReceived -= OnChangingActivePlayer;
		PhotonNetwork.NetworkingClient.EventReceived -= OnTetonDamEventReceived;
	}

	void Start()
	{
		_uiManager = GameManager.Instance.uiManager;
		_pMove = GetComponent<PlayerMove>();
		_rpUpdater = GetComponent<RemotePlayerUpdater>();

		UpdateMyCash(5000);
		UpdateMyNotes(5000);
		//UpdateMyOtbCount(0);
		UpdateMyHay(10);
		UpdateMyGrain(10);
		UpdateMyFruit(0);
		UpdateMyFCows(0);
		UpdateMyRCows(0);
		UpdateMySpuds(0);
		UpdateMyTractor(false);
		UpdateMyHarvester(false);
		UpdateMyNetworth(CalculateNetworth());
		UpdateOxfordRange(false);
		UpdateTargheeRange(false);
		UpdateLostRiverRange(false);
		UpdateLemhiRange(false);
		_myOtbCount = _myOtbs.Count;
		//UpdateMyOtbCount(_myOtbCount);
		//_rpUpdater.UpdateRemotePlayerData();

		UpdateMyUI();
		//StartCoroutine(UpdateOtbProperty(_myOtbs.Count));
	}
	#endregion

	#region Custom Properties

	public void UpdateMyCash(int amount)
	{
		_pCash += amount;
		ExitGames.Client.Photon.Hashtable cashProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Cash, _pCash } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(cashProp);
		UpdateMyNetworth(CalculateNetworth());
		if (photonView.IsMine)
		{
			if (amount < 0)
			{
				StartCoroutine(GetForcedLoanRoutine());
			}
		}
	}

	public void UpdateMyNotes(int amount)
	{
		_pNotes += amount;
		ExitGames.Client.Photon.Hashtable notesProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Notes, _pNotes } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(notesProp);
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyOtbCount(int amount)
	{
		Debug.Log("In UpdateOTBCount: " + amount);
		ExitGames.Client.Photon.Hashtable otbProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Otb_Count, _myOtbCount = amount } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(otbProp);
		if (photonView.IsMine)
		{
			_uiManager.UpdateUI();
		}
	}

	public void UpdateMyHay(int amount)
	{
		_pHay += amount;
		ExitGames.Client.Photon.Hashtable hayProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Hay, _pHay } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(hayProp);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyGrain(int amount)
	{
		_pGrain += amount;
		ExitGames.Client.Photon.Hashtable grainProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Hay, _pGrain } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(grainProp);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyFruit(int amount)
	{
		_pFruit += amount;
		ExitGames.Client.Photon.Hashtable fruitProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Fruit, _pFruit } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(fruitProp);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyFCows(int amount)
	{
		_pFarmCows += amount;
		ExitGames.Client.Photon.Hashtable fCowsProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_FCows, _pFarmCows } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(fCowsProp);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyRCows(int amount)
	{
		_pRangeCows += amount;
		ExitGames.Client.Photon.Hashtable rCowsProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_RCows, _pRangeCows } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(rCowsProp);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMySpuds(int amount)
	{
		_pSpuds += amount;
		ExitGames.Client.Photon.Hashtable spudsProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Spuds, _pSpuds } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(spudsProp);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyTractor(bool status)
	{
		_pTractor = status;
		ExitGames.Client.Photon.Hashtable tractorProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Tractor, _pTractor } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(tractorProp);
		_uiManager._tractorImage.color = status ? Color.white : new Color(0.5943396f, 0.5943396f, 0.5943396f);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateMyHarvester(bool status)
	{
		_pHarvester = status;
		ExitGames.Client.Photon.Hashtable harvesterProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Harvester, _pHarvester } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(harvesterProp);
		_uiManager._harvesterImage.color = status ? Color.white : new Color(0.5943396f, 0.5943396f, 0.5943396f);
		if (!IFG.CompleteFarmerBonusGiven)
			CheckForCompleteFarmerBonus();
		UpdateMyNetworth(CalculateNetworth());
	}

	public void UpdateOxfordRange(bool status)
	{
		_oxfordOwned = status;
		ExitGames.Client.Photon.Hashtable oxfordProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Oxford_Range_Owned, _oxfordOwned } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(oxfordProp);
	}

	public void UpdateTargheeRange(bool status)
	{
		_targheeOwned = status;
		ExitGames.Client.Photon.Hashtable targheeProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Targhee_Range_Owned, _targheeOwned } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(targheeProp);
	}

	public void UpdateLostRiverRange(bool status)
	{
		_lostRiverOwned = status;
		ExitGames.Client.Photon.Hashtable lostRiverProp = new ExitGames.Client.Photon.Hashtable() { { IFG.LostRiver_Range_Owned, _lostRiverOwned } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(lostRiverProp);
	}

	public void UpdateLemhiRange(bool status)
	{
		_lemhiOwned = status;
		ExitGames.Client.Photon.Hashtable lemhiProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Lemhi_Range_Owned, _lemhiOwned } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(lemhiProp);
	}

	public void UpdateMyNetworth(int amount)
	{
		_pNetworth = amount;
		ExitGames.Client.Photon.Hashtable networthProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Networth, _pNetworth } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(networthProp);
		if (GameManager.Instance._gameMode == "Networth Game")
		{
			if (_pNetworth >= GameManager.Instance._networthGameAmount)
			{
				//game over!
				//data
				object[] sndData = new object[] { PhotonNetwork.LocalPlayer.NickName, GameManager.Instance.myFarmerName, _pNetworth };
				//event options
				RaiseEventOptions eventOptions = new RaiseEventOptions()
				{
					Receivers = ReceiverGroup.All,
					CachingOption = EventCaching.DoNotCache
				};
				//send options
				SendOptions sendOptions = new SendOptions() { Reliability = true };
				//fire it
				PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.End_Networth_Game_Event_Code, sndData, eventOptions, sendOptions);
			}
		}
		//if (photonView.IsMine)
		_uiManager.UpdateUI();
	}

	//TESTING

	IEnumerator UpdateOtbProperty(int amount)
	{
		Debug.Log("In UpdateOTBCount: " + amount);
		ExitGames.Client.Photon.Hashtable otbProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Otb_Count, amount } };
		PhotonNetwork.LocalPlayer.SetCustomProperties(otbProp);
		if (photonView.IsMine)
			_uiManager.UpdateUI();
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(UpdateOtbProperty(amount));
	}
	#endregion

	#region Public Methods

	public void EndTurn()
	{
		_uiManager._endTurnButton.interactable = false;
		GameManager.Instance.IncrementTheActivePlayer();
	}

	public void DrawOTBCard()
	{
		//fire the DrawOtbCard Event...
		//event data
		object[] data = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.AddToRoomCache
		};
		//send options
		SendOptions sendOptions = new SendOptions
		{
			Reliability = true
		};
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Draw_Otb_Event_Code, data, eventOptions, sendOptions);
	}

	public void DrawFFCard()
	{
		//fire the DrawFFCard event...
		//event data
		object[] data = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Draw_Ff_Event_Code, data, eventOptions, sendOptions);
	}

	//called by DeckManager
	public void TetonDam()
	{
		UpdateMyCash(500 * _pHay);
		//FIRE THE TETON_DAM_EVENT...
		object[] sndData = new object[] { };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Teton_Dam_Event_Code, sndData, eventOptions, sendOptions);
	}

	void OnTetonDamEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Teton_Dam_Event_Code)
		{
			if (_uiManager._ffPanel.activeSelf)
				_uiManager._ffPanel.SetActive(false);

			_uiManager._tetonDamPanel.SetActive(true);
			_uiManager._completeModalPanel.SetActive(true);
			_uiManager._tetonRollButton.onClick.AddListener(OnTetonDamRollButtonClicked);
			_uiManager._tetonOkButton.onClick.AddListener(OnTetonOkButtonClicked);
			_uiManager._tetonHeaderText.text = IFG.TetonDamHeaderText;
			_uiManager._tetonMessageText.text = IFG.TetonDamMessageText;
			

		}
	}

	public void OnTetonDamRollButtonClicked()
	{
		if (_diceRoll == null)
			_diceRoll = GameManager.Instance.myDiceRoll;

		_diceRoll.isOtherRoll = true;

		//Debug.Log("TETON DAM BUTTON CLICKED");
		StartCoroutine(AsFateWillHaveIt());
	}

	IEnumerator AsFateWillHaveIt()
	{
		int penalty = 0;

		_uiManager._tetonHeaderText.enabled = false;
		_uiManager._tetonMessageText.enabled = false;
		//_uiManager._tetonDamImage.enabled = false;
		_uiManager._tetonRollButton.enabled = false;

		_diceRoll.OnRollButton();

		yield return new WaitUntil(() => _diceRoll.Pip >= 1);

		int roll = _diceRoll.Pip;

		if (roll < 0)
			roll = Random.Range(1, 7);

		//Debug.Log("DICE ROLL: " + roll);

		roll = 4;   //TESTING
		yield return new WaitForSeconds(1.5f);

		if (roll % 2 == 0)
		{
			penalty = -(100 * (_pFruit + _pGrain + _pHay + _pSpuds));
			//even Hit
			//_uiManager._tetonDamImage.enabled = true;
			_uiManager._tetonHeaderText.text = "You Were Hit!! " + roll;
			_uiManager._tetonHeaderText.enabled = true;
			//Debug.Log("HIT: " + -(100 * (_pFruit + _pGrain + _pHay + _pSpuds)));
			//play bad sound
			AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
		}
		else
		{
			//odd escaped
			//_uiManager._tetonDamImage.enabled = true;
			_uiManager._tetonHeaderText.text = "You Escaped!! " + roll;
			_uiManager._tetonHeaderText.enabled = true;
			//play good sound
			AudioManager.Instance.PlaySound(AudioManager.Instance._good);
		}

		while (_uiManager._tetonDamPanel.activeSelf)
			yield return null;

		Debug.Log("PENALTY B4 IF: " + penalty);

		if (penalty < 0)
		{
			UpdateMyCash(penalty);
			Debug.Log("PENALTY: " + penalty);
		}

		_diceRoll.isOtherRoll = false;
	}

	public void OnTetonOkButtonClicked()
	{
		_uiManager._tetonDamPanel.SetActive(false);
		_uiManager._completeModalPanel.SetActive(false);
		_uiManager._tetonHeaderText.text = IFG.TetonDamHeaderText;
		_uiManager._tetonHeaderText.enabled = true;
		_uiManager._tetonMessageText.text = IFG.TetonDamMessageText;
		_uiManager._tetonMessageText.enabled = true;
		_uiManager._tetonOkButton.enabled = false;
		_uiManager._tetonRollButton.enabled = true;
	}

	public void CustomHireHarvester()   //called from DeckManager
	{
		int hitPlayers = 0;

		foreach (Player player in PhotonNetwork.PlayerList)
		{
			object harvester;
			if (player.CustomProperties.TryGetValue(IFG.Player_Harvester, out harvester))
			{
				if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
					continue;

				if (!(bool)harvester)
				{
					hitPlayers++;
					//fire a custom hire event to this player
					//event data
					object[] data = new object[] { };
					//event options
					RaiseEventOptions eventOptions = new RaiseEventOptions
					{
						TargetActors = new int[] { player.ActorNumber },
						CachingOption = EventCaching.DoNotCache
					};
					//send options
					SendOptions sendOptions = new SendOptions() { Reliability = true };
					//fire the event...
					PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Custom_Hire_Harvester_Code, data, eventOptions, sendOptions);
				}
			}
		}
		//TODO: message for how many hit players
		//get my money...
		UpdateMyCash(2000 * hitPlayers);
	}

	public void UncleCheester()
	{
		_uiManager._uncleCheesterPanel.SetActive(true);
	}

	public void DiscardOtbCard(OTBCard discard)
	{
		//check to see if it's still in your hand...
		for (int i = 0; i < _myOtbs.Count; i++)
		{
			if (_myOtbs[i].cardNumber == discard.cardNumber)
			{
				//Debug.Log("Card Found");
				_myOtbs.Remove(_myOtbs[i]);
				_myOtbCount = _myOtbs.Count;
				//UpdateMyOtbCount(_myOtbCount);
				//_rpUpdater.UpdateRemotePlayerData();
				UpdateMyUI();
			}
		}

		//return the card to the deck event...
		//event data: //cardNum, description, summary, totalCost
		object[] sendData = new object[] { discard.cardNumber, discard.description, discard.summary, discard.totalCost };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Replace_Otb_Event_Code, sendData, eventOptions, sendOptions);
	}
	#endregion

	#region Private Methods

	void UpdateMyUI()
	{
		if (photonView.IsMine)
			_uiManager.StartCoroutine(_uiManager.UpdateUIRoutine());
	}

	[PunRPC]
	void ShowFfCard(string cardToShow, PhotonMessageInfo info)
	{
		if (info.Sender.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
		{
			object sendingFarmer;

			if (info.Sender.CustomProperties.TryGetValue(IFG.Selected_Farmer, out sendingFarmer))
			{
				string farmer = (string)sendingFarmer;
				_uiManager._ffCardBackground.color = _uiManager.SelectFontColorForFarmer(farmer);
			}
		}
		else
			_uiManager._ffCardBackground.color = Color.white;

		_uiManager._ffCardText.text = cardToShow;
		_uiManager._ffPanel.SetActive(true);
	}

	void OnChangingActivePlayer(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Change_Active_Player_Event_Code)
		{
			//retrieve data...
			object[] recData = (object[])eventData.CustomData;
			GameManager.Instance._activePlayer = (int)recData[0];

			//Debug.Log("IN ONCHANGEPLAYER " + "AP: " + GameManager.Instance._activePlayer + " LP: " + PhotonNetwork.LocalPlayer.ActorNumber);

			if (PhotonNetwork.LocalPlayer.ActorNumber == GameManager.Instance._activePlayer)
			{
				_uiManager._rollButton.interactable = true;
				_isMyTurn = true;
				AudioManager.Instance.PlaySound(AudioManager.Instance._yourTurn);
			}
			else
			{
				AudioManager.Instance._aSource.Stop();
				_uiManager._rollButton.interactable = false;
				_isMyTurn = false;
			}
			//Debug.Log("INSIDE IF!");
		}
	}

	void OnReceiveInitialOtbCards(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Get_Initial_Otb_Event_Code)
		{
			//recover the card's field values...
			object[] recData = (object[])eventData.CustomData;
			int cardNum = (int)recData[0];
			string desc = (string)recData[1];
			string summary = (string)recData[2];
			int cost = (int)recData[3];

			OTBCard drawnCard = new OTBCard();
			drawnCard.cardNumber = cardNum;
			drawnCard.description = desc;
			drawnCard.summary = summary;
			drawnCard.totalCost = cost;

			//add the card to myOtbs
			_myOtbs.Add(drawnCard);
			//update the number of my OTB's to everyone...
			_myOtbCount = _myOtbs.Count;
			//UpdateMyOtbCount(_myOtbCount);
			//_rpUpdater.UpdateRemotePlayerData();
			UpdateMyUI();
		}
	}

	void OnOtbCardReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Receive_Otb_Event_code)
		{
			//recover the card's field values...
			object[] recData = (object[])eventData.CustomData;
			int cardNum = (int)recData[0];
			string desc = (string)recData[1];
			string summary = (string)recData[2];
			int cost = (int)recData[3];

			OTBCard drawnCard = new OTBCard();
			drawnCard.cardNumber = cardNum;
			drawnCard.description = desc;
			drawnCard.summary = summary;
			drawnCard.totalCost = cost;

			//TESTING
			//Debug.Log("OTB card count:" + _myOtbs.Count);
			//Debug.Log("Card Number: " + drawnCard.cardNumber);
			//Debug.Log("Description: " + drawnCard.description);
			//Debug.Log("Summary: " + drawnCard.summary);
			//Debug.Log("Total Cost: " + drawnCard.totalCost);

			StartCoroutine(ShowOtbCardRoutine(drawnCard));
		}
	}

	IEnumerator ShowOtbCardRoutine(OTBCard card)
	{
		_uiManager._otbCardText.text = card.description;
		_uiManager._otbTotalCostText.text = "Total Cost: " + card.totalCost;
		_uiManager._otbPanel.SetActive(true);

		yield return new WaitWhile(() => _uiManager._otbPanel.activeSelf);

		//add the card to myOtbs
		_myOtbs.Add(card);
		//update the number of my OTB's to everyone...
		_myOtbCount = _myOtbs.Count;
		//UpdateMyOtbCount(_myOtbCount);
		//_rpUpdater.UpdateRemotePlayerData();
		UpdateMyUI();
	}

	void OnFfCardReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Receive_Ff_Event_Code)
		{
			//recover the card's field values...
			object[] recData = (object[])eventData.CustomData;
			int cardNum = (int)recData[0];
			string desc = (string)recData[1];

			FFCard drawnCard = new FFCard();
			drawnCard.cardNumber = cardNum;
			drawnCard.description = desc;
			//start the ShowFfCardRoutine
			StartCoroutine(ShowFfCardRoutine(drawnCard));

			//Debug.Log("End OnFfCardReceived()");
		}
	}

	IEnumerator ShowFfCardRoutine(FFCard cardToShow)
	{
		photonView.RPC("ShowFfCard", RpcTarget.All, new object[] { cardToShow.description });

		while (_uiManager._ffPanel.activeSelf)
			yield return null;

		//perform action
		//Debug.Log("FF Action...");
		if (photonView.IsMine)
			DeckManager.Instance.PerformFfActions(cardToShow.cardNumber);

		//return the card to the deck event...
		//event data: //cardNum, description
		object[] sendData = new object[] { cardToShow.cardNumber, cardToShow.description };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Replace_Ff_Event_Code, sendData, eventOptions, sendOptions);
	}

	void OnCustomHireHarvester(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Custom_Hire_Harvester_Code)
		{
			_customHireOkButton = _uiManager._customHarvesterOkButton;
			_customHireOkButton.onClick.AddListener(OnCustomHarvesterOkButtonClicked);
			if (_uiManager._ffPanel.activeSelf)
				_uiManager._ffPanel.SetActive(false);

			_uiManager._customHarvesterPanel.SetActive(true);
		}
	}

	public void OnCustomHarvesterOkButtonClicked()
	{
		UpdateMyCash(-2000);
		//play bad sound
		AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
		//_customHireOkButton.onClick.RemoveAllListeners();
	}

	int CalculateNetworth()
	{
		int bottomLine = _pCash - _pNotes;
		bottomLine += _pHay * 2000;
		bottomLine += _pGrain * 2000;
		bottomLine += _pFruit * 5000;
		bottomLine += _pSpuds * 2000;
		bottomLine += (_pFarmCows + _pRangeCows) * 500;
		if (_pHarvester)
			bottomLine += 10000;
		if (_pTractor)
			bottomLine += 10000;

		return bottomLine;
	}

	IEnumerator GetForcedLoanRoutine()
	{
		_uiManager._modalPanel.SetActive(true);
		_uiManager._forcedLoanPanel.SetActive(true);
		_uiManager._forcedLoanInput.Select();
		_uiManager.UpdateForcedLoanFunds(_pCash, _pNotes);
		yield return new WaitUntil(() => _pCash >= 0);
		_uiManager._modalPanel.SetActive(false);
		_uiManager._forcedLoanPanel.SetActive(false);
		_uiManager._loanAmount = 0;
		_uiManager._forcedLoanInput.text = "";
		_uiManager._forcedLoanInput.placeholder.GetComponent<Text>().text = "Enter the Loan Amount...";
	}

	void CheckForCompleteFarmerBonus()
	{
		bool first = true;

		if (_pHay == 0)
			first = false;
		if (_pGrain == 0)
			first = false;
		if (_pFruit == 0)
			first = false;
		if (_pSpuds == 0)
			first = false;
		if (_pFarmCows == 0 && _pRangeCows == 0)
			first = false;
		if (!_pHarvester)
			first = false;
		if (!_pTractor)
			first = false;

		if (first)
		{
			UpdateMyCash(5000);
			//send event to MasterClient GameManager - so next message fires only once
			//data - //player nickname, player.farmerName
			object[] sndData = new object[] { PhotonNetwork.LocalPlayer.NickName, GameManager.Instance.myFarmerName };
			//event options
			RaiseEventOptions eventOptions = new RaiseEventOptions()
			{
				Receivers = ReceiverGroup.MasterClient,
				CachingOption = EventCaching.DoNotCache
			};
			//send options
			SendOptions sendOptions = new SendOptions() { Reliability = true };
			PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Complete_Farmer_Bonus_Given_Event_Code, sndData, eventOptions, sendOptions);
		}
	}
	#endregion
}
