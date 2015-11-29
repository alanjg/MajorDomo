//#define DETERMINISTIC_RANDOM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dominion
{
	public class Randomizer
	{
#if !DETERMINISTIC_RANDOM
		[ThreadStatic]
#endif
		private static Random globalRandom;
		private static Random GlobalRandom
		{
			get
			{
				if (globalRandom == null)
				{
					globalRandom = new Random();
				}
				return globalRandom;
			}
		}
		public static int Next()
		{
			int result = 0;		
#if DETERMINISTIC_RANDOM
			lock (Randomizer.GlobalRandom)
#endif
			{
				result = Randomizer.GlobalRandom.Next();
			}
			return result;
		}

		public static int Next(int max)
		{
			int result = 0;
#if DETERMINISTIC_RANDOM
			lock (Randomizer.GlobalRandom)
#endif
			{
				result = Randomizer.GlobalRandom.Next(max);
			}
			return result;
		}
		public static int Next(int min, int max)
		{
			int result = 0;
#if DETERMINISTIC_RANDOM
			lock (Randomizer.GlobalRandom)
#endif
			{
				result = Randomizer.GlobalRandom.Next(min, max);
			}
			return result;
		}

		public static double NextDouble()
		{
			double result = 0.0;
#if DETERMINISTIC_RANDOM
			lock (Randomizer.GlobalRandom)
#endif
			{
				result = Randomizer.GlobalRandom.NextDouble();
			}
			return result;
		}

		public static void SetRandomSeed(int seed)
		{
			globalRandom = new Random(seed);
		}
	}
}
