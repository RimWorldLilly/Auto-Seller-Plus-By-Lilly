using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using RWAutoSell.Mergers;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000022 RID: 34
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterPerishable : IBaseFilter, IRequiredMerger, IWikiHelp
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000CCD7 File Offset: 0x0000AED7
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagFood"
		};

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000115 RID: 277 RVA: 0x0000CCDF File Offset: 0x0000AEDF
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FPerish;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000116 RID: 278 RVA: 0x0000CCE6 File Offset: 0x0000AEE6
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterPerish".Translate();
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000117 RID: 279 RVA: 0x0000CCF7 File Offset: 0x0000AEF7
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Time-Until-Rot-Filter";
			}
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000CD00 File Offset: 0x0000AF00
		public bool CheckItem(Transferable t, List<string> Data)
		{
			CompRottable compRottable = t.AnyThing.TryGetComp<CompRottable>();
			if (compRottable != null)
			{
				bool flag = Data.Any((string x) => x == "ops.irf");
				int num = int.Parse(Data.FirstOrFallback((string x) => x.StartsWith("ths."), "ths.0").Substring(4));
				if (flag)
				{
					if (num > compRottable.TicksUntilRotAtTemp(18f))
					{
						return true;
					}
				}
				else if (num > compRottable.TicksUntilRotAtCurrentTemp)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000CD98 File Offset: 0x0000AF98
		public List<string> DataFromTradeable(Transferable t)
		{
			CompRottable compRottable = t.AnyThing.TryGetComp<CompRottable>();
			List<string> list = new List<string>();
			if (compRottable != null)
			{
				list.Add("ths." + compRottable.TicksUntilRotAtCurrentTemp.ToString());
			}
			return list;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000CDD9 File Offset: 0x0000AFD9
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000CDDC File Offset: 0x0000AFDC
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			Rect rect = new Rect(inrect.x, inrect.y, inrect.width, 24f);
			bool flag = Data.Any((string x) => x == "ops.irf");
			bool flag2 = flag;
			Widgets.CheckboxLabeled(rect, "RWAutoSell.FilterPerishIgnRfg".Translate(), ref flag, false, null, null, false, false);
			if (flag != flag2)
			{
				if (flag)
				{
					Data.Add("ops.irf");
				}
				else
				{
					Data.Remove("ops.irf");
				}
			}
			ASTable astable = new ASTable(new Rect(rect.x, rect.yMax, rect.width, 60f), 2, 2, 0f, 0f, false);
			Widgets.Label(astable.GetRectangle(0, 0, -1, -1, false), "RWAutoSell.FilterPerishDays".Translate());
			Widgets.Label(astable.GetRectangle(1, 0, -1, -1, false), "RWAutoSell.FilterPerishHours".Translate());
			Widgets.TextFieldNumeric<int>(astable.GetRectangle(0, 1, -1, -1, false), ref this.days, ref this.cacheday, 0f, 1E+09f);
			Widgets.TextFieldNumeric<int>(astable.GetRectangle(1, 1, -1, -1, false), ref this.hours, ref this.cachehours, 0f, 1E+09f);
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000CF20 File Offset: 0x0000B120
		public void RenderStarted(List<string> Data, Map map)
		{
			if (!Data.Any((string x) => x.StartsWith("ths.")))
			{
				Data.Add("ths.0");
				this.days = 0;
				this.hours = 0;
			}
			else
			{
				int num = int.Parse(Data.First((string x) => x.StartsWith("ths.")).Substring(4));
				if (num >= 60000)
				{
					this.days = num / 60000;
				}
				else
				{
					this.days = 0;
				}
				if (num >= 2500)
				{
					this.hours = num % 60000 / 2500;
				}
				else
				{
					this.hours = 0;
				}
			}
			this.cachehours = null;
			this.cacheday = null;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x0000CFF0 File Offset: 0x0000B1F0
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.RemoveAll((string x) => x.StartsWith("ths."));
			long num = (long)(this.days * 60000 + this.hours * 2500);
			Data.Add("ths." + ((int)num).ToString());
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000D058 File Offset: 0x0000B258
		public string RequiredEnabled()
		{
			return typeof(RotMerger).FullName;
		}

		// Token: 0x040000CD RID: 205
		private string cacheday = string.Empty;

		// Token: 0x040000CE RID: 206
		private string cachehours = string.Empty;

		// Token: 0x040000CF RID: 207
		private int days;

		// Token: 0x040000D0 RID: 208
		private int hours;
	}
}
