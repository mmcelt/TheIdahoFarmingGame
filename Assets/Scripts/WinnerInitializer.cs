using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class WinnerInitializer : MonoBehaviourPun
{
	#region Public / Serialized Fields

	[Header("Winner's Panel")]
	[SerializeField] Text _dateText, _nicknameText, _winningNetworthText, _gameConditionText, _nopText;
	[SerializeField] Button _gameInspectionButton;

	[Header("Runners Up Panel")]
	[SerializeField] GameObject _runnersUpPanel;
	[SerializeField] Text _nicknameRText, _networthRText;
	[SerializeField] Button _closeButton;

	#endregion

	#region Public Methods

	public void Initialize()
	{
		
	}

	public void OnWinnerButtonClicked()
	{

	}

	public void OnCloseButtonClicked()
	{
		_runnersUpPanel.SetActive(false);
	}
	#endregion

	#region Private Methods


	#endregion
}
