using System;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using Verse;

namespace RWAutoSell.Mergers
{
	// Token: 0x02000029 RID: 41
	[NullableContext(1)]
	[Nullable(0)]
	public class AllMerger : IUnmerge
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x06000148 RID: 328 RVA: 0x0000E3D9 File Offset: 0x0000C5D9
		public string Label
		{
			get
			{
				return "RWAutoSell.MergerAll".Translate();
			}
		}

		// Token: 0x06000149 RID: 329 RVA: 0x0000E3EA File Offset: 0x0000C5EA
		public bool AllowMerge(Thing thing, Thing thing2)
		{
			return false;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x0000E3ED File Offset: 0x0000C5ED
		public bool AppliesTo(Thing thing)
		{
			return true;
		}
	}
}
