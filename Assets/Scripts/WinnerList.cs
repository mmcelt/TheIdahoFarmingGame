using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinnerList : MonoBehaviour
{
	#region Public / Serialized Fields

	[SerializeField] Transform _content;
	[SerializeField] GameObject _winnerEntryPerfab;

	#endregion

	#region Private Fields / References


	#endregion

	#region Properties

	public static WinnerList Instance;

	#endregion

	#region MonoBehaviour Methods

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(gameObject);
	}

	void Start() 
	{
		
	}
	
	void Update() 
	{
		
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	public void AddWinnerListEntry(string pName, int nw, int endGameNW, int nop, string[] runnersUpNames, int[] runnerUpNetworkths)
	{
		//create WinnerListEntry
		WinnerListEntry winnerListEntry = new WinnerListEntry
		{
			date = System.DateTime.Now.ToShortDateString(),
			playerName = pName,
			networth=nw,
			endGameNetworth=endGameNW,
			numberOfPlayers=nop,
			rPlayerNames=runnersUpNames,
			rNetworths=runnerUpNetworkths
		};
		//load saved data...
		string jsonString = PlayerPrefs.GetString("WinnerList");

	}
	#endregion
}

class Winners
{
	public List<WinnerListEntry> winnerListEntryList;
}

[System.Serializable]
class WinnerListEntry
{
	//winner & game data
	public string date;
	public string playerName;
	public int networth;
	public int endGameNetworth;
	public int numberOfPlayers;
	//runners up data
	public string[] rPlayerNames;
	public int[] rNetworths;
}
