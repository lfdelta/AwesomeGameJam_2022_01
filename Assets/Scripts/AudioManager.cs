using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip Music;
	public float MusicVolume = 0.3f;
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

	private AudioSource AmbientSource;
	private AudioSource SFXSource;

	private void Start()
	{
		SFXSource = gameObject.AddComponent<AudioSource>();
		AmbientSource = gameObject.AddComponent<AudioSource>();

		AmbientSource.loop = true;
		AmbientSource.clip = Music;
		AmbientSource.Play();
	}

	public void PlaySound(AudioClip Sound, float Volume = 1.0f)
	{
		SFXSource.PlayOneShot(Sound, Volume);
	}
}
