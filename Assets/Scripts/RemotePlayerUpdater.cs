﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//THIS SRIPT WILL UPDATE THE LOCAL PLAYER'S DATA ON THE REMOTE PLAYER'S REMOTE DATA FIELDS
public class RemotePlayerUpdater : MonoBehaviourPun
{
	#region Public / Serialized Fields

	public bool _coroutineStopped;

	#endregion

	#region Private Fields / References

	PlayerManager _pManager;
	UIManager _uiManager;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_pManager = GetComponent<PlayerManager>();
		_uiManager = GameManager.Instance.uiManager;
		if (photonView.IsMine && GameManager.Instance._numberOfPlayers > 1)
			StartCoroutine(UpdateRemotePlayerData());
	}
	#endregion

	#region Public Methods

	public void UpdateMyDataToOthers()
	{
		if (_pManager == null)
			_pManager = GetComponent<PlayerManager>();

		Debug.Log("In UpdtMyData2Others");
		photonView.RPC("UpdateTheData", RpcTarget.Others, _pManager._pCash, _pManager._pNotes, _pManager._myOtbCount, _pManager._pNetworth);
	}

	#endregion

	#region Private Methods

	public IEnumerator UpdateRemotePlayerData()
	{
		for (int i = 0; i < 4; i++)
		{
			Debug.Log("NOP in URPD: " + GameManager.Instance._numberOfPlayers);
			photonView.RPC("UpdateTheData", RpcTarget.Others, _pManager._pCash, _pManager._pNotes, _pManager._myOtbCount, _pManager._pNetworth);
			yield return new WaitForSeconds(0.75f);
		}
		//StartCoroutine(UpdateRemotePlayerData());
		_coroutineStopped = true;
	}

	[PunRPC]
	void UpdateTheData(int myCash, int myNotes, int myOtbs, int myNetworth, PhotonMessageInfo info)
	{
		if (_pManager == null)
			_uiManager = GameManager.Instance.uiManager;

		string player = info.Sender.NickName;

		for(int i=0; i<GameManager.Instance._cachedPlayerList.Count; i++)
		{
			if (_uiManager._otherPlayerNameTexts[i].text == player)
			{
				_uiManager._otherPlayerCashTexts[i].text = myCash.ToString("c0");
				if (myCash <= 0)
					_uiManager._otherPlayerCashTexts[i].color = Color.red;
				else
					_uiManager._otherPlayerCashTexts[i].color = Color.black;

				_uiManager._otherPlayerNotesTexts[i].text = myNotes.ToString("c0");
				if (myNotes >= 50000)
					_uiManager._otherPlayerNotesTexts[i].color = Color.red;
				else
					_uiManager._otherPlayerNotesTexts[i].color = Color.black;

				_uiManager._otherPlayerOtbTexts[i].text = myOtbs.ToString();
				_uiManager._remotePlayerNetworthTexts[i].text = myNetworth.ToString("c0");
			}
		}
	}
	#endregion
}
