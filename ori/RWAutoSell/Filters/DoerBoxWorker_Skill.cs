using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASWidgets;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000032 RID: 50
	[NullableContext(1)]
	[Nullable(0)]
	public class DoerBoxWorker_Skill : DoerBoxWorker
	{
		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000183 RID: 387 RVA: 0x0000FB18 File Offset: 0x0000DD18
		public override string Doerclass
		{
			get
			{
				return "RimWorld.BookOutcomeDoerGainSkillExp";
			}
		}

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000184 RID: 388 RVA: 0x0000FB1F File Offset: 0x0000DD1F
		public override string DoerLabel
		{
			get
			{
				return "RWAutoSell.FilterLearningSkill";
			}
		}

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000185 RID: 389 RVA: 0x0000FB26 File Offset: 0x0000DD26
		public override int Expandedsize
		{
			get
			{
				return 30;
			}
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0000FB2C File Offset: 0x0000DD2C
		public DoerBoxWorker_Skill()
		{
			this.Settingsdefault = new List<string>
			{
				"cmp.any",
				"unl.False",
				"tpr.False",
				"rnc.False"
			};
			this.Box = new ASMultiBox<string>(25f, 4f, true, false);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x0000FB8D File Offset: 0x0000DD8D
		public override void DoerSettingsWorker(string[] rvtlist)
		{
			this.Settings.Add(rvtlist.Contains("cmp.all") ? "cmp.all" : "cmp.any");
			base.PopBox<SkillDef>(rvtlist, null, null);
		}

		// Token: 0x06000188 RID: 392 RVA: 0x0000FBC4 File Offset: 0x0000DDC4
		public override bool CheckDoerWorker(Book b, string[] Data)
		{
			string a;
			Data.FirstSubString("cmp.", out a, "any");
			List<SkillDef> list = base.CheckValueList<SkillDef, BookOutcomeDoerGainSkillExp>(b);
			return list.Count != 0 && base.CheckBox<SkillDef>(list, a == "all", Data);
		}
	}
}
