using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    private TreasurePickup[] AllTreasures;
    private bool[] RetrievedTreasures;
    private TreasurePickup CarriedTreasure = null;
	private TreasureUI UIObject;

	private void Start()
	{
		AllTreasures = FindObjectsOfType<TreasurePickup>();
		RetrievedTreasures = new bool[AllTreasures.Length];
		for (int i = 0; i < RetrievedTreasures.Length; ++i)
		{
			RetrievedTreasures[i] = false;
		}
		UIObject = FindObjectOfType<TreasureUI>();
		UIObject.UpdateUI(AllTreasures, RetrievedTreasures, CarriedTreasure);
	}

	public bool TryCarryTreasure(TreasurePickup treasure)
	{
		if (CarriedTreasure != null)
		{
			return false;
		}
		CarriedTreasure = treasure;
		UIObject.UpdateUI(AllTreasures, RetrievedTreasures, CarriedTreasure);
		return true;
	}

	public void DropOffTreasure()
	{
		if (CarriedTreasure == null)
		{
			Debug.LogError("DropOffTreasure called but CarriedTreasure is null");
			return;
		}
		int ind = -1;
		for (int i = 0; i < AllTreasures.Length; ++i)
		{
			if (AllTreasures[i] == CarriedTreasure)
			{
				ind = i;
				break;
			}
		}
		if (ind < 0)
		{
			Debug.LogError("DropOffTreasure failed to find CarriedTreasure in AllTreasures");
			return;
		}
		RetrievedTreasures[ind] = true;
		CarriedTreasure = null;
		UIObject.UpdateUI(AllTreasures, RetrievedTreasures, CarriedTreasure);
	}
}
