using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	#region Public / Serialized Fields

	[Header("Main Sounds")]
	public AudioClip _incomeTax;
	public AudioClip _yourTurn;
	public AudioClip _neenerNeener;
	public AudioClip _bad;
	public AudioClip _good;
	public AudioClip _garnished;
	public AudioClip _droughtYear;
	public AudioClip _tetonDam;
	public AudioClip _gotoSpringPlanting;
	public AudioClip _getOnYourTractor;
	public AudioClip _ff;
	public AudioClip _otb;
	public AudioClip _move;
	public AudioClip _passGo;
	public AudioClip _goBonus;
	public AudioClip _hurtBack;
	public AudioClip _farmingLikeAnIdiot;
	public AudioClip _shuffle;

	[Header("UI Sounds")]
	public AudioClip _buttonClick;
	public AudioClip _lifeAlteringButtonClick;

	[HideInInspector] public AudioSource _aSource;

	#endregion

	#region Private Fields / References


	#endregion

	#region Properties

	public static AudioManager Instance;

	#endregion

	#region MonoBehaviour Methods

	void Awake()
	{
		if (Instance == null)
			Instance = this;
		else if (Instance != this)
			Destroy(gameObject);
	}

	void Start() 
	{
		_aSource = GetComponent<AudioSource>();
	}
	#endregion

	#region Public Methods

	public void PlaySound(AudioClip clip, float volume = 1)
	{
		_aSource.PlayOneShot(clip, volume);
	}
	#endregion

	#region Private Methods


	#endregion
}
