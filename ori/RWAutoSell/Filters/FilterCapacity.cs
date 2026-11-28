using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200003C RID: 60
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterCapacity : IBaseFilter, IWikiHelp
	{
		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060001DE RID: 478 RVA: 0x000121AB File Offset: 0x000103AB
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x060001DF RID: 479 RVA: 0x000121B3 File Offset: 0x000103B3
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterCapacity".Translate();
			}
		}

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x060001E0 RID: 480 RVA: 0x000121C4 File Offset: 0x000103C4
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FCap;
			}
		}

		// Token: 0x17000069 RID: 105
		// (get) Token: 0x060001E1 RID: 481 RVA: 0x000121CB File Offset: 0x000103CB
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Carrying-Capacity-Filter";
			}
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x000121D2 File Offset: 0x000103D2
		private int Capacity(Pawn p)
		{
			if (!p.RaceProps.ToolUser && !p.RaceProps.packAnimal)
			{
				return 0;
			}
			return (int)(p.BodySize * 35f);
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x00012200 File Offset: 0x00010400
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				int num = this.Capacity(pawn);
				int val = 1;
				int val2 = 0;
				bool flag = false;
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					if (!(a == "cpc."))
					{
						if (!(a == "min."))
						{
							if (a == "nlc.")
							{
								int num2 = 0;
								int.TryParse(text.Substring(4), out num2);
								if (num2 == 1)
								{
									flag = true;
								}
							}
						}
						else
						{
							int.TryParse(text.Substring(4), out val);
						}
					}
					else
					{
						int.TryParse(text.Substring(4), out val2);
					}
				}
				if (!flag && num == 0)
				{
					return false;
				}
				if (num >= Math.Min(val2, val) && num <= Math.Max(val2, val))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00012304 File Offset: 0x00010504
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				if (!pawn.RaceProps.ToolUser && !pawn.RaceProps.packAnimal)
				{
					return list;
				}
				list.Add("cpc." + Mathf.CeilToInt(pawn.BodySize * 35f).ToString());
				list.Add("min.0");
				list.Add("nlc.0");
			}
			return list;
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x00012382 File Offset: 0x00010582
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x00012388 File Offset: 0x00010588
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Rect rect = new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 8f, 30f);
				Widgets.CheckboxLabeled(rect, "RWAutoSell.FilterCapacityIncNull".Translate(), ref this.IncludeNull, false, null, null, false, false);
				Widgets.IntRange(new Rect(rect.x, rect.yMax, rect.width, 31f), 38, ref this.fr, 0, this.MaxCap, "RWAutoSell.FilterCapacityRange", 0);
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0001242C File Offset: 0x0001062C
		public void RenderStarted(List<string> Data, Map map)
		{
			int val = 0;
			int val2 = 0;
			this.IncludeNull = false;
			new Pawn();
			if (this.MaxCap == 0)
			{
				float num = 0f;
				foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefs)
				{
					float b = Mathf.Max((from el in pawnKindDef.RaceProps.lifeStageAges
					select el.def.bodySizeFactor).ToArray<float>()) * pawnKindDef.RaceProps.baseBodySize;
					num = Mathf.Max(num, b);
				}
				this.MaxCap = Mathf.CeilToInt(num * 35f);
			}
			foreach (string text in Data)
			{
				string a = text.Substring(0, 4);
				if (!(a == "cpc."))
				{
					if (!(a == "min."))
					{
						if (a == "nlc.")
						{
							int num2 = 0;
							int.TryParse(text.Substring(4), out num2);
							if (num2 == 1)
							{
								this.IncludeNull = true;
							}
						}
					}
					else
					{
						int.TryParse(text.Substring(4), out val);
					}
				}
				else
				{
					int.TryParse(text.Substring(4), out val2);
				}
			}
			new IntRange(Math.Min(val2, val), Math.Max(val2, val));
			this.AllowRender = true;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x000125CC File Offset: 0x000107CC
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			int num = Math.Min(this.fr.min, this.fr.max);
			Data.Add("cpc." + Math.Max(this.fr.min, this.fr.max).ToString());
			Data.Add("min." + num.ToString());
			Data.Add("nlc." + (this.IncludeNull ? "1" : "0"));
		}

		// Token: 0x0400010F RID: 271
		private int MaxCap;

		// Token: 0x04000110 RID: 272
		private bool AllowRender;

		// Token: 0x04000111 RID: 273
		private bool IncludeNull;

		// Token: 0x04000112 RID: 274
		private IntRange fr = new IntRange(0, 1);
	}
}
