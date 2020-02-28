using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UncleChester : MonoBehaviour
{
	#region Public / Serialized Fields

	[SerializeField] Dropdown downpaymentDropdown;
	[SerializeField] Text _pCashText, _pNotesText;

	#endregion

	#region Private Fields / References

	List<string> _downpayments = new List<string>();

	int _downpayment, _pCash, _pNotes, _tempCash, _tempNotes;

	PlayerManager _pManager;
	StickerManager _sManager;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		if (_pManager==null)
			_pManager = GetComponentInParent<PlayerManager>();

		PopulateTheDropdown();

		_pCash = _pManager._pCash;
		_pNotes = _pManager._pNotes;
		_tempCash = _pCash;
		_tempNotes = _pNotes;
		_pCashText.text = _pCash.ToString();
		_pNotesText.text = _pNotes.ToString();
	}

	void Start()
	{
		_sManager = GameManager.Instance.sManager;
	}

	#endregion

	#region Public Methods

	public void OnDownpaymentChanged(int index)
	{
		ResetTempValues();

		if (index > 0)
		{
			_downpayment = int.Parse(_downpayments[index]);

			Debug.Log("Downpayment "+_downpayment+" "+index);

			UpdateTheNumbers();
		}
	}

	public void OnBuyButtonClicked()
	{
		_pManager.UpdateMyCash(-_downpayment);
		_pManager.UpdateMyNotes(10000 - _downpayment);
		_pManager.UpdateMyHay(10);
		//TODO: update stickers
		_sManager.PlaceFarmSticker(GameManager.Instance.myFarmerName, "Hay", _pManager._pHay, _pManager._pHayDoubled);

		Debug.Log("Hay: " + _pManager._pHay);
	}
	#endregion

	#region Private Methods

	void PopulateTheDropdown()
	{
		_downpayments.Clear();
		downpaymentDropdown.ClearOptions();

		string header = "SELECT YOUR DOWNPAYMENT...";

		_downpayments.Add(header);

		for(int i=2000; i<=10000; i += 500)
		{
			_downpayments.Add(i.ToString());
		}

		downpaymentDropdown.AddOptions(_downpayments);
	}

	void UpdateTheNumbers()
	{
		_tempCash -= _downpayment;
		_pCashText.text = _tempCash.ToString();
		_tempNotes += (10000 - _downpayment);
		_pNotesText.text = _tempNotes.ToString();
	}

	void ResetTempValues()
	{
		_tempCash = _pCash;
		_tempNotes = _pNotes;
	}
	#endregion
}
