using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class BoxPlatformUtil : MonoBehaviour
{
    public Vector2 Size = new Vector2(10.0f, 10.0f);

	private void OnValidate()
	{
		SpriteRenderer render = GetComponent<SpriteRenderer>();
		render.size = Size;
		BoxCollider2D collider = GetComponent<BoxCollider2D>();
		collider.size = Size;
	}
}
