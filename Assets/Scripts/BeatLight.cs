using UnityEngine;
using System.Collections;

public class BeatLight : MonoBehaviour {
	public TunePlayer player;
	public float maxIntensity = 1;
	public float minIntensity = 0;
	public float fadeSpeed = 4;
	public Light targetLight;
	
	void OnEnable()
	{
		player.OnBeat += OnBeat;
	}
	
	void OnDisable()
	{
		player.OnBeat -= OnBeat;
	}
	
	void OnBeat(int number)
	{
		light.intensity = maxIntensity;
	}
	
	void Update ()
	{
		light.intensity = Mathf.MoveTowards(light.intensity, minIntensity, maxIntensity * player.bpm * Time.deltaTime / 60);
	}
}
