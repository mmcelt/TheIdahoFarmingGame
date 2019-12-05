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
	public string[] PlayerNames { get; set; }
	public string[] FarmerNames { get; set; }
	public int[] CurrentPositions { get; set; }
	public int[] CurrentBoardSpaces { get; set; }
	public bool ActivePlayer { get; set; }
	public bool HasRolled { get; set; }

	#endregion

	#region Public Methods


	#endregion

	#region Private Methods


	#endregion
}
