using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class WinnerList : MonoBehaviour
{
	#region Public / Serialized Fields

	[SerializeField] Transform _content;
	[SerializeField] GameObject _winnerEntryPerfab;
	public GameObject _winnersPanel;
	public Transform _ruContent;
	public GameObject _runnerUpEntryPrefab;
	public GameObject _runnersUpPanel;

	#endregion

	#region Private Fields / References

	//UIManager _uiManager;
	Winners _winners;
	
	#endregion

	#region Properties

	public static WinnerList Instance;

	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnUpdateWinnersListEventReceived;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnUpdateWinnersListEventReceived;
	}

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	void Start()
	{
		//PlayerPrefs.DeleteAll();
		
		SendListToOthers();
	}

	#endregion

	#region Public Methods

	public void AddWinnerListEntry(string pName, string fName, int nw, int endGameNW, int nop, string[] runnersUpNames, string[] runnerUpFarmers, int[] runnerUpNetworkths)
	{
		//string testDate = "11/21/2019";
		//create WinnerListEntry
		WinnerEntry winnerListEntry = new WinnerEntry
		{
			date = System.DateTime.Now.ToShortDateString(),
			//date = testDate,
			playerName = pName,
			farmerName=fName,
			networth = nw,
			endGameNetworth = endGameNW,
			numberOfPlayers = nop,
			rPlayerNames = runnersUpNames,
			rFarmerNames = runnerUpFarmers,
			rNetworths = runnerUpNetworkths
		};
		//load saved data...
		//string jsonString = PlayerPrefs.GetString("WinnerList");
		//_winners = JsonUtility.FromJson<Winners>(jsonString);

		//load saved data...
		if (File.Exists(Application.dataPath + "/WinnerList.txt"))
		{
			string jsonString = File.ReadAllText(Application.dataPath + "/WinnerList.txt");
			_winners = JsonUtility.FromJson<Winners>(jsonString);
		}
		else
		{
			Debug.Log("No Saved List Found!");
		}

		if (_winners == null)
		{
			//no stored list, initialize
			_winners = new Winners()
			{
				winnerListEntryList = new List<WinnerEntry>()
			};

			Debug.Log("NO WINNERS LIST...");
		}

		//add new entry to Winners
		_winners.winnerListEntryList.Add(winnerListEntry);

		//save updated Winners
		//string json = JsonUtility.ToJson(_winners);
		//PlayerPrefs.SetString("WinnerList", json);
		//PlayerPrefs.Save();

		//save updated Winners
		string json = JsonUtility.ToJson(_winners);
		File.WriteAllText(Application.dataPath + "/WinnerList.txt", json);
	}

	public void SendListToOthers()
	{
			//load saved data...
			if (File.Exists(Application.dataPath + "/WinnerList.txt"))
			{
				string jsonString = File.ReadAllText(Application.dataPath + "/WinnerList.txt");
				_winners = JsonUtility.FromJson<Winners>(jsonString);
			}
			else
			{
				Debug.LogWarning("NO DAMN LIST PRESENT");
			}

		if (GameManager.Instance._numberOfPlayers > 1)
		{
			//send MasterClient send his list to all
			if (PhotonNetwork.IsMasterClient)
			{
				//event data
				string myList = File.ReadAllText(Application.dataPath + "/WinnerList.txt");
				object[] sndData = new object[] { myList };
				//event options
				RaiseEventOptions eventOptions = new RaiseEventOptions
				{
					Receivers = ReceiverGroup.Others,
					CachingOption = EventCaching.DoNotCache
				};
				//send options
				SendOptions sendOptions = new SendOptions { Reliability = true };
				//fire the event...
				PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Update_WinnersList_Event_Code, sndData, eventOptions, sendOptions);
			}
		}
	}

	public void PopulateAndShowWinnersList()
	{
		List<GameObject> winnerPrefabs = GameObject.FindGameObjectsWithTag("WinnerPrefab").ToList();
		foreach (GameObject prefab in winnerPrefabs)
			Destroy(prefab);

		foreach (WinnerEntry winnerEntry in _winners.winnerListEntryList)
		{
			//main winner entries
			GameObject listEntry = Instantiate(_winnerEntryPerfab, _content);
			listEntry.transform.localScale = Vector3.one;
			listEntry.GetComponent<WinnerInitializer>().Initialize(winnerEntry.date, winnerEntry.playerName, winnerEntry.farmerName, winnerEntry.networth, winnerEntry.endGameNetworth, winnerEntry.numberOfPlayers, winnerEntry.rPlayerNames, winnerEntry.rFarmerNames, winnerEntry.rNetworths);
		}
	}

	public void OnCloseWPButtonClicked()
	{
		_winnersPanel.SetActive(false);
	}

	public void OnCloseRUPButtonClicked()
	{
		_runnersUpPanel.SetActive(false);
	}
	#endregion

	#region Private Methods

	void OnUpdateWinnersListEventReceived(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Update_WinnersList_Event_Code)
		{
			//extract data
			object[] recData = (object[])eventData.CustomData;
			string jsonString = (string)recData[0];
			File.WriteAllText(Application.dataPath + "/WinnerList.txt", jsonString);

			if (File.Exists(Application.dataPath + "/WinnerList.txt"))
			{
				jsonString = File.ReadAllText(Application.dataPath + "/WinnerList.txt");
				_winners = JsonUtility.FromJson<Winners>(jsonString);
			}
			else
			{
				Debug.Log("No Saved List Found!");
			}
		}
	}
	#endregion
}

class Winners	//Highscores
{
	public List<WinnerEntry> winnerListEntryList;
}

[System.Serializable]
class WinnerEntry	//HighscoreEntry
{
	//winner & game data
	public string date;
	public string playerName;
	public string farmerName;
	public int networth;
	public int endGameNetworth;
	public int numberOfPlayers;
	//runners up data
	public string[] rPlayerNames;
	public string[] rFarmerNames;
	public int[] rNetworths;
}
