using UnityEngine;
using System.Collections;

/// <summary>
/// Flashes a light to the beat received from a TunePlayer.
/// </summary>
public class BeatLight : MonoBehaviour {
	public enum Mode
	{
		Beat,
		Note
	}
	public TunePlayer player;
	public Mode mode = Mode.Note;
	public float maxIntensity = 1;
	public float minIntensity = 0;
	public Light targetLight;
	public float fadeSpeed = 1;
	
	void OnEnable()
	{
		player.OnBeat += OnBeat;
		player.OnPlayNote += OnPlayNote;
	}
	
	void OnDisable()
	{
		player.OnBeat -= OnBeat;
		player.OnPlayNote -= OnPlayNote;
	}
	
	void OnPlayNote(AudioSource source)
	{
		if (mode == Mode.Note) light.intensity = maxIntensity;
	}
	
	void OnBeat(int number)
	{
		if (mode == Mode.Beat) light.intensity = maxIntensity;
	}
	
	void Update ()
	{		
		light.intensity = Mathf.MoveTowards(light.intensity, minIntensity, fadeSpeed * Time.deltaTime);
	}
}
