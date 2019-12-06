using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
	#region Public / Serialized Fields


	#endregion

	#region Private Fields / References


	#endregion

	#region Properties

	//ALL THESE HAVE TO BE CUSTOM PLAYER PROPERTIES IN THE GAME FOR SETTING & RETRIEVAL
	public int NumberOfPlayers { get; set; }
	public List<OTBCard> OtbDeck { get; set; }
	public List<FFCard> FfDeck { get; set; }
	public List<OECard> OeDeck { get; set; }
	public bool ActivePlayer { get; set; }
	public bool HasRolled { get; set; }
	public string[] PlayerNames { get; set; }
	public string[] FarmerNames { get; set; }
	public int[] CurrentPositions { get; set; }
	public int[] CurrentBoardSpaces { get; set; }

	#endregion

	#region Public Methods

	public void SaveGame()
	{

	}

	public void LoadGame()
	{

	}
	#endregion

	#region Private Methods


	#endregion
}
