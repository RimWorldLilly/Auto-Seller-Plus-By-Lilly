using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000034 RID: 52
	[NullableContext(1)]
	[Nullable(0)]
	public class DoerBoxWorker_MentalBreak : DoerBoxWorker
	{
		// Token: 0x1700004A RID: 74
		// (get) Token: 0x0600018F RID: 399 RVA: 0x0000FCD4 File Offset: 0x0000DED4
		public override string Doerclass
		{
			get
			{
				return "RimWorld.ReadingOutcomeDoerMentalBreak";
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000190 RID: 400 RVA: 0x0000FCDB File Offset: 0x0000DEDB
		public override string DoerLabel
		{
			get
			{
				return "RWAutoSell.FilterLearningMental";
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000191 RID: 401 RVA: 0x0000FCE2 File Offset: 0x0000DEE2
		public override int Expandedsize
		{
			get
			{
				return 60;
			}
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0000FCE6 File Offset: 0x0000DEE6
		public DoerBoxWorker_MentalBreak()
		{
			this.Settingsdefault = new List<string>
			{
				"mbk.0.2"
			};
		}

		// Token: 0x06000193 RID: 403 RVA: 0x0000FD04 File Offset: 0x0000DF04
		public override void DoerSettingsWorker(string[] rvtlist)
		{
			string text;
			if (rvtlist.FirstSubString("mbk.", out text, string.Empty))
			{
				this.Settings.Add((text == null) ? "mbk.0.2" : ("mbk." + text));
				return;
			}
			this.Settings.Add("mbk.0.2");
		}

		// Token: 0x06000194 RID: 404 RVA: 0x0000FD58 File Offset: 0x0000DF58
		public override bool CheckDoerWorker(Book b, string[] Data)
		{
			float num;
			Data.FirstSubString("mbk.", out num, 0.2f);
			return b.MentalBreakChancePerHour <= num;
		}
	}
}
