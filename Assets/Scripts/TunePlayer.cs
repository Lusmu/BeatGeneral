using UnityEngine;
using System.Collections;
using BeatGeneral;

/// <summary>
/// Tune player to play tunes created by BeatGenerator.
/// </summary>
public class TunePlayer : MonoBehaviour
{
	/// <summary>
	/// Max notes (audio sources) that can be created.
	/// </summary>
	public const int maxNotes = 32;
	
	/// <summary>
	/// Occurs on every beat.
	/// For eacmple when bpm is 60, occurs once a second,
	/// regardless if there are any notes playing.
	/// </summary>
	public event System.Action<int> OnBeat;
	/// <summary>
	/// Occurs every time a note is player.
	/// Sends as a parameter the audio source playing the note.
	/// </summary>
	public event System.Action<AudioSource> OnPlayNote;
	/// <summary>
	/// Occurs when init has been done and playing is about to commence.
	/// </summary>
	public event System.Action OnInitDone;
	
	/// <summary>
	/// The base audio clip that whose pitch gets modified.
	/// </summary>
	public AudioClip baseSound;
	
	public float volume = 1;
	
	public GeneratorSettings generatorSettings;
	public TuningSettings tuningSettings;
	
	/// <summary>
	/// Beat per minute.
	/// </summary>
	public float bpm = 60;
	
	/// <summary>
	/// Ticks per beat.
	/// Ticks per minute are bpm * ticksPerBeat.
	/// </summary>
	public int ticksPerBeat = 4;
	
	/// <summary>
	/// How many octaves of note players to create.
	/// For example, if pentatonic tuning and scale is used,
	/// each octave has 5 notes, and setting octaves to 2
	/// will create 15 audio sources, if there are that many
	/// tracks in the beat generator settings.
	/// </summary>
	public int octaves = 1;
	
	/// <summary>
	/// Multiply the pitch of each audio source by this number.
	/// </summary>
	public float pitchOffset = 1;
		
	/// <summary>
	/// The audio sources that play the notes.
	/// Public, so can be accessed from script, but nonserializable,
	/// and shouldn't be set from editor.
	/// </summary>
	[System.NonSerializedAttribute]
	public AudioSource[] audioSources;
	
	float timer = 0;
	float interval = 1;
	int beatCount = 0;
	int noteCount = 0;
	int[][] tune;
	BeatGenerator generator;
	int[] currentNotes;
	
	void Start ()
	{
		Init();
	}
	
	public void Init()
	{
		SetInterval(bpm, ticksPerBeat);
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
		
		// 0 or less octaves don't make sense
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
		
		generator = new BeatGenerator(generatorSettings);
		if (generatorSettings.trackLength > 0) tune = generator.GenerateTune();
		
		if (OnInitDone != null) OnInitDone();
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
		SetInterval(bpm, ticksPerBeat);
#endif
		timer += interval;
		
		if (tune != null)
		{
			currentNotes = tune[noteCount % tune.Length];
		}
		else
		{
			currentNotes = generator.GetNextNotes(currentNotes);
		}
		
		for (int i = 0; i < currentNotes.Length && i < audioSources.Length; i++)
		{
			if (currentNotes[i] > 0.1f)
			{
				audioSources[i].volume = currentNotes[i] * volume;
				audioSources[i].Play();
				if (OnPlayNote != null) OnPlayNote(audioSources[i]);
			}
		}
		
		noteCount ++;
		
		if (noteCount % ticksPerBeat == 1) Beat ();
		
		// If interval is less then framerate, play more notes
		//if (timer <= 0) PlayNote();
	}
	
	void Beat()
	{
		beatCount ++;
		
		if (OnBeat != null) OnBeat(beatCount);
	}
	
	/// <summary>
	/// Sets the tick interval.
	/// </summary>
	/// <param name='bpm'>
	/// Beats per minuts.
	/// </param>
	/// <param name='ticksPerBeat'>
	/// Ticks per beat.
	/// </param>
	public void SetInterval(float bpm, int ticksPerBeat)
	{
		if (bpm <= float.MinValue) bpm = float.MinValue;
		if (ticksPerBeat < 1) ticksPerBeat = 1;
		
		this.bpm = bpm;
		this.ticksPerBeat = ticksPerBeat;
		
		interval = 60f / (bpm * ticksPerBeat);
	}
}
