using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000040 RID: 64
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterHealth : IBaseFilter, IWikiHelp, IRuleSorter
	{
		// Token: 0x17000076 RID: 118
		// (get) Token: 0x06000219 RID: 537 RVA: 0x0001409F File Offset: 0x0001229F
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FHealth;
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x000140A8 File Offset: 0x000122A8
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
				Pawn pawn2 = First.AnyThing as Pawn;
				if (First.AnyThing.MaxHitPoints > 0 || pawn != null)
				{
					if (Second.AnyThing.MaxHitPoints <= 0 && pawn2 == null)
					{
						return -1;
					}
					float num;
					if (pawn != null)
					{
						num = pawn.health.summaryHealth.SummaryHealthPercent;
					}
					else if (First.AnyThing.def.useHitPoints)
					{
						num = (float)First.AnyThing.HitPoints / (float)First.AnyThing.MaxHitPoints;
					}
					else
					{
						num = 1f;
					}
					float num2;
					if (pawn2 != null)
					{
						num2 = pawn2.health.summaryHealth.SummaryHealthPercent;
					}
					else if (Second.AnyThing.def.useHitPoints)
					{
						num2 = (float)Second.AnyThing.HitPoints / (float)Second.AnyThing.MaxHitPoints;
					}
					else
					{
						num2 = 1f;
					}
					if (num > num2)
					{
						return -1;
					}
					if (num2 > num)
					{
						return 1;
					}
				}
				else if (Second.AnyThing.MaxHitPoints > 0 || pawn2 != null)
				{
					return 1;
				}
				return 0;
			}
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600021B RID: 539 RVA: 0x00014200 File Offset: 0x00012400
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Health-Filter";
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600021C RID: 540 RVA: 0x00014207 File Offset: 0x00012407
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterHp".Translate();
			}
		}

		// Token: 0x0600021D RID: 541 RVA: 0x00014218 File Offset: 0x00012418
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (Data.Count == 2)
			{
				float val;
				float.TryParse(Data[0], out val);
				float val2;
				float.TryParse(Data[1], out val2);
				Pawn pawn = t.AnyThing as Pawn;
				float num;
				if (pawn != null)
				{
					num = pawn.health.summaryHealth.SummaryHealthPercent;
				}
				else
				{
					if (t.AnyThing.MaxHitPoints <= 0)
					{
						return false;
					}
					num = (float)t.AnyThing.HitPoints / (float)t.AnyThing.MaxHitPoints;
				}
				if (num >= Math.Min(val, val2) && num <= Math.Max(val, val2))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600021E RID: 542 RVA: 0x000142BC File Offset: 0x000124BC
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				float summaryHealthPercent = pawn.health.summaryHealth.SummaryHealthPercent;
				list = new List<string>
				{
					summaryHealthPercent.ToString(),
					summaryHealthPercent.ToString()
				};
			}
			else if (t.AnyThing.MaxHitPoints > 0)
			{
				list.Add(((float)t.AnyThing.HitPoints / (float)t.AnyThing.MaxHitPoints).ToString());
				list.Add(((float)t.AnyThing.HitPoints / (float)t.AnyThing.MaxHitPoints).ToString());
			}
			return list;
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0001436E File Offset: 0x0001256E
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x00014374 File Offset: 0x00012574
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.allowrender)
			{
				Widgets.FloatRange(new Rect(inrect.x + 4f, inrect.y + 2f, inrect.width - 4f, 31f), 1, ref this.range, 0f, 1f, "HitPoints", ToStringStyle.PercentZero, 0f, GameFont.Small, null, 0f);
			}
		}

		// Token: 0x06000221 RID: 545 RVA: 0x000143EC File Offset: 0x000125EC
		public void RenderStarted(List<string> Data, Map map)
		{
			if (Data.Count == 2)
			{
				float a = 0f;
				float b = 1f;
				float num;
				if (float.TryParse(Data[0], out num))
				{
					a = num;
				}
				float num2;
				if (float.TryParse(Data[1], out num2))
				{
					b = num2;
				}
				this.range.min = Mathf.Min(a, b);
				this.range.max = Mathf.Max(a, b);
			}
			else
			{
				this.range.min = 0f;
				this.range.max = 1f;
			}
			this.allowrender = true;
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0001447F File Offset: 0x0001267F
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			Data.Add(this.range.min.ToString());
			Data.Add(this.range.max.ToString());
			this.allowrender = false;
		}

		// Token: 0x17000079 RID: 121
		// (get) Token: 0x06000223 RID: 547 RVA: 0x000144BA File Offset: 0x000126BA
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople",
			"RWAutoSell.TagItems",
			"RWAutoSell.TagFood"
		};

		// Token: 0x0400012A RID: 298
		private bool allowrender;

		// Token: 0x0400012B RID: 299
		private FloatRange range = new FloatRange(0f, 1f);
	}
}
