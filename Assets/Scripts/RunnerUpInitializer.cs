using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunnerUpInitializer : MonoBehaviour
{
	#region Public / Serialized Fields

	[Header("Runners Up Panel")]
	//[SerializeField] GameObject _runnersUpPanel;
	[SerializeField] Text _nicknameRText, _networthRText;
	//[SerializeField] Button _closeButton;

	#endregion

	#region Private Fields / References


	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	#endregion

	#region Public Methods

	public void Initialize(string playerName, string farmerName, int networth)
	{
		_nicknameRText.text = playerName;
		_nicknameRText.color = GameManager.Instance.uiManager.SelectFontColorForFarmer(farmerName);
		_networthRText.text = networth.ToString("C0");
	}

	#endregion

	#region Private Methods

	void Testing()
	{
		//runners up entries
		//if (winnerEntry.numberOfPlayers > 1)
		//{
		//	for (int i = 0; i < winnerEntry.numberOfPlayers - 1; i++)
		//	{
		//		GameObject ruListEntry = Instantiate(_runnerUpEntryPrefab, _ruContent);
		//		ruListEntry.transform.localScale = Vector3.one;
		//		ruListEntry.GetComponent<RunnerUpInitializer>().Initialize(winnerEntry.rPlayerNames[i], winnerEntry.rFarmerNames[i], winnerEntry.rNetworths[i]);
		//	}
		//}

	}
	#endregion
}
