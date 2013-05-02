using UnityEngine;
using System.Collections;

namespace BeatGeneral
{
	[System.Serializable]
	public class GeneratorSettings 
	{
		public int trackLength = 16;
		public int tracks = 14;
		public float density = 0.25f;
		public TextAsset loadTuneFromFile;
	}
}
