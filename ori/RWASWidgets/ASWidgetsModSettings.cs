using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000004 RID: 4
	[NullableContext(1)]
	[Nullable(0)]
	public class ASWidgetsModSettings : ModSettings
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600000C RID: 12 RVA: 0x00002560 File Offset: 0x00000760
		// (set) Token: 0x0600000D RID: 13 RVA: 0x00002568 File Offset: 0x00000768
		public Color TabBackColor { get; private set; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x0600000E RID: 14 RVA: 0x00002571 File Offset: 0x00000771
		// (set) Token: 0x0600000F RID: 15 RVA: 0x00002579 File Offset: 0x00000779
		public Color SelectedTabBackColor { get; private set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000010 RID: 16 RVA: 0x00002582 File Offset: 0x00000782
		// (set) Token: 0x06000011 RID: 17 RVA: 0x0000258A File Offset: 0x0000078A
		public Color TabBorderColor { get; private set; }

		// Token: 0x06000012 RID: 18 RVA: 0x00002593 File Offset: 0x00000793
		public ASWidgetsModSettings()
		{
			this.ResetTheme(string.Empty);
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000025B4 File Offset: 0x000007B4
		public void ResetTheme(string SpecificSetting)
		{
			if (SpecificSetting == "TabBackColor" || SpecificSetting == string.Empty)
			{
				this.TabBackColor = new Color(0f, 0.3f, 0.37f);
			}
			if (SpecificSetting == "SelectedTabBackColor" || SpecificSetting == string.Empty)
			{
				this.SelectedTabBackColor = new Color(0f, 0.45f, 0.5f);
			}
			if (SpecificSetting == "TabBorderColor" || SpecificSetting == string.Empty)
			{
				this.TabBorderColor = new Color(0f, 0.7f, 0.8f);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002660 File Offset: 0x00000860
		public override void ExposeData()
		{
			base.ExposeData();
			float r = this.TabBackColor.r;
			float g = this.TabBackColor.g;
			float b = this.TabBackColor.b;
			Scribe_Values.Look<float>(ref r, "TabBackColorRed", 0f, false);
			Scribe_Values.Look<float>(ref g, "TabBackColorGreen", 0.3f, false);
			Scribe_Values.Look<float>(ref b, "TabBackColorBlue", 0.37f, false);
			this.TabBackColor = new Color(r, g, b);
			float r2 = this.SelectedTabBackColor.r;
			float g2 = this.SelectedTabBackColor.g;
			float b2 = this.SelectedTabBackColor.b;
			Scribe_Values.Look<float>(ref r2, "SelectedTabBackColorRed", 0f, false);
			Scribe_Values.Look<float>(ref g2, "SelectedTabBackColorGreen", 0.45f, false);
			Scribe_Values.Look<float>(ref b2, "SelectedTabBackColorBlue", 0.5f, false);
			this.SelectedTabBackColor = new Color(r2, g2, b2);
			float r3 = this.TabBorderColor.r;
			float g3 = this.TabBorderColor.g;
			float b3 = this.TabBorderColor.b;
			Scribe_Values.Look<float>(ref r3, "TabBorderColorRed", 0f, false);
			Scribe_Values.Look<float>(ref g3, "TabBorderColorGreen", 0.7f, false);
			Scribe_Values.Look<float>(ref b3, "TabBorderColorBlue", 0.8f, false);
			this.TabBorderColor = new Color(r3, g3, b3);
			Scribe_Collections.Look<string>(ref this.Collapsedlist, "CollapsedCategories", LookMode.Value, Array.Empty<object>());
			if (this.Collapsedlist == null)
			{
				this.Collapsedlist = new List<string>();
			}
		}

		// Token: 0x04000004 RID: 4
		public List<string> Collapsedlist = new List<string>();
	}
}
