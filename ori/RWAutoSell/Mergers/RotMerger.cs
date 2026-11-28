using System;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Mergers
{
	// Token: 0x0200002C RID: 44
	[NullableContext(1)]
	[Nullable(0)]
	public class RotMerger : IUnmerge
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000155 RID: 341 RVA: 0x0000E75C File Offset: 0x0000C95C
		public string Label
		{
			get
			{
				return "RWAutoSell.MergerPerishables".Translate();
			}
		}

		// Token: 0x06000156 RID: 342 RVA: 0x0000E770 File Offset: 0x0000C970
		public bool AllowMerge(Thing thing, Thing thing2)
		{
			float num = -1f;
			CompRottable compRottable = thing.TryGetComp<CompRottable>();
			if (compRottable != null)
			{
				num = compRottable.RotProgressPct;
			}
			float num2 = -1f;
			CompRottable compRottable2 = thing2.TryGetComp<CompRottable>();
			if (compRottable2 != null)
			{
				num2 = compRottable2.RotProgressPct;
			}
			return Mathf.Abs(num - num2) <= 0.1f;
		}

		// Token: 0x06000157 RID: 343 RVA: 0x0000E7BD File Offset: 0x0000C9BD
		public bool AppliesTo(Thing thing)
		{
			return thing.def.HasComp(typeof(CompRottable));
		}
	}
}
