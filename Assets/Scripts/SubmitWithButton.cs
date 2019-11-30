using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Submits an InputField with the specified button.</summary>
//Prevents MonoBehaviour of same type (or subtype) being added more than once to a GameObject.
[DisallowMultipleComponent]
//This automatically adds required components as dependencies.
[RequireComponent(typeof(InputField))]
public class SubmitWithButton : MonoBehaviour
{
	public string _submitKey = "Submit";
	public bool _trimWhitespace = true;
	public UIManager _uiManager;

	InputField _inputField;

	// Use this for initialization
	void Start()
	{
		_inputField = GetComponent<InputField>();

		_inputField.onEndEdit.AddListener(fieldValue =>
		{
			if (_trimWhitespace)
				_inputField.text = fieldValue = fieldValue.Trim();
			if (Input.GetButton(_submitKey))
				validateAndSubmit(fieldValue);
		});
	}

	private bool isInvalid(string fieldValue)
	{
		// change to the validation you want
		return SelectCorrectValidation(_inputField.name);
	}
	private void validateAndSubmit(string fieldValue)
	{
		if (isInvalid(fieldValue))
			return;

		// change to the submit code
		SelectCorrectAction(_inputField.name);
	}
	// to be called from a submit button onClick event
	public void validateAndSubmit()
	{
		validateAndSubmit(_inputField.text);
	}

	private void SelectCorrectAction(string target)
	{
		switch (target)
		{
			//TODO: IMPLEMENT SUBMIT W/BUTTON
			case "Repay Loan Input":
				_uiManager.OnRepayLoanButtonClicked();
				break;

			case "Get Loan Input":
				_uiManager.OnGetLoanButtonClicked();
				break;

			case "Downpayment Input":
				_uiManager.OnBuyOptionButtonClicked();
				break;

			case "Forced Loan Input":
				_uiManager.OnGetFocedLoanButtonClicked();
				break;

			case "Selling Price Input":
				_uiManager.OnDoOtbSaleToPlayerButtonClicked();
				break;
		}
		//_uiManager._actionsPanel.SetActive(false);
		_uiManager._actionsPanel.GetComponent<DOTweenAnimation>().DOPlayBackwards();
	}

	private bool SelectCorrectValidation(string target)
	{
		//Debug.Log("IN Validation " + target);
		switch (target)
		{
			case "Repay Loan Input":
				if (!_uiManager._repayLoanButton.interactable)
				{
					return true;
				}
				break;

			case "Get Loan Input":
				if (!_uiManager._getLoanButton.interactable)
				{
					return true;
				}
				break;

			case "Downpayment Input":
				if (!_uiManager._buyOptionButton.interactable)
				{
					return true;
				}
				break;

			case "Selling Price Input":
				if (!_uiManager._sellTheOtbToPlayerButton.interactable)
				{
					return true;
				}
				break;

			//case "FLInputField":
			//	if (!_uiManager._getFLButton.interactable)
			//	{
			//		return true;
			//	}
			//	break;
		}
		return false;
	}
}
