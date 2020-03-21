using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using TMPro;

[SerializeField]
public class MyDiceRoll : MonoBehaviour
{
	enum RollStarte
	{
		None = 0,
		Roll,
		Stop,
	};

	[SerializeField] int m_pip = -1;
	[SerializeField] Text buttonTxt;
	[SerializeField] Vector2 rollVec = Vector2.one * 200f;
	[SerializeField] AudioClip _dieRoll;
	[SerializeField] float stopRollAmount = 10f;
	[SerializeField] float startMoveAmount = 0.25f;

	public int Pip { get { return m_pip; } }

	public bool isOtherRoll;
	public bool isHarvestRoll;
	public bool isTetonDamRoll;			//used as a roll type designator
	public bool tetonDamRollComplete;	//used to coroutine wait point

	RollStarte state;
	Animator anm;
	Rigidbody2D rb;
	AudioSource ac;
	Vector3 defPos;
	bool toRoll;

	SpriteRenderer _sprite;
	PlayerMove _pMove;
	UIManager _uiManager;
	PlayerManager _pManager;
	HarvestManager _hManager;

	Button harvestOkButton;
	Text harvestOkButtonText;
	Button harvestRollButton;
	TextMeshProUGUI tetonDamMessageText;

	// Use this for initialization
	void Start()
	{
		anm = GetComponent<Animator>();
		ac = GetComponent<AudioSource>();
		rb = GetComponent<Rigidbody2D>();

		_uiManager = GameManager.Instance.uiManager;
		harvestRollButton = _uiManager._harvestRollButton;
		harvestOkButton = _uiManager._harvestOk1Button;
		buttonTxt = _uiManager._rollButton.GetComponentInChildren<Text>();
		harvestRollButton = _uiManager._harvestRollButton;
		harvestOkButtonText = _uiManager._harvestOk1Button.GetComponentInChildren<Text>();
		tetonDamMessageText = _uiManager._tetonMessageText;

		_sprite = GetComponent<SpriteRenderer>();
		_pMove = GameManager.Instance.myFarmer.GetComponent<PlayerMove>();
		_pManager = GameManager.Instance.myFarmer.GetComponent<PlayerManager>();
		_hManager = GameManager.Instance.hManager;

		rb.isKinematic = true;
		defPos = transform.position;
		toRoll = false;
		StartCoroutine(diceRollCo());
	}

	void OnCollisionEnter2D(Collision2D collision)
	{
		//Debug.Log(rb.velocity.sqrMagnitude);
		if(ac != null)
			ac.Play();

		if(rb != null)
		{
			if (rb.velocity.sqrMagnitude < stopRollAmount)  //10
			{
				state = RollStarte.Stop;
			}
			if (rb.velocity.sqrMagnitude < startMoveAmount) //0.25f/4.5f
			{
				StartCoroutine(StartMove());
			}
		}
	}

	IEnumerator StartMove()
	{
		yield return new WaitForSeconds(1.0f);
		if (!isOtherRoll)
		{
			//Debug.Log("MOVE! " + Pip);
			_pMove.InitMove(Pip);
		}
		yield return new WaitForSeconds(0.95f);
		_sprite.enabled = false;
	}

	IEnumerator diceRollCo()
	{
		tetonDamRollComplete = false;

		while (true)
		{
			if(!isOtherRoll)
				buttonTxt.text = "ROLL";
			if (isHarvestRoll)
			{
				harvestOkButtonText.text = "";
				harvestOkButton.interactable = false;
			}

			rb.isKinematic = true;
			transform.position = defPos;
			state = RollStarte.Stop;
			m_pip = -1;
			while (!toRoll)
			{
				yield return null;
			}
			state = RollStarte.Roll;
			if (state == RollStarte.Roll)
			{
				_sprite.color = ChoosePlayerDieTint();
				_sprite.enabled = true;
			}

			rb.isKinematic = false;
			rb.velocity = Vector2.zero;
			rb.AddForce(rollVec);

			Roll();

			while (state == RollStarte.Roll)
			{
				yield return null;
			}

			SetPip(Random.Range(0, 6) + 1);

			yield return new WaitForSeconds(0.75f);

			if (!isOtherRoll)
				buttonTxt.text = Pip.ToString();
			else if (isHarvestRoll)
			{
				harvestOkButtonText.text = Pip.ToString();
				harvestOkButton.interactable = true;

				//send harvest roll msg to others
				_hManager._dieRoll = Pip;
				//SendHarvestRollMessage();	//MOVE TO HARVEST MANAGER
				_hManager._rollButtonPressed = true;
				yield return new WaitForSeconds(1.2f);
				harvestOkButtonText.text = "OK";
			}
			else if (isTetonDamRoll)
			{
				tetonDamRollComplete = true;
			}

			toRoll = false;
			while (!toRoll)
			{
				yield return null;
			}
			buttonTxt.text = "ROLL";
			harvestOkButtonText.text = "OK";
		}
	}

	public void OnRollButton()
	{
		toRoll = true;
		ac.PlayOneShot(_dieRoll);
	}

	/// <summary>
	/// Start roll animation
	/// </summary>
	public void Roll()
	{
		anm.Play("roll");
	}

	/// <summary>
	/// Set pip and start pip animation
	/// </summary>
	/// <param name="_pip">pip of dice</param>
	public void SetPip(int _pip)
	{
		m_pip = Mathf.Clamp(_pip, 1, 7);
		anm.Play("to" + m_pip.ToString());
	}

	Color ChoosePlayerDieTint()
	{
		Color dieTint = Color.white;

		switch (GameManager.Instance.myFarmerName)
		{
			case IFG.Becky:
				dieTint = Color.white;
				break;

			case IFG.Jerry:
				dieTint = new Color(0.6923f, 0.5518f, 0.7558f);
				break;

			case IFG.Kay:
				dieTint = new Color(0.8773f, 0.8773f, 0.4345f);
				break;

			case IFG.Mike:
				dieTint = new Color(0.8679f, 0.4134f, 0.4134f);
				break;

			case IFG.Ric:
				dieTint = new Color(0.5188f, 0.5188f, 0.5188f);
				break;

			case IFG.Ron:
				dieTint = new Color(0.5047f, 0.5047f, 1);
				break;
		}

		return dieTint;
	}

	void SendHarvestRollMessage()
	{
		//data - nickname, farmerName, die
		object[] sndData = new object[] { PhotonNetwork.LocalPlayer.NickName, GameManager.Instance.myFarmerName, Pip };
		//event options
		RaiseEventOptions eventOptions = new RaiseEventOptions()
		{
			Receivers = ReceiverGroup.Others,
			CachingOption = EventCaching.DoNotCache
		};
		//send options
		SendOptions sendOptions = new SendOptions() { Reliability = true };
		//fire the event to the UIManagers
		PhotonNetwork.RaiseEvent((byte)RaiseEventCodes.Harvest_Roll_Message_Event_Code, sndData, eventOptions, sendOptions);
	}
}
