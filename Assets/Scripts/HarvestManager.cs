using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using DG.Tweening;

public class HarvestManager : MonoBehaviour
{
	public enum HarvestType
	{
		HAY,
		CHERRIES,
		WHEAT,
		LIVESTOCK,
		SPUDS,
		APPLES,
		CORN
	}

	#region Public / Serialized Fields

	[Header("Harvest 'Fudge-Factors'")]
	[SerializeField] float _hayFudgeFactor = 1.10F;
	[SerializeField] float _fruitFudgeFactor = 1.20F;
	[SerializeField] float _spudFudgeFactor = 1.5F;

	[Header("Misc Harvest Booleans")]
	public bool _cutHarvestInHalf;
	public bool _doubleHarvest;
	public bool _add50PerWheatAcre;
	public bool _cut50PerWheatAcre;
	
	public bool _rollButtonPressed;
	public bool _isOkToCloseOePanel;
	public bool _isOkToCloseGarnishedPanel;

	public int _dieRoll;
	
	#endregion

	#region Private Fields / References

	int _harvestCheck;
	int _operatingExpenses;
	int _netCheck;

	bool _okButton1Pressed;
	bool _okButton2Pressed;
	[SerializeField] bool _waitingForOeCard;

	Text _messageText;
	Button _rollButton;
	Button _ok1Button;
	Button _ok2Button;
	Button _ok3Button;
	string _commodity;

	//Garnished Stuff
	Text _gMessageText;
	Button _ok1GButton, _ok2GButton, _ok3GButton;
	bool _ok1GButtonPressed;

