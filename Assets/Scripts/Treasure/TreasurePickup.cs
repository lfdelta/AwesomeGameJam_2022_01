using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TreasurePickup : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<PlayerController2D>() != null)
		{
			GlobalManager mgr = FindObjectOfType<GlobalManager>();
			if (mgr != null)
			{
				if (mgr.TryCarryTreasure(this))
				{
					gameObject.SetActive(false);
				}
			}
		}
	}
}
