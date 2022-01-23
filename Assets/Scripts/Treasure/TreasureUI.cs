using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureUI : MonoBehaviour
{
	public GameObject IconPrefab;
	public float IconPadding = 10.0f;
	public Texture2D HiddenTreasureIcon;
	public Texture2D CollectedTreasureIcon;
	public Texture2D CarryingTreasureIcon;

	private RawImage[] IconRenderers;

	private void InitializeSprites(int NumIcons)
	{
		Canvas canvas = FindObjectOfType<Canvas>();
		if (!canvas)
		{
			Debug.LogError("TreasureUI Initialize found no Canvas");
			return;
		}
		IconRenderers = new RawImage[NumIcons];
		for (int i = 0; i < NumIcons; ++i)
		{
			GameObject newIcon = Instantiate(IconPrefab, canvas.transform);
			IconRenderers[i] = newIcon.GetComponent<RawImage>();
			RectTransform iconRect = newIcon.GetComponent<RectTransform>();
			if (iconRect)
			{
				iconRect.anchoredPosition = new Vector2((iconRect.rect.width + IconPadding) * i, 0.0f);
			}
		}
	}

	public void UpdateUI(TreasurePickup[] treasures, bool[] collected, TreasurePickup carried)
	{
		if (IconRenderers == null)
		{
			InitializeSprites(treasures.Length);
		}
		if (IconRenderers.Length != treasures.Length)
		{
			Debug.LogError("TreasureUI Update found bad number of treasures");
			return;
		}
		for (int i = 0; i < treasures.Length; ++i)
		{
			if (treasures[i] == carried)
			{
				IconRenderers[i].texture = CarryingTreasureIcon;
			}
			else if (collected[i])
			{
				IconRenderers[i].texture = CollectedTreasureIcon;
			}
			else
			{
				IconRenderers[i].texture = HiddenTreasureIcon;
			}
		}
	}
}
