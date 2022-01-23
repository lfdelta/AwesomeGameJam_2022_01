using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TreasureDropoff : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		GlobalManager mgr = FindObjectOfType<GlobalManager>();
		if (mgr != null)
		{
			mgr.TryDropOffTreasure();
		}
	}
}
