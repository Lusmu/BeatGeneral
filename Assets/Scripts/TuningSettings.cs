using UnityEngine;
using System.Collections;

namespace BeatGeneral
{
	/// <summary>
	/// Settings holder for tuning settings.
	/// Serialized so it can be displayed in editor.
	/// </summary>
	[System.Serializable]
	public class TuningSettings
	{
		public TuningSystem tuning = TuningSystem.PentatonicPythagorean;
		public DiatonicScale scale = DiatonicScale.None;
		public int noteOffset = 0;
	}
}