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
		AudioManager audio = FindObjectOfType<AudioManager>();
		if (CarriedTreasure != null)
		{
			audio.PlaySound(audio.TreasurePickupFail);
			return false;
		}
		CarriedTreasure = treasure;
		UIObject.UpdateUI(AllTreasures, RetrievedTreasures, CarriedTreasure);
		audio.PlaySound(audio.TreasurePickup);
		return true;
	}

	public void TryDropOffTreasure()
	{
		if (CarriedTreasure == null)
		{
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

		bool allTreasuresCollected = true;
		for (int i = 0; i < RetrievedTreasures.Length; ++i)
		{
			if (RetrievedTreasures[i] == false)
			{
				allTreasuresCollected = false;
				break;
			}
		}
		AudioManager audio = FindObjectOfType<AudioManager>();
		if (allTreasuresCollected)
		{
			audio.PlaySound(audio.AllTreasuresCollected, 0.3f);
		}
		else
		{
			audio.PlaySound(audio.TreasureDropoff);
		}
	}
}
