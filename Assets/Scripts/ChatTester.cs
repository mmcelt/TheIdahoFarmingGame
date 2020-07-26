using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatTester : MonoBehaviour
{
	#region Fields

	[SerializeField] GameObject _chatPrefab;

	#endregion

	#region MonoBehaviour Methods

	void Start() 
	{
		Instantiate(_chatPrefab);
	}
	
	void Update() 
	{
		
	}
	#endregion

	#region Public Methods


	#endregion

	#region Private Methods


	#endregion
}
