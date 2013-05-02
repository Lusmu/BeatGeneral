using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatGeneral
{
	/// <summary>
	/// Different tuning systems.
	/// </summary>
	public enum TuningSystem
	{
		Random, 					// Used to select a random one
		DiatonicEqualTemperament,	// The standard western piano tuning
		DiatonicJustIntonation,		// A more natural tuning
		PentatonicMinor,			// Pentatonic (5 notes per octave) minor tuning
		PentatonicMajor,			// Pentatonic (5 notes per octave) major tuning
		PentatonicPythagorean,		// Pentatonic (5 notes per octave) tuning by Ben Johnston
	}
	
	/// <summary>
	/// Scale to be used,
	/// set to None for none-diatonic tunings.
	/// Selects 7 notes from the full tuning range.
	/// 0 and 1 reserved, will not be picked by Random.
	/// </summary>
	public enum DiatonicScale
	{
		None = 0,			// Use all the notes in the tuning system
		Random = 1,			// Used to select a random one
		Bbm,
		Fm,
		Cm,
		Gm,
		Dm,
		Am,
		Em,
		Bm,
		Fsm,
		Csm,
		Gsm,
		Dsm,
		PentatonicMinor,	// Only selects 5 notes
		PentatonicMajor,	// Only selects 5 notes
	}
	
	/// <summary>
	/// Static utility class for getting good pitch intervals.
	/// Create TuningSettings and get pitches as float array 
	/// by calling GetPitches.
	/// </summary>
	public static class Tuning
	{
		/// <summary>
		/// Tuning intervals for various tuning systems.
		/// </summary>
		static readonly Dictionary<TuningSystem, float[]> tunings = new Dictionary<TuningSystem, float[]>()
		{
			{TuningSystem.DiatonicEqualTemperament, new float[] {
					1,
					1.059463f,
					1.122462f,
					1.189207f,
					1.259921f,
					1.334840f,
					1.414214f,
					1.498307f,
					1.587401f,
					1.681793f,
					1.781797f,
					1.887749f
			}},
			{TuningSystem.DiatonicJustIntonation, new float[] {
					1,
					16f/15f,
					9f/8f,
					6f/5f,
					5f/4f,
					4f/3f,
					7f/5f,
					3f/2f,
					8f/5f,
					5f/3f,
					16f/9f,
					15f/8f
			}},
			{TuningSystem.PentatonicMinor, new float[] {
					1,
					36f/30f,
					40f/30f,
					45/30f,
					54/30f
			}},
			{TuningSystem.PentatonicMajor, new float[] {
					1,
					27f/24f,
					30f/24f,
					36f/24f,
					40f/24f
			}},
			{TuningSystem.PentatonicPythagorean, new float[] {
					1,
					32f/27f,
					4f/3f,
					3f/2f,
					16f/9f
			}},
		};
		
		/// <summary>
		/// Selections of notes from diatonic tuning.
		/// Use GetScale to get the right scale.
		/// The numbers represent which note to pick for which key, from lowest to highest.
		/// Numbers are the array indexes of the pitches in the tuning arrays.
		/// Numbers below zero or at or above array size represent different octave, eg.
		/// -1 pick index 11 from a 12 sized array, and 14 picks index 2.
		/// </summary>
		static readonly Dictionary<DiatonicScale, int[]> scales = new Dictionary<DiatonicScale, int[]>()
		{
			{DiatonicScale.Am, new int[]{-3,-1,0,2,4,5,7}},
			{DiatonicScale.Bbm, new int[]{-2,0,1,3,5,6,8}},
			{DiatonicScale.Bm, new int[]{-1,1,2,4,6,7,9}},
			{DiatonicScale.Cm, new int[]{0,2,3,5,7,8,10}},
			{DiatonicScale.Csm, new int[]{1,3,4,6,8,9,11}},
			{DiatonicScale.Dm, new int[]{2,4,5,7,9,10,12}},
			{DiatonicScale.Dsm, new int[]{3,5,6,8,10,11,13}},
			{DiatonicScale.Em, new int[]{4,6,7,9,11,12,14}},
			{DiatonicScale.Fm, new int[]{5,7,8,10,12,13,15}},
			{DiatonicScale.Fsm, new int[]{-6,-4,-5,-1,1,2,4}},
			{DiatonicScale.Gm, new int[]{-5,-3,-2,0,2,3,5}},
			{DiatonicScale.Gsm, new int[]{1,3,4,6,8,9,11}},
			{DiatonicScale.PentatonicMajor, new int[]{0,2,4,5,7}},
			{DiatonicScale.PentatonicMinor, new int[]{0,2,4,6,7}},
			
		};
		
		/// <summary>
		/// Gets an array of pitches to be used by audio sources.
		/// </summary>
		/// <returns>
		/// The pitches as an array of floats.
		/// </returns>
		/// <param name='settings'>
		/// Settings object.
		/// </param>
		public static float[] GetPitches(TuningSettings settings)
		{
			float[] tunes;
			float[] pitches;
			int[] scale = GetScale(settings.scale);
			
			if (!tunings.ContainsKey(settings.tuning))
			{
				List<float[]> values = new List<float[]>(tunings.Values);
				tunes = values[Random.Range(0, values.Count - 1)];
			}
			else
			{
				tunes = tunings[settings.tuning];
			}
				
			if (scale == null)
			{
				pitches = new float[tunes.Length];
				for (int i = 0; i < pitches.Length; i++)
				{
					pitches[i] = GetPitch(i + settings.noteOffset, tunes);
				}
			}
			else
			{
				pitches = new float[scale.Length];
				for (int i = 0; i < scale.Length; i++)
				{
					pitches[i] = GetPitch(scale[i] + settings.noteOffset, tunes);
				}
			}
			
			return pitches;
		}
		
		/// <summary>
		/// Gets a scale as an array of array indexes to be used with tuning arrays.
		/// </summary>
		/// <returns>
		/// The scale as an array of array indexes.
		/// </returns>
		/// <param name='scaleName'>
		/// Scale type.
		/// </param>
		static int[] GetScale(DiatonicScale scaleName)
		{
			if (scaleName == DiatonicScale.None) return null;
			
			if (scaleName == DiatonicScale.Random)
			{
				// Don't select None or Random
				scaleName = (DiatonicScale)(Random.Range(2, System.Enum.GetValues(typeof(DiatonicScale)).Length - 1));
			}
			
			if (!scales.ContainsKey(scaleName)) return null;
			
			return scales[scaleName];
		}
		
		/// <summary>
		/// Gets a pitch with an offset from an array of pitches.
		/// Changes octave when necessary.
		/// </summary>
		/// <returns>
		/// The pitch modified by the offset.
		/// </returns>
		/// <param name='offset'>
		/// Offset in steps.
		/// </param>
		/// <param name='scale'>
		/// Scale as an array of pitches.
		/// </param>
		public static float GetPitch(int offset, float[] scale)
		{
			if (scale == null || scale.Length == 0) return 1;
			
			float octave = 1;
			
			while (offset < 0)
			{
				offset += scale.Length;
				octave *= 0.5f;
			}
			while (offset >= scale.Length)
			{
				offset -= scale.Length;
				octave *= 2f;
			}
			
			return scale[offset] * octave;
		}
	}
	
	
}
