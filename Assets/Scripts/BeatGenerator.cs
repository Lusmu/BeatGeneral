using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatGeneral
{
	public class BeatGenerator
	{
		GeneratorSettings settings;
		
		public BeatGenerator(GeneratorSettings settings)
		{
			this.settings = settings;
		}
		
		public int[][] GenerateTune()
		{
			if (settings.loadTuneFromFile != null) return ParseFromFile(settings.loadTuneFromFile);
			else return GenerateRandom();
		}
		
		public int[][] GenerateRandom()
		{
			int[][] tune = new int[settings.trackLength][];
			
			for (int i = 0; i < tune.Length; i++)
			{
				tune[i] = new int[settings.tracks];
				
				for (int j = 0; j < tune[i].Length; j++)
				{
					tune[i][j] = (Random.value < settings.density / (float)settings.tracks ? 1 : 0);
				}
			}
			
			return tune;
		}
		
		public int[][] ParseFromFile(TextAsset file)
		{
			if (file == null) return null;
				
			string raw = file.text;
			
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
	}
}
