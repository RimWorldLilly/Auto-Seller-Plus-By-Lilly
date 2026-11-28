using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.AI;

namespace RWAutoSell.AI
{
	// Token: 0x0200005A RID: 90
	public class JobDriver_AutoSell : JobDriver
	{
		// Token: 0x06000346 RID: 838 RVA: 0x0001E575 File Offset: 0x0001C775
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A), this.job, 1, -1, null, true, false);
		}

		// Token: 0x06000348 RID: 840 RVA: 0x0001E5A1 File Offset: 0x0001C7A1
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.tick, "curtick", 0, false);
			Scribe_Values.Look<int>(ref this.maxticks, "maxticks", 0, false);
		}

		// Token: 0x06000349 RID: 841 RVA: 0x0001E5CD File Offset: 0x0001C7CD
		[NullableContext(1)]
		protected override IEnumerable<Toil> MakeNewToils()
		{
			JobDriver_AutoSell.<MakeNewToils>d__7 <MakeNewToils>d__ = new JobDriver_AutoSell.<MakeNewToils>d__7(-2);
			<MakeNewToils>d__.<>4__this = this;
			return <MakeNewToils>d__;
		}

		// Token: 0x0600034A RID: 842 RVA: 0x0001E5E0 File Offset: 0x0001C7E0
		private bool FailToil()
		{
			return this.pawn.IsPrisoner || this.pawn.Dead || this.FailOnNoComm() || ASMapComp.GetSingleton(base.Map).Rules == null || ASMapComp.GetSingleton(base.Map).Rules.Count == 0;
		}

		// Token: 0x0600034B RID: 843 RVA: 0x0001E63C File Offset: 0x0001C83C
		private bool FailOnNoComm()
		{
			bool result;
			try
			{
				result = !(base.TargetThingA as Building_CommsConsole).CanUseCommsNow;
			}
			catch
			{
				result = true;
			}
			return result;
		}

		// Token: 0x040001A8 RID: 424
		private int tick;

		// Token: 0x040001A9 RID: 425
		private int maxticks;

		// Token: 0x040001AA RID: 426
		private const int dowork = 600;

		// Token: 0x040001AB RID: 427
		private int lasttickindex;
	}
}
