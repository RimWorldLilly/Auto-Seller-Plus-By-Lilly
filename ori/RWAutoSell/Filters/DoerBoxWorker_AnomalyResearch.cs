using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000030 RID: 48
	[NullableContext(1)]
	[Nullable(0)]
	public class DoerBoxWorker_AnomalyResearch : DoerBoxWorker_Research
	{
		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000177 RID: 375 RVA: 0x0000F603 File Offset: 0x0000D803
		public override string Doerclass
		{
			get
			{
				return "RimWorld.ReadingOutcomeDoerGainAnomalyResearch";
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000178 RID: 376 RVA: 0x0000F60A File Offset: 0x0000D80A
		public override string DoerLabel
		{
			get
			{
				return "RWAutoSell.FilterLearningAnom";
			}
		}

		// Token: 0x06000179 RID: 377 RVA: 0x0000F611 File Offset: 0x0000D811
		public DoerBoxWorker_AnomalyResearch()
		{
			this.Settingsdefault = new List<string>
			{
				"cmp.any",
				"unl.False",
				"und.False",
				"rnc.False"
			};
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000F650 File Offset: 0x0000D850
		public override void DoerSettingsWorker(string[] rvtlist)
		{
			this.Settings.Add(rvtlist.Contains("cmp.all") ? "cmp.all" : "cmp.any");
			this.Settings.Add(rvtlist.Contains("unl.True") ? "unl.True" : "unl.False");
			this.Settings.Add(rvtlist.Contains("und.True") ? "und.True" : "und.False");
			this.Settings.Add(rvtlist.Contains("rnc.True") ? "rnc.True" : "rnc.False");
			base.PopBox<ResearchProjectDef>(rvtlist, (ResearchProjectDef x) => x.tab != null && x.tab.defName == "Anomaly", new Func<ResearchProjectDef, string>(base.Colouredboxitem));
		}

		// Token: 0x0600017B RID: 379 RVA: 0x0000F734 File Offset: 0x0000D934
		public override bool CheckDoerWorker(Book b, string[] Data)
		{
			string text;
			Data.FirstSubString("cmp.", out text, "any");
			bool flag;
			Data.FirstSubString("unl.", out flag, false);
			bool flag2;
			Data.FirstSubString("und.", out flag2, false);
			bool flag3;
			Data.FirstSubString("rnc.", out flag3, false);
			List<ResearchProjectDef> list = base.CheckValueList<ResearchProjectDef, ReadingOutcomeDoerGainAnomalyResearch>(b);
			bool result = false;
			if (flag)
			{
				if (base.CmpSet<ResearchProjectDef>(text, list, (ResearchProjectDef x) => !x.IsHidden && !x.IsFinished))
				{
					result = true;
				}
			}
			if (flag2)
			{
				if (base.CmpSet<ResearchProjectDef>(text, list, (ResearchProjectDef x) => x.IsHidden))
				{
					result = true;
				}
			}
			if (flag3)
			{
				if (base.CmpSet<ResearchProjectDef>(text, list, (ResearchProjectDef x) => x.IsFinished))
				{
					result = true;
				}
			}
			if (base.CheckBox<ResearchProjectDef>(list, text == "all", Data))
			{
				result = true;
			}
			return result;
		}
	}
}
