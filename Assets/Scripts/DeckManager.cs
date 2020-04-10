using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Security.Cryptography;

public class OTBCard
{
	public int cardNumber;
	public string description;
	public string summary;
	public int totalCost;
	public bool bottomCard;
}

public class FFCard
{
	public int cardNumber;
	public string description;
	public bool bottomCard;
}

public class OECard
{
	public int cardNumber;
	public string description;
	public bool bottomCard;
}

public class DeckManager : MonoBehaviourPun
{
	#region Public / Serialized Fields

	public List<OTBCard> _otbCards;
	public List<FFCard> _ffCards;
	public List<OECard> _oeCards;

	[SerializeField] StickerManager _sManager;

	#endregion

	#region Private Fields / References

	PlayerManager _pManager;
	PlayerMove _pMove;
	UIManager _uiManager;

	int _otbDeckShuffleCounter;
	int _oeDeckShuffleCounter;
	int _ffDeckShuffleCounter;

	#endregion

	#region Properties

	public static DeckManager Instance = null;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnDrawOtbCard;
		PhotonNetwork.NetworkingClient.EventReceived += OnDrawFfCard;
		PhotonNetwork.NetworkingClient.EventReceived += OnDrawOeCard;
		PhotonNetwork.NetworkingClient.EventReceived += OnReplaceOtbCard;
		PhotonNetwork.NetworkingClient.EventReceived += OnReplaceFfCard;
		PhotonNetwork.NetworkingClient.EventReceived += OnReplaceOeCard;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnDrawOtbCard;
		PhotonNetwork.NetworkingClient.EventReceived -= OnDrawFfCard;
		PhotonNetwork.NetworkingClient.EventReceived += OnDrawOeCard;
		PhotonNetwork.NetworkingClient.EventReceived -= OnReplaceOtbCard;
		PhotonNetwork.NetworkingClient.EventReceived -= OnReplaceFfCard;
		PhotonNetwork.NetworkingClient.EventReceived -= OnReplaceOeCard;
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
		_otbDeckShuffleCounter = -1;
		_oeDeckShuffleCounter = -1;
		_ffDeckShuffleCounter = -1;

		InitializeDecks();
		//_farmer = GameManager.Instance.myFarmerName;
		//Debug.Log("IN DM START: GM FARMER: " + GameManager.Instance.myFarmerName);
		//Debug.Log("IN DM START: farmer: " + _farmer);

