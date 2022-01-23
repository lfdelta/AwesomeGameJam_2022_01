using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip Jump;
	public AudioClip DoubleJump;
	public AudioClip Land;
	public AudioClip Swing;
	public AudioClip ReleaseSwing;
	public AudioClip FallTooFar;
	public AudioClip TreasurePickup;
	public AudioClip TreasurePickupFail;
	public AudioClip TreasureDropoff;
	public AudioClip AllTreasuresCollected;

	private AudioSource Source;

	private void Start()
	{
		Source = gameObject.AddComponent<AudioSource>();
	}

	public void PlaySound(AudioClip Sound)
	{
		Source.PlayOneShot(Sound);
	}
}
