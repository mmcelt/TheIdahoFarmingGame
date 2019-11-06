using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;

public class UIManager : MonoBehaviourPun
{
	#region Public / Serialized Fields

	[Header("Left Panel")]
	public TextMeshProUGUI _playerNameText, _farmerNameText;
	public TextMeshProUGUI[] _otherPlayerNameTexts;
	public TextMeshProUGUI[] _otherPlayerCashTexts;
	public TextMeshProUGUI[] _otherPlayerNotesTexts;
	public TextMeshProUGUI[] _otherPlayerOtbTexts;
	public GameObject _wagesGarnishedWarning;
	public GameObject _cherriesCutInHalfWarning;
	public GameObject _wheatCutInHalfWarning;

	public Button _rollButton;
	public Button _endTurnButton;
	public TextMeshProUGUI _currentYearText, _otbText, _playerCashText, _playerNotesText, _networthText;
	public Image _harvesterImage, _tractorImage;
	public Image _hayImage, _grainImage, _spudsImage, _farmCowImage, _rangeCowImage;
	[SerializeField] Text _hayDoubledCounterText;
	[SerializeField] Sprite _hayNormal, _hayDoubled, _grainNormal, _grainDoubled, _spudsNormal, _spudsDoubled, _cowNormal, _cowDoubled;
	[SerializeField] Text _hayText, _grainText, _fruitText, _spudText, _fCowText, _rCowText;

	[Header("Right Panel")]
	[SerializeField] Text[] _remotePlayerNameTexts;
	[SerializeField] Text[] _remotePlayerNetworthTexts;
	[SerializeField] Text _otbShuffleText, _oeShuffleText, _ffShuffleText;
	[SerializeField] Text _otbLeftText, _oeLeftText, _ffLeftText;
	[SerializeField] Text _otbTotalText, _oeTotalText, _ffTotalText;

	[Header("Actions Panel")]
	public GameObject _actionsPanel;
	public Text _cashText, _notesText, _otbMessageText;
	public Button _sellOtbButton, _buyOptionButton, _payInFullButton, _cancelButton, _otbOkButton;
	public Button _repayLoanButton, _getLoanButton;
	public Dropdown _otbDropdown;
	public InputField _downPaymentInput;
	public InputField _getLoanInput;
	public InputField _repayLoanInput;

	[Header("Board Space Panel")]
	public GameObject _boardSpacePanel;
	public Text _headerText;
	public TextMeshProUGUI _spaceText;
	public Button _okButton;

	[Header("Otb Card Panel")]
	public GameObject _otbPanel;
	public Text _otbCardText, _otbTotalCostText;

	[Header("Ff Card Panel")]
	public GameObject _ffPanel;
	public Text _ffCardText;
	public Image _ffCardBackground;

	[Header("Oe Card Panel")]
	public GameObject _oePanel;
	public Text _oeCardText;
	public Image _oeCardBackground;

	[Header("Teton Dam Panel")]
	public GameObject _tetonDamPanel;
	public Image _tetonDamImage;
	public TextMeshProUGUI _tetonHeaderText, _tetonMessageText;
	public Button _tetonRollButton, _tetonOkButton;

	[Header("Custom Hire Harvester Panel")]
	public GameObject _customHarvesterPanel;

	[Header("Uncle Cheester Panel")]
	public GameObject _uncleCheesterPanel;

	[Header("Harvest Panel")]
	public GameObject _harvestPanel;
	public Text _harvestMessageText;
	public Button _harvestRollButton, _harvestOk1Button, _harvestOk2Button, _harvestOk3Button;

	[Header("Garnished Harvest Panel")]
	public GameObject _gHarvestPanel;
	public Button _ok1GarnishedButton, _ok2GarnishedButton, _ok3GarnishedButton;
	public Text _gHarvestMessageText;

	[Header("Forced Loan Panel")]
	public GameObject _forcedLoanPanel;
	public Text _flCashText, _flNotesText;
	public Button _getFlButton;
	public InputField _forcedLoanInput;

