using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000023 RID: 35
	internal static class MergePatcher
	{
		// Token: 0x06000120 RID: 288 RVA: 0x0000D09C File Offset: 0x0000B29C
		[NullableContext(1)]
		public static void MergeAllowedTradeableMatching(ref bool __result, Thing a, Thing b, TransferAsOneMode mode)
		{
			if (!__result || mode == TransferAsOneMode.InactiveTradeable || ASLibMod.GetSettings().EnabledUnmergers == null || ASLibMod.GetSettings().EnabledUnmergers.Count == 0 || (a != null && b != null && a.def == ThingDefOf.Silver && b.def == ThingDefOf.Silver))
			{
				return;
			}
			foreach (IUnmerge unmerge in from el in ASLibMod.GetSingleton.GetBaseUnMergers
			where ASLibMod.GetSettings().EnabledUnmergers.Any((string x) => x == el.GetType().FullName)
			select el)
			{
				if (unmerge.AppliesTo(a) && !unmerge.AllowMerge(a, b))
				{
					__result = false;
					return;
				}
			}
		}
	}
}
