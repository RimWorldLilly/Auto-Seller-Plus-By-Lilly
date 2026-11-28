using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000016 RID: 22
	[NullableContext(1)]
	[Nullable(0)]
	public class ASModSettings : ModSettings
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00007CC8 File Offset: 0x00005EC8
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00007CD0 File Offset: 0x00005ED0
		public Color NodeWindowbackcolor { get; private set; }

		// Token: 0x060000B4 RID: 180 RVA: 0x00007CDC File Offset: 0x00005EDC
		public ASModSettings()
		{
			this.ResetTheme(string.Empty);
			this.WhiteList = new Dictionary<string, bool>
			{
				{
					"RimWorld.Dialog_Trade",
					true
				},
				{
					"Glitterworld_Prime.DialogPrimeTrade",
					true
				}
			};
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00007D28 File Offset: 0x00005F28
		public void ResetTheme(string SpecificSetting)
		{
			if (SpecificSetting == "NodeWindowBackground" || SpecificSetting == string.Empty)
			{
				this.NodeWindowbackcolor = new Color(0.1f, 0.12f, 0.15f);
			}
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00007D60 File Offset: 0x00005F60
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.IncCat, "IncCat", false, false);
			Scribe_Values.Look<int>(ref this.Build, "Version", 0, false);
			Scribe_Values.Look<bool>(ref this.devlettershow, "DevLetter", false, false);
			Scribe_Values.Look<bool>(ref this.devTestAdditional, "DevTest", false, false);
			Scribe_Values.Look<bool>(ref this.devAILog, "DevAILog", false, false);
			Scribe_Values.Look<bool>(ref this.Kaching, "Kaching", true, false);
			float r = this.NodeWindowbackcolor.r;
			float g = this.NodeWindowbackcolor.g;
			float b = this.NodeWindowbackcolor.b;
			Scribe_Values.Look<float>(ref r, "NodeWindowbackcolorRed", 0.1f, false);
			Scribe_Values.Look<float>(ref g, "NodeWindowbackcolorGreen", 0.12f, false);
			Scribe_Values.Look<float>(ref b, "NodeWindowbackcolorBlue", 0.15f, false);
			this.NodeWindowbackcolor = new Color(r, g, b);
			Scribe_Collections.Look<string, bool>(ref this.WhiteList, "WhiteList", LookMode.Value, LookMode.Undefined);
			if (this.WhiteList == null || this.WhiteList.Count<KeyValuePair<string, bool>>() == 0)
			{
				this.WhiteList = new Dictionary<string, bool>
				{
					{
						"RimWorld.Dialog_Trade",
						true
					},
					{
						"Glitterworld_Prime.DialogPrimeTrade",
						true
					}
				};
			}
		}

		// Token: 0x04000075 RID: 117
		public bool IncCat;

		// Token: 0x04000076 RID: 118
		public bool devlettershow;

		// Token: 0x04000077 RID: 119
		public bool devTestAdditional;

		// Token: 0x04000078 RID: 120
		public bool devAILog;

		// Token: 0x04000079 RID: 121
		public bool Kaching;

		// Token: 0x0400007B RID: 123
		public Dictionary<string, bool> WhiteList = new Dictionary<string, bool>();

		// Token: 0x0400007C RID: 124
		public int Build;
	}
}
