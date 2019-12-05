using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class GameTimer : MonoBehaviourPun
{
	#region Public / Serialized Fields

	[SerializeField] float _time, _startTime, _testTime;

	#endregion

	#region Private Fields / References

	Text _timerText;
	string _gameMode;
	bool _canCountDown;
	bool _doneOnce;

	#endregion

	//bool _endTimedGame;

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_gameMode = GameManager.Instance._gameMode;

		if (_gameMode =="Networth Game")
		{
			_startTime = Time.time;
			_canCountDown = false;
		}
		else
		{
			_startTime = GameManager.Instance._timedGameLength * 60;
			_time = _startTime;
			_canCountDown = true;
		}

		if (PhotonNetwork.IsMasterClient && _gameMode == "Networth Game")
			StartCoroutine("Timer");
	}

	void Update()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		if (_time >= 0.0f && _canCountDown)
		{
			_time -= Time.deltaTime;
			photonView.RPC("UpdateTimer", RpcTarget.All, _time);
		}
		else if (_time < 0.0f && _canCountDown)
		{
			_canCountDown = false;
			DetermineTheWinner();
		}
	}

	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	IEnumerator Timer()
	{
		_time = _startTime + Time.time;

		photonView.RPC("UpdateTimer", RpcTarget.All, _time);

		yield return new WaitForSecondsRealtime(1.0f);
		StartCoroutine("Timer");
	}

	[PunRPC]
	void UpdateTimer(float time)
	{
		if (_timerText==null)
			_timerText = GameManager.Instance._timerText;

		string seconds = ((int)time % 60).ToString("00");
		string minutes = ((int)(time / 60) % 60).ToString("00");
		string hours = ((int)time / 3600).ToString();
		_timerText.text = hours + ":" + minutes + ":" + seconds;
	}

	void DetermineTheWinner()
	{
		Debug.Log("IN DETERMINE WINNER B4 IF");
		//StopCoroutine("Timer");

		if (!PhotonNetwork.IsMasterClient | _doneOnce) return;
		_doneOnce = true;

		Debug.Log("IN DETERMINE WINNER AFTER IF");

		//if (_endTimedGame) return;

		//_endTimedGame = true;

		int highestNetworth = 0;
		string winnerPlayer = "";
		string winningFarmer = "";

		//this finds the winner...
		foreach(Player player in GameManager.Instance._cachedPlayerList)
		{
			int playerNetworth = (int)player.CustomProperties[IFG.Player_Networth];
			if (playerNetworth > highestNetworth)
			{
				highestNetworth = playerNetworth;
				winnerPlayer = player.NickName;
				winningFarmer = (string)player.CustomProperties[IFG.Selected_Farmer];
			}
		}
		//send event to all UIManagers...
		//data - winnerName,farmerName,highestNetworth
		object[] winnerData = new object[] { winnerPlayer, winningFarmer, highestNetworth };
		//event option
		RaiseEventOptions eventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.All,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event...
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.End_Timed_Game_Event_Code, winnerData, eventOptions, sendOptions);
	}
	#endregion
}
