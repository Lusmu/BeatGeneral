using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatGeneral
{
	/// <summary>
	/// Type of generation algorithm.
	/// </summary>
	public enum GenerationType
	{
		Undefined,				
		UniformNoise,		// Simply randomizes each note
		AutomataDefault,	// Cellular automata using a 'default' ruleset
		AutomataRandom		// Cellular automata using a dynamically created ruleset
	}
	
	/// <summary>
	/// Music generator.
	/// Can create tunes from files, randomly or randomly on the fly.
	/// Usage:
	/// 1. Create new BeatGenerator object with GeneratorSettings object for passing settings.
	/// 2. Either call GenerateTune() to get a tune as a two dimensional array of note velocites,
	/// 	or create int array for holding note velocities for one tick, and
	/// 	on each tick call NextNotes(int[] notes) to get new array of velocities.
	/// </summary>
	public class BeatGenerator
	{
		GeneratorSettings settings;
		int[] currentRule;
		
		#region Premade automata rules
		public static readonly int[] rule30 = new int[]{0,0,0,1,1,1,1,0};
		public static readonly int[] rule110 = new int[]{0,1,1,0,1,1,1,0};
		public static readonly int[] rule90 = new int[]{0,1,0,1,1,0,1,0};
		#endregion
		
		#region Public API
		/// <summary>
		/// Main constructor for BeatGenerator.
		/// </summary>
		/// <param name='settings'>
		/// Settings object. It is serializable so it can be shown in editor.
		/// </param>
		public BeatGenerator(GeneratorSettings settings)
		{
			this.settings = settings;
		}
		
		/// <summary>
		/// Generates a tune with using the given settings.
		/// </summary>
		/// <returns>
		/// The tune as a two-dimensional array of note velocities.
		/// First dimension hold the ticks, and the seconds dimension
		/// contains the velocities for each note in a tick.
		/// </returns>
		public int[][] GenerateTune()
		{
			if (settings.loadTuneFromFile != null) return ParseFromFile(settings.loadTuneFromFile);
			else 
			{
				if (settings.generationType == GenerationType.AutomataDefault) currentRule = rule110;
				else if (settings.generationType == GenerationType.AutomataRandom) currentRule = GetRandomAutomataRule(settings.density);
				
				return GenerateRandom();
			}
		}
		
		/// <summary>
		/// Generates notes for the next tick.
		/// </summary>
		/// <returns>
		/// The notes used last tick.
		/// </returns>
		/// <param name='lastColumn'>
		/// Last tick, used by the automata generation patterns.
		/// </param>
		public int[] GetNextNotes(int[] lastColumn)
		{		
			if (lastColumn == null || lastColumn.Length == 0)
			{
				// Either this is the first tick, or something has gone terribly wrong. Create random note.
				return SetOneRandomToOne(settings.tracks);
			}
			else
			{
				int[] row = new int[lastColumn.Length];
				for (int i = 0; i < lastColumn.Length; i++)
				{
					row[i] = GetNext (lastColumn, i);
				}
				return row;
			}
		}
		#endregion
		
		#region private
		
		/// <summary>
		/// Next note on the track.
		/// </summary>
		/// <param name='lastColumn'>
		/// Last tick's note column.
		/// </param>
		/// <param name='track'>
		/// Current track index.
		/// </param>
		int GetNext(int[] lastColumn, int track)
		{
			// Get random noise if orderer, or if last column data insufficient for automata.
			if (lastColumn == null || lastColumn.Length < 3 || settings.generationType == GenerationType.UniformNoise)
				return GetRandomNoise();
			
			// Prepare array of last tick's cells for the automata.
			int[] old = new int[3];
			if (track <= 0)
			{
				old[0] = lastColumn[lastColumn.Length - 1];
				old[1] = lastColumn[0];
				old[2] = lastColumn[1];
			}
			else if (track >= lastColumn.Length - 1)
			{
				old[0] = lastColumn[track - 1];
				old[1] = lastColumn[track];
				old[2] = lastColumn[0];
			}
			else
			{
				old[0] = lastColumn[track - 1];
				old[1] = lastColumn[track];
				old[2] = lastColumn[track + 1];
			}
			
			if (settings.generationType == GenerationType.AutomataDefault
				|| settings.generationType == GenerationType.AutomataRandom) return GetNextAutomata(old, currentRule);
			else return GetRandomNoise();
		}
		
		/// <summary>
		/// Generates a tune of random noise.
		/// </summary>
		/// <returns>
		/// The tune as a two-dimensional array of note velocities.
		/// </returns>
		int[][] GenerateRandom()
		{
			int[][] tune = new int[settings.trackLength][];
			
			for (int i = 0; i < tune.Length; i++)
			{
				if (i == 0) tune[i] = GetNextNotes(null);
				else tune[i] = GetNextNotes(tune[i - 1]);
			}
			
			return tune;
		}
		
		/// <summary>
		/// Gets single note for random, uniform noise.
		/// </summary>
		/// <returns>
		/// Note velocity.
		/// </returns>
		int GetRandomNoise()
		{
			return (Random.value < settings.density / (float)settings.tracks ? 1 : 0);
		}
		
		/// <summary>
		/// Parses a tune from file.
		/// The file must have note velocities separated by spaces,
		/// each row represents one tick.
		/// For example, file containing
		/// 1 0 0
		/// 0 1 0
		/// 0 0 1
		/// 0 0 1
		/// 0 1 0
		/// 1 0 0
		/// will play three notes, from the lowest to the highest and then back, in six ticks.
		/// Text files allow // comments.
		/// </summary>
		/// <returns>
		/// A tune as a two-dimensional array of note velocites.
		/// </returns>
		/// <param name='file'>
		/// A text file containing the tune.
		/// </param>
		int[][] ParseFromFile(TextAsset file)
		{
			if (file == null) return null;
				
			string raw = file.text;
			
			// Split by rows
			string[] tracks = raw.Split(new char[]{'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
			
			List<int[]> tuneList = new List<int[]>();
			
			int trackCount = 0;
			
			for (int i = 0; i < tracks.Length; i++)
			{
				// Check for // comments
				int commentIndex = tracks[i].IndexOf("//");
				if (commentIndex >= 0)
				{
					tracks[i] = tracks[i].Substring(0, commentIndex);
				}
				
				if (tracks[i].Trim().Length > 0)
				{
					List<int> notes = new List<int>();
					
					string[] notesStrings = tracks[i].Split(new char[]{' '}, System.StringSplitOptions.RemoveEmptyEntries);
					
					for (int n = 0; n < notesStrings.Length; n++)
					{
						int note = 0;
						if (System.Int32.TryParse(notesStrings[n], out note))
						{
							notes.Add(note);
						}
					}
					
					if (notes.Count > 0) 
					{
						// Only add new tick when the row contains notes.
						if (notes.Count > trackCount) trackCount = notes.Count;
						tuneList.Add(notes.ToArray());
					}
				}
			}
			
			if (tuneList.Count > 0)
			{
				settings.tracks = trackCount;
				settings.trackLength = tuneList.Count;
			}
			else
			{
				Debug.LogError(file.name + " does not contain notes!");
			}
			
			return tuneList.ToArray();
		}
				
		/// <summary>
		/// Sets one number in the array to one.
		/// </summary>
		/// <returns>
		/// An array containing one one and many zeroes.
		/// </returns>
		/// <param name='size'>
		/// Size of the array.
		/// </param>
		int[] SetOneRandomToOne(int size)
		{
			int[] row = new int[size];
			row[Random.Range(0, size - 1)] = 1;
			return row;
		}
		
		/// <summary>
		/// Elementary cellular automata.
		/// http://en.wikipedia.org/wiki/Elementary_cellular_automaton
		/// </summary>
		/// <returns>
		/// The original array modified by the rule.
		/// </returns>
		/// <param name='old'>
		/// The previous array.
		/// </param>
		/// <param name='rule'>
		/// The rule as a 8-sized array of integers (of only 0s and 1s).
		/// </param>
		int GetNextAutomata(int[] old, int[] rule)
		{
			// Create a three-digit binary number from the old cells
			int pseudoBinary = 
				Mathf.RoundToInt(Mathf.Clamp01(old[0])) * 100
				+ Mathf.RoundToInt(Mathf.Clamp01(old[1])) * 10
				+ Mathf.RoundToInt(Mathf.Clamp01(old[2]));
			
			// Each number has a spesific rule attached to it.
			if (pseudoBinary == 111) return rule[0];
			if (pseudoBinary == 110) return rule[1];
			if (pseudoBinary == 101) return rule[2];
			if (pseudoBinary == 100) return rule[3];
			if (pseudoBinary == 011) return rule[4];
			if (pseudoBinary == 010) return rule[5];
			if (pseudoBinary == 001) return rule[6];
			
			return rule[7];
		}
		
		/// <summary>
		/// Gets a random automata rule.
		/// </summary>
		/// <returns>
		/// A random automata rule.
		/// </returns>
		/// <param name='density'>
		/// Density between 0 and 1, exclusively.
		/// </param>
		int[] GetRandomAutomataRule(float density)
		{	
			// Avoid extreme values
			density = Mathf.Clamp(density, 0.25f, 0.75f);
			
			int[] rule = new int[8];
			int sum = 0;
			for (int i = 0; i < rule.Length; i++)
			{
				if (Random.value < density) rule[i] = 1;
				else rule[i] = 0;
				
				sum += rule[i];
			}
			
			// Avoid extreme values
			if (sum < 2 || sum > 5) return GetRandomAutomataRule(density);
			
			return rule;
		}
		#endregion
	}
}
