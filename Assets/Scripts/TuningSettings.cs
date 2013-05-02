using UnityEngine;
using System.Collections;

namespace BeatGeneral
{
	[System.Serializable]
	public class TuningSettings
	{
		public TuningSystem tuning = TuningSystem.PentatonicPythagorean;
		public DiatonicScale scale = DiatonicScale.None;
		public int noteOffset = 0;
	}
}