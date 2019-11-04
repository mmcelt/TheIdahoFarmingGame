using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class PlayerSelection : MonoBehaviour
{

	#region Public / Serialized Fields

	public List<GameObject> selectableFarmers;
	public int farmerSelectionNumber;
	public string selectedFarmer;
	//[SerializeField] Button prevButton, nextButton;

	#endregion

	#region Private Fields / References

	bool farmerWasSelected;
	//int mySelectedFarmer;
	//int myPreviousSelectedFarmer;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		farmerSelectionNumber = 0;
		ActivatePlayer(farmerSelectionNumber);
	}

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnFarmerSelectedEvent;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnFarmerSelectedEvent;
	}

	#endregion

	#region Public Methods

	public void OnNextPlayerButtonClick()
	{
		farmerSelectionNumber++;

		if (farmerSelectionNumber >= selectableFarmers.Count)
			farmerSelectionNumber = 0;

		ActivatePlayer(farmerSelectionNumber);
	}

	public void OnPreviousPlayerButtonClick()
	{
		farmerSelectionNumber--;

		if (farmerSelectionNumber < 0)
			farmerSelectionNumber = selectableFarmers.Count - 1;

		ActivatePlayer(farmerSelectionNumber);
	}

	public void OnFarmerButtonClicked(int index)
	{
		//THIS IS USING RAISEVENTS...
		//local actions...
		//prevButton.interactable = false;
		//nextButton.interactable = false;
		
		SelectFarmer();


		//event data
		object[] data = new object[] { farmerSelectionNumber };
		//RaiseEventOptions
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.AddToRoomCache
		};
		//SendOptions
		SendOptions sendOptions = new SendOptions
		{
			Reliability = true
		};

		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Farmer_Selected_Event_Code, data, raiseEventOptions, sendOptions);
	}
	#endregion

	#region Private Methods

	void ActivatePlayer(int x)
	{
		foreach (GameObject farmer in selectableFarmers)
			farmer.SetActive(false);

		selectableFarmers[x].SetActive(true);
	}

	void SelectFarmer()
	{
		//set up farmer selection custom property
		selectedFarmer = selectableFarmers[farmerSelectionNumber].gameObject.GetComponentInChildren<Text>().text;
		ExitGames.Client.Photon.Hashtable farmerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Selected_Farmer, selectedFarmer } };

		PhotonNetwork.LocalPlayer.SetCustomProperties(farmerSelectionProp);
	}

	void OnFarmerSelectedEvent(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Farmer_Selected_Event_Code)
		{
			object[] data = (object[])eventData.CustomData;
			int selectedFarmer = (int)data[0];
			selectableFarmers[selectedFarmer].GetComponent<Button>().interactable = false;
		}
	}
	#endregion
}
