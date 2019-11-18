using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
	#region Public / Serialized Fields

	[SerializeField] HarvestManager _hManager;
	[SerializeField] GameObject _fireworksPrefab;

	#endregion

	#region Private Fields / References

	UIManager _uiManager;
	PlayerManager _pManager;
	PlayerMove _pMove;
	StickerManager _sManager;

	GameObject _boardSpacePanel, _modalPanel;
	Text _headerText;
	TextMeshProUGUI _spaceText;
	Button _okButton;

	SpriteRenderer _gameboardRenderer;
	List<SpriteRenderer> _activeSprites;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_uiManager = GameManager.Instance.uiManager;
		_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();
		_pMove = GameManager.Instance.myFarmer.GetComponent<PlayerMove>();
		_sManager = GetComponent<StickerManager>();

		_boardSpacePanel = _uiManager._boardSpacePanel;
		_modalPanel = _uiManager._modalPanel;
		_headerText = _uiManager._headerText;
		_spaceText = _uiManager._spaceText;
		_okButton = _uiManager._okButton;   //TODO: make an OnClick() in code

		_gameboardRenderer = GetComponent<SpriteRenderer>();

		//Debug.Log("GM FARMER: " + GameManager.Instance.myFarmerName);
	}
	#endregion

	#region Public Methods

	public void ShowSpace(int space)
	{
		switch (space)
		{
			case 0:
				_headerText.text = "CHRISTMAS VACATION";
				_spaceText.text = "<color=green>COLLECT</color> $1000 Christmas Bonus.\nCollect years wage of $5000 as you pass.";

				break;

			case 1:
				_headerText.text = "1ST WEEK OF JANUARY";
				_spaceText.text = "<color=red>Pay 10%</color> on Bank Notes On Hand.";

				break;

			case 2:
				_headerText.text = "2ND WEEK OF JANUARY";
				_spaceText.text = "Hibernate.\nDraw O.T.B.";

				break;

			case 3:
				_headerText.text = "3RD WEEK OF JANUARY";
				_spaceText.text = "Bitter cold spell.\n<color=red>PAY $500</color>, if you own cows.";

				break;

			case 4:
				_headerText.text = "4TH WEEK OF JANUARY";
				_spaceText.text = "Beautiful Days!\nDouble all your Hay Harvests this year.";

				break;

			case 5:
				_headerText.text = "1ST WEEK OF FEBRUARY";
				_spaceText.text = "Warm snap, you're in the field two weeks early.\n<color=green>COLLECT $1000</color>.";

				break;

			case 6:
				_headerText.text = "2ND WEEK OF FEBRUARY";
				_spaceText.text = "Stuck in a muddy corral.\nDraw Farmer's Fate.";

				break;

			case 7:
				_headerText.text = "3RD WEEK OF FEBRUARY";
				_spaceText.text = "Ground thaws. Start planting early.\nGo directly to Spring Planting.";

				break;

			case 8:
				_headerText.text = "4TH WEEK OF FEBRUARY";
				_spaceText.text = "Rainy Day.  Draw O.T.B.";

				break;

			case 9:
				_headerText.text = "1ST WEEK OF MARCH";
				_spaceText.text = "Becomes obvious your wheat has winter killed.\n<color=red>PAY $2000</color> to replant.";

				break;

			case 10:
				_headerText.text = "2ND WEEK OF MARCH";
				_spaceText.text = "Start plowing late.  <color=red>PAY $500</color>.";

				break;

			case 11:
				_headerText.text = "3RD WEEK OF MARCH";
				_spaceText.text = "Hurt your back.\nGo back to second week of January.";

				break;

			case 12:
				_headerText.text = "4TH WEEK OF MARCH";
				_spaceText.text = "Frost forces you to heat fruit.\n<color=red>PAY $2000</color>, if you own fruit.";

				break;

			case 13:
				_headerText.text = "1ST WEEK OF APRIL";
				_spaceText.text = "Done plowing. Take a day off.\nDraw O.T.B.";

				break;

			case 14:
				_headerText.text = "SPRING PLANTING";
				_spaceText.text = "Plant corn on time.\nDouble corn yield this year.";

				break;

			case 15:
				_headerText.text = "3RD WEEK OF APRIL";
				_spaceText.text = "More rain.  Field work shutdown.\n<color=red>PAY $500</color>.";

				break;

			case 16:
				_headerText.text = "4TH WEEK OF APRIL";
				_spaceText.text = "Equipment breakdown.\n<color=red>PAY $1000</color>.";

				break;

			case 17:
				_headerText.text = "1ST WEEK OF MAY";
				_spaceText.text = "The whole valley is green!\n<color=green>COLLECT $500</color>.";

				break;

			case 18:
				_headerText.text = "2ND WEEK OF MAY";
				_spaceText.text = "Windstorm makes you replant your corn.\n<color=red>PAY $500</color>.";

				break;

			case 19:
				_headerText.text = "3RD WEEK OF MAY";
				_spaceText.text = "Cut your hay just right.\n<color=green>COLLECT a $1000 bonus</color>.";

				break;

			case 20:
				_headerText.text = "4TH WEEK OF MAY";
				_spaceText.text = "Memorial Day weekend.\nDraw O.T.B.";

				break;

			case 21:
				_headerText.text = "1ST WEEK OF JUNE";
				_spaceText.text = "Rain storm ruins your unbaled hay.\n<color=red>Cut your harvest check in half</color>.";

				break;

			case 22:
				_headerText.text = "2ND WEEK OF JUNE";
				_spaceText.text = "Good growing.\n<color=green>COLLECT a $500 bonus</color>.";

				break;

			case 23:
				_headerText.text = "3RD WEEK OF JUNE";
				_spaceText.text = "Rain splits your cherries.\n<color=red>Cut your harvest check in half</color>.";

				break;

			case 24:
				_headerText.text = "4TH WEEK OF JUNE";
				_spaceText.text = "Dust storm.  Draw Farmer's Fate.";

				break;

			case 25:
				_headerText.text = "INDEPENDENCE DAY BASH";
				_spaceText.text = "WHOOPIE!!";

				break;

			case 26:
				_headerText.text = "1ST WEEK OF JULY";
				_spaceText.text = "Good weather for your second cutting of hay.\n<color=green>Double Hay harvest check</color>.";

				break;

			case 27:
				_headerText.text = "2ND WEEK OF JULY";
				_spaceText.text = "Hot! Wish you were in the mountains.\nDraw O.T.B.";

				break;

			case 28:
				_headerText.text = "3RD WEEK OF JULY";
				_spaceText.text = "It's a cooker! 114 deg in the shade.\nWipe your brow and go to Harvest Moon,\nafter getting Hay check.";

				break;

			case 29:
				_headerText.text = "4TH WEEK OF JULY";
				_spaceText.text = "85 deg, wheat heads filling out beautifully.\n<color=green>Add $50 per acre</color> to your harvest check.";

				break;

			case 30:
				_headerText.text = "1ST WEEK OF AUGUST";
				_spaceText.text = "You're right on time and farming like a Pro.\nGo to the forth week of February.\n<color=green>COLLECT</color> your years wages of <color=green>$5000</color>.";

				break;

			case 31:
				_headerText.text = "2ND WEEK OF AUGUST";
				_spaceText.text = "Storm clouds brewing.\n<color=green>COLLECT $1000</color>, if you have a Harvester.";

				break;

			case 32:
				_headerText.text = "3RD WEEK OF AUGUST";
				_spaceText.text = "Finish wheat harvest with no breakdowns.\n<color=green>COLLECT $500</color>.";

				break;

			case 33:
				_headerText.text = "4TH WEEK OF AUGUST";
				_spaceText.text = "Rain sprouts unharvested wheat.\n<color=red>Cut price $50 per acre</color> on harvest check.";

				break;

			case 34:
				_headerText.text = "1ST WEEK OF SEPTEMBER";
				_spaceText.text = "Tractor owners: bale Hay, then go to fourth week of November.\nCOLLECT your $1000 there,\nthen harvest your Fruit.";

				break;

			case 35:
				_headerText.text = "2ND WEEK OF SEPTEMBER";
				_spaceText.text = "Sunny skies at the County Fair.\nDraw O.T.B.";

				break;

			case 36:
				_headerText.text = "HARVEST MOON";
				_spaceText.text = "Smiles on you. <color=green>COLLECT $500</color>.";

				break;

			case 37:
				_headerText.text = "3RD WEEK OF SEPTEMBER";
				_spaceText.text = "Market collapses.  <color=red>Cut livestock check in half</color>.";

				break;

			case 38:
				_headerText.text = "4TH WEEK OF SEPTEMBER";
				_spaceText.text = "Codling Moth damage to apples lowers fruit grade.\n<color=red>PAY $2000</color>, if you own fruit.";

				break;

			case 39:
				_headerText.text = "1ST WEEK OF OCTOBER";
				_spaceText.text = "Indian Summer.  <color=green>COLLECT $500</color>.";

				break;

			case 40:	//4TH HAY CUT
				_headerText.text = "2ND WEEK OF OCTOBER";
				_spaceText.text = "Lucked-out and got a 4th Hay Cut.  <color=green>COLLECT $500</color>.";
				break;

			case 41:	//START SPUD HARVEST
				_headerText.text = "3RD WEEK OF OCTOBER";
				_spaceText.text = "Good Pheasant Hunting.\nDraw Farmer's Fate.";

				break;

			case 42:
				_headerText.text = "4TH WEEK OF OCTOBER";
				_spaceText.text = "Park your baler for the winter.\nDraw O.T.B.";

				break;

			case 43:	//END OF SPUD HARVEST
				_headerText.text = "1ST WEEK OF NOVEMBER";
				_spaceText.text = "Annual Deer Hunt.  Draw Farmer's Fate.";

				break;

			case 44:	//START APPLE HARVEST
				_headerText.text = "2ND WEEK OF NOVEMBER";
				_spaceText.text = "Irrigation Season over.  Draw O.T.B.";

				break;

			case 45:
				_headerText.text = "3RD WEEK OF NOVEMBER";
				_spaceText.text = "Good weather, harvest winding up.\n<color=green>COLLECT $500</color>.";

				break;

			case 46:	//END APPLE HARVEST
				_headerText.text = "4TH WEEK OF NOVEMBER";
				_spaceText.text = "Good weather holding.  <color=green>COLLECT $1000</color>.";

				break;

			case 47:	//START CORN HARVEST
				_headerText.text = "1ST WEEK OF DECEMBER";
				_spaceText.text = "Early freeze kills fruit buds.\n<color=red>PAY $1000</color>, if you have Fruit.";

				break;

			case 48:
				_headerText.text = "2ND WEEK OF DECEMBER";
				_spaceText.text = "Cold and dry, perfect Field Corn Harvesting.\n<color=green>COLLECT $500</color>.";

				break;

			case 49:	//END CORN HARVEST
				_headerText.text = "3RD WEEK OF DECEMBER";
				_spaceText.text = "First Snow.  Draw Farmer's Fate.";

				break;
		}

		if (space >= 0 && space <= 18)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.black;
		}
		else if (space >=19 && space <= 22)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.green;
		}
		else if (space >= 23 && space <= 25)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.red;
		}
		else if (space >= 26 && space <= 28)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.green;
		}
		else if (space >= 29 && space <= 33)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.yellow;
		}
		else if (space >= 34 && space <= 35)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.green;
		}
		else if (space >= 36 && space <= 39)
		{
			_headerText.GetComponent<Outline>().effectColor = IFG.Brown;
		}
		else if (space == 40)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.green;
		}
		else if (space >= 41 && space <= 43)
		{
			_headerText.GetComponent<Outline>().effectColor = IFG.SpudBlue;
		}
		else if (space >= 44 && space <= 46)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.red;
		}
		else if (space >= 47 && space <= 49)
		{
			_headerText.GetComponent<Outline>().effectColor = Color.yellow;
		}

		_boardSpacePanel.SetActive(true);
		_modalPanel.SetActive(true);

		StartCoroutine(WaitForPlayer(space));
	}
	#endregion

	#region Private Methods

	IEnumerator WaitForPlayer(int space)
	{
		while (_boardSpacePanel.activeSelf)
			yield return null;

		PerformBoardSpaceActions(space);

		//Debug.Log("SPACE ON CLOSING BOARDSPACE PANEL: " + space);
	}

	void PerformBoardSpaceActions(int space)
	{
		switch (space)
		{
			case 0:  //$1000 bonus
				AudioManager.Instance.PlaySound(AudioManager.Instance._goBonus);
				_pManager.UpdateMyCash(1000);
				break;

			case 1:  //10% on notes
				if (_pManager._pNotes > 0)
				{
					_pManager.UpdateMyCash(-(int)(_pManager._pNotes * 0.1f));
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				}
				break;

			case 2:  //draw OTB
			case 8:
			case 13:
				_pManager.DrawOTBCard();
				AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				break;

			case 3:	//pay $500 if you have cows
				if (_pManager._pFarmCows > 0 || _pManager._pRangeCows > 0)
				{
					_pManager.UpdateMyCash(-500);
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				}
				break;

			case 4:	//double your hay harvest this year
				_pManager._pHayDoubled = true;
				_pManager._pHayDoubledCounter++;
				//Debug.Log("HAY COUNTER: " + _pManager._pHayDoubledCounter);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Hay", _pManager._pHay, true);
				_uiManager.UpdateUI();
				break;

			case 5:  //collect $1000
				_pManager.UpdateMyCash(1000);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				break;

			case 6:  //draw FF
				_pManager.DrawFFCard();
				AudioManager.Instance.PlaySound(AudioManager.Instance._ff);
				break;

			case 7:  //goto spring planting
				_pMove.DirectedForwardMove(14);
				AudioManager.Instance.PlaySound(AudioManager.Instance._gotoSpringPlanting);
				break;

			case 9:  //replant wheat
				_pManager.UpdateMyCash(-2000);
				AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				break;

			case 10: //start plowing late
				_pManager.UpdateMyCash(-500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				break;

			case 11: //hurt your back
				_pMove.GoBackMove(2);
				AudioManager.Instance.PlaySound(AudioManager.Instance._hurtBack);
				break;

			case 12: //heat fruit
				if (_pManager._pFruit > 0)
				{
					_pManager.UpdateMyCash(-2000);
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				}
				break;

			case 14: //double corn
				_pManager._pCornDoubled = true;
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Grain", _pManager._pGrain, true);
				_uiManager.UpdateUI();
				break;

			case 15: //more rain
				_pManager.UpdateMyCash(-500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				break;

			case 16: //equipment breakdown
				_pManager.UpdateMyCash(-1000);
				AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				break;

			case 17: //valley is green
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				break;

			case 18: //replant corn
				_pManager.UpdateMyCash(-500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				break;

			case 19: //START 1ST HAY CUT - 1000 bonus
				_pManager.UpdateMyCash(1000);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				_pManager._firstHay = true;
				if (!_pManager._pWagesGarnished)
					StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
				else
					StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));
				break;

			case 20: //memorial day - draw OTB
				if (_pManager._firstHay)
				{
					_pManager.DrawOTBCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				}
				else
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));
					//drawOTB in post harvest
					_pManager._firstHay = true;
				}
				break;

			case 21: //cut harvest check in half
				if (!_pManager._firstHay)
				{
					_hManager._cutHarvestInHalf = true;
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._firstHay = true;
				}
				break;

			case 22: //$500 bonus - END 1ST HAY CUT
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);

				if (!_pManager._firstHay)
				{
					if(!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._firstHay = true;
				}
				break;

			case 23: //START CHERRY HARVEST - cut harvest in half
				if (_pManager._pFruit > 0)
				{
					_hManager._cutHarvestInHalf = true;
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Cherries", _pManager._pFruit));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._cherries = true;
				}
				break;

			case 24: //draw FF
				if(_pManager._pFruit > 0 && !_pManager._cherries)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Cherries", _pManager._pFruit));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._cherries = true;
					//draw FF in post harvest
				}
				else
				{
					_pManager.DrawFFCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._ff);
				}
				break;

			case 25: //independence day - END OF CHERRY HARVEST
						//fireworks...
				if(_pManager._pFruit > 0 && !_pManager._cherries)
				{
					if(!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Cherries", _pManager._pFruit));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._cherries = true;
				}
				else
					StartCoroutine(PlayFireworksRoutine());
				break;

			case 26: //START 2ND HAY CUT - double harvest check
				_hManager._doubleHarvest = true;
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if(!_pManager._pWagesGarnished)
					StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
				else
					StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

				_pManager._secondHay = true;
				break;

			case 27: //draw OTB
				if (_pManager._secondHay)
				{
					_pManager.DrawOTBCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				}
				else
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
					else
					{
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));
						//draw OTB in post harvest
					}
					_pManager._secondHay = true;
				}
				break;

			case 28: //go to Harvest Moon - END OF 2ND HAY CUT
				if (_pManager._secondHay)
				{
					_pMove.DirectedForwardMove(36);
					AudioManager.Instance.PlaySound(AudioManager.Instance._getOnYourTractor);
				}
				else
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
					else
					{
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));
						//move to HM in post harvest
					}
					_pManager._secondHay = true;
				}
				break;

			case 29: //START OF WHEAT HARVEST - increase harvest by 50/acre
				_hManager._add50PerWheatAcre = true;
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (!_pManager._pWagesGarnished)
					StartCoroutine(_hManager.PerformHarvestRoutine(space, "Wheat", _pManager._pGrain));
				else
					StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

				_pManager._wheat = true;
				break;

			case 30: //farming like an idiot - goto 4th week of Feb (space 8)
				if (_pManager._wheat)
				{
					_pMove.DirectedForwardMove(58);
					AudioManager.Instance.PlaySound(AudioManager.Instance._farmingLikeAnIdiot);
				}
				else
				{
					if(!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Wheat", _pManager._pGrain));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//direectedFowrwardMove(58) in post harvest
					_pManager._wheat = true;
				}
				break;

			case 31: //collect $1000, if you have a Harvester
				if (_pManager._pHarvester)
				{
					_pManager.UpdateMyCash(1000);
					AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				}
				if (!_pManager._wheat)
				{
					if(!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Wheat", _pManager._pGrain));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._wheat = true;
				}
				break;

			case 32: //collect $500
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (!_pManager._wheat)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Wheat", _pManager._pGrain));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._wheat = true;
				}
				break;

			case 33: //cut $50/acre - END OF WHEAT HARVEST
				if (!_pManager._wheat)
				{
					_hManager._cut50PerWheatAcre = true;
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Wheat", _pManager._pGrain));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));
				}
				break;

			case 34: //START OF 3RD HAY CUT - tractor owners to space 46
				if(!_pManager._pWagesGarnished)
					StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
				else
					StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

				_pManager._thirdHay = true;
				//DirectedMove(46) in post harvest
				break;

			case 35: //draw OTB - END OF 3RD HAY CUT
				if (!_pManager._thirdHay)
				{
					if(!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//DrawOTB in post harvest
					_pManager._thirdHay = true;
				}
				else
				{
					_pManager.DrawOTBCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				}
				break;

			case 36: //Harvest Moon - START LIVESTOCK HARVEST - collect $500
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (_pManager._pFarmCows > 0 || _pManager._pRangeCows > 0)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Livestock", _pManager._pRangeCows + _pManager._pFarmCows));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._livestock = true;
				}
				break;

			case 37: //cut harvest in half
				if ((_pManager._pFarmCows > 0 || _pManager._pRangeCows > 0) && !_pManager._livestock)
				{
					_hManager._cutHarvestInHalf = true;
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Livestock", _pManager._pRangeCows + _pManager._pFarmCows));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._livestock = true;
				}
				break;

			case 38: //pay $2000, if you own fruit
				if (_pManager._pFruit > 0)
				{
					_pManager.UpdateMyCash(-2000);
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				}

				if ((_pManager._pFarmCows > 0 || _pManager._pRangeCows > 0) && !_pManager._livestock)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Livestock", _pManager._pRangeCows + _pManager._pFarmCows));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._livestock = true;
				}
				break;

			case 39: //collect $500  - END OF LIVESTOCK HARVEST
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if ((_pManager._pFarmCows > 0 || _pManager._pRangeCows > 0) && !_pManager._livestock)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Livestock", _pManager._pRangeCows + _pManager._pFarmCows));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._livestock = true;
				}
				break;

			case 40: //4TH HAY CUT - $500 bonus
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (!_pManager._pWagesGarnished)
					StartCoroutine(_hManager.PerformHarvestRoutine(space, "Hay", _pManager._pHay));
				else
					StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

				_pManager._fourthHay = true;
				break;

			case 41: //START SPUD HARVEST - DrawFF card
				if (_pManager._pSpuds > 0)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Spuds", _pManager._pSpuds));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//do draw ff in post harvest
					_pManager._spuds = true;
				}
				else
				{
					_pManager.DrawFFCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._ff);
				}
				break;

			case 42: //draw OTB
				if (_pManager._pSpuds > 0 && !_pManager._spuds)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Spuds", _pManager._pSpuds));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//do draw otb in post harvest
					_pManager._spuds = true;
				}
				else
				{
					_pManager.DrawOTBCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				}
				break;

			case 43: //draw FF - END OF SPUD HARVEST
				if (_pManager._pSpuds > 0 && !_pManager._spuds)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Spuds", _pManager._pSpuds));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//do draw ff in post harvest
					_pManager._spuds = true;
				}
				else
				{
					_pManager.DrawFFCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._ff);
				}
				break;

			case 44: //START APPLE HARVEST - draw OTB
				if (_pManager._pFruit > 0)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Apples", _pManager._pFruit));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//draw otb in post harvest
					_pManager._apples = true;
				}
				else
				{
					_pManager.DrawOTBCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._otb);
				}
				break;

			case 45: //collect $500
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (_pManager._pFruit>0 && !_pManager._apples)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Apples", _pManager._pFruit));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._apples = true;
				}
				break;

			case 46: //collect $1000 - END OF APPLE HARVEST
				_pManager.UpdateMyCash(1000);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (_pManager._pFruit > 0 && !_pManager._apples)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Apples", _pManager._pFruit));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._apples = true;
				}
				break;

			case 47: //START CORN HARVEST
				if (_pManager._pFruit > 0)
				{
					_pManager.UpdateMyCash(-1000);
					AudioManager.Instance.PlaySound(AudioManager.Instance._bad);
				}
				if (!_pManager._pWagesGarnished)
					StartCoroutine(_hManager.PerformHarvestRoutine(space, "Corn", _pManager._pGrain));
				else
					StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

				_pManager._corn = true;
				break;

			case 48: //collect $500
				_pManager.UpdateMyCash(500);
				AudioManager.Instance.PlaySound(AudioManager.Instance._good);
				if (!_pManager._corn)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Corn", _pManager._pGrain));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					_pManager._corn = true;
				}
				break;

			case 49: //END CORN HARVEST - draw FF
				if (!_pManager._corn)
				{
					if (!_pManager._pWagesGarnished)
						StartCoroutine(_hManager.PerformHarvestRoutine(space, "Corn", _pManager._pGrain));
					else
						StartCoroutine(_hManager.PerformGarnishedHarvestRoutine(space));

					//draw FF in post harvest
					_pManager._corn = true;
				}
				else
				{
					_pManager.DrawFFCard();
					AudioManager.Instance.PlaySound(AudioManager.Instance._ff);
				}
				break;

			default:
				Debug.LogWarning("OOPS, Space not found " + space);
				break;
		}
	}

	 public IEnumerator PlayFireworksRoutine()
	{
		_activeSprites = new List<SpriteRenderer>();
		CyclePlayersAndStickers(false);
		_gameboardRenderer.enabled = false;
		GameObject fireworks = Instantiate(_fireworksPrefab);
		yield return new WaitForSeconds(6f);
		CyclePlayersAndStickers(true);
		_gameboardRenderer.enabled = true;
		Destroy(fireworks);
	}

	void CyclePlayersAndStickers(bool status)
	{
		//players
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject sprite in players)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//hay
		GameObject[] hayStickers = GameObject.FindGameObjectsWithTag("HaySticker");
		foreach (GameObject sprite in hayStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//grain
		GameObject[] grainStickers = GameObject.FindGameObjectsWithTag("GrainSticker");
		foreach (GameObject sprite in grainStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//fruit
		GameObject[] fruitStickers = GameObject.FindGameObjectsWithTag("FruitSticker");
		foreach (GameObject sprite in fruitStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//cows
		GameObject[] cowStickers = GameObject.FindGameObjectsWithTag("CowSticker");
		foreach (GameObject sprite in cowStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//spuds
		GameObject[] spudStickers = GameObject.FindGameObjectsWithTag("SpudsSticker");
		foreach (GameObject sprite in spudStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//harvester
		GameObject[] harvesterStickers = GameObject.FindGameObjectsWithTag("HarvesterSticker");
		foreach (GameObject sprite in harvesterStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//tractor
		GameObject[] tractorStickers = GameObject.FindGameObjectsWithTag("TractorSticker");
		foreach (GameObject sprite in tractorStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;

		//ranges
		GameObject[] rangeStickers = GameObject.FindGameObjectsWithTag("RangeSticker");
		foreach (GameObject sprite in rangeStickers)
			sprite.GetComponent<SpriteRenderer>().enabled = status;
	}
	#endregion
}
