using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Linq;

public class WinnerInitializer : MonoBehaviour
{
	#region Public / Serialized Fields

	//UIManager _uiManager;

	[Header("Winner's Panel")]
	[SerializeField] Text _dateText, _nicknameText, _winningNetworthText, _gameConditionText, _nopText;
	[SerializeField] Button _gameInspectionButton;

	#endregion

	#region Private Fields

	int _nop;
	string[] _ruPlayers, _ruFarmers;
	int[] _ruNetworths;

	#endregion

	#region MonoBehaviour Methods

	void Awake()
	{
		//_uiManager = GameManager.Instance.uiManager;
	}
	#endregion

	#region Public Methods

	public void Initialize(string date, string winnerName, string farmerName, int networth, int gameEnd, int nop, string[] ruPlayers, string[] ruFarmers, int[] ruNetworths)
	{
		_dateText.text = date;
		_nicknameText.text = winnerName;
		_nicknameText.color = GameManager.Instance.uiManager.SelectFontColorForFarmer(farmerName);
		_nopText.text = nop.ToString();
		_nop = nop;
		_winningNetworthText.text = networth.ToString("C0");

		if (gameEnd > 500)
			_gameConditionText.text = gameEnd.ToString("C0");
		else
			_gameConditionText.text = gameEnd + " Min";

		_ruPlayers = ruPlayers;
		_ruFarmers = ruFarmers;
		_ruNetworths = ruNetworths;

		if (nop > 1)
		{
			_gameInspectionButton.onClick.AddListener(OnWinnerButtonClicked);
		}
		else
			_gameInspectionButton.onClick.RemoveListener(OnWinnerButtonClicked);
	}

	public void OnWinnerButtonClicked()
	{
		char[] charactersToTrim = { '$' };
		string networthString = _gameConditionText.text;

		networthString = networthString.Trim(charactersToTrim);
		networthString = networthString.Replace(",", "");
		//Debug.Log(networthString);
		int endingNetworth = int.Parse(networthString);
		//Debug.Log("NW: " + endingNetworth);

		WinnerList.Instance._runnersUpPanel.SetActive(true);
		List<GameObject> runnerUpPrefabs = GameObject.FindGameObjectsWithTag("RunnerUpPrefab").ToList();
		foreach (GameObject prefab in runnerUpPrefabs)
			Destroy(prefab);

		for (int i = 0; i < _nop; i++)	//change _nop-1 to _nop
		{
			if (_ruNetworths[i] > 0)
			{
				GameObject ruGO = Instantiate(WinnerList.Instance._runnerUpEntryPrefab, WinnerList.Instance._ruContent);
				ruGO.transform.Find("Name Text").GetComponent<Text>().text = _ruPlayers[i];
				ruGO.transform.Find("Name Text").GetComponent<Text>().color = GameManager.Instance.uiManager.SelectFontColorForFarmer(_ruFarmers[i]);

				if (ruGO.transform.Find("Name Text").GetComponent<Text>().color != Color.black)
					ruGO.transform.Find("Name Text").GetComponent<Outline>().enabled = true;
				else
					ruGO.transform.Find("Name Text").GetComponent<Outline>().enabled = false;

				ruGO.transform.Find("Networth Text").GetComponent<Text>().text = _ruNetworths[i].ToString("C0");
				ruGO.transform.Find("Networth Text").GetComponent<Text>().color = SetRunnerUpNetworthColor(_ruNetworths[i], endingNetworth);

				if (ruGO.transform.Find("Networth Text").GetComponent<Text>().color != Color.black)
					ruGO.transform.Find("Networth Text").GetComponent<Outline>().enabled = true;
				else
					ruGO.transform.Find("Networth Text").GetComponent<Outline>().enabled = false;
			}
		}
	}
	#endregion

	#region Private Methods

	Color SetRunnerUpNetworthColor(int networth,int endGame)
	{
		if (networth >= endGame * 0.25f && networth < endGame * 0.5f)
		{
			
			return Color.green;
		}
		else if (networth >= endGame * 0.5f && networth < endGame * 0.75f)
		{
			return Color.yellow;
		}
		else if (networth >= endGame * 0.75f && networth < endGame * 0.875f)
		{
			return IFG.Orange;
		}
		else if (networth >= endGame * 0.875f)
		{
			return Color.red;
		}
		else
		{
			return Color.black;
		}
	}
	#endregion
}
