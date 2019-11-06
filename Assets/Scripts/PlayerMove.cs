using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
	#region Public / Serialized Fields

	[SerializeField] float _normalMoveSpeed = 0.25f;
	[SerializeField] float _directedMoveSpeed = 0.2f;

	public int _currentSpace;
	public Vector3 _currentPosition;
	public int _currentYear;

	#endregion

	#region Private Fields / References
	//player hard set positions
	//starting space (Christmas Vacation)
	Vector3 _ronPosition0 = new Vector3(-21.9f, 28.4f);
	Vector3 _kayPosition0 = new Vector3(-21.9f, 26.4f);
	Vector3 _jerPosition0 = new Vector3(-21.9f, 24.4f);
	Vector3 _ricPosition0 = new Vector3(-23.6f, 28.4f);
	Vector3 _mikPosition0 = new Vector3(-23.6f, 26.4f);
	Vector3 _becPosition0 = new Vector3(-23.6f, 24.4f);
	//space 01
	Vector3 _ronPosition1 = new Vector3(-16.9f, 28.4f);
	Vector3 _kayPosition1 = new Vector3(-16.9f, 26.4f);
	Vector3 _jerPosition1 = new Vector3(-16.9f, 24.4f);
	Vector3 _ricPosition1 = new Vector3(-18.6f, 28.4f);
	Vector3 _mikPosition1 = new Vector3(-18.6f, 26.4f);
	Vector3 _becPosition1 = new Vector3(-18.6f, 24.4f);
	//space 14 (Spring Planting)
	Vector3 _ronPosition14 = new Vector3(31.9f, 25.2f);
	Vector3 _kayPosition14 = new Vector3(29.9f, 25.2f);
	Vector3 _jerPosition14 = new Vector3(28.0f, 25.2f);
	Vector3 _ricPosition14 = new Vector3(31.9f, 27.4f);
	Vector3 _mikPosition14 = new Vector3(29.9f, 27.4f);
	Vector3 _becPosition14 = new Vector3(28.0f, 27.4f);
	//space 15
	Vector3 _ronPosition15 = new Vector3(31.9f, 19.7f);
	Vector3 _kayPosition15 = new Vector3(29.9f, 19.7f);
	Vector3 _jerPosition15 = new Vector3(28.0f, 19.7f);
	Vector3 _ricPosition15 = new Vector3(31.9f, 21.9f);
	Vector3 _mikPosition15 = new Vector3(29.9f, 21.9f);
	Vector3 _becPosition15 = new Vector3(28.0f, 21.9f);
	//space 25 (Independence Day)
	Vector3 _ronPosition25 = new Vector3(29.0f, -28.4f);
	Vector3 _kayPosition25 = new Vector3(29.0f, -26.4f);
	Vector3 _jerPosition25 = new Vector3(29.0f, -24.4f);
	Vector3 _ricPosition25 = new Vector3(31.0f, -28.4f);
	Vector3 _mikPosition25 = new Vector3(31.0f, -26.4f);
	Vector3 _becPosition25 = new Vector3(31.0f, -24.4f);
	//space 26
	Vector3 _ronPosition26 = new Vector3(23.4f, -28.4f);
	Vector3 _kayPosition26 = new Vector3(23.4f, -26.4f);
	Vector3 _jerPosition26 = new Vector3(23.4f, -24.4f);
	Vector3 _ricPosition26 = new Vector3(25.4f, -28.4f);
	Vector3 _mikPosition26 = new Vector3(25.4f, -26.4f);
	Vector3 _becPosition26 = new Vector3(25.4f, -24.4f);
	//space 36 (Harvest Moon)
	Vector3 _ronPosition36 = new Vector3(-24.7f, -25.5f);
	Vector3 _kayPosition36 = new Vector3(-22.8f, -25.5f);
	Vector3 _jerPosition36 = new Vector3(-21.0f, -25.5f);
	Vector3 _ricPosition36 = new Vector3(-24.7f, -27.3f);
	Vector3 _mikPosition36 = new Vector3(-22.8f, -27.3f);
	Vector3 _becPosition36 = new Vector3(-21.0f, -27.3f);
	//space 37
	Vector3 _ronPosition37 = new Vector3(-24.7f, -20.4f);
	Vector3 _kayPosition37 = new Vector3(-22.8f, -20.4f);
	Vector3 _jerPosition37 = new Vector3(-21.0f, -20.4f);
	Vector3 _ricPosition37 = new Vector3(-24.7f, -22.2f);
	Vector3 _mikPosition37 = new Vector3(-22.8f, -22.2f);
	Vector3 _becPosition37 = new Vector3(-21.0f, -22.2f);

	int _die;
	float _moveSpeed;
	int _cowCounter;
	int _spudCounter;

	MyDiceRoll _diceRoll;
	UIManager _uiManager;
	BoardManager _bManager;
	PlayerManager _pManager;
	StickerManager _sManager;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_currentPosition = transform.position;
		_uiManager = GameManager.Instance.uiManager;
		_diceRoll = GameManager.Instance.uiManager._diceRoll;
		_bManager = GameManager.Instance.bManager;
		_pManager = GetComponent<PlayerManager>();
		_sManager = GameManager.Instance.sManager;
		_currentYear = 1;
	}
	#endregion

	#region Public Methods

	//called from the RollButton
	public void InitMove(int die)
	{
		_die = die;
		//_die = 20;	//TESTING

//#if UNITY_EDITOR	//TODO: UNCOMMENT THIS WHEN TESTING COMPLETE

		LoadedDie();  

//#endif

		_moveSpeed = _normalMoveSpeed;

		_currentPosition = transform.position;
		StartCoroutine(MoveRoutine(_die));
	}

	public void DirectedForwardMove(int space)
	{
		_uiManager._endTurnButton.interactable = false;
		_moveSpeed = _directedMoveSpeed;
		//Debug.Log("Directed Move: " + (space - _currentSpace));
		StartCoroutine(MoveRoutine(space - _currentSpace));
	}

	//Hurt your back move
	public void GoBackMove(int space)
	{
		_uiManager._endTurnButton.interactable = false;

		switch (GameManager.Instance.myFarmerName)
		{
			case "Rigby Ron":
				_currentPosition = _ronPosition1;
				break;

			case "Blackfoot Becky":
				_currentPosition = _becPosition1;
				break;

			case "Menan Mike":
				_currentPosition = _mikPosition1;
				break;

			case "Kimberly Kay":
				_currentPosition = _kayPosition1;
				break;

			case "Ririe Ric":
				_currentPosition = _ricPosition1;
				break;

			case "Jerome Jerry":
				_currentPosition = _jerPosition1;
				break;
		}
		Vector2 newPos = new Vector3(_currentPosition.x + 3.56f, _currentPosition.y);
		_currentPosition = newPos;

		transform.position = _currentPosition;
		_currentSpace = space;
		EndOfMove(_currentSpace);
	}
	#endregion

	#region Private Methods

	int GetDieRoll()
	{
		int die = Random.Range(1, 7);

		return die;
	}

	void LoadedDie()
	{
		if (Input.GetKey(KeyCode.Alpha1))
		{
			_die = 1;
		}
		if (Input.GetKey(KeyCode.Alpha2))
		{
			_die = 2;
		}
		if (Input.GetKey(KeyCode.Alpha3))
		{
			_die = 3;
		}
		if (Input.GetKey(KeyCode.Alpha4))
		{
			_die = 4;
		}
		if (Input.GetKey(KeyCode.Alpha5))
		{
			_die = 5;
		}
		if (Input.GetKey(KeyCode.Alpha6))
		{
			_die = 6;
		}
	}

	IEnumerator MoveRoutine(int die)
	{
		int startSpace = _currentSpace;
		int endSpace = startSpace + die;

		while (startSpace < endSpace)
		{
			startSpace++;
			_currentSpace++;

			//_aManager.PlayClip(_aManager._move);
			//CmdPlayRemoteSOund();
			//Space west of Christmas Vacation.
			if (_currentSpace == 1)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition = _ronPosition1;
						break;

					case "Blackfoot Becky":
						_currentPosition = _becPosition1;
						break;

					case "Menan Mike":
						_currentPosition = _mikPosition1;
						break;

					case "Kimberly Kay":
						_currentPosition = _kayPosition1;
						break;

					case "Ririe Ric":
						_currentPosition = _ricPosition1;
						break;

					case "Jerome Jerry":
						_currentPosition = _jerPosition1;
						break;

					default:
						Debug.LogWarning("Invalid Farmer: " + GameManager.Instance.myFarmerName);
						break;
				}

				transform.position = _currentPosition;
			}

			//Going across the north board path.
			if (_currentSpace >= 2 && _currentSpace < 14)
			{
				transform.Translate(3.56f, 0, 0);
				_currentPosition= transform.position;
			}
			//Spring Planting.
			if (_currentSpace == 14)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition14;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition14;
						break;

					case "Menan Mike":
						_currentPosition= _mikPosition14;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition14;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition14;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition14;
						break;
				}

				transform.position = _currentPosition;

			}

			//1st space south of Spring Planting.
			if (_currentSpace == 15)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition15;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition15;
						break;

					case "Menan Mike":
						_currentPosition= _mikPosition15;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition15;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition15;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition15;
						break;
				}

				transform.position = _currentPosition;

			}
			//Going down the east board path.
			if (_currentSpace >= 16 && _currentSpace < 25)
			{
				transform.Translate(0, -4.6f, 0);
				_currentPosition= transform.position;
			}
			//Independence Day Bash.
			if (_currentSpace == 25)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition25;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition25;
						break;

					case "Menan Mike":
						_currentPosition= _mikPosition25;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition25;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition25;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition25;
						break;
				}

				transform.position = _currentPosition;

			}
			//1st space west of Independence Day Bash.
			if (_currentSpace == 26)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition26;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition26;
						break;

					case "Menan Mike":
						_currentPosition= _mikPosition26;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition26;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition26;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition26;
						break;
				}

				transform.position = _currentPosition;
				//if (_hManager._cherriesCutInHalf)
				//{
				//	//Reset this after player past cherries.
				//	_hManager.ChangeMiscBooleans("cherriesCutInHalf", false);
				//}
			}
			//Going across the south board path.
			if (_currentSpace >= 27 && _currentSpace < 36)
			{
				transform.Translate(-4.6f, 0, 0);
				_currentPosition= transform.position;

				//if (_currentSpace == 34)
				//{
				//	_hManager.ChangeMiscBooleans("wheatCutInHalf", false);
				//}
			}
			//Harvest Moon
			if (_currentSpace == 36)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition36;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition36;
						break;

					case "Menan Mike":
						_currentPosition= _mikPosition36;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition36;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition36;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition36;
						break;
				}

				transform.position = _currentPosition;

			}
			//1st space north of Harvest Moon.
			if (_currentSpace == 37)
			{
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition37;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition37;
						break;

					case "Menan Mike":
						_currentPosition= _mikPosition37;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition37;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition37;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition37;
						break;
				}

				transform.position = _currentPosition;

			}
			//Going up the west board path.
			if (_currentSpace >= 38 && _currentSpace < 50)
			{
				transform.Translate(0, 3.56f, 0);
				_currentPosition= transform.position;
			}

			if (_currentSpace == 50)
			{
				_currentSpace = 0;

				AudioManager.Instance.PlaySound(AudioManager.Instance._passGo);

				//Set the player to his Christmas Vacation spot.
				switch (GameManager.Instance.myFarmerName)
				{
					case "Rigby Ron":
						_currentPosition= _ronPosition0;
						break;

					case "Blackfoot Becky":
						_currentPosition= _becPosition0;
						break;


					case "Menan Mike":
						_currentPosition= _mikPosition0;
						break;

					case "Kimberly Kay":
						_currentPosition= _kayPosition0;
						break;

					case "Ririe Ric":
						_currentPosition= _ricPosition0;
						break;

					case "Jerome Jerry":
						_currentPosition= _jerPosition0;
						break;
				}

				transform.position = _currentPosition;

				if (!_pManager._pNoWages)
				{
					_pManager.UpdateMyCash(5000);
					//_aManager.PlayClip(_aManager._passGo);
				}

				_currentYear++;
				_uiManager.UpdateUI();
				ResetPlayer();
			}

			yield return new WaitForSeconds(_moveSpeed);

			//Debug.Log(_moveSpeed);

		}
		//Move has ended...
		_moveSpeed = _normalMoveSpeed;   //Reset moveSpeed back to normal.

		EndOfMove(_currentSpace);
	}

	void EndOfMove(int space)
	{
		//Debug.Log("End of Move");

		_uiManager._rollButton.GetComponentInChildren<Text>().text = "ROLL";

		_die = 0;

		if (_pManager._pNoWages)
		{
			_pManager._pNoWages = false;
		}

		_bManager.ShowSpace(_currentSpace);

		_uiManager._endTurnButton.interactable = true;
	}

	void ResetPlayer()
	{
		ResetHarvestModifiers();
		ResetStickers();
		ResetHarvests();

		_uiManager.UpdateUI();
	}

	void ResetHarvestModifiers()
	{
		_pManager._pHayDoubled = false;
		_pManager._pHayDoubledCounter = 0;
		_pManager._pCornDoubled = false;
		_pManager._pWagesGarnished = false;
		_pManager._pWheatCutInHalf = false;
		_pManager._pCherriesCutInHalf = false;

		if (_pManager._pSpudsDoubled)
		{
			_spudCounter++;
			if (_spudCounter == 2)
			{
				_pManager._pSpudsDoubled = false;
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Spuds", _pManager._pSpuds, _pManager._pSpudsDoubled);
				_spudCounter = 0;
			}
		}

		if (_pManager._pCowsIncreased)
		{
			_cowCounter++;
			if (_cowCounter == 2)
			{
				_pManager._pCowsIncreased = false;
				_cowCounter = 0;
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Cow", _pManager._pFarmCows + _pManager._pRangeCows, _pManager._pCowsIncreased);
				GameManager.Instance.dManager.UpdateOwnedRangeStickers();
			}
		}
	}

	void ResetStickers()
	{
		_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Hay", _pManager._pHay, false);
		_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Grain", _pManager._pGrain, false);
	}

	void ResetHarvests()
	{
		_pManager._firstHay = false;
		_pManager._cherries = false;
		_pManager._secondHay = false;
		_pManager._wheat = false;
		_pManager._thirdHay = false;
		_pManager._livestock = false;
		_pManager._fourthHay = false;
		_pManager._spuds = false;
		_pManager._apples = false;
		_pManager._corn = false;
	}
	#endregion
}

