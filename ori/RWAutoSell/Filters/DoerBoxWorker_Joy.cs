using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000033 RID: 51
	[NullableContext(1)]
	[Nullable(0)]
	public class DoerBoxWorker_Joy : DoerBoxWorker
	{
		// Token: 0x17000047 RID: 71
		// (get) Token: 0x06000189 RID: 393 RVA: 0x0000FC09 File Offset: 0x0000DE09
		public override string Doerclass
		{
			get
			{
				return "RimWorld.ReadingOutcomeDoerJoyFactorModifier";
			}
		}

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x0600018A RID: 394 RVA: 0x0000FC10 File Offset: 0x0000DE10
		public override string DoerLabel
		{
			get
			{
				return "RWAutoSell.FilterLearningJoy";
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x0600018B RID: 395 RVA: 0x0000FC17 File Offset: 0x0000DE17
		public override int Expandedsize
		{
			get
			{
				return 60;
			}
		}

		// Token: 0x0600018C RID: 396 RVA: 0x0000FC1B File Offset: 0x0000DE1B
		public DoerBoxWorker_Joy()
		{
			this.Settingsdefault = new List<string>
			{
				"joy.1"
			};
		}

		// Token: 0x0600018D RID: 397 RVA: 0x0000FC3C File Offset: 0x0000DE3C
		public override void DoerSettingsWorker(string[] rvtlist)
		{
			for (int i = 0; i < rvtlist.Length; i++)
			{
				Log.Message(rvtlist[i]);
			}
			string text;
			if (rvtlist.FirstSubString("joy.", out text, string.Empty))
			{
				this.Settings.Add((text == null) ? "joy.1" : ("joy." + text));
				return;
			}
			this.Settings.Add("joy.1");
		}

		// Token: 0x0600018E RID: 398 RVA: 0x0000FCA8 File Offset: 0x0000DEA8
		public override bool CheckDoerWorker(Book b, string[] Data)
		{
			float num;
			Data.FirstSubString("joy.", out num, 2.5f);
			return b.JoyFactor > num;
		}
	}
}
