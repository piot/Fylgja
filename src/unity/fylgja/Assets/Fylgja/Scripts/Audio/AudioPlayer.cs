using UnityEngine;
using System.Collections;

public class AudioPlayer : AudioHandler
{
	public bool destroyAfterPlaying = false;
	public float minTimeInterval = 10;
	public float maxTimeInterval = 30;
	private float playTimer;
	private float intervalTimer;
	private AudioClip intervalClip;
	private bool hasInterval = true;

	public override void Awake()
	{
		if (minTimeInterval == 0 && maxTimeInterval == 0)
		{
			hasInterval = false;

			if (playAutomatically)
			{
				TriggerSound();

				if (destroyAfterPlaying)
				{
					StartCoroutine("PlayThenDestroy");
				}
			}
		}
		else
		{
			if (playAutomatically)
			{
				StartCoroutine("PlayInterval");
			}
		}
	}

	void OnTriggerEnter(Collider c)
	{
		// Debug.Log(gameObject.name + " triggered by " + c.name);
		if (!GetComponent<AudioSource>().isPlaying && !hasInterval)
		{
			TriggerSound();

			if (destroyAfterPlaying)
			{
				StartCoroutine("PlayThenDestroy");
			}
		}

		if (hasInterval)
		{
			StartCoroutine("PlayInterval");
		}
	}

	IEnumerator PlayInterval()
	{
		playTimer = Random.Range(minTimeInterval, maxTimeInterval);
		intervalClip = PickSound();
		intervalTimer = intervalClip.length + playTimer;
		GetComponent<AudioSource>().clip = intervalClip;
		yield return new WaitForSeconds(playTimer);
		GetComponent<AudioSource>().Play();
		yield return new WaitForSeconds(intervalTimer);

		StartCoroutine("PlayInterval");
	}

	IEnumerator PlayThenDestroy()
	{
		yield return new WaitForSeconds(GetComponent<AudioSource>().clip.length);

		Destroy(gameObject);
	}

	void OnTriggerExit(Collider c)
	{
		//      Debug.Log(gameObject.name + " stopped playing by " + c.name);
		if (!destroyAfterPlaying && GetComponent<AudioSource>().isPlaying)
		{
			GetComponent<AudioSource>().Stop();
		}
		StopCoroutine("PlayInterval");
	}
}
