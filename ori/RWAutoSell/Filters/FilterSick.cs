using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200004C RID: 76
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterSick : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x00018E44 File Offset: 0x00017044
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FSick;
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060002A7 RID: 679 RVA: 0x00018E4B File Offset: 0x0001704B
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Sick-Filter";
			}
		}

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x00018E52 File Offset: 0x00017052
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterIll".Translate();
			}
		}

		// Token: 0x060002A9 RID: 681 RVA: 0x00018E64 File Offset: 0x00017064
		public bool CheckItem(Transferable t, List<string> Data)
		{
			int num = 0;
			int num2 = 0;
			bool flag = false;
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				foreach (string text in Data)
				{
					if (text.Substring(0, 4) == "inf.")
					{
						foreach (Hediff hd in pawn.health.hediffSet.hediffs)
						{
							HediffComp_Immunizable hediffComp_Immunizable = hd.TryGetComp<HediffComp_Immunizable>();
							if (hediffComp_Immunizable != null)
							{
								num2++;
								if (hediffComp_Immunizable.FullyImmune)
								{
									num++;
								}
							}
						}
						if (num2 == 0 || (num2 == num && text.Substring(4) == "recv"))
						{
							flag = flag;
						}
						else if (num2 != 0)
						{
							flag = true;
						}
					}
					else if (text.Substring(0, 4) == "bld." && pawn.RaceProps.IsFlesh && pawn.health.hediffSet.BleedRateTotal > 0f)
					{
						flag |= true;
					}
				}
			}
			return flag;
		}

		// Token: 0x060002AA RID: 682 RVA: 0x00018FA8 File Offset: 0x000171A8
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				int num = 0;
				int num2 = 0;
				foreach (Hediff hd in pawn.health.hediffSet.hediffs)
				{
					HediffComp_Immunizable hediffComp_Immunizable = hd.TryGetComp<HediffComp_Immunizable>();
					if (hediffComp_Immunizable != null)
					{
						num2++;
						if (hediffComp_Immunizable.FullyImmune)
						{
							num++;
						}
					}
				}
				string text = string.Empty;
				if (num2 != 0)
				{
					text += "inf.";
				}
				if (num2 == num)
				{
					text += "recv";
				}
				else
				{
					text += "any";
				}
				if (num2 != 0)
				{
					list.Add(text);
				}
				if (pawn.health.hediffSet.BleedRateTotal > 0f)
				{
					list.Add("bld.any");
				}
			}
			return list;
		}

		// Token: 0x060002AB RID: 683 RVA: 0x000190A0 File Offset: 0x000172A0
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060002AC RID: 684 RVA: 0x000190A4 File Offset: 0x000172A4
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 2f, inrect.width - 6f, 24f), "RWAutoSell.FilterIllChkInf".Translate(), ref this.infection, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 24f + 2f, inrect.width - 6f, 24f), "RWAutoSell.FilterIllIgrRcv".Translate(), ref this.ignorerecovering, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 72f + 2f, inrect.width - 6f, 24f), "RWAutoSell.FilterIllChkBld".Translate(), ref this.bleeding, false, null, null, false, false);
			}
		}

		// Token: 0x060002AD RID: 685 RVA: 0x000191B8 File Offset: 0x000173B8
		public void RenderStarted(List<string> Data, Map map)
		{
			this.infection = false;
			this.ignorerecovering = false;
			this.bleeding = false;
			foreach (string text in Data)
			{
				if (text.Substring(0, 4) == "inf.")
				{
					this.infection = true;
					if (text.Substring(4) == "recv")
					{
						this.ignorerecovering = true;
					}
				}
				else if (text.Substring(0, 4) == "bld.")
				{
					this.bleeding = true;
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060002AE RID: 686 RVA: 0x00019270 File Offset: 0x00017470
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			if (this.infection)
			{
				if (this.ignorerecovering)
				{
					Data.Add("inf.recv");
				}
				else
				{
					Data.Add("inf.any");
				}
			}
			if (this.bleeding)
			{
				Data.Add("bld.any");
			}
		}

		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060002AF RID: 687 RVA: 0x000192C5 File Offset: 0x000174C5
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x04000165 RID: 357
		private bool AllowRender;

		// Token: 0x04000166 RID: 358
		private bool infection;

		// Token: 0x04000167 RID: 359
		private bool ignorerecovering;

		// Token: 0x04000168 RID: 360
		private bool bleeding;
	}
}
