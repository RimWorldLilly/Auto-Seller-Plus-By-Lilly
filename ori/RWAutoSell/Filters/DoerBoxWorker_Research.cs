using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000031 RID: 49
	[NullableContext(1)]
	[Nullable(0)]
	public class DoerBoxWorker_Research : DoerBoxWorker
	{
		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600017C RID: 380 RVA: 0x0000F838 File Offset: 0x0000DA38
		public override string Doerclass
		{
			get
			{
				return "RimWorld.ReadingOutcomeDoerGainResearch";
			}
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x0600017D RID: 381 RVA: 0x0000F83F File Offset: 0x0000DA3F
		public override string DoerLabel
		{
			get
			{
				return "RWAutoSell.FilterLearningResearch";
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x0600017E RID: 382 RVA: 0x0000F846 File Offset: 0x0000DA46
		public override int Expandedsize
		{
			get
			{
				return 120;
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0000F84C File Offset: 0x0000DA4C
		public DoerBoxWorker_Research()
		{
			if (this.Settingsdefault == null || this.Settingsdefault.Count == 0)
			{
				this.Settingsdefault = new List<string>
				{
					"cmp.any",
					"unl.False",
					"tpr.False",
					"rnc.False"
				};
			}
			this.Box = new ASMultiBox<string>(25f, 4f, true, false);
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000F8C4 File Offset: 0x0000DAC4
		public override void DoerSettingsWorker(string[] rvtlist)
		{
			this.Settings.Add(rvtlist.Contains("cmp.all") ? "cmp.all" : "cmp.any");
			this.Settings.Add(rvtlist.Contains("unl.True") ? "unl.True" : "unl.False");
			this.Settings.Add(rvtlist.Contains("tpr.True") ? "tpr.True" : "tpr.False");
			this.Settings.Add(rvtlist.Contains("rnc.True") ? "rnc.True" : "rnc.False");
			base.PopBox<ResearchProjectDef>(rvtlist, (ResearchProjectDef x) => x.tab != null && x.tab.defName != "Anomaly", new Func<ResearchProjectDef, string>(this.Colouredboxitem));
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0000F9A8 File Offset: 0x0000DBA8
		protected string Colouredboxitem(ResearchProjectDef def)
		{
			string text = def.LabelCap;
			if (def.IsFinished)
			{
				text = text.Colorize(Color.blue);
			}
			if (def.CanStartNow)
			{
				text = text.Colorize(Color.green);
			}
			if (!def.AnalyzedThingsRequirementsMet || !def.TechprintRequirementMet)
			{
				text = text.Colorize(Color.grey);
			}
			else
			{
				text = text.Colorize(Color.white);
			}
			return text;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000FA14 File Offset: 0x0000DC14
		public override bool CheckDoerWorker(Book b, string[] Data)
		{
			string text;
			Data.FirstSubString("cmp.", out text, "any");
			bool flag;
			Data.FirstSubString("unl.", out flag, false);
			bool flag2;
			Data.FirstSubString("tpr.", out flag2, false);
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
				if (base.CmpSet<ResearchProjectDef>(text, list, (ResearchProjectDef x) => x.TechprintRequirementMet))
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
