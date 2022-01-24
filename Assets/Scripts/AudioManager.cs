using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip Music;
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
		PlaySound(Music, 0.3f);
	}

	public void PlaySound(AudioClip Sound, float Volume = 1.0f)
	{
		Source.PlayOneShot(Sound, Volume);
	}
}
