using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerListEntryInitializer : MonoBehaviourPun
{
	[Header("UI References")]
	public Text playerNameText;
	public GameObject[] selectFarmerButtons;
	public Button playerReadyButton;
	public Image playerReadyImage;


	bool isPlayerReady;
	string selectedFarmer;
	//int playerSelectionNumber;

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
	}
	#endregion

	#region UI Callbacks

	public void OnFarmerSelectionButtonClicked(int index)
	{
		if (!isPlayerReady)
		{
			int myPositionInPlayerList = PhotonNetwork.LocalPlayer.ActorNumber;

			//foreach(Player player in PhotonNetwork.PlayerList)
			//{
			//	Debug.Log("Player AN: " + player.ActorNumber);

			//}

			//deactivate all farmer buttons to prevent dual selection
			foreach (GameObject button in selectFarmerButtons)
			{
				button.GetComponent<Button>().interactable = false;
			}

			SetFarmer(index);

			//fire the Farmer_Selected Event...
			//event data
			object[] data = new object[] { index, myPositionInPlayerList };
			//event options
			RaiseEventOptions eventOptions = new RaiseEventOptions
			{
				Receivers = ReceiverGroup.All,
				CachingOption = EventCaching.AddToRoomCache
			};
			//send options
			SendOptions sendOptions = new SendOptions
			{
				Reliability = true
			};
			//fire the event...
			PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Farmer_Selected_Event_Code, data, eventOptions, sendOptions);

			playerReadyButton.interactable = true;
		}
	}
	#endregion

	#region Public Methods

	public void Initialize(int playerID, string playerName)
	{
		playerNameText.text = playerName;

		if (PhotonNetwork.LocalPlayer.ActorNumber != playerID)
		{
			//remote player
			playerReadyButton.gameObject.SetActive(false);
			playerReadyImage.enabled = false;
			//turn off the other player's farmer selection buttons
			foreach (GameObject button in selectFarmerButtons)
				button.GetComponent<Button>().interactable = false;
		}
		else
		{
			//I am the local player
			//set up Player_Ready custom property
			ExitGames.Client.Photon.Hashtable initialProps = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Ready, isPlayerReady } };
			PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);

			//set up Selected_Farmer custom property
			ExitGames.Client.Photon.Hashtable SelectedFarmerProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Selected_Farmer, selectedFarmer } };
			PhotonNetwork.LocalPlayer.SetCustomProperties(SelectedFarmerProp);

			playerReadyButton.onClick.AddListener(() =>
			{
				isPlayerReady = !isPlayerReady;
				SetPlayerReady(isPlayerReady);

				ExitGames.Client.Photon.Hashtable newProps = new ExitGames.Client.Photon.Hashtable() { { IFG.Player_Ready, isPlayerReady } };
				PhotonNetwork.LocalPlayer.SetCustomProperties(newProps);

				//Debug.Log("Is Player Ready? " + isPlayerReady);
			});
		}
	}

	public void SetPlayerReady(bool playerReady)
	{
		playerReadyImage.enabled = playerReady;

		if (playerReady)
		{
			playerReadyButton.GetComponentInChildren<Text>().text = "Ready!";
		}
		else
		{
			playerReadyButton.GetComponentInChildren<Text>().text = "Ready?";
		}
	}

	public void SetFarmer(int index)
	{
		//Debug.Log("In SetFarmer " + index);
		string farmer = "";

		//selectedFarmerImage.enabled = true;
		switch (index)
		{
			case 0:
				playerNameText.color = Color.blue;
				farmer = "Rigby Ron";
				break;
			case 1:
				playerNameText.color = Color.yellow;
				farmer = "Kimberly Kay";
				break;
			case 2:
				playerNameText.color = IFG.Purple;
				farmer = "Jerome Jerry";
				break;
			case 3:
				playerNameText.color = Color.black;
				farmer = "Ririe Ric";
				break;
			case 4:
				playerNameText.color = Color.red;
				farmer = "Menan Mike";
				break;
			case 5:
				playerNameText.color = Color.white;
				farmer = "Blackfoot Becky";
				break;
		}

		selectedFarmer = farmer;

		//set up farmer selection custom property
		ExitGames.Client.Photon.Hashtable farmerSelectionProp = new ExitGames.Client.Photon.Hashtable() { { IFG.Selected_Farmer, selectedFarmer } };

		PhotonNetwork.LocalPlayer.SetCustomProperties(farmerSelectionProp);

		//Debug.Log("Selected_Farmer: " + farmerSelectionProp.Values);
	}
	#endregion

	#region Private Methods

	void OnEvent(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Farmer_Selected_Event_Code)
		{
			object[] data = (object[])eventData.CustomData;
			int index = (int)data[0];		//the index of the affected button
			int position = (int)data[1];	//the position of the prefab in the list

			selectFarmerButtons[index].GetComponent<Button>().interactable = false;
			NetworkManager.Instance.playerListGameobjects[position].GetComponentInChildren<Text>().color = selectFarmerButtons[index].GetComponent<Image>().color;
		}
	}
	#endregion
}