	PlayerManager _pManager;
	UIManager _uiManager;
	MyDiceRoll _myDiceRoll;
	PlayerMove _pMove;
	BoardManager _bManager;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnOeCardReceived;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnOeCardReceived;
	}

	void Start() 
	{
		ResetHarvestModifiers();
	}
	#endregion

	#region Public Methods

	public void OnHarvestRollButtonClicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		_myDiceRoll.isOtherRoll = true;
		_myDiceRoll.isHarvestRoll = true;
		_myDiceRoll.OnRollButton();
		_messageText.text = "Rolling...You rolled a...";
	}

	public void OnOkButton1Clicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		_myDiceRoll.isOtherRoll = false;
		_okButton1Pressed = true;
		_messageText.text = "Performing Harvest Calculations...";
		Invoke("OnOkButton2Clicked", 0.3f);
	}

	public void OnOkButton1GarnishedClicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		_gMessageText.text = "Getting your Operating Expenses...";
		_ok1GButtonPressed = true;
	}

	public void OnOkButton2Clicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		_okButton2Pressed = true;
		_messageText.text = "Your Harvest Check is: " + _harvestCheck;
	}

	public void OnOkButton2GarnishedClicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		_isOkToCloseGarnishedPanel = true;
		//play the hide animation
		_uiManager._gHarvestPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards(); //scale down
		//_ok2GButtonPressed = true;
	}

	public void OnOkButton3GarnishedClicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		//play the hide animation
		_uiManager._gHarvestPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards(); //scale down
		//_ok3GButton.gameObject.SetActive(false);
	}

	public void OnOkButton3Clicked()
	{
		//play click
		AudioManager.Instance.PlaySound(AudioManager.Instance._buttonClick);

		//play hide animation...
		_uiManager._harvestPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();	//scale down
		//_uiManager._harvestPanel.SetActive(false);
		_ok3Button.gameObject.SetActive(false);
		_messageText.text = IFG.HarvestBaseMessage;
	}

	public IEnumerator PerformHarvestRoutine(int space, string commodity, int amount)
	{
		_harvestCheck = 0;
		_commodity = commodity;

		//Debug.Log("In PerformHarvestRoutine: " + space + " " + commodity + " " + amount);

		if (_pManager == null)
			_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();
		if (_uiManager == null)
		{
			_uiManager = GameManager.Instance.uiManager;
			_messageText = _uiManager._harvestMessageText;
			_rollButton = _uiManager._harvestRollButton;
			_rollButton.onClick.AddListener(OnHarvestRollButtonClicked);
			_ok1Button = _uiManager._harvestOk1Button;
			_ok1Button.onClick.AddListener(OnOkButton1Clicked);
			_ok2Button = _uiManager._harvestOk2Button;
			_ok2Button.onClick.AddListener(OnOkButton2Clicked);
			_ok3Button = _uiManager._harvestOk3Button;
			_ok3Button.onClick.AddListener(OnOkButton3Clicked);
			//Garnished Stuff
			_gMessageText = _uiManager._gHarvestMessageText;
			_ok1GButton = _uiManager._ok1GarnishedButton;
			_ok1GButton.onClick.AddListener(OnOkButton1GarnishedClicked);
			_ok2GButton = _uiManager._ok2GarnishedButton;
			_ok2GButton.onClick.AddListener(OnOkButton2GarnishedClicked);
			//_ok3GButton = _uiManager._ok2GarnishedButton;
			//_ok3GButton.onClick.AddListener(OnOkButton3GarnishedClicked);
		}
		if (_myDiceRoll == null)
			_myDiceRoll = GameManager.Instance.myDiceRoll;
		if (_pMove == null)
			_pMove = GameManager.Instance.myFarmer.GetComponent<PlayerMove>();

		//show the Harvest UI
		_uiManager._harvestPanel.SetActive(true);
		_uiManager._forcedLoanModalPanel.SetActive(true);
		//play the show animation
		_uiManager._harvestPanel.GetComponent<DOTweenAnimation>().DOPlayForward();	//scale up

		_uiManager._harvestRollButton.interactable = true;

		//WAIT roll the die
		yield return new WaitUntil(() => _rollButtonPressed);
		//Debug.Log("Roll Button Pressed");
		_myDiceRoll.isHarvestRoll = false;

		yield return new WaitUntil(() => _okButton1Pressed);
		//Debug.Log("OK1 Pressed");
		SendHarvestRollMessage();

		//_dieRoll = 1;	//TESTING

		switch (commodity)
		{
			case "Hay":
				_harvestCheck = GetHayCheck(_dieRoll, amount);
				//Debug.Log("Die: " + _dieRoll);
				//Debug.Log("CHART AMOUNT: "+_harvestCheck);
				//special conditions
				SpecialBoardConditions(commodity);
				SpecialPlayerConditions(commodity);
				break;

			case "Cherries":
				_harvestCheck = GetFruitCheck(_dieRoll, amount);
				SpecialBoardConditions(commodity);
				SpecialPlayerConditions(commodity);
				break;

			case "Wheat":
				_harvestCheck = GetGrainCheck(_dieRoll, amount);
				SpecialBoardConditions(commodity);
				SpecialPlayerConditions(commodity);
				break;

			case "Livestock":
				_harvestCheck = GetCowCheck(_dieRoll, amount);
				SpecialBoardConditions(commodity);
				SpecialPlayerConditions(commodity);
				break;

			case "Spuds":
				_harvestCheck = GetSpudCheck(_dieRoll, amount);
				SpecialPlayerConditions(commodity);
				break;
			case "Apples":
				_harvestCheck = GetFruitCheck(_dieRoll, amount);
				break;

			case "Corn":
				_harvestCheck = GetGrainCheck(_dieRoll, amount);
				SpecialPlayerConditions(commodity);
				break;
		}
		//wait to show the harvest check
		yield return new WaitUntil(() => _okButton2Pressed);

		//draw OE card...
		DrawOECard();

		yield return new WaitUntil(() => !_waitingForOeCard);

		//Debug.Log("Operating Expense is: " + _operatingExpenses);

		yield return new WaitWhile(() => _uiManager._harvestPanel.activeSelf);

		_uiManager._forcedLoanModalPanel.SetActive(false);
		_pManager.UpdateMyCash(_netCheck);

		if (_netCheck < 0)
			AudioManager.Instance.PlaySound(AudioManager.Instance._bad);

		_okButton1Pressed = false;
		_netCheck = 0;

		PerformPostHarvestActions(space);
		ResetHarvestModifiers();
	}

	public IEnumerator PerformGarnishedHarvestRoutine(int space)
	{
		_harvestCheck = 0;

		if (_pManager == null)
			_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();
		if (_uiManager == null)
		{
			_uiManager = GameManager.Instance.uiManager;
			_gMessageText = _uiManager._gHarvestMessageText;
			_ok1GButton = _uiManager._ok1GarnishedButton;
			_ok1GButton.onClick.AddListener(OnOkButton1GarnishedClicked);
			_ok2GButton = _uiManager._ok2GarnishedButton;
			_ok2GButton.onClick.AddListener(OnOkButton2GarnishedClicked);
			//_ok3GButton = _uiManager._ok2GarnishedButton;
			//_ok3GButton.onClick.AddListener(OnOkButton3GarnishedClicked);
			//Normal Harvest Stuff
			_messageText = _uiManager._harvestMessageText;
			_rollButton = _uiManager._harvestRollButton;
			_rollButton.onClick.AddListener(OnHarvestRollButtonClicked);
			_ok1Button = _uiManager._harvestOk1Button;
			_ok1Button.onClick.AddListener(OnOkButton1Clicked);
			_ok2Button = _uiManager._harvestOk2Button;
			_ok2Button.onClick.AddListener(OnOkButton2Clicked);
			_ok3Button = _uiManager._harvestOk3Button;
			_ok3Button.onClick.AddListener(OnOkButton3Clicked);
		}
		if (_pMove == null)
			_pMove = GameManager.Instance.myFarmer.GetComponent<PlayerMove>();

		_ok1GButtonPressed = false;
		_uiManager._gHarvestPanel.SetActive(true);
		_uiManager._forcedLoanModalPanel.SetActive(true);
		//play the show animation
		_uiManager._gHarvestPanel.GetComponent<DOTweenAnimation>().DOPlayForward(); //scale up

		_ok1GButton.gameObject.SetActive(true);
		_gMessageText.text = "Wages Garnished - No Harvest Check!";
		//_ok1GButton.gameObject.SetActive(true);

		yield return new WaitUntil(() => _ok1GButtonPressed);
		_ok1GButtonPressed = false;

		Debug.Log(" draw  & show OE Card");
		DrawOECard();
		yield return new WaitUntil(() => !_waitingForOeCard);
		_okButton1Pressed = false;
		_ok2GButton.gameObject.SetActive(true);

		yield return new WaitWhile(() => _uiManager._gHarvestPanel.activeSelf);

		_pManager.UpdateMyCash(_netCheck);

		if (_netCheck < 0)
			AudioManager.Instance.PlaySound(AudioManager.Instance._bad);

		PerformPostHarvestActions(space);
		ResetHarvestModifiers();
	}
	#endregion

	#region HARVEST CHARTS

	int GetHayCheck(int die, int hay)
	{
		int baseAmt = 0;

		switch (die)
		{
			case 1:
				baseAmt = (int)(400 * _hayFudgeFactor);
				break;

			case 2:
				baseAmt = (int)(600 * _hayFudgeFactor);
				break;

			case 3:
				baseAmt = (int)(1000 * _hayFudgeFactor);
				break;

			case 4:
				baseAmt = (int)(1500 * _hayFudgeFactor);
				break;

			case 5:
				baseAmt = (int)(2200 * _hayFudgeFactor);
				break;

			case 6:
				baseAmt = (int)(3000 * _hayFudgeFactor);
				break;
		}

		int calcHay = hay / 10;

		return baseAmt * calcHay;
	}

	int GetFruitCheck(int die, int fruit)
	{
		int baseAmt = 0;

		switch (die)
		{
			case 1:
				baseAmt = (int)(2000 * _fruitFudgeFactor);
				break;

			case 2:
				baseAmt = (int)(3500 * _fruitFudgeFactor);
				break;

			case 3:
				baseAmt = (int)(6000 * _fruitFudgeFactor);
				break;

			case 4:
				baseAmt = (int)(9000 * _fruitFudgeFactor);
				break;

			case 5:
				baseAmt = (int)(13000 * _fruitFudgeFactor);
				break;

			case 6:
				baseAmt = (int)(17500 * _fruitFudgeFactor);
				break;
		}

		int calcFruit = fruit / 5;

		return baseAmt * calcFruit;
	}

	int GetGrainCheck(int die, int grain)
	{
		int baseAmt = 0;
		int calcGrain = grain / 10;

		if (die == 2 || die == 3 || die == 6)
		{
			switch (die)
			{
				case 2:
					baseAmt = 1500;
					break;

				case 3:
					baseAmt = 2500;
					break;

				case 6:
					baseAmt = 7000;
					break;
			}
			return baseAmt * calcGrain;
		}
		else if (die == 1)
		{
			switch (grain)
			{
				case 10:
					baseAmt = 800;
					break;

				case 20:
					baseAmt = 1500;
					break;

				case 30:
					baseAmt = 2300;
					break;

				case 40:
					baseAmt = 3000;
					break;

				case 50:
					baseAmt = 3800;
					break;

				case 60:
					baseAmt = 4500;
					break;

				case 70:
					baseAmt = 5300;
					break;

				case 80:
					baseAmt = 6000;
					break;

				case 90:
					baseAmt = 6800;
					break;

				case 100:
					baseAmt = 7600;
					break;
			}
			return baseAmt;
		}
		else if (die == 4)
		{
			switch (grain)
			{
				case 10:
					baseAmt = 3800;
					break;

				case 20:
					baseAmt = 7500;
					break;

				case 30:
					baseAmt = 11300;
					break;

				case 40:
					baseAmt = 15000;
					break;

				case 50:
					baseAmt = 18800;
					break;

				case 60:
					baseAmt = 22500;
					break;

				case 70:
					baseAmt = 26300;
					break;

				case 80:
					baseAmt = 30000;
					break;

				case 90:
					baseAmt = 33800;
					break;

				case 100:
					baseAmt = 37600;
					break;
			}
			return baseAmt;
		}
		else if (die == 5)
		{
			switch (grain)
			{
				case 10:
					baseAmt = 5300;
					break;

				case 20:
					baseAmt = 10500;
					break;

				case 30:
					baseAmt = 15800;
					break;

				case 40:
					baseAmt = 21000;
					break;

				case 50:
					baseAmt = 26300;
					break;

				case 60:
					baseAmt = 31500;
					break;

				case 70:
					baseAmt = 36800;
					break;

				case 80:
					baseAmt = 42000;
					break;

				case 90:
					baseAmt = 47300;
					break;

				case 100:
					baseAmt = 52600;
					break;
			}
		}
		return baseAmt;
	}

	int GetCowCheck(int die, int cows)
	{
		int baseAmt = 0;

		switch (die)
		{
			case 1:
				baseAmt = 1400;
				break;

			case 2:
				baseAmt = 2000;
				break;

			case 3:
				baseAmt = 2800;
				break;

			case 4:
				baseAmt = 3800;
				break;

			case 5:
				baseAmt = 5000;
				break;

			case 6:
				baseAmt = 7500;
				break;
		}

		int calcCows = cows / 10;

		return baseAmt * calcCows;
	}

	int GetSpudCheck(int die, int spuds)
	{
		int baseAmt = 0;

		switch (die)
		{
			case 1:
				baseAmt = (int)(800 * _spudFudgeFactor);
				break;

			case 2:
				baseAmt = (int)(1500 * _spudFudgeFactor);
				break;

			case 3:
				baseAmt = (int)(2500 * _spudFudgeFactor);
				break;

			case 4:
				baseAmt = (int)(3800 * _spudFudgeFactor);
				break;

			case 5:
				baseAmt = (int)(5300 * _spudFudgeFactor);
				break;

			case 6:
				baseAmt = (int)(7000 * _spudFudgeFactor);
				break;
		}

		int calcSpuds = spuds / 10;

		return baseAmt * calcSpuds;
	}
	#endregion

	#region Private Methods

	void SpecialBoardConditions(string commodity)
	{
		if (commodity == "Hay"||commodity=="Cherries"||commodity=="Livestock")
		{
			if (_cutHarvestInHalf)
				_harvestCheck /= 2;
			if (_doubleHarvest)
				_harvestCheck *= 2;
		}
		if (commodity == "Wheat")
		{
			if (_add50PerWheatAcre)
				_harvestCheck += (50 * _pManager._pGrain);
			if (_cut50PerWheatAcre)
				_harvestCheck -= (50 * _pManager._pGrain);
		}
	}

	void SpecialPlayerConditions(string commodity)
	{
		if (commodity == "Hay" && _pManager._pHayDoubled)
			_harvestCheck *= (int)(Mathf.Pow(2, _pManager._pHayDoubledCounter));
		if (commodity == "Cherries" && _pManager._pCherriesCutInHalf)
			_harvestCheck /= 2;
		if (commodity == "Wheat" && _pManager._pWheatCutInHalf)
			_harvestCheck /= 2;
		if (commodity == "Livestock" && _pManager._pCowsIncreased)
		{
			float tempCheck = _harvestCheck;
			tempCheck *= 1.5f;
			_harvestCheck = (int)tempCheck;
		}
		if (commodity == "Spuds" && _pManager._pSpudsDoubled)
			_harvestCheck *= 2;
		if (commodity == "Corn" && _pManager._pCornDoubled)
			_harvestCheck *= 2;
	}

	void DrawOECard()
	{
		_waitingForOeCard = true;

		//fire the DrawOECard event...
		//event data
		object[] sendData = new object[] { PhotonNetwork.LocalPlayer.ActorNumber };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Draw_Oe_Event_Code, sendData, eventOptions, sendOptions);
	}

	void OnOeCardReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Receive_Oe_Event_Code)
		{
			object[] recData = (object[])eventData.CustomData;
			int cNum = (int)recData[0];
			string desc = (string)recData[1];
			bool bottomCard = (bool)recData[2];

			OECard drawnCard = new OECard();
			drawnCard.cardNumber = cNum;
			drawnCard.description = desc;
			drawnCard.bottomCard = bottomCard;

			StartCoroutine(ShowOeCardRoutine(drawnCard));
		}
	}

	IEnumerator ShowOeCardRoutine(OECard drawnCard)
	{
		//show the OE card panel
		_uiManager._oeCardText.text = drawnCard.description;
		_uiManager._oePanel.SetActive(true);
		_uiManager._forcedLoanModalPanel.SetActive(true);

		//play the open animation
		_uiManager._oePanel.GetComponent<DOTweenAnimation>().DOPlayForward();   //scale up
		_uiManager._oePanel.transform.DOLocalMove(new Vector3(0, -33), 0.3f);	//move to center

		//assign the operating expense
		_operatingExpenses = GetOperatingExpenses(drawnCard.cardNumber);

		_netCheck = (_harvestCheck - _operatingExpenses);

		while (!_isOkToCloseOePanel)
			yield return null;

		_isOkToCloseOePanel = false;

		//play the close animation
		_uiManager._oePanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();   //scale down
		_uiManager._oePanel.transform.DOLocalMove(new Vector3(638, -219), 0.3f);   //move to side
		//_uiManager._forcedLoanModalPanel.SetActive(false);

		//yield return new WaitForSeconds(0.3f);

		if (!_pManager._pWagesGarnished)
			_messageText.text = "Your Harvest results are: You received $" + _harvestCheck + " for your commodity and your Operating Expenses were $" + _operatingExpenses + " for a net Harvest of $" + _netCheck;
		else
			_gMessageText.text= "Your Harvest results are: You received $" + _harvestCheck + " for your commodity and your Operating Expenses were $" + _operatingExpenses + " for a net Harvest of $" + _netCheck;

		_waitingForOeCard = false;

		if (!_pManager._pWagesGarnished)
			_ok3Button.gameObject.SetActive(true);
		else
			_ok2GButton.gameObject.SetActive(true);

		//return the card to the deck
		//fire the replace OE event...
		//event data
		object[] sendData = new object[] { drawnCard.cardNumber, drawnCard.description, drawnCard.bottomCard };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.MasterClient,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Replace_Oe_Event_Code, sendData, eventOptions, sendOptions);
	}

	int GetOperatingExpenses(int cardNum)
	{
		int expenses = 0;

		switch (cardNum)
		{
			case 1:	//no tractor
			case 2:
				if(!_pManager._pTractor)
					expenses = 2000;
				break;

			case 3:	//no harvester
			case 4:
				if (!_pManager._pHarvester)
					expenses = 2000;
				break;

			case 5:	//100 per cow
				expenses = 100 * (_pManager._pFarmCows + _pManager._pRangeCows);
				break;

			case 6:  //500 if you have cows
				if (_pManager._pRangeCows > 0 || _pManager._pFarmCows > 0)
					expenses = 500;
				break;

			case 7:  //electric bill
				expenses = 500;
				break;

			case 8:  //real estate taxes
				expenses = 1500;
				break;

			case 9:  //fertilizer bill
			case 10:
				expenses = 100 * (_pManager._pHay + _pManager._pFruit + _pManager._pGrain + _pManager._pSpuds);
				break;

			case 11: //equipment in shop
			case 12:
				expenses = 1000;
				break;

			case 13: //10% on bank notes
			case 14:
				expenses = (int)(_pManager._pNotes * 0.1f);
				break;

			case 15: //equipment breakdown
			case 16:
				expenses = 500;
				break;

			case 17: //farmer's insurance
				expenses = 1500;
				break;

			case 18: //fuel bill
			case 19:
				expenses = 1000;
				break;

			case 20: //parts bill
			case 21:
				expenses = 500;
				break;

			case 22: //seed bill
			case 23:
				expenses = 3000;
				break;

			case 24: //wire worms
				expenses = 100 * _pManager._pGrain;
				break;

			case 25: //pay fruit pickers - EXTRA CARD NOT IN NEW DECKS
				expenses = 400 * _pManager._pFruit;
				break;

			case 26: //pay spud pickers
				expenses = 200 * _pManager._pSpuds;
				break;

			case 27: //no spud harvest vacation
				expenses = 150 * _pManager._pSpuds;
				break;
		}
		return expenses;
	}

	void PerformPostHarvestActions(int space)
	{
		if (_bManager == null)
			_bManager = GameManager.Instance.bManager;

		switch (space)
		{
			case 20: //draw OTB
			case 27:
			case 35:
			case 42:
			case 44:
				_pManager.DrawOTBCard();
				AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				break;

			case 24: //draw FF
			case 41:
			case 43:
			case 49:
				_pManager.DrawFFCard();
				AudioManager.Instance.PlaySound(AudioManager.Instance._ff);
				break;

			case 25: //fireworks
				StartCoroutine(_bManager.PlayFireworksRoutine());
				break;

			case 28: //goto Harvest Moon
				_pMove.DirectedForwardMove(36);
				AudioManager.Instance.PlaySound(AudioManager.Instance._getOnYourTractor);
				break;

			case 30: //farming like an idiot
				_pMove.DirectedForwardMove(58);
				AudioManager.Instance.PlaySound(AudioManager.Instance._farmingLikeAnIdiot);
				break;

			case 34: //tractor owners go to space 46
				if (_pManager._pTractor)
				{
					_pMove.DirectedForwardMove(46);
					AudioManager.Instance.PlaySound(AudioManager.Instance._getOnYourTractor);
				}
				break;

			default:
				Debug.LogWarning("Nothing to do here... " + space);
				break;
		}

		if (!_pManager._pWagesGarnished)
			_rollButton.gameObject.SetActive(true);
		else
			_ok1GButton.gameObject.SetActive(true);
	}

	void ResetHarvestModifiers()
	{
		_cutHarvestInHalf = false;
		_doubleHarvest = false;
		_add50PerWheatAcre = false;
		_cut50PerWheatAcre = false;
	}

	void SendHarvestRollMessage()
	{
		//data - nickname, farmerName, die, commodity
		object[] sndData = new object[] { PhotonNetwork.LocalPlayer.NickName, GameManager.Instance.myFarmerName, _dieRoll, _commodity };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event to the UIManagers
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Harvest_Roll_Message_Event_Code, sndData, eventOptions, sendOptions);
	}

	#endregion
}