	[Header("Message Texts")]
	public Text _bonusMessageText;

	[Header("Modal Panels")]
	public GameObject _modalPanel;
	public GameObject _completeModalPanel;

	[Header("Misc Stuff")]
	public int _tempCash;
	public int _tempNotes;
	public int _otbCost;
	public int _minDownPayment;
	public int _downPayment;
	public int _repayLoanAmount;
	public int _loanAmount;
	public bool _transactionBlocked;
	public bool _stopBuying;

	#endregion

	#region Private Fields / References

	[HideInInspector] public MyDiceRoll _diceRoll;
	PlayerSetup _pSetup;
	PlayerManager _pManager;
	PlayerMove _pMove;
	DeckManager _dManager;
	RemotePlayerUpdater _rpUpdater;

	//OTB Stuff
	List<string> _otbCards = new List<string>();

	int _selectedIndex;
	OTBCard _selectedCard;
	int _sellingPrice;

	bool _isSellMsg;
	bool _isWarningMsg;
	bool _cancelOtbSale;
	//deck info
	int _otbShuffleCount;
	int _oeShuffleCount;
	int _ffShuffleCount;
	int _otbCardsLeft;
	int _oeCardsLeft;
	int _ffCardsLeft;
	int _otbTotalCards;
	int _oeTotalCards;
	int _ffTotalCards;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		//InitializeTheActionsPanel();
		PhotonNetwork.NetworkingClient.EventReceived += OnSpudBonusEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnShuffleADeckEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnUpdateDeckDataEventReceived;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnSpudBonusEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnShuffleADeckEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnUpdateDeckDataEventReceived;
	}

	void Awake()
	{
		GameManager.Instance.uiManager = this;
	}
	void Start() 
	{
		_wagesGarnishedWarning.SetActive(false);
		_cherriesCutInHalfWarning.SetActive(false);
		_wheatCutInHalfWarning.SetActive(false);
		_tetonMessageText.text = IFG.TetonDamMessageText;

		_pSetup = GetComponentInParent<PlayerSetup>();
		_pManager = GetComponentInParent<PlayerManager>();
		_pMove = GetComponentInParent<PlayerMove>();
		_diceRoll = GameManager.Instance.myDiceRoll;
		_rpUpdater = GameManager.Instance.myFarmer.GetComponent<RemotePlayerUpdater>();

		InitialPlayerInfoUpdate();
		UpdateUI();

		StartCoroutine("UpdateUIRoutine");
	}
	#endregion

	#region UI Callback Methods

	//left panel methods
	public void OnRollButtonClicked()
	{
		_actionsPanel.SetActive(false);
		_diceRoll.OnRollButton();
		_rollButton.interactable = false;
	}

	public void OnEndTurnButtonClicked()
	{
		_actionsPanel.SetActive(false);
		_pManager.EndTurn();
	}

	public void OnActionsButtonClicked()
	{
		//toggle Actions Panel
		bool actionsPanelActive = _actionsPanel.activeSelf;

		_actionsPanel.SetActive(!actionsPanelActive);

		if (_actionsPanel.activeSelf)
		{
			InitializeTheActionsPanel();
		}
		else
			ResetOtbListPanel();
	}

	//actions panel methods
	public void OnCloseButtonClicked()
	{
		ResetOtbListPanel();
		_actionsPanel.SetActive(false);
	}

	public void OnBuyOptionButtonClicked()
	{
		if (!_transactionBlocked)
		{
			_buyOptionButton.interactable = false;
			UseOtb(_selectedCard);
			_actionsPanel.SetActive(false);
		}
		else
		{
			_isWarningMsg = true;
			_otbMessageText.text = "Insufficient Funds!";
		}
	}

	public void OnSellOtbButtonClicked()
	{
		if (_pManager._isMyTurn)
		{
			_isSellMsg = true;
			StartCoroutine("SellOtbRoutine");
		}
		else
		{
			_isWarningMsg = true;
			_otbMessageText.text = "You can only sell an OTB card on your turn...";
			_otbOkButton.gameObject.SetActive(true);
		}
	}

	public void OnOtbOkButtonClicked()
	{
		_isWarningMsg = false;
		_isSellMsg = false;

		StopCoroutine("BuyOptionRoutine");
		_stopBuying = true;
		_transactionBlocked = true;

		_otbMessageText.text = "";
		_otbOkButton.gameObject.SetActive(false);
		_cancelButton.gameObject.SetActive(false);

	}

	public void OnOtbCancelButtonClicked()
	{
		_cancelOtbSale = true;
		_otbOkButton.gameObject.SetActive(false);
		_cancelButton.gameObject.SetActive(false);
		_otbMessageText.text = "";
		ResetOtbListPanel();
	}

	public void OnRepayLoanButtonClicked()
	{
		_pManager.UpdateMyCash(-_repayLoanAmount);
		_pManager.UpdateMyNotes(-_repayLoanAmount);
		_repayLoanAmount = 0;
		ResetTempFunds();
		_repayLoanInput.text = "";
		_repayLoanInput.placeholder.GetComponent<Text>().text = "Enter your Loan Repayment...";
	}

	public void OnGetLoanButtonClicked()
	{
		//Debug.Log("LoanAmt: " + _loanAmount);

		_pManager.UpdateMyCash(_loanAmount);
		_pManager.UpdateMyNotes(_loanAmount + (int)(_loanAmount * 0.2f));
		_loanAmount = 0;
		_getLoanInput.text = "";
		_getLoanInput.placeholder.GetComponent<Text>().text = "Enter your Loan Amount...";
		ResetTempFunds();
	}

	public void OnOtbDropdownValueChanged(int index)
	{
		_stopBuying = false;
		//Debug.Log("INDEX: " + index);

		if (index > 0)
		{
			_downPaymentInput.interactable = true;
			_downPaymentInput.Select();
			_selectedCard = _pManager._myOtbs[index - 1];
			_otbCost = _selectedCard.totalCost;
			_minDownPayment = (int)(_otbCost * 0.2f);
			_downPaymentInput.placeholder.GetComponent<Text>().text = _minDownPayment.ToString();

			if (!_stopBuying)
				StartCoroutine("BuyOptionRoutine");

			if (_pManager._isMyTurn)
			{
				_sellOtbButton.gameObject.SetActive(true);
				_sellOtbButton.interactable = true;
			}

			if (_pManager._pCash >= _otbCost)
				_payInFullButton.interactable = true;

			//Debug.Log(_selectedCard.summary);
		}
		else
		{
			StopCoroutine("BuyOptionRoutine");
			ResetOtbListPanel();
			_repayLoanInput.Select();
			_sellOtbButton.interactable = false;
		}
	}

	public void OnPayInFullButtonClicked()
	{
		_downPaymentInput.text = _otbCost.ToString();
	}

	public void OnGetFocedLoanButtonClicked()
	{
		_pManager.UpdateMyCash(_loanAmount);
		_pManager.UpdateMyNotes(_loanAmount + (int)(_loanAmount * 0.2f));
		_loanAmount = 0;
		_forcedLoanInput.text = "";
		_forcedLoanInput.placeholder.GetComponent<Text>().text = "Enter the Loan Amount...";
		//_modalPanel.SetActive(false);
	}

	public void UpdateActionsPanelFunds(int cash, int notes)
	{
		_cashText.text = cash.ToString("c0");
		_notesText.text = notes.ToString("c0");

		if (cash <= 0)
			_cashText.color = Color.red;
		else
			_cashText.color = Color.green;

		if (notes >= 50000)
			_notesText.color = Color.red;
		else
			_notesText.color = Color.green;

		if (cash < 0 || notes > 50000)
			_transactionBlocked = true;
		else
			_transactionBlocked = false;
	}
	#endregion

	#region Public Methods

	public void UpdateUI()
	{
		_otbText.text = _pManager._myOtbs.Count.ToString();
		_playerCashText.text = _pManager._pCash.ToString("c0");

		if (_pManager._pCash <= 0)
			_playerCashText.color = Color.red;
		else
			_playerCashText.color = Color.black;

		_playerNotesText.text = _pManager._pNotes.ToString("c0");

		if (_pManager._pNotes >= 50000)
			_playerNotesText.color = Color.red;
		else
			_playerNotesText.color = Color.black;

		_networthText.text = _pManager._pNetworth.ToString("c0");
		_currentYearText.text = _pMove._currentYear.ToString();

		if (_pManager._pHarvester)
			_harvesterImage.color = Color.white;
		else
			_harvesterImage.color = IFG.GreyedOut;

		if (_pManager._pTractor)
			_tractorImage.color = Color.white;
		else
			_tractorImage.color = IFG.GreyedOut;

		if (_pManager._pHayDoubled)
		{
			_hayImage.sprite = _hayDoubled;
			_hayDoubledCounterText.text = "x" + _pManager._pHayDoubledCounter;
		}
		else
		{
			_hayImage.sprite = _hayNormal;
			_hayDoubledCounterText.text = "x" + _pManager._pHayDoubledCounter;
		}

		//_hayDoubledCounterText.text = "x" + _pManager._hayDoubledCounter;

		if (_pManager._pCornDoubled)
			_grainImage.sprite = _grainDoubled;
		else
			_grainImage.sprite = _grainNormal;

		if (_pManager._pSpudsDoubled)
			_spudsImage.sprite = _spudsDoubled;
		else
			_spudsImage.sprite = _spudsNormal;

		if (_pManager._pCowsIncreased)
		{
			_farmCowImage.sprite = _cowDoubled;
			_rangeCowImage.sprite = _cowDoubled;
		}
		else
		{
			_farmCowImage.sprite = _cowNormal;
			_rangeCowImage.sprite = _cowNormal;
		}

		_hayText.text = _pManager._pHay.ToString();
		_grainText.text = _pManager._pGrain.ToString();
		_fruitText.text = _pManager._pFruit.ToString();
		_spudText.text = _pManager._pSpuds.ToString();
		_fCowText.text = _pManager._pFarmCows.ToString();
		_rCowText.text = _pManager._pRangeCows.ToString();

		if (!_pManager._pWagesGarnished)
			_wagesGarnishedWarning.SetActive(false);
		if (!_pManager._pCherriesCutInHalf)
			_cherriesCutInHalfWarning.SetActive(false);
		if (!_pManager._pWheatCutInHalf)
			_wheatCutInHalfWarning.SetActive(false);
		//deck info
		_otbShuffleText.text = _otbShuffleCount.ToString();
		_otbLeftText.text = _otbCardsLeft.ToString();
		_otbTotalText.text = _otbTotalCards.ToString();
		_oeShuffleText.text = _oeShuffleCount.ToString();
		_oeLeftText.text = _oeCardsLeft.ToString();
		_oeTotalText.text = _oeTotalCards.ToString();
		_ffShuffleText.text = _ffShuffleCount.ToString();
		_ffLeftText.text = _ffCardsLeft.ToString();
		_ffTotalText.text = _ffTotalCards.ToString();
	}

	public Color SelectFontColorForFarmer(string farmer)
	{
		switch (farmer)
		{
			case "Rigby Ron":
				return Color.blue;

			case "Kimberly Kay":
				return Color.yellow;

			case "Jerome Jerry":
				return IFG.Purple;

			case "Ririe Ric":
				return Color.black;

			case "Menan Mike":
				return Color.red;

			case "Blackfoot Becky":
				return Color.white;
		}
		return Color.black;
	}

	public IEnumerator BuyOptionRoutine()
	{
		_transactionBlocked = true;
		_buyOptionButton.interactable = false;
		yield return new WaitUntil(() => CheckIfAbleToBuyOption());
		//Debug.Log("Past CIATBO");
		if (_selectedCard.cardNumber >= 29 && _selectedCard.cardNumber <= 40)
		{
			yield return new WaitUntil(() => !CheckIfRangeIsAlreadyOwned());
			//Debug.Log("Past CIRIAO");
		}
		if (_selectedCard.cardNumber >= 6 && _selectedCard.cardNumber <= 11)
		{
			yield return new WaitUntil(() => !CheckIfMaxFarmCowsReached());
		}
		yield return new WaitUntil(() => !_transactionBlocked);
		//Debug.Log("Past TB");
		_buyOptionButton.interactable = true;
	}

	//forced loan panel methods
	public void UpdateForcedLoanFunds(int cash, int notes)
	{
		_flCashText.text = cash.ToString("c0");
		_flNotesText.text = notes.ToString("c0");

		if (cash < 0)
			_flCashText.color = Color.red;
		else
			_flCashText.color = Color.green;

		if (notes > 50000)
			_flNotesText.color = Color.red;
		else
			_flNotesText.color = Color.green;

		if (cash < 0 || notes > 50000)
			_transactionBlocked = true;
		else
			_transactionBlocked = false;

		_getFlButton.interactable = !_transactionBlocked;
	}

	#endregion

	#region Private Methods

	//TESTING
	IEnumerator UpdateUIRoutine()
	{
		Debug.Log("IN UPDATE ROUTINE");
		UpdateUI();
		yield return null;
		StartCoroutine("UpdateUIRoutine");
		yield return new WaitForSeconds(1f);
		StopCoroutine("UpdateUIRoutine");
	}
	//END TESTING

	void InitialPlayerInfoUpdate()
	{
		_playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
		_farmerNameText.text = GameManager.Instance.myFarmerName;
		_farmerNameText.color = SelectFontColorForFarmer(GameManager.Instance.myFarmerName);

		if (PhotonNetwork.PlayerList.Length > 1)
			StartCoroutine(UpdateRemotePlayerInfo());

		_currentYearText.text = _pMove._currentYear.ToString();
	}

	IEnumerator UpdateRemotePlayerInfo()
	{
		//update the remote players info...
		int index = 0;
		foreach (Player player in PhotonNetwork.PlayerList)
		{
			if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				_otherPlayerNameTexts[index].text = player.NickName;

				object farmer;
				if (player.CustomProperties.TryGetValue(IFG.Selected_Farmer, out farmer))
				{
					_otherPlayerNameTexts[index].color = SelectFontColorForFarmer((string)farmer);
				}

				object cash;
				if (player.CustomProperties.TryGetValue(IFG.Player_Cash, out cash))
				{
					int playersCash = 0;
					playersCash = (int)cash;
					_otherPlayerCashTexts[index].text = playersCash.ToString("c0");
					if (playersCash <= 0)
						_otherPlayerCashTexts[index].color = Color.red;
					else
						_otherPlayerCashTexts[index].color = Color.black;
				}

				object notes;
				if (player.CustomProperties.TryGetValue(IFG.Player_Notes, out notes))
				{
					int playersNotes = 0;
					playersNotes = (int)notes;
					_otherPlayerNotesTexts[index].text = playersNotes.ToString("c0");
					if (playersNotes >= 50000)
						_otherPlayerNotesTexts[index].color = Color.red;
					else
						_otherPlayerNotesTexts[index].color = Color.black;
				}

				//object otbs;
				//if (player.CustomProperties.TryGetValue(IFG.Player_Otb_Count, out otbs))
				//{
				//	int playersOtbs = 0;
				//	playersOtbs = (int)otbs;
				//	Debug.Log("In UpdateRemoteOTB: " + playersOtbs);
				//	_otherPlayerOtbTexts[index].text = playersOtbs.ToString();
				//}

				object networth;
				if (player.CustomProperties.TryGetValue(IFG.Player_Networth, out networth))
				{
					int playerNetworth = 0;
					playerNetworth = (int)networth;
					_remotePlayerNameTexts[index].text = player.NickName;
					_remotePlayerNameTexts[index].color = SelectFontColorForFarmer((string)farmer);
					_remotePlayerNetworthTexts[index].text = playerNetworth.ToString("c0");
				}
				index++;
				//Debug.Log("Index: " + index);
			}
		}

		yield return new WaitForSeconds(0.5f);
		StartCoroutine(UpdateRemotePlayerInfo());
	}

	void InitializeTheActionsPanel()
	{
		ResetTempFunds();
		PopulateOtbDropdown();
		_repayLoanInput.Select();
	}

	public void ResetTempFunds()
	{
		_tempCash = _pManager._pCash;
		_tempNotes = _pManager._pNotes;
		UpdateActionsPanelFunds(_tempCash, _tempNotes);
	}

	void PopulateOtbDropdown()
	{
		_otbCards.Clear();
		_otbDropdown.ClearOptions();

		string dummyEntry = "                  My OTB Cards";
		_otbCards.Add(dummyEntry); //this will be index 0

		foreach (OTBCard card in _pManager._myOtbs)
		{
			_otbCards.Add(card.cardNumber + " : " + card.summary);
		}

		_otbDropdown.AddOptions(_otbCards);
		_otbDropdown.value = 0;
		_otbDropdown.Select();
		_otbDropdown.RefreshShownValue();
		_selectedIndex = 0;
	}

	void ResetOtbListPanel()
	{
		_selectedCard = null;
		_otbCost = 0;
		_minDownPayment = 0;
		_downPaymentInput.text = "";
		_downPaymentInput.placeholder.GetComponent<Text>().text = "Enter your Downpayment...";
		_sellOtbButton.interactable = false;
		_buyOptionButton.interactable = false;
		_payInFullButton.interactable = false;
		_stopBuying = false;
		StopCoroutine("BuyOptionRoutine");
		ResetTempFunds();
		PopulateOtbDropdown();
	}

	//called by the otbOk button
	IEnumerator SellOtbRoutine()
	{
		_otbMessageText.text = SelectSellMessageText();
		_otbOkButton.gameObject.SetActive(true);
		_cancelButton.gameObject.SetActive(true);

		yield return new WaitWhile(() => _otbMessageText.text != "");

		//Debug.Log("OUT OF SELL WAIT");
		_sellOtbButton.gameObject.SetActive(false);
		if (!_cancelOtbSale)
		{
			_pManager.UpdateMyCash(_sellingPrice);
			_pManager.DiscardOtbCard(_selectedCard);
			ResetOtbListPanel();
			PopulateOtbDropdown();
		}
		_cancelOtbSale = false;
		ResetOtbListPanel();
		_actionsPanel.SetActive(false);
	}

	string SelectSellMessageText()
	{
		string msgText = "";

		switch (_selectedCard.cardNumber)
		{
			case 1:  //grain
			case 2:
			case 3:
			case 4:
			case 5:
				msgText = "Selling a 10 acres of Grain OTB will give you 10% of its purchase price: $2000.";
				_sellingPrice = 2000;
				return msgText;

			case 6:  //livestock
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				msgText = "Selling a Livestock OTB will give you 10% of its purchase price: $500.";
				_sellingPrice = 500;
				return msgText;

			case 12: //hay
			case 13:
			case 14:
			case 15:
			case 16:
				msgText = "Selling a 10 acres of Hay OTB will give you 10% of its purchase price: $2000.";
				_sellingPrice = 2000;
				return msgText;

			case 17: //tractor
			case 18:
			case 19:
				msgText = "Selling a Tractor OTB will give you 10% of its purchase price: $1000.";
				_sellingPrice = 1000;
				return msgText;

			case 20: //harvester
			case 21:
			case 22:
				msgText = "Selling a Harvester OTB will give you 10% of its purchase price: $1000.";
				_sellingPrice = 1000;
				return msgText;

			case 23: //fruit
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
				msgText = "Selling a 5 acres of Fruit OTB will give you 10% of its purchase price: $2500.";
				_sellingPrice = 2500;
				return msgText;

			case 29: //lemhi range
			case 30:
			case 31:
				msgText = "Selling a Lemhi Range OTB will only give you 10% of its cow's purchase price: $2500.";
				_sellingPrice = 2500;
				return msgText;

			case 32: //lost river range
			case 33:
			case 34:
				msgText = "Selling a Lost River Range OTB will only give you 10% of its cows purchase price: $2000.";
				_sellingPrice = 2000;
				return msgText;

			case 35: //targhee range
			case 36:
			case 37:
				msgText = "Selling a Targhee Range OTB will only give you 10% of its cows purchase price: $1500.";
				_sellingPrice = 2000;
				return msgText;

			case 38: //oxford range
			case 39:
			case 40:
				msgText = "Selling a Oxford Range OTB will only give you 10% of its cows purchase price: $1000.";
				_sellingPrice = 1000;
				return msgText;

			case 41: //spuds
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
				msgText = "Selling a 10 acres of Spuds OTB will give you 10% of its purchase price: $2000.";
				_sellingPrice = 2000;
				return msgText;
		}
		return "Error!";
	}

	bool CheckIfAbleToBuyOption()
	{
		if ((_pMove._currentSpace <= 14) && (_pManager._pCash >= _minDownPayment) && (_pManager._pNotes <= 50000 - (_otbCost - _minDownPayment) && (_pManager._isMyTurn)))
		{
			_stopBuying = false;
			return true;
		}
		else
		{
			_isWarningMsg = true;
			_otbMessageText.text = "You can't buy that because of financial reasons, or it's not your turn!";
			_otbOkButton.gameObject.SetActive(true);

			return false;
		}
	}

	bool CheckIfRangeIsAlreadyOwned()
	{
		object owned;

		switch (_selectedCard.cardNumber)
		{
			case 29: //lemhi
			case 30:
			case 31:
				foreach (Player player in GameManager.Instance._cachedPlayerList)
				{
					if (player.CustomProperties.TryGetValue(IFG.Lemhi_Range_Owned,out owned))
					{
						if ((bool)owned)
						{
							_otbMessageText.text = "Lemhi Range is already owned!";
							_isWarningMsg = true;
							_otbOkButton.gameObject.SetActive(true);
							_stopBuying = true;
						}

						return (bool)owned;
					}
				}
				break;

			case 32: //lost river
			case 33:
			case 34:
				foreach (Player player in GameManager.Instance._cachedPlayerList)
				{
					if (player.CustomProperties.TryGetValue(IFG.LostRiver_Range_Owned, out owned))
					{
						if ((bool)owned)
						{
							_otbMessageText.text = "Lost River Range is already owned!";
							_isWarningMsg = true;
							_otbOkButton.gameObject.SetActive(true);
							_stopBuying = true;
						}

						return (bool)owned;
					}
				}
				break;

			case 35: //targhee
			case 36:
			case 37:
				foreach (Player player in GameManager.Instance._cachedPlayerList)
				{
					if (player.CustomProperties.TryGetValue(IFG.Targhee_Range_Owned, out owned))
					{
						if ((bool)owned)
						{
							_otbMessageText.text = "Targhee Range is already owned!";
							_isWarningMsg = true;
							_otbOkButton.gameObject.SetActive(true);
							_stopBuying = true;
						}

						return (bool)owned;
					}
				}
				break;

			case 38: //oxford
			case 39:
			case 40:
				foreach (Player player in GameManager.Instance._cachedPlayerList)
				{
					if (player.CustomProperties.TryGetValue(IFG.Oxford_Range_Owned, out owned))
					{
						if ((bool)owned)
						{
							_otbMessageText.text = "Oxford Range is already owned!";
							_isWarningMsg = true;
							_otbOkButton.gameObject.SetActive(true);
							_stopBuying = true;
						}

						return (bool)owned;
					}
				}
				break;
		}
		return true;
	}

	bool CheckIfMaxFarmCowsReached()
	{
		bool maxReached = true;

		if (_pManager._pFarmCows <= 10)
			maxReached = false;
		else
		{
			_otbMessageText.text = "You already have your 20 Farm Cows!";
			maxReached = true;
		}

		return maxReached;
	}

	void UseOtb(OTBCard cardToUse)
	{
		if (_dManager == null)
			_dManager = GameManager.Instance.dManager;

		//remove the card from your deck
		_pManager._myOtbs.Remove(cardToUse);
		_pManager._myOtbCount = _pManager._myOtbs.Count;
		//_pManager.UpdateMyOtbCount(_pManager._myOtbCount);
		//_rpUpdater.UpdateRemotePlayerData();
		UpdateUI();
		_pManager.UpdateMyCash(-_downPayment);
		_pManager.UpdateMyNotes(_otbCost - _downPayment);
		_dManager.UseOtbCard(cardToUse.cardNumber);
		_pManager.DiscardOtbCard(cardToUse);
		ResetOtbListPanel();
	}

	IEnumerator ShowMessageRoutine(string typeOfMessage, string msg, Color fontColor)
	{
		switch (typeOfMessage)
		{
			case "Bonus":
				_bonusMessageText.text = msg;
				_bonusMessageText.color = fontColor;
				_bonusMessageText.gameObject.SetActive(true);

				yield return new WaitForSeconds(5f);

				_bonusMessageText.gameObject.SetActive(false);
				break;
		}
	}
	#endregion

	#region Photon Event Handlers

	void OnSpudBonusEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Spud_Message_Event_Code)
		{
			//extract data...IFG.SpudBonusGivenS,fontColor,Message
			object[] recData = (object[])eventData.CustomData;
			IFG.SpudBonusGiven = (bool)recData[0];
			Vector3 fontColor = (Vector3)recData[1];
			string msg = (string)recData[2];
			Color combinedColor = new Color(fontColor.x, fontColor.y, fontColor.z);
			_bonusMessageText.text = msg;
			_bonusMessageText.color = combinedColor;
			StartCoroutine(ShowMessageRoutine("Bonus",msg,combinedColor));
		}
	}

	void OnShuffleADeckEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Shuffle_Deck_Event_Code)
		{
			//extract the data...
			object[] recData = (object[])eventData.CustomData;
			string deck = (string)recData[0];
			int counter = (int)recData[1];

			switch (deck)
			{
				case "OTB":
					_otbShuffleCount = counter;
					break;

				case "OE":
					_oeShuffleCount = counter;
					break;

				case "FF":
					_ffShuffleCount = counter;
					break;

				default:
					Debug.Log("No such deck " + deck);
					break;
			}
			UpdateUI();
		}
	}

	void OnUpdateDeckDataEventReceived(EventData eventData)
	{
		Debug.Log("IN UPDATE DECK DATA");
		if (eventData.Code == (byte)RaiseEventCodes.Update_Deck_Data_Event_Code)
		{
			//extract data..."deck",total,left
			object[] recData = (object[])eventData.CustomData;
			string deck = (string)recData[0];
			int totalCards = (int)recData[1];
			int cardsLeft = (int)recData[2];

			switch (deck)
			{
				case "OTB":
					_otbTotalCards = totalCards;
					_otbCardsLeft = cardsLeft;
					break;

				case "OE":
					_oeTotalCards = totalCards;
					_oeCardsLeft = cardsLeft;
					break;

				case "FF":
					_ffTotalCards = totalCards;
					_ffCardsLeft = cardsLeft;
					break;
			}

			UpdateUI();
		}
	}
	#endregion
}
