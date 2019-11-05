using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RemotePlayerUpdater : MonoBehaviourPun
{
	#region Public / Serialized Fields


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
		if (photonView.IsMine)
			StartCoroutine(UpdateRemotePlayerData());
	}
	#endregion

	#region Public Methods

	IEnumerator UpdateRemotePlayerData()
	{
		Debug.Log("NOP in URPD: " + GameManager.Instance._numberOfPlayers);
		if (GameManager.Instance._numberOfPlayers > 1)
		{
			//object[] data = new object[] { _pManager._myOtbCount };
			photonView.RPC("UpdateTheData", RpcTarget.Others, _pManager._myOtbs.Count);
			yield return new WaitForSeconds(0.5f);
			StartCoroutine(UpdateRemotePlayerData());
		}
	}
	#endregion

	#region Private Methods

	[PunRPC]
	void UpdateTheData(int myOtbs, PhotonMessageInfo info)
	{
		if (_pManager == null)
			_uiManager = GameManager.Instance.uiManager;

		string player = info.Sender.NickName;

		for(int i=0; i<GameManager.Instance._cachedPlayerList.Count; i++)
		{
			if (_uiManager._otherPlayerNameTexts[i].text == player)
			{
				_uiManager._otherPlayerOtbTexts[i].text = myOtbs.ToString();
			}
		}
	}
	#endregion
}
