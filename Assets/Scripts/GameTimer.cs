using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameTimer : MonoBehaviourPun
{
	#region Public / Serialized Fields


	#endregion

	#region Private Fields / References

	Text _timerText;

	[SerializeField] float _time, _startTime, _testTime;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_startTime = Time.time;
		//_testTime = 3550;

		if (PhotonNetwork.IsMasterClient)
			StartCoroutine(Timer());
	}
	
	void Update() 
	{
		
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods

	IEnumerator Timer()
	{
		_time = _startTime + Time.time;
		photonView.RPC("UpdateTimer", RpcTarget.All, _time);
		_testTime++;
		yield return new WaitForSecondsRealtime(1.0f);

		StartCoroutine(Timer());
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
	#endregion
}
