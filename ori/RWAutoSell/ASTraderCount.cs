using System;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000004 RID: 4
	[NullableContext(1)]
	[Nullable(0)]
	public class ASTraderCount : IExposable, IEquatable<Thing>, IEquatable<ASTraderCount>, IEquatable<Pawn>, IEquatable<Building>
	{
		// Token: 0x0600000D RID: 13 RVA: 0x00002282 File Offset: 0x00000482
		public ASTraderCount()
		{
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00002295 File Offset: 0x00000495
		public ASTraderCount(Thing t)
		{
			this.Trader = t.ThingID;
			this.LastTick = 60000;
		}

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000022BF File Offset: 0x000004BF
		public bool CanRemove
		{
			get
			{
				return this.LastTick < 0;
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000022CA File Offset: 0x000004CA
		public void DoTick()
		{
			this.LastTick--;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000022DA File Offset: 0x000004DA
		public bool Equals(Thing other)
		{
			return other != null && !(this.Trader == string.Empty) && other.ThingID == this.Trader;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002304 File Offset: 0x00000504
		public bool Equals(ASTraderCount other)
		{
			return other != null && !(this.Trader == string.Empty) && other.Trader == this.Trader;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000232E File Offset: 0x0000052E
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002338 File Offset: 0x00000538
		public override bool Equals(object obj)
		{
			Thing thing = obj as Thing;
			if (thing != null)
			{
				return this.Equals(thing);
			}
			ASTraderCount astraderCount = obj as ASTraderCount;
			if (astraderCount != null)
			{
				return this.Equals(astraderCount);
			}
			Building building = obj as Building;
			if (building != null)
			{
				return this.Equals(building);
			}
			Pawn pawn = obj as Pawn;
			return pawn != null && this.Equals(pawn);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000238E File Offset: 0x0000058E
		public void ExposeData()
		{
			Scribe_Values.Look<string>(ref this.Trader, "Trader", null, false);
			Scribe_Values.Look<int>(ref this.LastTick, "LastTick", 0, false);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000023B4 File Offset: 0x000005B4
		public bool Equals(Pawn other)
		{
			return other != null && !(this.Trader == string.Empty) && other.ThingID == this.Trader;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000023DE File Offset: 0x000005DE
		public bool Equals(Building other)
		{
			return other != null && !(this.Trader == string.Empty) && other.ThingID == this.Trader;
		}

		// Token: 0x04000005 RID: 5
		private string Trader = string.Empty;

		// Token: 0x04000006 RID: 6
		private int LastTick;
	}
}
