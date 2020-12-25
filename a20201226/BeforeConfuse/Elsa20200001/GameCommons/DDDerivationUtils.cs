using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.GameCommons
{
	public static class DDDerivationUtils
	{
		public static List<DDPicture> Derivations = new List<DDPicture>();

		public static void Add(DDPicture derivation)
		{
			Derivations.Add(derivation);
		}

		public static void UnloadAll()
		{
			foreach (DDPicture derivation in Derivations)
				derivation.Unload();
		}
	}
}
