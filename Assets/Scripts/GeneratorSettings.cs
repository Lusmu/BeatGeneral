using UnityEngine;
using System.Collections;

namespace BeatGeneral
{
	/// <summary>
	/// Settings holder for generator settings.
	/// Serialized so it can be displayed in editor.
	/// </summary>
	[System.Serializable]
	public class GeneratorSettings 
	{
		/// <summary>
		/// The length of the track,
		/// or how many ticks (columns of notes) to create.
		/// Set 0 or less to create infinite notes on the fly.
		/// </summary>
		public int trackLength = 16;
		/// <summary>
		/// Max amount of tracks, i.e. different notes.
		/// </summary>
		public int tracks = 14;
		/// <summary>
		/// Note density.
		/// Density of 1 will generate an average of 1 note
		/// per tick when using GenerationType.UniformNoise.
		/// </summary>
		public float density = 0.25f;
		/// <summary>
		/// Text file from which to load a tune, or null if none.
		/// Space separated note velocities, each row represents one tick.
		/// For example, file containing
		/// 1 0 0
		/// 0 1 0
		/// 0 0 1
		/// will play the lowest note on the first tick, second lowest on the second
		/// and the third lowest on the third.
		/// Text files allow // comments.
		/// </summary>
		public TextAsset loadTuneFromFile;
		/// <summary>
		/// The type of the generation.
		/// Does not have effect if loading tune from file.
		/// </summary>
		public GenerationType generationType = GenerationType.UniformNoise;
	}
}
