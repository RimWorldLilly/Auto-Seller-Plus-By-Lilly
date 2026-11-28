using System;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;

namespace RWAutoSell.Sorters
{
	// Token: 0x02000028 RID: 40
	[NullableContext(1)]
	[Nullable(0)]
	public class SorterMarketValue : IRuleSorter
	{
		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000145 RID: 325 RVA: 0x0000E363 File Offset: 0x0000C563
		public string Label
		{
			get
			{
				return TransferableSorterDefOf.MarketValue.LabelCap;
			}
		}

		// Token: 0x06000146 RID: 326 RVA: 0x0000E374 File Offset: 0x0000C574
		public int Compare<[Nullable(0)] T>(T First, T Second) where T : Transferable
		{
			float num = 0f;
			float value = 0f;
			if (!First.HasAnyThing)
			{
				num = First.AnyThing.MarketValue;
			}
			if (!Second.HasAnyThing)
			{
				value = Second.AnyThing.MarketValue;
			}
			return num.CompareTo(value);
		}
	}
}
