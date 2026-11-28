using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000037 RID: 55
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterAge : IBaseFilter, IWikiHelp, IRuleSorter
	{
		// Token: 0x060001A5 RID: 421 RVA: 0x00010028 File Offset: 0x0000E228
		public int Compare<[Nullable(0)] T>(T First, T Second) where T : Transferable
		{
			float num = 0f;
			float value = 0f;
			Pawn pawn = First.AnyThing as Pawn;
			if (pawn != null)
			{
				num = pawn.ageTracker.AgeBiologicalYearsFloat;
			}
			Pawn pawn2 = Second.AnyThing as Pawn;
			if (pawn2 != null)
			{
				value = pawn2.ageTracker.AgeBiologicalYearsFloat;
			}
			return num.CompareTo(value);
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001A6 RID: 422 RVA: 0x00010089 File Offset: 0x0000E289
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FAge;
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001A7 RID: 423 RVA: 0x00010090 File Offset: 0x0000E290
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Age-Filter";
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001A8 RID: 424 RVA: 0x00010097 File Offset: 0x0000E297
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterAge".Translate();
			}
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x000100A8 File Offset: 0x0000E2A8
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				if (Data.Contains("bio.growth"))
				{
					float num = -1f;
					float num2 = -1f;
					if (!float.TryParse(Data.FirstOrFallback((string x) => x.Substring(0, 4) == "lwf.", "lwf.0").Substring(4), out num))
					{
						num = 0f;
					}
					if (num < 0f)
					{
						num = 0f;
					}
					if (!float.TryParse(Data.FirstOrFallback((string x) => x.Substring(0, 4) == "upf.", "upf.1").Substring(4), out num2))
					{
						num2 = 1f;
					}
					if (num2 < 0f || num2 > 1f)
					{
						num2 = 1f;
					}
					if (num > num2)
					{
						float num3 = num;
						num = num2;
						num2 = num3;
					}
					return pawn.ageTracker.Growth >= num && pawn.ageTracker.Growth <= num2;
				}
				int num4 = -1;
				int num5 = -1;
				if (!int.TryParse(Data.FirstOrFallback((string x) => x.Substring(0, 4) == "lwr.", "lwr.0").Substring(4), out num4))
				{
					num4 = 0;
				}
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (!int.TryParse(Data.FirstOrFallback((string x) => x.Substring(0, 4) == "upr.", "upr." + 2147483647.ToString()).Substring(4), out num5))
				{
					num5 = int.MaxValue;
				}
				if (num5 < 0)
				{
					num5 = int.MaxValue;
				}
				if (num4 > num5)
				{
					int num6 = num4;
					num4 = num5;
					num5 = num6;
				}
				if (Data.Contains("bio.false"))
				{
					return pawn.ageTracker.AgeChronologicalYears >= num4 && pawn.ageTracker.AgeChronologicalYears <= num5;
				}
				if (Data.Contains("bio.true"))
				{
					return pawn.ageTracker.AgeBiologicalYears >= num4 && pawn.ageTracker.AgeBiologicalYears <= num5;
				}
			}
			return false;
		}

		// Token: 0x060001AA RID: 426 RVA: 0x000102C4 File Offset: 0x0000E4C4
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x060001AB RID: 427 RVA: 0x000102CB File Offset: 0x0000E4CB
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001AC RID: 428 RVA: 0x000102D0 File Offset: 0x0000E4D0
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Rect rect = new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 4f, 24f);
				if (Widgets.RadioButtonLabeled(rect, "RWAutoSell.FilterAgeBio".Translate(), this.Biological == 1, false))
				{
					this.Biological = 1;
				}
				Rect rect2 = new Rect(inrect.x + 4f, rect.yMax + 4f, inrect.width - 4f, 24f);
				if (Widgets.RadioButtonLabeled(rect2, "RWAutoSell.FilterAgeChr".Translate(), this.Biological == 2, false))
				{
					this.Biological = 2;
				}
				Rect rect3 = new Rect(inrect.x + 4f, rect2.yMax + 4f, inrect.width - 4f, 24f);
				if (Widgets.RadioButtonLabeled(rect3, "RWAutoSell.FilterAgeGrowth".Translate(), this.Biological == 3, false))
				{
					this.Biological = 3;
				}
				ASTable astable = new ASTable(new Rect(inrect.x, rect3.yMax + 4f, inrect.width, 25f), 2, 1, 2f, 0f, true);
				string text = this.LowerRange.ToString();
				string text2 = this.UpperRange.ToString();
				if (this.Biological != 3)
				{
					Widgets.TextFieldNumericLabeled<int>(astable.GetRectangle(0, 0, -1, -1, false), "RWAutoSell.Minimum".Translate(), ref this.LowerRange, ref text, 0f, 2.1474836E+09f);
					Widgets.TextFieldNumericLabeled<int>(astable.GetRectangle(1, 0, -1, -1, false), "RWAutoSell.Maximum".Translate(), ref this.UpperRange, ref text2, 0f, 2.1474836E+09f);
					return;
				}
				Widgets.FloatRange(astable.GetRectangle(0, 0, 1, -1, false), 10, ref this.growthrange, 0f, 1f, null, ToStringStyle.PercentTwo, 0f, GameFont.Small, null, 0f);
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x000104F4 File Offset: 0x0000E6F4
		public void RenderStarted(List<string> Data, Map map)
		{
			this.Biological = 1;
			this.LowerRange = 0;
			this.UpperRange = int.MaxValue;
			foreach (string text in Data)
			{
				string a = text.Substring(0, 4);
				if (!(a == "bio."))
				{
					if (!(a == "lwr."))
					{
						if (!(a == "upr."))
						{
							if (!(a == "lwf."))
							{
								if (a == "upf.")
								{
									float max;
									float.TryParse(text.Substring(4), out max);
									this.growthrange.max = max;
								}
							}
							else
							{
								float min;
								float.TryParse(text.Substring(4), out min);
								this.growthrange.min = min;
							}
						}
						else
						{
							int.TryParse(text.Substring(4), out this.UpperRange);
						}
					}
					else
					{
						int.TryParse(text.Substring(4), out this.LowerRange);
					}
				}
				else if (text.Substring(4) == "false")
				{
					this.Biological = 2;
				}
				else if (text.Substring(4) == "growth")
				{
					this.Biological = 3;
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x00010660 File Offset: 0x0000E860
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			if (this.Biological == 1)
			{
				Data.Add("bio.true");
			}
			else if (this.Biological == 2)
			{
				Data.Add("bio.false");
			}
			else if (this.Biological == 3)
			{
				Data.Add("bio.growth");
			}
			Data.Add("lwr." + Math.Min(this.LowerRange, this.UpperRange).ToString());
			Data.Add("upr." + Math.Max(this.LowerRange, this.UpperRange).ToString());
			Data.Add("lwf." + this.growthrange.TrueMin.ToString());
			Data.Add("upf." + this.growthrange.TrueMax.ToString());
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001AF RID: 431 RVA: 0x00010752 File Offset: 0x0000E952
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x040000F4 RID: 244
		private bool AllowRender;

		// Token: 0x040000F5 RID: 245
		private int Biological = 1;

		// Token: 0x040000F6 RID: 246
		private int LowerRange;

		// Token: 0x040000F7 RID: 247
		private int UpperRange = int.MaxValue;

		// Token: 0x040000F8 RID: 248
		private FloatRange growthrange = new FloatRange(0f, 1f);
	}
}
