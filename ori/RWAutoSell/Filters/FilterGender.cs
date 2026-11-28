using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200003E RID: 62
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterGender : IBaseFilter, IWikiHelp, IRuleSorter
	{
		// Token: 0x1700006E RID: 110
		// (get) Token: 0x060001FE RID: 510 RVA: 0x00013383 File Offset: 0x00011583
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FGender;
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0001338C File Offset: 0x0001158C
		public int Compare<[Nullable(0)] T>(T First, T Second) where T : Transferable
		{
			if (First == null)
			{
				if (Second == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (Second == null)
				{
					return 1;
				}
				Pawn pawn = First.AnyThing as Pawn;
				Pawn pawn2 = Second.AnyThing as Pawn;
				if (pawn == null)
				{
					if (pawn2 == null)
					{
						return 0;
					}
					return 1;
				}
				else
				{
					if (pawn2 == null)
					{
						return -1;
					}
					if (pawn.gender == pawn2.gender)
					{
						return 0;
					}
					if (pawn.gender == Gender.None)
					{
						return 1;
					}
					if (pawn2.gender == Gender.None)
					{
						return -1;
					}
					if (pawn.gender == Gender.Female)
					{
						return -1;
					}
					return 1;
				}
			}
		}

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000200 RID: 512 RVA: 0x00013418 File Offset: 0x00011618
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Gender-Filter";
			}
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x06000201 RID: 513 RVA: 0x0001341F File Offset: 0x0001161F
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterGender".Translate();
			}
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00013430 File Offset: 0x00011630
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (Data.Count == 0)
			{
				return false;
			}
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.hasGenders)
			{
				if (Data[0] == "gnd.ldy" && pawn.gender == Gender.Female)
				{
					return true;
				}
				if (Data[0] == "gnd.man" && pawn.gender == Gender.Male)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x000134A0 File Offset: 0x000116A0
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.hasGenders)
			{
				if (pawn.gender == Gender.Female)
				{
					list.Add("gnd.ldy");
				}
				else if (pawn.gender == Gender.Male)
				{
					list.Add("gnd.man");
				}
			}
			return list;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x000134FA File Offset: 0x000116FA
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000205 RID: 517 RVA: 0x00013500 File Offset: 0x00011700
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				if (Widgets.RadioButtonLabeled(new Rect(inrect.x + 4f, inrect.y + 2f, inrect.width - 6f, 24f), "Male".Translate(), this.GenderSelected == 1, false))
				{
					this.GenderSelected = 1;
				}
				if (Widgets.RadioButtonLabeled(new Rect(inrect.x + 4f, inrect.y + 2f + 24f, inrect.width - 6f, 24f), "Female".Translate(), this.GenderSelected == 2, false))
				{
					this.GenderSelected = 2;
				}
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x000135CE File Offset: 0x000117CE
		public void RenderStarted(List<string> Data, Map map)
		{
			if (Data.Count == 0)
			{
				this.GenderSelected = 1;
			}
			else if (Data[0] == "gnd.ldy")
			{
				this.GenderSelected = 2;
			}
			else
			{
				this.GenderSelected = 1;
			}
			this.AllowRender = true;
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0001360B File Offset: 0x0001180B
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			if (this.GenderSelected == 1)
			{
				Data.Add("gnd.man");
				return;
			}
			Data.Add("gnd.ldy");
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x06000208 RID: 520 RVA: 0x0001363A File Offset: 0x0001183A
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x0400011E RID: 286
		private bool AllowRender;

		// Token: 0x0400011F RID: 287
		private int GenderSelected = 1;
	}
}