		//Debug.Log("OTB DECK COUNT IN DM: " + _otbCards.Count);
	}
	#endregion

	#region Public Methods

	public void PerformFfActions(int card)
	{
		Debug.Log("IN DECKMANAGER PA: " + card);

		if (_pManager == null)
			_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();
		if (_pMove == null)
			_pMove = GameManager.Instance.myFarmer.GetComponent<PlayerMove>();
		if (_uiManager == null)
			_uiManager = GameManager.Instance.uiManager;
		//if (_farmer =="")
		//	_farmer = GameManager.Instance.myFarmerName;

		switch (card)
		{
			case 1:  //income tax
				_pManager.UpdateMyCash(-7000);
				break;

			case 2:  //apple maggots
				_pManager.UpdateMyCash(-500 * _pManager._pFruit);
				break;

			case 3:  //pay interest on bank notes
				_pManager.UpdateMyCash(-(int)(_pManager._pNotes * 0.1f));
				break;

			case 4:  //co-op
				_pManager.UpdateMyCash(1000);
				break;

			case 5:  //custom hire a tractor
			case 6:
				if (!_pManager._pTractor)
					_pManager.UpdateMyCash(-3000);
				break;

			case 7:  //cut cherries in half - talk show host
				if (_pManager._pFruit > 0 || GameManager.Instance.myFarmer.GetComponent<PlayerMove>()._currentSpace == 6)
				{
					if (GameManager.Instance.myFarmer.GetComponent<PlayerMove>()._currentSpace <= 25 && !_pManager._cherries)
					{
						_pManager._pCherriesCutInHalf = true;
						_uiManager._cherriesCutInHalfWarning.SetActive(true);
					}
				}
				break;

			case 8:  //Russian sale
				_pManager.UpdateMyCash(2000);
				break;

			case 9:  //cut worms
				if (_pManager._pFruit > 0)
					_pManager.UpdateMyCash(-300 * _pManager._pFruit);
				break;

			case 10: //teton dam
			case 27:
				_pManager.TetonDam();
				break;

			case 11: //oil company pays you
				_pManager.UpdateMyCash(100 * (_pManager._pFruit + _pManager._pGrain + _pManager._pHay + _pManager._pSpuds));
				break;

			case 12: //trucker strike
				if (_pManager._pFruit > 0)
				{
					_pManager.UpdateMyCash(-1000 * _pManager._pFruit);
					//bad sound
				}
				break;

			case 13: //custom hire out your harvester
				if (_pManager._pHarvester)
					_pManager.CustomHireHarvester();
				break;

			case 14: //IRS
				_pManager.UpdateMyGarnishedStatus(true);
				//_uiManager._wagesGarnishedWarning.SetActive(true);
				//TODO: play hideous sound
				AudioManager.Instance._aSource.Stop();
				AudioManager.Instance.PlaySound(AudioManager.Instance._garnished);
				break;

			case 15: //uncle cheester
				_pManager.UncleCheester();
				break;

			case 16: //drought year
			case 17:
				//TODO: play sound
				AudioManager.Instance.PlaySound(AudioManager.Instance._droughtYear);
				_pManager._pNoWages = true;
				_pMove.DirectedForwardMove(52);
				break;

			case 18: //hold some calves
				if (_pManager._pFarmCows > 0 || _pManager._pRangeCows > 0)
				{
					_pManager.UpdateMyCash(2000);
					//good sound
					AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				}
				break;

			case 19: //kill your farm cows
				if (_pManager._pFarmCows > 0)
				{
					_pManager.UpdateMyFCows(-_pManager._pFarmCows);
					_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Cow", _pManager._pFarmCows, _pManager._pCowsIncreased, true);
					//play bad sound
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);

				}
				break;

			case 20: //increase cows 2 yrs
				_pManager._pCowsIncreased = true;
				if (_pManager._pFarmCows > 0)
				{
					_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Cow", _pManager._pFarmCows, _pManager._pCowsIncreased);
				}
				UpdateOwnedRangeStickers();
				_uiManager.UpdateUI();
				break;

			case 21: //grain embargo
				if (!_pManager._pHarvester)
				{
					_pManager.UpdateMyCash(-2500);
					//TODO: bad sound
				}
				break;

			case 22: //windy spring
				if (_pMove._currentSpace < 29)
				{
					_uiManager._wheatCutInHalfWarning.SetActive(true);
					_pManager._pWheatCutInHalf = true;
				}
				break;

			case 23: //crop disaster
				_pManager.UpdateMyCash(100 * _pManager._pGrain);
				break;

			case 24: //rich folk
				_pManager.UpdateMyCash(100 * _pManager._pHay);
				break;

			case 25: //buggy spuds
				if (_pManager._pSpuds > 0)
				{
					_pManager.UpdateMyCash(-500 * _pManager._pSpuds);
					//play bas sound
				}
				break;

			case 26: //spuds in high demand for 2 years
				_pManager._pSpudsDoubled = true;
				if (_pManager._pSpuds > 0)
				{
					_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Spuds", _pManager._pSpuds, _pManager._pSpudsDoubled);
				}
				_uiManager.UpdateUI();
				break;

			default:
				Debug.LogWarning("OOPS, card not found " + card);
				break;
		}
	}

	public void UseOtbCard(int card)
	{
		if (_pManager == null)
			_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();

		switch (card)
		{
			case 1:  //grain
			case 2:
			case 3:
			case 4:
			case 5:
				_pManager.UpdateMyGrain(10);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Grain", _pManager._pGrain, _pManager._pCornDoubled);
				break;

			case 6:  //livestock
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				_pManager.UpdateMyFCows(10);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Cow", _pManager._pFarmCows, _pManager._pCowsIncreased);
				break;

			case 12: //hay
			case 13:
			case 14:
			case 15:
			case 16:
				_pManager.UpdateMyHay(10);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Hay", _pManager._pHay, _pManager._pHayDoubled);
				break;

			case 17: //tractor
			case 18:
			case 19:
				_pManager.UpdateMyTractor(true);
				_sManager.PlaceEquipmentSticker(GameManager.Instance.myFarmerName, "Tractor", _pManager._pTractor);
				break;

			case 20: //harvester
			case 21:
			case 22:
				_pManager.UpdateMyHarvester(true);
				_sManager.PlaceEquipmentSticker(GameManager.Instance.myFarmerName, "Harvester", _pManager._pHarvester);
				break;

			case 23: //fruit
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
				_pManager.UpdateMyFruit(5);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Fruit", _pManager._pFruit, false);
				break;

			case 29: //lemhi range
			case 30:
			case 31:
				_pManager.UpdateLemhiRange(true);
				_pManager.UpdateMyRCows(50);
				_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Lemhi", _pManager._pCowsIncreased);
				break;

			case 32: //lost river range
			case 33:
			case 34:
				_pManager.UpdateLostRiverRange(true);
				_pManager.UpdateMyRCows(40);
				_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Lost River", _pManager._pCowsIncreased);
				break;

			case 35: //targhee range
			case 36:
			case 37:
				_pManager.UpdateTargheeRange(true);
				_pManager.UpdateMyRCows(30);
				_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Targhee", _pManager._pCowsIncreased);
				break;

			case 38: //oxford range
			case 39:
			case 40:
				_pManager.UpdateOxfordRange(true);
				_pManager.UpdateMyRCows(20);
				_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Oxford", _pManager._pCowsIncreased);
				break;

			case 41: //spuds
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
				_pManager.UpdateMySpuds(10);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Spuds", _pManager._pSpuds, _pManager._pSpudsDoubled);

				if (!IFG.SpudBonusGiven)
				{
					//get the bonus
					_pManager.UpdateMyCash(1000);

					//send event to MasterClient's GameManager for Spud Bonus (only once)
					//event data
					object[] sndData = new object[] { PhotonNetwork.LocalPlayer.NickName, GameManager.Instance.myFarmerName };
					//event options
					RaiseEventOptions eventOptions = new RaiseEventOptions
					{
						Receivers = ReceiverGroup.MasterClient,
						CachingOption = EventCaching.DoNotCache
					};
					//send data
					SendOptions sendOptions = new SendOptions() { Reliability = true };
					//fire the event...
					PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Spud_Bonus_Given_Event_Code, sndData, eventOptions, sendOptions);
				}

				break;
		}
	}
	#endregion

	#region Private Methods

	void InitializeDecks()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		//Debug.Log("IN DM InitializeDecks");

		_otbCards = new List<OTBCard>();
		_ffCards = new List<FFCard>();
		_oeCards = new List<OECard>();

		MakeTheOtbDeck();
		MakeTheFfDeck();
		MakeTheOeDeck();

		//Debug.Log("IN DM:InitializeDecks OTB's " + _otbCards.Count);
		//Debug.Log("IN DM:InitializeDecks FF's " + _ffCards.Count);
		//Debug.Log("IN DM:InitializeDecks OE's " + _oeCards.Count);
	}

	void MakeTheOtbDeck()   //46 cards
	{
		CreateAnOTBCard(
	01,
		"<color=yellow>NEIGHBOR SELLS OUT</color>\n10 acres of Grain at $2000 per acre",
		"10 acres GRAIN - Total $20,000", 20000);
		CreateAnOTBCard(
			02,
		"<color=yellow>NEIGHBOR SELLS OUT</color>\n10 acres of Grain at $2000 per acre",
		"10 acres GRAIN - Total $20,000", 20000);
		CreateAnOTBCard(
			03,
		"<color=yellow>NEIGHBOR SELLS OUT</color>\n10 acres of Grain at $2000 per acre",
		"10 acres GRAIN - Total $20,000", 20000);
		CreateAnOTBCard(
			04,
		"<color=yellow>NEIGHBOR SELLS OUT</color>\n10 acres of Grain at $2000 per acre",
		"10 acres GRAIN - Total $20,000", 20000);
		CreateAnOTBCard(
			05,
		"<color=yellow>NEIGHBOR SELLS OUT</color>\n10 acres of Grain at $2000 per acre",
		"10 acres GRAIN - Total $20,000", 20000);
		CreateAnOTBCard(
			06,
			"<color=brown>LIVESTOCK AUCTION</color>\n10 pregnant cows at $500 each",
			"10 Cows - Total $5,000", 5000);
		CreateAnOTBCard(
			07,
			"<color=brown>LIVESTOCK AUCTION</color>\n10 pregnant cows at $500 each",
			"10 Cows - Total $5,000", 5000);
		CreateAnOTBCard(
			08,
			"<color=brown>LIVESTOCK AUCTION</color>\n10 pregnant cows at $500 each",
			"10 Cows - Total $5,000", 5000);
		CreateAnOTBCard(
			09,
			"<color=brown>LIVESTOCK AUCTION</color>\n10 pregnant cows at $500 each",
			"10 Cows - Total $5,000", 5000);
		CreateAnOTBCard(
			10,
			"<color=brown>LIVESTOCK AUCTION</color>\n10 pregnant cows at $500 each",
			"10 Cows - Total $5,000", 5000);
		CreateAnOTBCard(
			11,
			"<color=brown>LIVESTOCK AUCTION</color>\n10 pregnant cows at $500 each",
			"10 Cows - Total $5,000", 5000);
		CreateAnOTBCard(
			12,
			"<color=green>NEIGHBOR SELLS OUT</color>\n10 acres of Hay at $2000 per acre",
			"10 acres HAY - Total $20,000", 20000);
		CreateAnOTBCard(
			13,
			"<color=green>NEIGHBOR SELLS OUT</color>\n10 acres of Hay at $2000 per acre",
			"10 acres HAY - Total $20,000", 20000);
		CreateAnOTBCard(
			14,
			"<color=green>NEIGHBOR SELLS OUT</color>\n10 acres of Hay at $2000 per acre",
			"10 acres HAY - Total $20,000", 20000);
		CreateAnOTBCard(
			15,
			"<color=green>NEIGHBOR SELLS OUT</color>\n10 acres of Hay at $2000 per acre",
			"10 acres HAY - Total $20,000", 20000);
		CreateAnOTBCard(
			16,
			"<color=green>NEIGHBOR SELLS OUT</color>\n10 acres of Hay at $2000 per acre",
			"10 acres HAY - Total $20,000", 20000);
		CreateAnOTBCard(
			17,
			"<color=orange>EQUIPMENT SALE</color>\nold but usable TRACTOR",
			"TRACTOR - Total $10,000", 10000);
		CreateAnOTBCard(
			18,
			"<color=orange>EQUIPMENT SALE</color>\nold but usable TRACTOR",
			"TRACTOR - Total $10,000", 10000);
		CreateAnOTBCard(
			19,
			"<color=orange>EQUIPMENT SALE</color>\nold but usable TRACTOR",
			"TRACTOR - Total $10,000", 10000);
		CreateAnOTBCard(
			20,
			"<color=orange>EQUIPMENT SALE</color>\nold but usable HARVESTER",
			"HARVESTER - Total $10,000", 10000);
		CreateAnOTBCard(
			21,
			"<color=orange>EQUIPMENT SALE</color>\nold but usable HARVESTER",
			"HARVESTER - Total $10,000", 10000);
		CreateAnOTBCard(
			22,
			"<color=orange>EQUIPMENT SALE</color>\nold but usable HARVESTER",
			"HARVESTER - Total $10,000", 10000);
		CreateAnOTBCard(
			23,
			"<color=red>NEIGHBOR GOES BROKE</color>\n5 acres of Fruit at $5000 per acre",
			"5 acres FRUIT - Total $25,000", 25000);
		CreateAnOTBCard(
			24,
			"<color=red>NEIGHBOR GOES BROKE</color>\n5 acres of Fruit at $5000 per acre",
			"5 acres FRUIT - Total $25,000", 25000);
		CreateAnOTBCard(
			25,
			"<color=red>NEIGHBOR GOES BROKE</color>\n5 acres of Fruit at $5000 per acre",
			"5 acres FRUIT - Total $25,000", 25000);
		CreateAnOTBCard(
			26,
			"<color=red>NEIGHBOR GOES BROKE</color>\n5 acres of Fruit at $5000 per acre",
			"5 acres FRUIT - Total $25,000", 25000);
		CreateAnOTBCard(
			27,
			"<color=red>NEIGHBOR GOES BROKE</color>\n5 acres of Fruit at $5000 per acre",
			"5 acres FRUIT - Total $25,000", 25000);
		CreateAnOTBCard(
			28,
			"<color=red>NEIGHBOR GOES BROKE</color>\n5 acres of Fruit at $5000 per acre",
			"5 acres FRUIT - Total $25,000", 25000);
		CreateAnOTBCard(
			29,
			"<color=brown>LEASE LEMHI RANGE</color>\nfor lifetime at $25,000\nand buy 50 pregnant cows to stock it at $500 each",
			"LEASE Lemhi Range - Total $50,000", 50000);
		CreateAnOTBCard(
			30,
			"<color=brown>LEASE LEMHI RANGE</color>\nfor lifetime at $25,000\nand buy 50 pregnant cows to stock it at $500 each",
			"LEASE Lemhi Range - Total $50,000", 50000);
		CreateAnOTBCard(
			31,
			"<color=brown>LEASE LEMHI RANGE</color>\nfor lifetime at $25,000\nand buy 50 pregnant cows to stock it at $500 each",
			"LEASE Lemhi Range - Total $50,000", 50000);
		CreateAnOTBCard(
			32,
			"<color=brown>LEASE LOST RIVER RANGE</color>\nfor lifetime at $20,000\nand buy 40 pregnant cows to stock it at $500 each",
			"LEASE Lost River Range - Total $40,000", 40000);
		CreateAnOTBCard(
			33,
			"<color=brown>LEASE LOST RIVER RANGE</color>\nfor lifetime at $20,000\nand buy 40 pregnant cows to stock it at $500 each",
			"LEASE Lost River Range - Total $40,000", 40000);
		CreateAnOTBCard(
			34,
			"<color=brown>LEASE LOST RIVER RANGE</color>\nfor lifetime at $20,000\nand buy 40 pregnant cows to stock it at $500 each",
			"LEASE Lost River Range - Total $40,000", 40000);
		CreateAnOTBCard(
			35,
			"<color=brown>LEASE TARGHEE RANGE</color>\nfor lifetime at $15,000\nand buy 30 pregnant cows to stock it at $500 each",
			"LEASE Targhee Range - Total $30,000", 30000);
		CreateAnOTBCard(
			36,
			"<color=brown>LEASE TARGHEE RANGE</color>\nfor lifetime at $15,000\nand buy 30 pregnant cows to stock it at $500 each",
			"LEASE Targhee Range - Total $30,000", 30000);
		CreateAnOTBCard(
			37,
			"<color=brown>LEASE TARGHEE RANGE</color>\nfor lifetime at $15,000\nand buy 30 pregnant cows to stock it at $500 each",
			"LEASE Targhee Range - Total $30,000", 30000);
		CreateAnOTBCard(
			38,
			"<color=brown>LEASE OXFORD RANGE</color>\nfor lifetime at $10,000\nand buy 20 pregnant cows to stock it at $500 each",
			"LEASE Oxford Range - Total $20,000", 20000);
		CreateAnOTBCard(
			39,
			"<color=brown>LEASE OXFORD RANGE</color>\nfor lifetime at $10,000\nand buy 20 pregnant cows to stock it at $500 each",
			"LEASE Oxford Range - Total $20,000", 20000);
		CreateAnOTBCard(
			40,
			"<color=brown>LEASE OXFORD RANGE</color>\nfor lifetime at $10,000\nand buy 20 pregnant cows to stock it at $500 each",
			"LEASE Oxford Range - Total $20,000", 20000);
		CreateAnOTBCard(
			41,
			"<color=#00A1D6FF>NEIGHBOR SELLS OUT</color>\n10 acres of Spuds at $2000 per acre",
			"10 acres SPUDS - Total $20,000", 20000);
		CreateAnOTBCard(
			42,
			"<color=#00A1D6FF>NEIGHBOR SELLS OUT</color>\n10 acres of Spuds at $2000 per acre",
			"10 acres SPUDS - Total $20,000", 20000);
		CreateAnOTBCard(
			43,
			"<color=#00A1D6FF>NEIGHBOR SELLS OUT</color>\n10 acres of Spuds at $2000 per acre",
			"10 acres SPUDS - Total $20,000", 20000);
		CreateAnOTBCard(
			44,
			"<color=#00A1D6FF>NEIGHBOR SELLS OUT</color>\n10 acres of Spuds at $2000 per acre",
			"10 acres SPUDS - Total $20,000", 20000);
		CreateAnOTBCard(
			45,
			"<color=#00A1D6FF>NEIGHBOR SELLS OUT</color>\n10 acres of Spuds at $2000 per acre",
			"10 acres SPUDS - Total $20,000", 20000);
		CreateAnOTBCard(
			46,
			"<color=#00A1D6FF>NEIGHBOR SELLS OUT</color>\n10 acres of Spuds at $2000 per acre",
			"10 acres SPUDS - Total $20,000", 20000);

		ShuffleOtbDeck(_otbCards);

		//foreach (OTBCard oTBCard in _otbCards)
		//	print(oTBCard.cardNumber + ":" + oTBCard.bottomCard + ":" + oTBCard.totalCost);
	}

	void CreateAnOTBCard(int cardNum, string desc, string sum, int cost)
	{
		//cardNum = 20;  //HARVESTER	TESTING
		//desc = "HARVESTER";
		//sum = "Harvester - 20000";
		//cost = 10000;

		//declare an OTBCard object
		OTBCard newOTBCard = new OTBCard();

		//set the fields
		newOTBCard.cardNumber = cardNum;
		newOTBCard.description = desc;
		newOTBCard.summary = sum;
		newOTBCard.totalCost = cost;

		//add the new card to the list
		_otbCards.Add(newOTBCard);
	}

	void ShuffleOtbDeck<T>(List<T> list)
	{
		_otbDeckShuffleCounter++;

		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
		int n = list.Count;
		while (n > 1)
		{
			byte[] box = new byte[1];
			do provider.GetBytes(box);
			while (!(box[0] < n * (byte.MaxValue / n)));
			int k = (box[0] % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		SetBottomOtbCard(_otbCards);

		//send msg to all to update their shuffle counter text
		//event data - deck,counter
		object[] sndData = new object[] { "OTB", _otbDeckShuffleCounter };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Shuffle_Deck_Event_Code, sndData, eventOptions, sendOptions);
	}

	void SetBottomOtbCard(List<OTBCard> deck)
	{
		for (int i = 0; i < deck.Count; i++)
		{
			if (i == deck.Count - 1)
				deck[i].bottomCard = true;
			else
				deck[i].bottomCard = false;
		}
	}

	void OnDrawOtbCard(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Draw_Otb_Event_Code)
		{
			if (_otbCards.Count > 0)
			{
				//target player
				object[] recData = (object[])eventData.CustomData;
				int targetPlayer = (int)recData[0];

				OTBCard drawnCard = _otbCards[0];
				_otbCards.RemoveAt(0);
				//shuffle the deck if required...
				if (drawnCard.bottomCard)
					ShuffleOtbDeck(_otbCards);

				//Raise the return event to the sender only...
				//event data: cardNumber/description/summary/totalCost
				object[] sndData = new object[] { drawnCard.cardNumber, drawnCard.description, drawnCard.summary, drawnCard.totalCost };
				//event options
				RaiseEventOptions eventOptions = new RaiseEventOptions
				{
					TargetActors = new int[] { targetPlayer },
					CachingOption = EventCaching.AddToRoomCache
				};
				//send option
				SendOptions sendOptions = new SendOptions
				{
					Reliability = true
				};
				//fire the event...
				PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Receive_Otb_Event_code, sndData, eventOptions, sendOptions);

				UpdateOtbDeckInfo();
			}
			else
			{
				SendOutOfOtbsMessage();
			}
		}
	}

	public void UpdateOtbDeckInfo()
	{
		//update all player's Deck Data info
		int otbsLeft = 0;
		foreach (OTBCard card in _otbCards)
		{
			otbsLeft++;
			if (card.bottomCard)
				break;
		}
		//event data: deck, otb Count,otb's left
		object[] deckData = new object[] { "OTB", _otbCards.Count, otbsLeft };
		//event options
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions deckSendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Update_Deck_Data_Event_Code, deckData, raiseEventOptions, deckSendOptions);
	}

	void OnReplaceOtbCard(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Replace_Otb_Event_Code)
		{
			//cardNumber/description/summary/totalCost
			object[] recData = (object[])eventData.CustomData;

			OTBCard disCard = new OTBCard();
			disCard.cardNumber = (int)recData[0];
			disCard.description = (string)recData[1];
			disCard.summary = (string)recData[2];
			disCard.bottomCard = false;

			_otbCards.Add(disCard);
			UpdateOtbDeckInfo();
		}
	}

	void MakeTheFfDeck()   //26 cards
	{
		CreateAnFFCard(1,
			"Income Taxes due.\n<color=red>PAY $7,000</color>.");
		CreateAnFFCard(2,
			"The Apple Maggot Fly, cousin of the dreaded Med Fly, is found in an insect trap in your orchard. Your orchard is quarantined and you lose a lucrative export contract.\nPAY $500 per Fruit acre.");
		CreateAnFFCard(3,
			"Banks raise Prime Rate.\n<color=red>PAY 10% of outstanding loan balance</color> as additional interest.");
		CreateAnFFCard(4,
			"Valley Wide Co-op holds out for higher price. Processor gives in.\n<color=green>COLLECT $1,000</color>.");
		CreateAnFFCard(5,
			"Custom hire bill due.\nIf you have no Tractor - PAY $3,000.");
		CreateAnFFCard(6,
			"Custom hire bill due.\nIf you have no Tractor - PAY $3,000.");
		CreateAnFFCard(7,
			"Some TV talk show host does a show on the dangers of a main control spray you use on your cherries. Even though the pseudo-science behind the inflated claims is nor more rigorous that a supermarket tabloid, the national cherry market crashes.\n<color=red>Cut your cherry crop in half</color> if you haven't already harvested this year.");
		CreateAnFFCard(8,
			"Russian sale boosts wheat prices.\n<color=green>COLLECT $2,000</color>.");
		CreateAnFFCard(9,
			"Cut worms eat sprouting Fruit buds. EPA bans control spray.\nPAY $300 per Fruit acre.");
		CreateAnFFCard(10,
			"HIDEOUS DISASTER - THE TETON DAM BREAKS! You are luckily out the Flood Zone. Your flood-free Hay jumps in price.  COLLECT $500 per Hay acre.\nOther players must roll to see if they escaped. Odd - escaped; Even - hit. Flood hit players PAY $100 per acre (all acres) to clean up the mess.");
		CreateAnFFCard(11,
			"Oil Company pays you $100 per acre for Oil and Gas lease on you farm.");
		CreateAnFFCard(12,
			"Truckers strike delay Fruit in transport, lots of spoilage.\nPAY $1,000 per Fruit acre.");
		CreateAnFFCard(13,
			"Custom hire out with your Harvester.\nIf you have a Harvester - COLLECT $2,000 from each player who has none.");
		CreateAnFFCard(14,
			"<color=red>IRS garnishes your income after finding errors on your tax return. For the rest of the year, draw OPERATING EXPENSE cards during Harvests, but do not roll for Harvest check</color>.");
		CreateAnFFCard(15,
		"Uncle Cheester dies and leaves you 10 acres of Hay, if you can raise the $10,000 cash to pay Inheritance Tax and small remaining mortgage.");
		CreateAnFFCard(16,
			"Drought year! Go to the 2nd week of January.\n<color=red>DO NOT COLLECT</color> your $5,000 year's wages.");
		CreateAnFFCard(17,
			"Drought year! Go to the 2nd week of January.\n<color=red>DO NOT COLLECT</color> your $5,000 year's wages.");
		CreateAnFFCard(18,
			"Held some of your calves and the market jumped.\nCOLLECT $2,000, if you have cows.");
		CreateAnFFCard(19,
			"A leaking electrical motor at Feed Mill contaminates you load of feed with PCB. State Ag Inspector requires you to slaughter cows on your farm (not cows on lease range land) with no reimbursement.");
		CreateAnFFCard(20,
			"Sharp management, production testing, and your computer record system cause your calf weaning weights to soar. RECEIVE a 50% bonus after you roll for your Livestock Harvest check each of the next two years.");
		CreateAnFFCard(21,
			"The President slaps on a Grain Embargo while you're waiting for the custom harvester to show up. Instant market collapse.\nPAY $2,500 if you don't own your own Harvester.");
		CreateAnFFCard(22,
			"Windy spring, didn't get your wheat sprayed. <color=red>Weeds cut your wheat crop in half</color>. Hold this card through Wheat Harvest for this year.");
		CreateAnFFCard(23,
			"Federal Crop Disaster payment saves your bacon.\n<color=green>COLLECT $100 per Grain acre</color>.");
		CreateAnFFCard(24,
			"Rich folks from the city bought the neighboring farm and pay you a premium for your best hay to feed their fancy show horses.\n<color=green>COLLECT $100 per Hay acre</color>.");
		CreateAnFFCard(25,
			"Your Spuds are showing signs of Blight. Should have sprayed earlier.\nPAY $500 per Spud acre.");
		CreateAnFFCard(26,
			"Your Spuds are in great demand!\nDOUBLE your Spud Harvest the next two years.");
		CreateAnFFCard(27,
			"HIDEOUS DISASTER - THE TETON DAM BREAKS! You are luckily out the Flood Zone. Your flood-free Hay jumps in price.  COLLECT $500 per Hay acre.\nOther players must roll to see if they escaped. Odd - escaped; Even - hit. Flood hit players PAY $100 per acre (all acres) to clean up the mess.");

		ShuffleFfDeck(_ffCards);
	}

	void CreateAnFFCard(int cardNum, string desc)
	{
		//cardNum = 14;              //TESTING
		//desc = "GARNISHED!";       //TESTING
		//cardNum = 13;					//TESTING
		//desc = "CUSTOM HARVESTER";	//TESTING
		//cardNum = 10;              //TESTING
		//desc = "TETON DAM!";       //TESTING
											//cardNum = 15;					//TESTING
											//desc = "UNCLE CHEESTER";		//TESTING
											//cardNum = 1;                //TESTING
											//desc = "INCOME TAX!";       //TESTING

		//declare an FFCard object
		FFCard newFFCard = new FFCard();

		//set the fields
		newFFCard.cardNumber = cardNum;
		newFFCard.description = desc;

		//add the new card to the list
		_ffCards.Add(newFFCard);
	}

	void ShuffleFfDeck<T>(List<T> list)
	{
		_ffDeckShuffleCounter++;

		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
		int n = list.Count;
		while (n > 1)
		{
			byte[] box = new byte[1];
			do provider.GetBytes(box);
			while (!(box[0] < n * (byte.MaxValue / n)));
			int k = (box[0] % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		SetBottomFfCard(_ffCards);

		//send msg to all to update their shuffle counter text
		//event data
		object[] sndData = new object[] { "FF", _ffDeckShuffleCounter };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Shuffle_Deck_Event_Code, sndData, eventOptions, sendOptions);
	}

	void SetBottomFfCard(List<FFCard> deck)
	{
		for (int i = 0; i < deck.Count; i++)
		{
			if (i == deck.Count - 1)
				deck[i].bottomCard = true;
			else
				deck[i].bottomCard = false;
		}
	}

	void OnDrawFfCard(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Draw_Ff_Event_Code)
		{
			//target player
			object[] recData = (object[])eventData.CustomData;
			int targetPlayer = (int)recData[0];

			FFCard drawnCard = _ffCards[0];
			_ffCards.RemoveAt(0);

			if (drawnCard.bottomCard)
				ShuffleFfDeck(_ffCards);

			//Raise the return event to the sender...
			//event data: cardNumber/description
			object[] sndData = new object[] { drawnCard.cardNumber, drawnCard.description };
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
			PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Receive_Ff_Event_Code, sndData, eventOptions, sendOptions);

			UpdateFfDeckInfo();
		}
	}

	public void UpdateFfDeckInfo()
	{
		//update all player's Deck Data info
		int ffsLeft = 0;
		foreach (FFCard card in _ffCards)
		{
			ffsLeft++;
			if (card.bottomCard)
				break;
		}
		//event data: deck, ff Count, ff's left
		object[] deckData = new object[] { "FF", _ffCards.Count, ffsLeft };
		//event options
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions deckSendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Update_Deck_Data_Event_Code, deckData, raiseEventOptions, deckSendOptions);
	}

	void OnReplaceFfCard(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Replace_Ff_Event_Code)
		{
			//cardNumber/description
			object[] recData = (object[])eventData.CustomData;

			FFCard disCard = new FFCard();
			disCard.cardNumber = (int)recData[0];
			disCard.description = (string)recData[1];
			disCard.bottomCard = false;

			_ffCards.Add(disCard);

			UpdateFfDeckInfo();
		}
	}

	void MakeTheOeDeck()   //27 cards
	{
		CreateAnOECard(1,
			"Custom hire bill due.\nPay $2,000 if you do not own a Tractor.");
		CreateAnOECard(2,
			"Custom hire bill due.\nPay $2,000 if you do not own a Tractor.");
		CreateAnOECard(3,
			"Custom hire bill due.\nPay $2,000 if you do not own a Harvester.");
		CreateAnOECard(4,
			"Custom hire bill due.\nPay $2,000 if you do not own a Harvester.");
		CreateAnOECard(5,
			"Feed bill due.\nPay $100 per cow.");
		CreateAnOECard(6,
			"Veterinary bill due.\nPay $500 if you own cows.");
		CreateAnOECard(7,
			"Monthly electric bill due for irrigation pumping.\nPay $500.");
		CreateAnOECard(8,
			"Farm real estate taxes due.\nPay $1,500.");
		CreateAnOECard(9,
			"Fertilizer bill due.\nPay $100 per acre.");
		CreateAnOECard(10,
			"Fertilizer bill due.\nPay $100 per acre.");
		CreateAnOECard(11,
			"Equipment in the shop. The delay costs.\nPay $1,000.");
		CreateAnOECard(12,
			"Equipment in the shop. The delay costs.\nPay $1,000.");
		CreateAnOECard(13,
			"Semi-annual interest due.\nPay 10% on Bank Notes on hand.");
		CreateAnOECard(14,
			"Semi-annual interest due.\nPay 10% on Bank Notes on hand.");
		CreateAnOECard(15,
			"Equipment breakdown.\nPay $500.");
		CreateAnOECard(16,
			"Equipment breakdown.\nPay $500.");
		CreateAnOECard(17,
			"FarmOwner's Insurance due.\nPay $1,500.");
		CreateAnOECard(18,
			"Fuel bill due.\nPay $1,000.");
		CreateAnOECard(19,
			"Fuel bill due.\nPay $1,000.");
		CreateAnOECard(20,
			"Parts bill due.\nPay $500.");
		CreateAnOECard(21,
			"Parts bill due.\nPay $500.");
		CreateAnOECard(22,
			"Seed bill due.\nPay $3,000.");
		CreateAnOECard(23,
			"Seed bill due.\nPay $3,000.");
		CreateAnOECard(24,
			"Wire worm infects Grain acreage.\nPay $100 per Grain acre to fumigate.");
		CreateAnOECard(25,
			"Pay Day.\nPay your Fruit Pickers $400 per Fruit acre.");
		CreateAnOECard(26,
			"Pay Day.\nPay your Spud Pickers $200 per Spud acre.");
		CreateAnOECard(27,
			"Schools Cancel Spud Harvest Vacation.\nPay $150 per Spud acre to hire extra spud workers.");

		ShuffleOeDeck(_oeCards);
	}

	void CreateAnOECard(int cardNum, string desc)
	{
		//declare an OECard object
		OECard newOECard = new OECard();

		//set the fields
		newOECard.cardNumber = cardNum;
		newOECard.description = desc;

		//add the new card to the list
		_oeCards.Add(newOECard);
	}

	void ShuffleOeDeck<T>(List<T> list)
	{
		_oeDeckShuffleCounter++;

		RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
		int n = list.Count;
		while (n > 1)
		{
			byte[] box = new byte[1];
			do provider.GetBytes(box);
			while (!(box[0] < n * (byte.MaxValue / n)));
			int k = (box[0] % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		SetBottomOeCard(_oeCards);

		//send msg to all to update their shuffle counter text
		//event data
		object[] sndData = new object[] { "OE", _oeDeckShuffleCounter };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Shuffle_Deck_Event_Code, sndData, eventOptions, sendOptions);
	}

	void SetBottomOeCard(List<OECard> deck)
	{
		for (int i = 0; i < deck.Count; i++)
		{
			if (i == deck.Count - 1)
				deck[i].bottomCard = true;
			else
				deck[i].bottomCard = false;
		}
	}

	void OnDrawOeCard(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Draw_Oe_Event_Code)
		{
			object[] recData = (object[])eventData.CustomData;
			int targetPlayer = (int)recData[0];

			OECard drawnCard = _oeCards[0];
			_oeCards.RemoveAt(0);

			if (drawnCard.bottomCard)
				ShuffleOeDeck(_oeCards);

			//Raise the return event to the sender...
			//event data
			object[] sndData = new object[] { drawnCard.cardNumber, drawnCard.description };
			//send options
			RaiseEventOptions eventOptions = new RaiseEventOptions
			{
				TargetActors = new int[] { targetPlayer },
				CachingOption = EventCaching.AddToRoomCache
			};
			//send option
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true
			};
			//fire the event...
			PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Receive_Oe_Event_Code, sndData, eventOptions, sendOptions);

			UpdateOeDeckInfo();
		}
	}

	public void UpdateOeDeckInfo()
	{
		//update all player's Deck Data info
		int oesLeft = 0;
		foreach (OECard card in _oeCards)
		{
			oesLeft++;
			if (card.bottomCard)
				break;
		}
		//event data: deck, otb Count,otb's left
		object[] deckData = new object[] { "OE", _oeCards.Count, oesLeft };
		//event options
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions deckSendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Update_Deck_Data_Event_Code, deckData, raiseEventOptions, deckSendOptions);
	}

	void OnReplaceOeCard(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Replace_Oe_Event_Code)
		{
			//cardNumber/description
			object[] recData = (object[])eventData.CustomData;

			OECard disCard = new OECard();
			disCard.cardNumber = (int)recData[0];
			disCard.description = (string)recData[1];
			disCard.bottomCard = false;

			_oeCards.Add(disCard);

			UpdateOeDeckInfo();
		}
	}

	public void UpdateOwnedRangeStickers()
	{
		//oxford
		if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[IFG.Oxford_Range_Owned])
		{
			_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Oxford", _pManager._pCowsIncreased);
		}

		//targhee
		if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[IFG.Targhee_Range_Owned])
		{
			_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Targhee", _pManager._pCowsIncreased);
		}

		//lost river
		if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[IFG.LostRiver_Range_Owned])
		{
			_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Lost River", _pManager._pCowsIncreased);
		}

		//lemhi
		if ((bool)PhotonNetwork.LocalPlayer.CustomProperties[IFG.Lemhi_Range_Owned])
		{
			_sManager.PlaceRangeSticker(GameManager.Instance.myFarmerName, "Lemhi", _pManager._pCowsIncreased);
		}
	}

	void SendOutOfOtbsMessage()
	{
		//send out of OTB msg to UI<Managers
		string msg = "There are No OTB Cards left in the Deck!!";
		//event data
		object[] sndData = new object[] { msg };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Out_Of_Otbs_Event_Code, sndData, eventOptions, sendOptions);
	}
	#endregion
}
