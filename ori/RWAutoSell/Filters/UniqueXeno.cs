using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000047 RID: 71
	[NullableContext(1)]
	[Nullable(0)]
	internal class UniqueXeno
	{
		// Token: 0x06000277 RID: 631 RVA: 0x00017174 File Offset: 0x00015374
		public UniqueXeno(string label, List<GeneDef> Genes)
		{
			this.name = label;
			this.genes = Genes;
		}

		// Token: 0x06000278 RID: 632 RVA: 0x00017195 File Offset: 0x00015395
		public override string ToString()
		{
			return this.name.ToString();
		}

		// Token: 0x0400014D RID: 333
		public string name = "Unset";

		// Token: 0x0400014E RID: 334
		public List<GeneDef> genes;
	}
}
