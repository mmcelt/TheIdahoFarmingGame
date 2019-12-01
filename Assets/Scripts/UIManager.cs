using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using System.IO;
using DG.Tweening;

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
	[SerializeField] Text _gameTyeText, _gameEndText;
	[SerializeField] Text[] _remotePlayerNameTexts;
	public Text[] _remotePlayerNetworthTexts;
	public Text _activePlayerText;
	[SerializeField] Image _otbImage, _oeImage, _ffImage;
	[SerializeField] Text _otbShuffleText, _oeShuffleText, _ffShuffleText;
	[SerializeField] Text _otbLeftText, _oeLeftText, _ffLeftText;
	[SerializeField] Text _otbTotalText, _oeTotalText, _ffTotalText;

	[Header("Actions Panel")]
	public GameObject _actionsPanel;
	public Text _cashText, _notesText, _otbMessageText;
	public Button _sellOtbToBankButton, _sellOtbToPlayerButton, _buyOptionButton, _payInFullButton, _cancelButton, _otbOkButton;
	public Button _repayLoanButton, _getLoanButton;
	public Dropdown _otbDropdown;
	public InputField _downPaymentInput;
	public InputField _getLoanInput;
	public InputField _repayLoanInput;

	[Header("Options Panel")]
	[SerializeField] GameObject _optionsPanel;
	[SerializeField] GameObject _resetWinnersListButton;
	[SerializeField] GameObject _makeTheMasterListButton;
	[SerializeField] Text _secondChanceWarningText;

	[Header("Sell OTB to Player Panel")]
	[SerializeField] GameObject _sellOtbToPlayerPanel;
	[SerializeField] Text _sellOtbToPlayerMessageText;
	public Button _cancelSaleButton, _sellTheOtbToPlayerButton;
	[SerializeField] Dropdown _playerSelectionDropdown;
	[SerializeField] Sprite[] _farmerSprites;
	[SerializeField] Sprite _headerSprite;
	[SerializeField] InputField _salePriceInput;

	[Header("Board Space Panel")]
	public GameObject _boardSpacePanel;
	public Text _headerText;
	public TextMeshProUGUI _spaceText;
	public Button _okButton;

	[Header("Otb Card Panel")]
	public GameObject _otbPanel;
	public Text _otbCardText, _otbTotalCostText;
	//public Button _otbPanelOkButton;

	[Header("Ff Card Panel")]
	public GameObject _ffPanel;
	public Text _ffCardText;
	public Image _ffCardBackground;
	//public Button _ffPanelOkButton;

	[Header("Oe Card Panel")]
	public GameObject _oePanel;
	public Text _oeCardText;
	public Image _oeCardBackground;
	//public Button _oePanelOkButton;

	[Header("Teton Dam Panel")]
	public GameObject _tetonDamPanel;
	public Image _tetonDamImage;
	public TextMeshProUGUI _tetonHeaderText, _tetonMessageText;
	public Button _tetonRollButton, _tetonOkButton;

	[Header("Custom Hire Harvester Panel")]
	public GameObject _customHarvesterPanel;
	public Button _customHarvesterOkButton;

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
	public Text _routineMessageText;
	public Text _shuffleMessageText;
	public Text _gameOverMessageText;

	[Header("Modal Panels")]
	public GameObject _modalPanel;
	public GameObject _completeModalPanel;
	public GameObject _optionsWarningPanelModalPanel;
	public GameObject _eogModalPanel;
	public GameObject _forcedLoanModalPanel;

	[Header("Winners List")]
	public Button _optionsButton;
	public GameObject _winnersListPanel;

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
	public int _sellingPrice;
	public int _minSalePrice;
	public int _salePrice;
	[HideInInspector] public OTBCard _selectedCard;
	public bool _warningGiven;
	public bool _okToProceed;

	#endregion

	#region Private Fields / References

	[HideInInspector] public MyDiceRoll _diceRoll;
	PlayerSetup _pSetup;
	PlayerManager _pManager;
	PlayerMove _pMove;
	DeckManager _dManager;
	RemotePlayerUpdater _rpUpdater;
	HarvestManager _hManager;
	BoardManager _bManager;

	//OTB Stuff
	List<string> _otbCards = new List<string>();       //for the myOTB Dropdown
	public List<Dropdown.OptionData> _otherPlayers = new List<Dropdown.OptionData>();   //for the playerSelection Dropdown

	int _selectedIndex;
	int _selectedPlayer;

	bool _isSellMsg;
	bool _isWarningMsg;
	bool _cancelOtbSale;
	bool _isSelling;

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
		PhotonNetwork.NetworkingClient.EventReceived += OnSpudBonusMessageEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnShuffleADeckEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnUpdateDeckDataEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnCompleteFarmerBonusMessageEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnHarvestRollMessaEventgeReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnEndOfNetworthGameEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnOutOfOtbCardsMessageEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived += OnEndOfTimedGameEventReceived;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnSpudBonusMessageEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnShuffleADeckEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnUpdateDeckDataEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnCompleteFarmerBonusMessageEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnHarvestRollMessaEventgeReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnEndOfNetworthGameEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnOutOfOtbCardsMessageEventReceived;
		PhotonNetwork.NetworkingClient.EventReceived -= OnEndOfTimedGameEventReceived;
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

		if (PhotonNetwork.IsMasterClient)
		{
			_resetWinnersListButton.SetActive(true);
			_makeTheMasterListButton.SetActive(true);
		}
		else
		{
			_resetWinnersListButton.SetActive(false);
			_makeTheMasterListButton.SetActive(false);
		}

		InitialPlayerInfoUpdate();
		StartCoroutine(UpdateUIRoutine());

		if (GameManager.Instance._gameMode == "Networth Game")
		{
			_gameEndText.text = GameManager.Instance._networthGameAmount.ToString("c0");
		}
		else
		{
			_gameEndText.text = GameManager.Instance._timedGameLength.ToString();
		}
		_gameTyeText.text = GameManager.Instance._gameMode;

		//StartCoroutine("UpdateUIRoutine");

	}

	//void Update()
	//{
	//	if (Input.GetMouseButtonDown(0))
	//	{
	//		Debug.Log("MOUSE: " + Input.mousePosition);
	//		Debug.Log("STVP " + Camera.main.ScreenToViewportPoint(Input.mousePosition));
	//	}
	//}

	#endregion

	#region UI Callback Methods

	//left panel methods
	public void OnRollButtonClicked()
	{
		_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
		ResetOtbListPanel();
		//_actionsPanel.SetActive(false);
		_diceRoll.OnRollButton();
		_rollButton.interactable = false;
	}

	public void OnEndTurnButtonClicked()
	{
		_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
		ResetOtbListPanel();
		//_actionsPanel.SetActive(false);
		_pManager.EndTurn();
	}

	public void OnActionsButtonClicked()
	{
		//toggle Actions Panel
		//bool actionsPanelActive = _actionsPanel.activeSelf;
		if (!_actionsPanel.activeSelf)
		{
			_actionsPanel.SetActive(true);
			_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayForward();
			InitializeTheActionsPanel();
		}

		else if (_actionsPanel.activeSelf)
		{
			_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
			ResetOtbListPanel();
		}
	}

	//actions panel methods
	public void OnCloseButtonClicked()
	{
		ResetOtbListPanel();
		_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
		ResetOtbListPanel();
		//_actionsPanel.SetActive(false);
	}

	public void OnBuyOptionButtonClicked()
	{
		if (!_transactionBlocked)
		{
			_buyOptionButton.interactable = false;
			UseOtb(_selectedCard);
			_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
			ResetOtbListPanel();
			//_actionsPanel.SetActive(false);
		}
		else
		{
			_isWarningMsg = true;
			_otbMessageText.text = "Insufficient Funds!";
		}
	}

	public void OnSellOtbToBankButtonClicked()
	{
		if (_pManager._isMyTurn)
		{
			_isSellMsg = true;
			_isSelling = true;
			_otbMessageText.text = "";
			StopCoroutine("BuyOptionRoutine");
			StartCoroutine("SellOtbRoutine");
		}
		else
		{
			_isWarningMsg = true;
			_isSelling = false;
			_otbMessageText.text = "You can only sell an OTB card on your turn...";
			_otbOkButton.gameObject.SetActive(true);
		}
	}

	public void OnSellOtbToPlayerButtonClicked()
	{
		if (_pManager._isMyTurn)
		{
			StartCoroutine("SellOtbToPlayerRoutine");
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
		_otbMessageText.text = "";
		_stopBuying = false;
		StopCoroutine("BuyOptionRoutine");
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
				_sellOtbToBankButton.gameObject.SetActive(true);
				_sellOtbToBankButton.interactable = true;
				if (GameManager.Instance._cachedPlayerList.Count > 1)
				{
					_sellOtbToPlayerButton.gameObject.SetActive(true);
					_sellOtbToPlayerButton.interactable = true;
				}
			}
			else
			{
				_sellOtbToBankButton.gameObject.SetActive(false);
				_sellOtbToBankButton.interactable = false;
				_sellOtbToPlayerButton.gameObject.SetActive(false);
				_sellOtbToPlayerButton.interactable = false;
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
			_sellOtbToBankButton.interactable = false;
			_sellOtbToPlayerButton.interactable = false;
		}
	}

	public void OnPlayerSelectionDropdownValueChanged(int index)
	{
		_selectedIndex = index;

		for (int i = 1; i < _otherPlayers.Count; i++)
		{
			for (int p = 0; p < GameManager.Instance._cachedPlayerList.Count; p++)
			{
				if (GameManager.Instance._cachedPlayerList[p].NickName == _otherPlayers[i].text)
				{
					_selectedPlayer = GameManager.Instance._cachedPlayerList[p].ActorNumber;
				}
			}
		}

		Debug.Log("In Select Buying Player: " + _selectedPlayer);
		if (index > 0)
		{
			_salePriceInput.interactable = true;
		}
		else
		{
			_salePriceInput.interactable = false;
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
		_forcedLoanModalPanel.SetActive(false);
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

	public void OnCancelOtbSaleToPlayerButtonClicked()
	{
		_completeModalPanel.SetActive(false);
		_sellOtbToPlayerPanel.SetActive(false);
	}

	public void OnDoOtbSaleToPlayerButtonClicked()
	{
		_sellTheOtbToPlayerButton.interactable = false;
		Debug.Log("Sell the OTB to the Player...");
		//get the money for the sale
		_pManager.UpdateMyCash(_salePrice);
		//create an Event to the receiving player...
		//data - cardNum,cardDesc,cardSummary,cardTotalCost,salePrice
		object[] sndData = new object[] { _selectedCard.cardNumber, _selectedCard.description, _selectedCard.summary, _selectedCard.totalCost, _salePrice };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			TargetActors = new int[] { _selectedPlayer },
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Sell_Otb_To_Player_Event_Code, sndData, eventOptions, sendOptions);
		//remove the selectedCard from seller's hand...
		_pManager._myOtbs.Remove(_selectedCard);
		_pManager._myOtbCount = _pManager._myOtbs.Count;
		_pManager.UpdateMyUI();

		_completeModalPanel.SetActive(false);
		_sellOtbToPlayerPanel.SetActive(false);
	}
	//Right Panel
	public void OnOptionsButtonClicked()
	{
		_optionsPanel.SetActive(true);
		_optionsPanel.GetComponent<DOTweenAnimation>().DOPlayForward();
	}
	//Options Panel
	public void OnOptionsPanelCloseButtonClicked()
	{
		_optionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
	}

	public void OnMakeMasterListButtonCLicked()
	{
		if (!_warningGiven)
		{
			_optionsWarningPanelModalPanel.SetActive(true);
			_warningGiven = true;
			StartCoroutine(WarningPanelRoutine("MakeMasterList"));
		}
	}

	public void OnResetWinnersListButtonClicked()
	{
		if (!_warningGiven)
		{
			_optionsWarningPanelModalPanel.SetActive(true);
			_warningGiven = true;
			StartCoroutine(WarningPanelRoutine("ResetWinnersList"));
		}
	}

	public void OnQuitButtonClicked()
	{
		if (!_warningGiven)
		{
			_optionsWarningPanelModalPanel.SetActive(true);
			_warningGiven = true;
			StartCoroutine(WarningPanelRoutine("Quit"));
		}
	}

	void QuitGame()
	{
#if UNITY_EDITOR
		// Application.Quit() does not work in the editor so
		// UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit();
	}

	public void OnYesButtonClicked()
	{
		_warningGiven = false;
		_okToProceed = true;
		_optionsWarningPanelModalPanel.SetActive(false);
	}

	public void OnNoButtonClicked()
	{
		_warningGiven = false;
		_okToProceed = false;
		_optionsWarningPanelModalPanel.SetActive(false);
	}

	public void OnOtbPanelOkButtonClicked()
	{
		_pManager._isOkToCloseOtbPanel = true;
	}

	public void OnFfPanelOkButtonClicked()
	{
		_pManager._isOkToCloseFfPanel = true;
	}

	public void OnOePanelOkButtonClicked()
	{
		if (_hManager == null)
			_hManager = GameManager.Instance.hManager;

		_hManager._isOkToCloseOePanel = true;
	}

	public void OnBoardSpacePanelOkButtonClicked()
	{
		if (_bManager == null)
			_bManager = GameManager.Instance.bManager;

		_bManager._isOkToCloseBoardSpacePanel = true;
	}
	#endregion

	#region Public Methods

	public void UpdateUI()
	{
		if (_pManager == null)
			_pManager = GetComponentInParent<PlayerManager>();
		if (_pMove == null)
			_pMove = GetComponentInParent<PlayerMove>();

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
			case IFG.Ron:
				return Color.blue;

			case IFG.Kay:
				return Color.yellow;

			case IFG.Jerry:
				return IFG.Purple;

			case IFG.Ric:
				return Color.black;

			case IFG.Mike:
				return Color.red;

			case IFG.Becky:
				return Color.white;
		}
		return Color.black;
	}

	public Color SelectColorFromPlayerNumber(int player)
	{
		string farmer = (string)GameManager.Instance._cachedPlayerList[player].CustomProperties[IFG.Selected_Farmer];

		return SelectFontColorForFarmer(farmer);
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
		if (_selectedCard.cardNumber >= 17 && _selectedCard.cardNumber <= 19)
		{
			yield return new WaitUntil(() => !CheckIfEquipmentIsAlreadyOwned("Tractor"));
		}
		if (_selectedCard.cardNumber >= 20 && _selectedCard.cardNumber <= 22)
		{
			yield return new WaitUntil(() => !CheckIfEquipmentIsAlreadyOwned("Harvester"));
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

	public int GetSalePrice()
	{
		int salePrice = 0;

		switch (_selectedCard.cardNumber)
		{
			case 1:  //grain
			case 2:
			case 3:
			case 4:
			case 5:
				salePrice = 4000;
				return salePrice;

			case 6:  //livestock
			case 7:
			case 8:
			case 9:
			case 10:
			case 11:
				salePrice = 1000;
				return salePrice;

			case 12: //hay
			case 13:
			case 14:
			case 15:
			case 16:
				salePrice = 4000;
				return salePrice;

			case 17: //tractor
			case 18:
			case 19:
				salePrice = 2000;
				return salePrice;

			case 20: //harvester
			case 21:
			case 22:
				salePrice = 2000;
				return salePrice;

			case 23: //fruit
			case 24:
			case 25:
			case 26:
			case 27:
			case 28:
				salePrice = 5000;
				return salePrice;

			case 29: //lemhi range
			case 30:
			case 31:
				salePrice = 5000;
				return salePrice;

			case 32: //lost river range
			case 33:
			case 34:
				salePrice = 4000;
				return salePrice;

			case 35: //targhee range
			case 36:
			case 37:
				salePrice = 4000;
				return salePrice;

			case 38: //oxford range
			case 39:
			case 40:
				salePrice = 2000;
				return salePrice;

			case 41: //spuds
			case 42:
			case 43:
			case 44:
			case 45:
			case 46:
				salePrice = 4000;
				return salePrice;
		}
		return 0;
	}
	#endregion

	#region Private Methods

	//TESTING
	public IEnumerator UpdateUIRoutine()
	{
		for (int i = 0; i < 10; i++)
		{
			Debug.Log("IN UPDATE ROUTINE");
			UpdateUI();
			yield return new WaitForSeconds(0.5f);
		}
		//StartCoroutine(UpdateUIRoutine());
	}
	//END TESTING

	void InitialPlayerInfoUpdate()
	{
		_playerNameText.text = PhotonNetwork.LocalPlayer.NickName;
		_farmerNameText.text = GameManager.Instance.myFarmerName;
		_farmerNameText.color = SelectFontColorForFarmer(GameManager.Instance.myFarmerName);

		if (PhotonNetwork.PlayerList.Length > 1)
			Invoke("StartRemotePlayerUpdating", 0.5f);

		_currentYearText.text = _pMove._currentYear.ToString();
	}

	void StartRemotePlayerUpdating()
	{
		if (GameManager.Instance._numberOfPlayers > 1)
			StartCoroutine("UpdateRemotePlayerInfo");
	}

	IEnumerator UpdateRemotePlayerInfo()
	{
		//update the remote players info...
		for (int i = 0; i < 9; i++)
		{
			int index = 0;
			foreach (Player player in PhotonNetwork.PlayerList)
			{
				if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					_otherPlayerNameTexts[index].text = player.NickName;
					_remotePlayerNameTexts[index].text = player.NickName;
					object farmer;
					if (player.CustomProperties.TryGetValue(IFG.Selected_Farmer, out farmer))
					{
						_otherPlayerNameTexts[index].color = SelectFontColorForFarmer((string)farmer);
						_remotePlayerNameTexts[index].color = SelectFontColorForFarmer((string)farmer);
					}
					index++;
				}
			}

			yield return new WaitForSeconds(1.0f);
		}
		//if (_remotePlayerNameTexts[0].text != "")
		//	StopCoroutine("UpdateRemotePlayerInfo");

		//StartCoroutine("UpdateRemotePlayerInfo");
	}

	void InitializeTheActionsPanel()
	{
		ResetTempFunds();
		PopulateDropdown(_otbDropdown.name);
		_repayLoanInput.Select();
	}

	public void ResetTempFunds()
	{
		_tempCash = _pManager._pCash;
		_tempNotes = _pManager._pNotes;
		UpdateActionsPanelFunds(_tempCash, _tempNotes);
	}

	void PopulateDropdown(string target)
	{
		string headerEntry = "";

		if (target == _otbDropdown.name)
		{
			_otbCards.Clear();
			_otbDropdown.ClearOptions();

			headerEntry = "               My OTB Cards";
			_otbCards.Add(headerEntry); //this will be index 0

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
		if (target == _playerSelectionDropdown.name)
		{
			_otherPlayers.Clear();
			_playerSelectionDropdown.ClearOptions();

			headerEntry = "   Select the Receiving Player";
			Sprite headerSprite = _farmerSprites[0];
			var headerOption = new Dropdown.OptionData(headerEntry, headerSprite);
			_otherPlayers.Add(headerOption);

			foreach (Player player in GameManager.Instance._cachedPlayerList)
			{
				if (player.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					string thePlayer = player.NickName;
					string theFarmer = (string)player.CustomProperties[IFG.Selected_Farmer];
					Sprite theSprite = _farmerSprites[SelectSpriteForFarmer(theFarmer)];

					var option = new Dropdown.OptionData(thePlayer, theSprite);
					_otherPlayers.Add(option);
				}
			}
			_playerSelectionDropdown.AddOptions(_otherPlayers);
			_playerSelectionDropdown.value = 0;
			_playerSelectionDropdown.Select();
			_playerSelectionDropdown.RefreshShownValue();
			_selectedIndex = 0;
		}
	}

	void ResetOtbListPanel()
	{
		_selectedCard = null;
		_otbCost = 0;
		_minDownPayment = 0;
		_downPaymentInput.text = "";
		_downPaymentInput.placeholder.GetComponent<Text>().text = "Enter your Downpayment...";
		_sellOtbToBankButton.interactable = false;
		_buyOptionButton.interactable = false;
		_payInFullButton.interactable = false;
		_stopBuying = false;
		_otbOkButton.gameObject.SetActive(false);
		StopCoroutine("BuyOptionRoutine");
		ResetTempFunds();
		PopulateDropdown(_otbDropdown.name);
	}

	//called by the otbOk button
	IEnumerator SellOtbRoutine()
	{
		_otbMessageText.text = SelectSellMessageText();
		_otbOkButton.gameObject.SetActive(true);
		_cancelButton.gameObject.SetActive(true);

		yield return new WaitWhile(() => _otbMessageText.text != "");

		//Debug.Log("OUT OF SELL WAIT");
		_sellOtbToBankButton.gameObject.SetActive(false);
		if (!_cancelOtbSale)
		{
			_pManager.UpdateMyCash(_sellingPrice);
			_pManager.DiscardOtbCard(_selectedCard);
			ResetOtbListPanel();
			PopulateDropdown(_otbDropdown.name);
		}
		_cancelOtbSale = false;
		ResetOtbListPanel();
		_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
		//_actionsPanel.SetActive(false);
		_isSelling = false;
	}

	IEnumerator SellOtbToPlayerRoutine()
	{
		_completeModalPanel.SetActive(true);
		_sellOtbToPlayerPanel.SetActive(true);
		_sellOtbToPlayerMessageText.text = "You are selling " + _selectedCard.summary;
		_sellOtbToPlayerMessageText.text += "\n\nMinimum Sale price is: " + GetSalePrice().ToString("c0");
		PopulateDropdown(_playerSelectionDropdown.name);

		yield return new WaitWhile(() => _sellOtbToPlayerPanel.activeSelf);
		Debug.Log("SELLING OTB TO PLAYER ROUTINE...");
		_selectedPlayer = 0;
		_sellingPrice = 0;
		_salePrice = 0;
		_minSalePrice = 0;
		_sellTheOtbToPlayerButton.interactable = false;
		ResetOtbListPanel();
		_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
		//_actionsPanel.SetActive(false);
	}

	int SelectSpriteForFarmer(string farmer)
	{
		switch (farmer)
		{
			case "Blackfoot Becky":
				return 0;

			case "Jerome Jerry":
				return 1;

			case "Kimberly Kay":
				return 2;

			case "Menan Mike":
				return 3;

			case "Ririe Ric":
				return 4;

			case "Rigby Ron":
				return 5;
		}
		return 0;
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
		if (!_isSelling)
		{
			if (_pManager._isMyTurn)
			{
				if (_pMove._currentSpace <= 14)
				{
					if (_pManager._pCash >= _minDownPayment)
					{
						if (_pManager._pNotes <= 50000 - (_otbCost - _downPayment))
						{
							_stopBuying = false;
							return true;
						}
						else
						{
							if (_downPayment > _minDownPayment)
							{
								_isWarningMsg = true;
								_otbMessageText.text = "You can't buy that because you have to much debt!";
								_otbOkButton.gameObject.SetActive(true);
								//return false;
							}
							return false;
						}
					}
					else
					{
						_isWarningMsg = true;
						_otbMessageText.text = "You can't buy that because you don't have enough cash on hand!";
						_otbOkButton.gameObject.SetActive(true);
						return false;
					}
				}
				else
				{
					_isWarningMsg = true;
					_otbMessageText.text = "You can't buy that because you're past Spring Planting!";
					_otbOkButton.gameObject.SetActive(true);
					return false;
				}
			}
			else
			{
				_isWarningMsg = true;
				_otbMessageText.text = "You can't buy that because it's not your turn!";
				_otbOkButton.gameObject.SetActive(true);
				return false;
			}

		}
		else
		{
			return false;
		}
		//if (() && () && ( && (_pManager._isMyTurn)))
		//{
		//	_stopBuying = false;
		//	return true;
		//}
		//else
		//{
		//	_isWarningMsg = true;
		//	_otbMessageText.text = "You can't buy that because of financial reasons, or it's not your turn!";
		//	_otbOkButton.gameObject.SetActive(true);

		//	return false;
		//}
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
					if (player.CustomProperties.TryGetValue(IFG.Lemhi_Range_Owned, out owned))
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

		if (!_pManager._pHarvester)
			maxReached = false;
		else
		{

		}
		return maxReached;
	}

	bool CheckIfEquipmentIsAlreadyOwned(string equipment)
	{
		bool alreadyOwned = true;

		if (equipment == "Tractor")
		{
			if (_pManager._pTractor)
			{
				alreadyOwned = true;
				_otbMessageText.text = "You already have a Tractor!";
			}
			else
				alreadyOwned = false;
		}
		if (equipment == "Harvester")
		{
			if (_pManager._pHarvester)
			{
				alreadyOwned = true;
				_otbMessageText.text = "You already have a Harvester!";
			}
			else
				alreadyOwned = false;
		}
		return alreadyOwned;
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

	public IEnumerator ShowMessageRoutine(string typeOfMessage, string msg, Color fontColor, float duration = 3f)
	{
		switch (typeOfMessage)
		{
			case "Bonus":
				_bonusMessageText.text = msg;
				_bonusMessageText.color = fontColor;
				_bonusMessageText.gameObject.SetActive(true);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				break;

			case "Routine":
				_routineMessageText.text = msg;
				_routineMessageText.color = fontColor;
				_routineMessageText.gameObject.SetActive(true);
				break;

			case "Shuffle":
				_shuffleMessageText.text = msg;
				_shuffleMessageText.color = fontColor;
				_shuffleMessageText.gameObject.SetActive(true);
				AudioManager.Instance.PlaySound(AudioManager.Instance._shuffle);
				break;

			case "Game Over":
				_gameOverMessageText.text = msg;
				_gameOverMessageText.color = fontColor;
				_gameOverMessageText.gameObject.SetActive(true);
				break;
		}
		yield return new WaitForSeconds(duration);

		if (typeOfMessage == "Bonus")
		{
			_bonusMessageText.gameObject.SetActive(false);
			_bonusMessageText.text = "";
			_bonusMessageText.color = Color.black;
		}
		if (typeOfMessage == "Routine")
		{
			_routineMessageText.gameObject.SetActive(false);
			_routineMessageText.text = "";
			_routineMessageText.color = Color.black;
		}
		if (typeOfMessage == "Shuffle")
		{
			_shuffleMessageText.gameObject.SetActive(false);
			_shuffleMessageText.text = "";
			_shuffleMessageText.color = Color.black;
			yield return new WaitForSeconds(0.1f);
			_otbImage.GetComponent<DOTweenAnimation>().DORewind();
			_oeImage.GetComponent<DOTweenAnimation>().DORewind();
			_ffImage.GetComponent<DOTweenAnimation>().DORewind();
		}
		//if (typeOfMessage=="Game Over")
		//{
		//	_gameOverMessageText.gameObject.SetActive(false);
		//	_gameOverMessageText.text = "";
		//	_gameOverMessageText.color = Color.black;
		//}
	}

	//everybody does this...
	IEnumerator ShowGameOverMessageRoutine(string msg, Color fontColor, float duration, string winnerName, string farmerName, int networth, int gameEnd, int nop, string[] ruNames, string[] ruFarmers, int[] ruNetworths)
	{
		_gameOverMessageText.text = msg;
		_gameOverMessageText.color = fontColor;
		_gameOverMessageText.gameObject.SetActive(true);
		_eogModalPanel.SetActive(true);
		_rollButton.interactable = false;
		_endTurnButton.interactable = false;
		yield return new WaitForSeconds(duration);

		_gameOverMessageText.gameObject.SetActive(false);
		_gameOverMessageText.text = "";
		_gameOverMessageText.color = Color.black;
		_eogModalPanel.SetActive(false);

		WinnerList.Instance.PopulateAndShowWinnersList();
	}

	public IEnumerator WarningPanelRoutine(string button)
	{
		//_warningGiven = true;
		switch (button)
		{
			case "Quit":
				_secondChanceWarningText.text = "QUIT BUTTON PRESSED\n";
				break;

			case "ResetWinnersList":
				_secondChanceWarningText.text = "RESET WINNERS LIST BUTTON PRESSED\n";
				break;

			case "MakeMasterList":
				_secondChanceWarningText.text = "MAKE MASTER LIST BUTTON PRESSED\n";
				break;
		}
		_secondChanceWarningText.text += "Are you sure you want to do this?";

		yield return new WaitUntil(() => !_warningGiven);

		switch (button)
		{
			case "Quit":
				if (_okToProceed)
				{
					QuitGame();
				}
				break;

			case "ResetWinnersList":
				if (_okToProceed)
				{
					WinnerList.Instance.ResetWinnersList();
				}
				break;

			case "MakeMasterList":
				if (_okToProceed)
				{
					WinnerList.Instance.MakeTheMasterList();
				}
				break;
		}
		_secondChanceWarningText.text = "";
		_okToProceed = false;
	}
	#endregion

	#region Photon Event Handlers

	void OnSpudBonusMessageEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Spud_Message_Event_Code)
		{
			IFG.SpudBonusGiven = true;

			//extract data...IFG.SpudBonusGiven,fontColor,Message
			object[] recData = (object[])eventData.CustomData;
			Vector3 fontColor = (Vector3)recData[0];
			string msg = (string)recData[1];
			Color combinedColor = new Color(fontColor.x, fontColor.y, fontColor.z);
			_bonusMessageText.text = msg;
			_bonusMessageText.color = combinedColor;
			//display the message...
			StartCoroutine(ShowMessageRoutine("Bonus", msg, combinedColor));
		}
	}

	void OnCompleteFarmerBonusMessageEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Complete_Farmer_Message_Event_Code)
		{
			IFG.CompleteFarmerBonusGiven = true;

			//extract data
			object[] recData = (object[])eventData.CustomData;
			Vector3 fontColor = (Vector3)recData[0];
			string msg = (string)recData[1];
			Color combinedColor = new Color(fontColor.x, fontColor.y, fontColor.z);
			_bonusMessageText.text = msg;
			_bonusMessageText.color = combinedColor;
			//display the message...
			StartCoroutine(ShowMessageRoutine("Bonus", msg, combinedColor));
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

			Color deckColor = Color.white;

			switch (deck)
			{
				case "OTB":
					_otbShuffleCount = counter;
					deckColor = new Color(0.7843f, 0.9372f, 1f);
					if (counter>0)
						_otbImage.GetComponent<DOTweenAnimation>().DOPlayForward();
					break;

				case "OE":
					_oeShuffleCount = counter;
					deckColor = new Color(1f, 0.9725f, 0.5803f);
					if (counter>0)
						_oeImage.GetComponent<DOTweenAnimation>().DOPlayForward();
					break;

				case "FF":
					_ffShuffleCount = counter;
					deckColor = new Color(0.9568f, 0.6352f, 0.7333f);
					if (counter>0)
						_ffImage.GetComponent<DOTweenAnimation>().DOPlayForward();
					break;

				default:
					Debug.Log("No such deck " + deck);
					break;
			}
			UpdateUI();
			if (counter > 0)
			{
				string message = "Shuffled the " + deck + " Deck...";
				StartCoroutine(ShowMessageRoutine("Shuffle", message, deckColor));
			}
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

	void OnHarvestRollMessaEventgeReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Harvest_Roll_Message_Event_Code)
		{
			//extract data - roller's nickname,roller's farmerName, die amount, commodity
			object[] recData = (object[])eventData.CustomData;
			string rollersName = (string)recData[0];
			string rollersFarmer = (string)recData[1];
			int rollAmount = (int)recData[2];
			string commodity = (string)recData[3];

			Color fontColor = SelectFontColorForFarmer(rollersFarmer);
			string message = rollersName + " rolled a " + rollAmount + " on his/her " + commodity;
			StartCoroutine(ShowMessageRoutine("Routine", message, fontColor));
		}
	}

	void OnOutOfOtbCardsMessageEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Out_Of_Otbs_Event_Code)
		{
			//extract data - message
			object[] recData = (object[])eventData.CustomData;
			string message = (string)recData[0];
			Color fontColor = Color.red;
			StartCoroutine(ShowMessageRoutine("Routine", message, fontColor));
		}
	}

	//sent to everyone...
	void OnEndOfNetworthGameEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.End_Networth_Game_Event_Code)
		{
			int gameEnd = (int)PhotonNetwork.CurrentRoom.CustomProperties[IFG.Networth_Game];
			int nop = GameManager.Instance._numberOfPlayers;
			string[] ruNames = new string[nop];
			string[] ruFarmers = new string[nop];
			int[] ruNetworths = new int[nop];

			//extract the data - the winners: nickname,farmer,networth
			object[] recData = (object[])eventData.CustomData;
			string winnerName = (string)recData[0];
			string farmerName = (string)recData[1];
			int networth = (int)recData[2];

			string message = winnerName + " HAS WON THE GAME WITH A NETWORTH OF: " + networth.ToString("c0");
			Color fontColor = SelectFontColorForFarmer(farmerName);
			_rollButton.interactable = false;
			_endTurnButton.interactable = false;
			_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
			ResetOtbListPanel();
			//_actionsPanel.SetActive(false);

			if (nop > 1)
			{
				Debug.Log("GM CPL.COUNT: " + GameManager.Instance._cachedPlayerList.Count);

				for (int i=0; i<GameManager.Instance._cachedPlayerList.Count; i++)
				{
					if (GameManager.Instance._cachedPlayerList[i].NickName == winnerName)
					{
						continue;
					}

					ruNames[i] = GameManager.Instance._cachedPlayerList[i].NickName;
					ruFarmers[i] = (string)GameManager.Instance._cachedPlayerList[i].CustomProperties[IFG.Selected_Farmer];
					ruNetworths[i] = (int)GameManager.Instance._cachedPlayerList[i].CustomProperties[IFG.Player_Networth];

					Debug.Log("i: " + i);
					Debug.Log("Remote Player: " + ruNames[i]);
					Debug.Log("Remote Farmer: " + ruFarmers[i]);
					Debug.Log("Remote Networth: " + ruNetworths[i]);
				}
			}
			else
			{
				ruNames = null;
				ruFarmers = null;
				ruNetworths = null;
			}

			WinnerList.Instance.AddWinnerListEntry(winnerName, farmerName, networth, gameEnd, nop, ruNames, ruFarmers, ruNetworths);

			StartCoroutine(ShowGameOverMessageRoutine(message, fontColor, 5.0f, winnerName, farmerName, networth, gameEnd, nop, ruNames, ruFarmers, ruNetworths));
		}
	}
	//sent to all
	void OnEndOfTimedGameEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.End_Timed_Game_Event_Code)
		{
			float gameEnd = (float)PhotonNetwork.CurrentRoom.CustomProperties[IFG.Timed_Game];
			int nop = GameManager.Instance._numberOfPlayers;
			string[] ruNames = new string[nop];
			string[] ruFarmers = new string[nop];
			int[] ruNetworths = new int[nop];
			//extract the data - the winner: nickname,farmer,networth
			object[] recData = (object[])eventData.CustomData;
			string winnerName = (string)recData[0];
			string farmerName = (string)recData[1];
			int networth = (int)recData[2];

			string message = winnerName + " HAS WON THE TIMED GAME WITH A NETWORTH OF: " + networth.ToString("c0");
			Color fontColor = SelectFontColorForFarmer(farmerName);
			_rollButton.interactable = false;
			_endTurnButton.interactable = false;
			_actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
			ResetOtbListPanel();
			//_actionsPanel.SetActive(false);

			if (nop > 1)
			{
				Debug.Log("GM CPL.COUNT: " + GameManager.Instance._cachedPlayerList.Count);

				for (int i = 0; i < GameManager.Instance._cachedPlayerList.Count; i++)
				{
					if (GameManager.Instance._cachedPlayerList[i].NickName == winnerName)
					{
						continue;
					}

					ruNames[i] = GameManager.Instance._cachedPlayerList[i].NickName;
					ruFarmers[i] = (string)GameManager.Instance._cachedPlayerList[i].CustomProperties[IFG.Selected_Farmer];
					ruNetworths[i] = (int)GameManager.Instance._cachedPlayerList[i].CustomProperties[IFG.Player_Networth];

					Debug.Log("i: " + i);
					Debug.Log("Remote Player: " + ruNames[i]);
					Debug.Log("Remote Farmer: " + ruFarmers[i]);
					Debug.Log("Remote Networth: " + ruNetworths[i]);
				}
			}
			else
			{
				ruNames = null;
				ruFarmers = null;
				ruNetworths = null;
			}

			WinnerList.Instance.AddWinnerListEntry(winnerName, farmerName, networth, (int)gameEnd, nop, ruNames, ruFarmers, ruNetworths);

			StartCoroutine(ShowGameOverMessageRoutine(message, fontColor, 5.0f, winnerName, farmerName, networth, (int)gameEnd, nop, ruNames, ruFarmers, ruNetworths));
		}
	}
	#endregion

	#region TESTING METHODS

	public void DisplayDebugMessageFromTween()
	{
		Debug.Log("Rewind Complete");
	}
	#endregion
}
