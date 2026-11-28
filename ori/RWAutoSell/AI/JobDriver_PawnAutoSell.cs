using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse.AI;

namespace RWAutoSell.AI
{
	// Token: 0x0200005B RID: 91
	[NullableContext(1)]
	[Nullable(0)]
	public class JobDriver_PawnAutoSell : JobDriver
	{
		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x0600034C RID: 844 RVA: 0x0001E678 File Offset: 0x0001C878
		private ITrader Trader
		{
			get
			{
				return (ITrader)base.TargetThingA;
			}
		}

		// Token: 0x0600034E RID: 846 RVA: 0x0001E694 File Offset: 0x0001C894
		public override void ExposeData()
		{
			base.ExposeData();
		}

		// Token: 0x0600034F RID: 847 RVA: 0x0001E69C File Offset: 0x0001C89C
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		// Token: 0x06000350 RID: 848 RVA: 0x0001E69F File Offset: 0x0001C89F
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_PawnAutoSell.<MakeNewToils>d__7 <MakeNewToils>d__ = new JobDriver_PawnAutoSell.<MakeNewToils>d__7(-2);
			<MakeNewToils>d__.<>4__this = this;
			return <MakeNewToils>d__;
		}

		// Token: 0x040001AC RID: 428
		private const int dowork = 600;

		// Token: 0x040001AD RID: 429
		private int tick = 1;
	}
}
