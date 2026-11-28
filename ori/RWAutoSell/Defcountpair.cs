using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	public class Defcountpair : IExposable
	{
		// Token: 0x06000071 RID: 113 RVA: 0x00006544 File Offset: 0x00004744
		public void IncCount(int add)
		{
			this.Count += Math.Abs(add);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00006559 File Offset: 0x00004759
		public Defcountpair(ThingDef Def, int Count)
		{
			this.Def = Def;
			this.Count = Math.Abs(Count);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00006574 File Offset: 0x00004774
		public Defcountpair()
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000657C File Offset: 0x0000477C
		public static void ModifyExisting(Tradeable tr, List<Defcountpair> cplist)
		{
			for (int i = 0; i < cplist.Count<Defcountpair>(); i++)
			{
				if (cplist[i].Def == tr.ThingDef)
				{
					cplist[i].IncCount(tr.CountToTransfer);
					return;
				}
			}
			cplist.Add(new Defcountpair(tr.ThingDef, tr.CountToTransfer));
		}

		// Token: 0x06000075 RID: 117 RVA: 0x000065D8 File Offset: 0x000047D8
		public void ExposeData()
		{
			Scribe_Defs.Look<ThingDef>(ref this.Def, "DefItem");
			Scribe_Values.Look<int>(ref this.Count, "DefCount", 0, false);
		}

		// Token: 0x04000050 RID: 80
		public ThingDef Def;

		// Token: 0x04000051 RID: 81
		public int Count;
	}
}
