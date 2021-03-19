using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerletChainTest
{
	public static class Utils
	{
		public static float Clamp(float value, float min, float max) => value < min ? min : value > max ? max : value;

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list) => list.OrderBy<T, int>((x) => Main.Rand.Next());
	}
}
