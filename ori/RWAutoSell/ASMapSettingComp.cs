using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000013 RID: 19
	public class ASMapSettingComp : IExposable
	{
		// Token: 0x06000091 RID: 145 RVA: 0x00006CFC File Offset: 0x00004EFC
		[NullableContext(1)]
		public void CopyFrom(ASMapSettingComp source)
		{
			this.ASPartTrade = source.ASPartTrade;
			this.ASStackBeh = source.ASStackBeh;
			this.ASAITog = source.ASAITog;
			this.IncludeUnavalible = source.IncludeUnavalible;
			this.IncEquip = source.IncEquip;
			this.IncInv = source.IncInv;
			this.Repitition = source.Repitition;
			this.CustomShopping = source.CustomShopping;
			this.FractionCheck = source.FractionCheck;
			this.NotifyTrades = source.NotifyTrades;
			this.SelectedNegogiators.Clear();
			foreach (string item in source.SelectedNegogiators)
			{
				this.SelectedNegogiators.Add(item);
			}
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00006DD8 File Offset: 0x00004FD8
		public void ExposeData()
		{
			Scribe_Values.Look<bool>(ref this.ASPartTrade, "AllowPartialStacks", true, false);
			Scribe_Values.Look<bool>(ref this.ASStackBeh, "SmartStackBehavior", true, false);
			Scribe_Values.Look<bool>(ref this.ASAITog, "AllowAI", false, false);
			Scribe_Values.Look<bool>(ref this.IncludeUnavalible, "IncludeUnavalible", false, false);
			Scribe_Values.Look<bool>(ref this.IncEquip, "IncEquip", false, false);
			Scribe_Values.Look<bool>(ref this.IncInv, "IncInventory", false, false);
			Scribe_Values.Look<bool>(ref this.CustomShopping, "CustomShopping", false, false);
			Scribe_Values.Look<bool>(ref this.FractionCheck, "FractionCheck", false, false);
			Scribe_Values.Look<bool>(ref this.NotifyZeroTrades, "NotifyZeroTrades", false, false);
			Scribe_Values.Look<bool>(ref this.NotifyTrades, "NotifyTrades", true, false);
			Scribe_Values.Look<int>(ref this.Repitition, "Repitition", 1, false);
			Scribe_Collections.Look<string>(ref this.SelectedNegogiators, "CustomNegogiators", LookMode.Undefined, Array.Empty<object>());
			if (Scribe.mode == LoadSaveMode.PostLoadInit && this.SelectedNegogiators == null)
			{
				this.SelectedNegogiators = new List<string>();
			}
		}

		// Token: 0x0400005C RID: 92
		public bool ASPartTrade = true;

		// Token: 0x0400005D RID: 93
		public bool ASStackBeh = true;

		// Token: 0x0400005E RID: 94
		public bool ASAITog;

		// Token: 0x0400005F RID: 95
		public bool IncludeUnavalible;

		// Token: 0x04000060 RID: 96
		public bool IncEquip;

		// Token: 0x04000061 RID: 97
		public bool IncInv;

		// Token: 0x04000062 RID: 98
		public int Repitition = 1;

		// Token: 0x04000063 RID: 99
		public bool CustomShopping;

		// Token: 0x04000064 RID: 100
		public bool FractionCheck;

		// Token: 0x04000065 RID: 101
		public bool NotifyZeroTrades;

		// Token: 0x04000066 RID: 102
		public bool NotifyTrades = true;

		// Token: 0x04000067 RID: 103
		[Nullable(1)]
		public List<string> SelectedNegogiators = new List<string>();
	}
}
