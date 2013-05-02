using UnityEngine;
using System.Collections;
using BeatGeneral;

public class TunePlayer : MonoBehaviour
{
	public const int maxNotes = 32;
	
	public event System.Action<int> OnBeat;
	public event System.Action<AudioSource> OnPlayNote;
	
	public AudioClip baseSound;
	public GeneratorSettings generatorSettings;
	public TuningSettings tuningSettings;

	public float bpm = 60;
	public int maxNotesPerBeat = 4;
	public int octaves = 1;
	public float pitchOffset = 1;
		
	AudioSource[] audioSources;
	float timer = 0;
	float interval = 1;
	int beatCount = 0;
	int noteCount = 0;
	int[][] tune;
	
	void Start ()
	{
		Init();
	}
	
	public void Init()
	{
		SetInterval(bpm, maxNotesPerBeat);
		timer = interval;
		
		// Destroy old audio sources
		if (audioSources != null)
		{
			for (int i = 0; i < audioSources.Length; i++)
			{
				Destroy(audioSources[i].gameObject);
			}
		}
		
		float[] pitches = Tuning.GetPitches(tuningSettings);
		
		if (octaves < 1) octaves = 1;
		
		// Create audio sources
		audioSources = new AudioSource[Mathf.Min(generatorSettings.tracks, Mathf.Min(pitches.Length * octaves, maxNotes))];

		int index = 0;
		for (int o = 0; o < octaves; o++)
		{
			for (int i = 0; index < audioSources.Length && index < generatorSettings.tracks && i < pitches.Length; i++)
			{
				GameObject go = new GameObject("NotePlayer-" + index);
				audioSources[index] = go.AddComponent<AudioSource>();
				audioSources[index].pitch = pitches[i] * (o + 1) * pitchOffset;
				audioSources[index].clip = baseSound;
				audioSources[index].transform.parent = transform;
				audioSources[index].playOnAwake = false;
				index ++;
			}
		}
		
		BeatGenerator generator = new BeatGenerator(generatorSettings);
		tune = generator.GenerateTune();
	}
	
	void Update ()
	{		
		timer -= Time.deltaTime;
		
		if (timer <= 0) PlayNote ();
	}
	
	void PlayNote()
	{
#if UNITY_EDITOR
		// Allow tweaking interval in editor during play mode
		SetInterval(bpm, maxNotesPerBeat);
#endif
		timer += interval;
		
		int noteIndex = noteCount % tune.Length;
		for (int i = 0; i < tune[noteIndex].Length && i < audioSources.Length; i++)
		{
			if (tune[noteIndex][i] > 0.1f)
			{
				audioSources[i].volume = tune[noteIndex][i];
				audioSources[i].Play();
			}
			if (OnPlayNote != null) OnPlayNote(audioSources[i]);
		}
		
		noteCount ++;
		
		if (noteCount % maxNotesPerBeat == 1) Beat ();
		
		// If interval is less then framerate, play more notes
		//if (timer <= 0) PlayNote();
	}
	
	void Beat()
	{
		beatCount ++;
		
		if (OnBeat != null) OnBeat(beatCount);
	}
	
	public void SetInterval(float bpm, int maxNotesPerBeat)
	{
		if (bpm <= float.MinValue) bpm = float.MinValue;
		if (maxNotesPerBeat < 1) maxNotesPerBeat = 1;
		
		this.bpm = bpm;
		this.maxNotesPerBeat = maxNotesPerBeat;
		
		interval = 60f / (bpm * maxNotesPerBeat);
	}
}
