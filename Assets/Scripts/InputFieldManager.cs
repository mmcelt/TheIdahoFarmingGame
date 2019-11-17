using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour
{
	#region Public / Serialized Fields


	#endregion

	#region Private Fields / References

	UIManager _uiManager;
	PlayerManager _pManager;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		_uiManager = GetComponent<UIManager>();
		_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();

	}
	#endregion

	#region Public Methods

	public void OnInputFieldValueChanged(InputField target)
	{
		_uiManager.ResetTempFunds();

		int amount = 0;
		int rounding = 0;

		if (int.TryParse(target.text, out amount))
		{
			if (target.name=="Repay Loan Input" || target.name=="Forced Loan Input")
			{
				rounding = 10;
			}
			else
			{
				rounding = 100;
			}
			amount = Mathf.RoundToInt(amount / rounding);
			amount *= rounding;
		}

		if (amount >= 0 && amount < rounding)
			amount = rounding;

		Debug.Log("AMOUNT AFTER ROUNDING: " + amount);

		if (amount > 0)
		{
			if (target.name == "Downpayment Input")
			{
				StopCoroutine("BuyOptionRoutine");
				_uiManager._transactionBlocked = true;
				StartCoroutine(_uiManager.BuyOptionRoutine());


				if (amount < _uiManager._minDownPayment || amount > _pManager._pCash)
				{
					Debug.Log("INSIDE DP IF: " + amount);

					_uiManager._tempCash -= amount;
					_uiManager._tempNotes += _uiManager._otbCost - amount;
					_uiManager.UpdateActionsPanelFunds(_uiManager._tempCash, _uiManager._tempNotes);
					_uiManager._transactionBlocked = true;
				}
				else
				{
					_uiManager._downPayment = amount;
					_uiManager._tempCash -= amount;
					_uiManager._tempNotes += _uiManager._otbCost - amount;
					_uiManager.UpdateActionsPanelFunds(_uiManager._tempCash, _uiManager._tempNotes);
					_uiManager._transactionBlocked = false;
				}
			}

			if(target.name == "Repay Loan Input")
			{
				_uiManager.UpdateActionsPanelFunds(_uiManager._tempCash -= amount, _uiManager._tempNotes -= amount);
				if (_uiManager._tempCash >= 0 && (_uiManager._tempNotes >= 0) && _uiManager._tempNotes <= 50000)
				{
					_uiManager._repayLoanAmount = amount;
					_uiManager._repayLoanButton.interactable = true;
				}
				else
					_uiManager._repayLoanButton.interactable = false;
			}

			if (target.name == "Get Loan Input")
			{
				//add 20% up-front loan fee
				_uiManager.UpdateActionsPanelFunds(_uiManager._tempCash += amount, _uiManager._tempNotes += amount + (int)(amount * 0.2f));
				if (_uiManager._tempCash >= 0 && (_uiManager._tempNotes >= 0) && _uiManager._tempNotes <= 50000)
				{
					_uiManager._loanAmount = amount;
					_uiManager._getLoanButton.interactable = true;
				}
				else
				{
					_uiManager._getLoanButton.interactable = false;
				}
			}

			if (target.name == "Forced Loan Input")
			{
				ResetTempFunds();
				_uiManager.UpdateForcedLoanFunds(_uiManager._tempCash += amount, _uiManager._tempNotes += amount + (int)(amount * 0.2f));
				_uiManager._loanAmount = amount;
			}

			if (target.name == "Selling Price Input")
			{
				Debug.Log("In IFM SELL OTB 2 PLAYER: " + amount);

				_uiManager._minSalePrice = _uiManager.GetSalePrice();

				if (amount >= _uiManager._minSalePrice)
				{
					_uiManager._sellTheOtbToPlayerButton.interactable = true;
					_uiManager._salePrice = amount;
				}
				else
				{
					_uiManager._sellTheOtbToPlayerButton.interactable = false;
				}
			}
		}
	}

	void ResetTempFunds()
	{
		_uiManager._tempCash = _pManager._pCash;
		_uiManager._tempNotes = _pManager._pNotes;
	}
	#endregion

	#region Private Methods


	#endregion
}
