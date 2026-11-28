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
	// Token: 0x0200004D RID: 77
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterSkill : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060002B1 RID: 689 RVA: 0x000192F1 File Offset: 0x000174F1
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FSkill;
			}
		}

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060002B2 RID: 690 RVA: 0x000192F8 File Offset: 0x000174F8
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Skill-Filter";
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x060002B3 RID: 691 RVA: 0x000192FF File Offset: 0x000174FF
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x060002B4 RID: 692 RVA: 0x00019307 File Offset: 0x00017507
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterSkill".Translate();
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x00019318 File Offset: 0x00017518
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.skills != null)
			{
				SkillDef skillDef = null;
				int num = 0;
				int num2 = 20;
				int num3 = 0;
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					string text2 = text.Substring(4);
					if (a == "low.")
					{
						int.TryParse(text2, out num);
					}
					else if (a == "hgh.")
					{
						int.TryParse(text2, out num2);
					}
					else if (a == "skl.")
					{
						skillDef = DefDatabase<SkillDef>.GetNamedSilentFail(text2);
					}
					else if (a == "pas.")
					{
						int.TryParse(text2, out num3);
					}
				}
				if (skillDef != null)
				{
					SkillRecord skill = pawn.skills.GetSkill(skillDef);
					if (skill.Level >= num && skill.Level <= num2)
					{
						if (num3 == 0)
						{
							return true;
						}
						if (num3 == 1 && skill.passion == Passion.None)
						{
							return true;
						}
						if (num3 == 2 && (skill.passion == Passion.Minor || skill.passion == Passion.Major))
						{
							return true;
						}
						if (num3 == 3 && skill.passion == Passion.Major)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x00019468 File Offset: 0x00017668
		public void RenderStarted(List<string> Data, Map map)
		{
			this.range = new IntRange(0, 20);
			this.passion = 0;
			this.SelectedBox = new ASListBox<string>(20f, 5f, false, false);
			foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefs)
			{
				this.SelectedBox.Add(new SelectedItem<string>
				{
					Item = skillDef.defName,
					label = skillDef.LabelCap,
					surpresslabelupdate = true
				});
			}
			if (Data.Count > 0)
			{
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					string text2 = text.Substring(4);
					if (a == "low.")
					{
						int min;
						if (int.TryParse(text2, out min))
						{
							this.range.min = min;
						}
					}
					else if (a == "hgh.")
					{
						int max;
						if (int.TryParse(text2, out max))
						{
							this.range.max = max;
						}
					}
					else if (a == "skl.")
					{
						int itemIndex = this.SelectedBox.GetItemIndex(text2);
						if (itemIndex != -1)
						{
							this.SelectedBox.ChangeSelected(itemIndex);
						}
					}
					else if (a == "pas.")
					{
						int.TryParse(text2, out this.passion);
					}
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00019608 File Offset: 0x00017808
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			if (this.SelectedBox.GetSelectedIndex != -1)
			{
				Data.Add("low." + this.range.min.ToString());
				Data.Add("hgh." + this.range.max.ToString());
				Data.Add("skl." + this.SelectedBox.GetSelected);
				Data.Add("pas." + this.passion.ToString());
			}
			this.SelectedBox = null;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x000196B0 File Offset: 0x000178B0
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Widgets.IntRange(new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 8f, 31f), 10, ref this.range, 0, 20, null, 0);
				ASRectMonitor asrectMonitor = new ASRectMonitor(new Rect(inrect.x + 4f, inrect.y + 10f + 31f, inrect.width - 4f, inrect.height - 10f - 31f));
				if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterSkillHas".Translate(), this.passion == 0, false))
				{
					this.passion = 0;
				}
				if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterSkillHasNull".Translate(), this.passion == 1, false))
				{
					this.passion = 1;
				}
				if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterSkillHasMinor".Translate(), this.passion == 2, false))
				{
					this.passion = 2;
				}
				if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterSkillHasMajor".Translate(), this.passion == 3, false))
				{
					this.passion = 3;
				}
				this.SelectedBox.DrawListBox(asrectMonitor.Remaining());
			}
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00019838 File Offset: 0x00017A38
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.skills != null)
			{
				int num = 0;
				string str = string.Empty;
				foreach (SkillRecord skillRecord in pawn.skills.skills)
				{
					if (!skillRecord.TotallyDisabled && skillRecord.Level > num)
					{
						num = skillRecord.Level;
						str = skillRecord.def.defName;
					}
				}
				list.Add("low." + num.ToString());
				list.Add("hgh." + 20.ToString());
				list.Add("skl." + str);
			}
			return list;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00019924 File Offset: 0x00017B24
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0400016A RID: 362
		private IntRange range = new IntRange(0, 20);

		// Token: 0x0400016B RID: 363
		private const int RangeMin = 0;

		// Token: 0x0400016C RID: 364
		private const int RangeMax = 20;

		// Token: 0x0400016D RID: 365
		private int passion;

		// Token: 0x0400016E RID: 366
		private ASListBox<string> SelectedBox;

		// Token: 0x0400016F RID: 367
		private bool AllowRender;
	}
}
