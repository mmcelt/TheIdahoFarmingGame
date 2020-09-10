using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class StickerManager : MonoBehaviour
{
	#region Public / Serialized Fields

	[SerializeField] GameObject[] _rangeStickerPrefabs;
	[SerializeField] GameObject[] _cowStickerPrefabs;
	[SerializeField] GameObject[] _fruitStickerPrefabs;
	[SerializeField] GameObject[] _grainStickerPrefabs;
	[SerializeField] GameObject[] _hayStickerPrefabs;
	[SerializeField] GameObject[] _spudStickerPrefabs;
	[SerializeField] GameObject _harvesterStickerPrefab, _tractorStickerPrefab;
	[SerializeField] Transform[] _rangeStickerSpawnPoints;
	[SerializeField] Transform[] _beckyFarmStickerSpawnPoints;
	[SerializeField] Transform[] _jerryFarmStickerSpawnPoints;
	[SerializeField] Transform[] _kayFarmStickerSpawnPoints;
	[SerializeField] Transform[] _mikeFarmStickerSpawnPoints;
	[SerializeField] Transform[] _ricFarmStickerSpawnPoints;
	[SerializeField] Transform[] _ronFarmStickerSpawnPoints;
	#endregion

	#region Private Fields / References

	Vector2 _spawnPoint;
	GameObject _stickerPrefab;
	bool _fCowsKilled;

	#endregion

	#region Properties


	#endregion

	#region MonoBehaviour Methods

	void OnEnable()
	{
		PhotonNetwork.NetworkingClient.EventReceived += OnPlaceInitialBoardStickers;
	}

	void OnDisable()
	{
		PhotonNetwork.NetworkingClient.EventReceived -= OnPlaceInitialBoardStickers;
	}
	#endregion

	#region Public Methods

	public void PlaceRangeSticker(string farmer,string range,bool doubled, bool remove = false)
	{
		switch (farmer)
		{
			case IFG.Becky:
				_stickerPrefab = _rangeStickerPrefabs[0];
				SelectRange(range);
				break;

			case IFG.Jerry:
				_stickerPrefab = _rangeStickerPrefabs[1];
				SelectRange(range);
				break;

			case IFG.Kay:
				_stickerPrefab = _rangeStickerPrefabs[2];
				SelectRange(range);
				break;

			case IFG.Mike:
				_stickerPrefab = _rangeStickerPrefabs[3];
				SelectRange(range);
				break;

			case IFG.Ric:
				_stickerPrefab = _rangeStickerPrefabs[4];
				SelectRange(range);
				break;

			case IFG.Ron:
				_stickerPrefab = _rangeStickerPrefabs[5];
				SelectRange(range);
				break;
		}

		//remove any existing sticker
		RemoveOldSticker("RangeSticker", _spawnPoint);

		//spawn the sticker if not being removed due to sale...
		if (!remove)
		{
			if (!doubled)
				PhotonNetwork.Instantiate(_stickerPrefab.name, _spawnPoint, Quaternion.identity);
			else
				PhotonNetwork.Instantiate(_stickerPrefab.name, _spawnPoint, Quaternion.Euler(0, 0, -90));
		}
	}

	public void PlaceFarmSticker(string farmer,string sticker,int amount,bool doubled,bool fCowsKilled=false)
	{
		//Debug.Log("IN PLACESTICKER: STICKER: " + farmer + " " + sticker + " " + amount);
		//if (amount == 0 && sticker == "Cow")
		//{
		_fCowsKilled = fCowsKilled;
		//}

		if (sticker == "Fruit")
			amount /= 5;
		else
			amount /= 10;

		switch (farmer)
		{
			case IFG.Becky:
				SelectBeckySticker(sticker, amount);
				break;

			case IFG.Jerry:
				SelectJerrySticker(sticker, amount);
				break;

			case IFG.Kay:
				SelectKaySticker(sticker, amount);
				break;

			case IFG.Mike:
				SelectMikeSticker(sticker, amount);
				break;

			case IFG.Ric:
				SelectRicSticker(sticker, amount);
				break;

			case IFG.Ron:
				SelectRonSticker(sticker, amount);
				break;

			default:
				Debug.LogWarning("No such farmer! " + farmer);
				break;
		}
		//remove existing sticker
		RemoveOldSticker(sticker + "Sticker", _spawnPoint);

		if (amount > 0)
		{
			//spawn the new sticker
			//Debug.Log("SPAWNING: " + _stickerPrefab.name);

			if (!doubled && !_fCowsKilled)
			{
				//Debug.Log("INSIDE SPAWN 1ST IF: " + _stickerPrefab.name + " " + _spawnPoint);
				PhotonNetwork.Instantiate(_stickerPrefab.name, _spawnPoint, Quaternion.identity);
			}
			else if (doubled && !_fCowsKilled)
			{
				Debug.Log("INSIDE SPAWN 2ND IF");
				PhotonNetwork.Instantiate(_stickerPrefab.name, _spawnPoint, Quaternion.Euler(0, 0, -90));
			}
		}
		if (fCowsKilled)
			_fCowsKilled = false;
	}

	public void PlaceEquipmentSticker(string farmer,string sticker,bool owned)
	{
		Debug.Log("PLACE EQUIPT: " + farmer + " " + sticker + " " + owned);

		switch (farmer)
		{
			case IFG.Becky:
				SelectBeckyEquipmentSticker(sticker);
				break;

			case IFG.Jerry:
				SelectJerryEquipmentSticker(sticker);
				break;

			case IFG.Kay:
				SelectKayEquipmentSticker(sticker);
				break;

			case IFG.Mike:
				SelectMikeEquipmentSticker(sticker);
				break;

			case IFG.Ric:
				SelectRicEquipmentSticker(sticker);
				break;

			case IFG.Ron:
				SelectRonEquipmentSticker(sticker);
				break;
		}

		if (owned)
		{
			PhotonNetwork.Instantiate(_stickerPrefab.name, _spawnPoint, Quaternion.identity);
		}
		else
		{
			RemoveOldSticker(sticker + "Sticker", _spawnPoint);
		}
	}
	#endregion

	#region Private Methods

	void OnPlaceInitialBoardStickers(EventData eventData)
	{
		if (eventData.Code == (byte)RaiseEventCodes.Place_Initial_Stickers_Event_Code)
		{
			PlaceFarmSticker(GameManager.Instance.myFarmerName, "Hay", 10, false);
			PlaceFarmSticker(GameManager.Instance.myFarmerName, "Grain", 10, false);
		}
	}

	void SelectRange(string range)
	{
		switch (range)
		{
			case "Oxford":
				_spawnPoint = _rangeStickerSpawnPoints[0].position;
				break;

			case "Targhee":
				_spawnPoint = _rangeStickerSpawnPoints[1].position;
				break;

			case "Lemhi":
				_spawnPoint = _rangeStickerSpawnPoints[2].position;
				break;

			case "Lost River":
				_spawnPoint = _rangeStickerSpawnPoints[3].position;
				break;
		}
	}

	void SelectBeckySticker(string sticker, int amount)
	{
		switch (sticker)
		{
			case "Cow":
				if (_fCowsKilled)
					_stickerPrefab = _cowStickerPrefabs[0];
				else
					_stickerPrefab = _cowStickerPrefabs[amount - 1];
				_spawnPoint = _beckyFarmStickerSpawnPoints[0].position;
				break;

			case "Fruit":
				if (amount>0)
					_stickerPrefab = _fruitStickerPrefabs[amount - 1];
				_spawnPoint = _beckyFarmStickerSpawnPoints[1].position;
				break;

			case "Spuds":
				if(amount>0)
					_stickerPrefab = _spudStickerPrefabs[amount - 1];
				_spawnPoint = _beckyFarmStickerSpawnPoints[2].position;
				break;

			case "Grain":
				if(amount>0)
					_stickerPrefab = _grainStickerPrefabs[amount - 1];
				_spawnPoint = _beckyFarmStickerSpawnPoints[3].position;
				break;

			case "Hay":
				if(amount>0)
					_stickerPrefab = _hayStickerPrefabs[amount - 1];
				_spawnPoint = _beckyFarmStickerSpawnPoints[4].position;
				break;
		}
	}

	void SelectBeckyEquipmentSticker(string sticker)
	{
		switch (sticker)
		{
			case "Tractor":
				_stickerPrefab = _tractorStickerPrefab;
				_spawnPoint = _beckyFarmStickerSpawnPoints[5].position;
				break;

			case "Harvester":
				_stickerPrefab = _harvesterStickerPrefab;
				_spawnPoint = _beckyFarmStickerSpawnPoints[6].position;
				break;
		}
	}

	void SelectJerrySticker(string sticker, int amount)
	{
		switch (sticker)
		{
			case "Cow":
				if (_fCowsKilled)
					_stickerPrefab = _cowStickerPrefabs[0];
				else
					_stickerPrefab = _cowStickerPrefabs[amount - 1];
				_spawnPoint = _jerryFarmStickerSpawnPoints[0].position;
				break;

			case "Fruit":
				if (amount > 0)
					_stickerPrefab = _fruitStickerPrefabs[amount - 1];
				_spawnPoint = _jerryFarmStickerSpawnPoints[1].position;
				break;

			case "Spuds":
				if (amount > 0)
					_stickerPrefab = _spudStickerPrefabs[amount - 1];
				_spawnPoint = _jerryFarmStickerSpawnPoints[2].position;
				break;

			case "Grain":
				if (amount > 0)
					_stickerPrefab = _grainStickerPrefabs[amount - 1];
				_spawnPoint = _jerryFarmStickerSpawnPoints[3].position;
				break;

			case "Hay":
				if (amount > 0)
					_stickerPrefab = _hayStickerPrefabs[amount - 1];
				_spawnPoint = _jerryFarmStickerSpawnPoints[4].position;
				break;

			//case "Tractor":
			//	_stickerPrefab = _tractorStickerPrefab;
			//	_spawnPoint = _jerryFarmStickerSpawnPoints[5].position;
			//	break;

			//case "Harvester":
			//	_stickerPrefab = _harvesterStickerPrefab;
			//	_spawnPoint = _jerryFarmStickerSpawnPoints[6].position;
			//	break;
		}
	}

	void SelectJerryEquipmentSticker(string sticker)
	{
		switch (sticker)
		{
			case "Tractor":
				_stickerPrefab = _tractorStickerPrefab;
				_spawnPoint = _jerryFarmStickerSpawnPoints[5].position;
				break;

			case "Harvester":
				_stickerPrefab = _harvesterStickerPrefab;
				_spawnPoint = _jerryFarmStickerSpawnPoints[6].position;
				break;
		}
	}

	void SelectKaySticker(string sticker, int amount)
	{
		switch (sticker)
		{
			case "Cow":
				if (_fCowsKilled)
					_stickerPrefab = _cowStickerPrefabs[0];
				else
					_stickerPrefab = _cowStickerPrefabs[amount - 1];
				_spawnPoint = _kayFarmStickerSpawnPoints[0].position;
				break;

			case "Fruit":
				if (amount > 0)
					_stickerPrefab = _fruitStickerPrefabs[amount - 1];
				_spawnPoint = _kayFarmStickerSpawnPoints[1].position;
				break;

			case "Spuds":
				if (amount > 0)
					_stickerPrefab = _spudStickerPrefabs[amount - 1];
				_spawnPoint = _kayFarmStickerSpawnPoints[2].position;
				break;

			case "Grain":
				if (amount > 0)
					_stickerPrefab = _grainStickerPrefabs[amount - 1];
				_spawnPoint = _kayFarmStickerSpawnPoints[3].position;
				break;

			case "Hay":
				if (amount > 0)
					_stickerPrefab = _hayStickerPrefabs[amount - 1];
				_spawnPoint = _kayFarmStickerSpawnPoints[4].position;
				break;

			//case "Tractor":
			//	_stickerPrefab = _tractorStickerPrefab;
			//	_spawnPoint = _kayFarmStickerSpawnPoints[5].position;
			//	break;

			//case "Harvester":
			//	_stickerPrefab = _harvesterStickerPrefab;
			//	_spawnPoint = _kayFarmStickerSpawnPoints[6].position;
			//	break;
		}
	}

	void SelectKayEquipmentSticker(string sticker)
	{
		switch (sticker)
		{
			case "Tractor":
				_stickerPrefab = _tractorStickerPrefab;
				_spawnPoint = _kayFarmStickerSpawnPoints[5].position;
				break;

			case "Harvester":
				_stickerPrefab = _harvesterStickerPrefab;
				_spawnPoint = _kayFarmStickerSpawnPoints[6].position;
				break;
		}
	}

	void SelectMikeSticker(string sticker, int amount)
	{
		switch (sticker)
		{
			case "Cow":
				if (_fCowsKilled)
					_stickerPrefab = _cowStickerPrefabs[0];
				else
					_stickerPrefab = _cowStickerPrefabs[amount - 1];
				_spawnPoint = _mikeFarmStickerSpawnPoints[0].position;
				break;

			case "Fruit":
				if (amount > 0)
					_stickerPrefab = _fruitStickerPrefabs[amount - 1];
				_spawnPoint = _mikeFarmStickerSpawnPoints[1].position;
				break;

			case "Spuds":
				if (amount > 0)
					_stickerPrefab = _spudStickerPrefabs[amount - 1];
				_spawnPoint = _mikeFarmStickerSpawnPoints[2].position;
				break;

			case "Grain":
				if (amount > 0)
					_stickerPrefab = _grainStickerPrefabs[amount - 1];
				_spawnPoint = _mikeFarmStickerSpawnPoints[3].position;
				break;

			case "Hay":
				if (amount > 0)
					_stickerPrefab = _hayStickerPrefabs[amount - 1];
				_spawnPoint = _mikeFarmStickerSpawnPoints[4].position;
				break;

			//case "Tractor":
			//	_stickerPrefab = _tractorStickerPrefab;
			//	_spawnPoint = _mikeFarmStickerSpawnPoints[5].position;
			//	break;

			//case "Harvester":
			//	_stickerPrefab = _harvesterStickerPrefab;
			//	_spawnPoint = _mikeFarmStickerSpawnPoints[6].position;
			//	break;
		}
	}

	void SelectMikeEquipmentSticker(string sticker)
	{
		switch (sticker)
		{
			case "Tractor":
				_stickerPrefab = _tractorStickerPrefab;
				_spawnPoint = _mikeFarmStickerSpawnPoints[5].position;
				break;

			case "Harvester":
				_stickerPrefab = _harvesterStickerPrefab;
				_spawnPoint = _mikeFarmStickerSpawnPoints[6].position;
				break;
		}
	}

	void SelectRicSticker(string sticker, int amount)
	{
		switch (sticker)
		{
			case "Cow":
				if (_fCowsKilled)
					_stickerPrefab = _cowStickerPrefabs[0];
				else
					_stickerPrefab = _cowStickerPrefabs[amount - 1];
				_spawnPoint = _ricFarmStickerSpawnPoints[0].position;
				break;

			case "Fruit":
				if (amount > 0)
					_stickerPrefab = _fruitStickerPrefabs[amount - 1];
				_spawnPoint = _ricFarmStickerSpawnPoints[1].position;
				break;

			case "Spuds":
				if (amount > 0)
					_stickerPrefab = _spudStickerPrefabs[amount - 1];
				_spawnPoint = _ricFarmStickerSpawnPoints[2].position;
				break;

			case "Grain":
				if (amount > 0)
					_stickerPrefab = _grainStickerPrefabs[amount - 1];
				_spawnPoint = _ricFarmStickerSpawnPoints[3].position;
				break;

			case "Hay":
				if (amount > 0)
					_stickerPrefab = _hayStickerPrefabs[amount - 1];
				_spawnPoint = _ricFarmStickerSpawnPoints[4].position;
				break;

			//case "Tractor":
			//	_stickerPrefab = _tractorStickerPrefab;
			//	_spawnPoint = _ricFarmStickerSpawnPoints[5].position;
			//	break;

			//case "Harvester":
			//	_stickerPrefab = _harvesterStickerPrefab;
			//	_spawnPoint = _ricFarmStickerSpawnPoints[6].position;
			//	break;
		}
	}

	void SelectRicEquipmentSticker(string sticker)
	{
		switch (sticker)
		{
			case "Tractor":
				_stickerPrefab = _tractorStickerPrefab;
				_spawnPoint = _ricFarmStickerSpawnPoints[5].position;
				break;

			case "Harvester":
				_stickerPrefab = _harvesterStickerPrefab;
				_spawnPoint = _ricFarmStickerSpawnPoints[6].position;
				break;
		}
	}

	void SelectRonSticker(string sticker, int amount)
	{
		Debug.Log("IN SELECTSTICKER: " + sticker + " " + amount);

		switch (sticker)
		{
			case "Cow":
				if (_fCowsKilled)
					_stickerPrefab = _cowStickerPrefabs[0];
				else
					_stickerPrefab = _cowStickerPrefabs[amount - 1];
				_spawnPoint = _ronFarmStickerSpawnPoints[0].position;
				break;

			case "Fruit":
				if (amount > 0)
					_stickerPrefab = _fruitStickerPrefabs[amount - 1];
				_spawnPoint = _ronFarmStickerSpawnPoints[1].position;
				break;

			case "Spuds":
				if (amount > 0)
					_stickerPrefab = _spudStickerPrefabs[amount - 1];
				_spawnPoint = _ronFarmStickerSpawnPoints[2].position;
				break;

			case "Grain":
				if (amount > 0)
					_stickerPrefab = _grainStickerPrefabs[amount - 1];
				_spawnPoint = _ronFarmStickerSpawnPoints[3].position;
				break;

			case "Hay":
				if (amount > 0)
					_stickerPrefab = _hayStickerPrefabs[amount - 1];
				_spawnPoint = _ronFarmStickerSpawnPoints[4].position;
				break;

			//case "Tractor":
			//	_stickerPrefab = _tractorStickerPrefab;
			//	_spawnPoint = _ronFarmStickerSpawnPoints[5].position;
			//	break;

			//case "Harvester":
			//	_stickerPrefab = _harvesterStickerPrefab;
			//	_spawnPoint = _ronFarmStickerSpawnPoints[6].position;
			//	break;
		}
	}

	void SelectRonEquipmentSticker(string sticker)
	{
		switch (sticker)
		{
			case "Tractor":
				_stickerPrefab = _tractorStickerPrefab;
				_spawnPoint = _ronFarmStickerSpawnPoints[5].position;
				break;

			case "Harvester":
				_stickerPrefab = _harvesterStickerPrefab;
				_spawnPoint = _ronFarmStickerSpawnPoints[6].position;
				break;
		}
	}

	void RemoveOldSticker(string tag, Vector3 location)
	{
		Vector3 spawnLocation = location;

		foreach (GameObject sticker in GameObject.FindGameObjectsWithTag(tag))
		{
			if (sticker.transform.position == spawnLocation)
			{
				PhotonNetwork.Destroy(sticker);
				break;
			}
		}
	}

	#endregion
}
