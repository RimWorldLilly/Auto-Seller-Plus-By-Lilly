using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200004B RID: 75
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterSerum : IBaseFilter, IWikiHelp
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x0600029B RID: 667 RVA: 0x00018654 File Offset: 0x00016854
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FSerum;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x0600029C RID: 668 RVA: 0x0001865B File Offset: 0x0001685B
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Serum-Filter";
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x0600029D RID: 669 RVA: 0x00018662 File Offset: 0x00016862
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterSerum".Translate();
			}
		}

		// Token: 0x0600029E RID: 670 RVA: 0x00018674 File Offset: 0x00016874
		public bool CheckItem(Transferable t, List<string> Data)
		{
			CompUseEffect_LearnSkill compUseEffect_LearnSkill = t.AnyThing.TryGetComp<CompUseEffect_LearnSkill>();
			if (compUseEffect_LearnSkill != null)
			{
				if (compUseEffect_LearnSkill.Props.skill == null)
				{
					return false;
				}
				string defName = compUseEffect_LearnSkill.Props.skill.defName;
				using (List<string>.Enumerator enumerator = Data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						if (text.Substring(0, 4) == "skl." && text.Substring(4) == defName)
						{
							return true;
						}
					}
					return false;
				}
			}
			CompUseEffect_GainAbility compUseEffect_GainAbility = t.AnyThing.TryGetComp<CompUseEffect_GainAbility>();
			if (compUseEffect_GainAbility != null && compUseEffect_GainAbility.Props.ability != null)
			{
				string defName2 = compUseEffect_GainAbility.Props.ability.defName;
				foreach (string text2 in Data)
				{
					if (text2.Substring(0, 4) == "psy." && text2.Substring(4) == defName2)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x000187B4 File Offset: 0x000169B4
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (t.HasAnyThing)
			{
				CompUseEffect_LearnSkill compUseEffect_LearnSkill = t.AnyThing.TryGetComp<CompUseEffect_LearnSkill>();
				if (compUseEffect_LearnSkill != null && compUseEffect_LearnSkill.Props.skill != null)
				{
					list.Add("skl." + compUseEffect_LearnSkill.Props.skill.defName);
				}
			}
			return list;
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0001880C File Offset: 0x00016A0C
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x00018810 File Offset: 0x00016A10
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				ASRectMonitor asrectMonitor = inrect;
				if (!ModLister.RoyaltyInstalled && Data.Count > 0 && Data.First<string>().StartsWith("psy.") && this.locked)
				{
					Widgets.Label(asrectMonitor.NextFromTop(25f), "Royalty Not Installed,");
					Widgets.Label(asrectMonitor.NextFromTop(25f), "but filter container has data specific to Royalty.");
					asrectMonitor.NextFromTop(25f);
					Widgets.Label(asrectMonitor.NextFromTop(25f), "Royalty Not Installed,");
					asrectMonitor.NextFromTop(25f);
					Widgets.Label(asrectMonitor.NextFromTop(25f), "Editing will wipe current filter container data,");
					Widgets.Label(asrectMonitor.NextFromTop(25f), "click in this area if you wish to continue.");
					Widgets.DrawHighlightIfMouseover(inrect);
					if (Widgets.ButtonInvisible(inrect, true))
					{
						this.rad = 0;
						this.locked = false;
						return;
					}
				}
				else
				{
					if (ModLister.RoyaltyInstalled)
					{
						ASTable astable = new ASTable(asrectMonitor.NextFromTop(30f), 2, 1, 4f, 0f, true);
						asrectMonitor.NextFromTop(30f);
						Color color = GUI.color;
						GUI.color = Color.grey;
						Widgets.DrawBox(astable.GetRectangle(0, 0, -1, -1, false), 1, null);
						Widgets.DrawBox(astable.GetRectangle(1, 0, -1, -1, false), 1, null);
						GUI.color = color;
						Rect rectangle = astable.GetRectangle(0, 0, -1, -1, false);
						Rect rectangle2 = astable.GetRectangle(1, 0, -1, -1, false);
						rectangle.x += 10f;
						rectangle.width -= 20f;
						rectangle2.x += 10f;
						rectangle2.width -= 20f;
						if (Widgets.RadioButtonLabeled(rectangle, "SkillNeurotrainerLabel".Translate(string.Empty).Replace(" ()", ""), this.rad == 0, false))
						{
							this.rad = 0;
						}
						if (Widgets.RadioButtonLabeled(rectangle2, "PsycastNeurotrainerLabel".Translate(string.Empty).Replace(" ()", ""), this.rad == 1, false))
						{
							this.rad = 1;
						}
					}
					if (this.rad == 0)
					{
						this.skills.DrawListBox(asrectMonitor.Remaining());
						return;
					}
					if (this.rad == 1)
					{
						this.psycastbox.DrawListBox(asrectMonitor.Remaining());
					}
				}
			}
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x00018AAC File Offset: 0x00016CAC
		public void RenderStarted(List<string> Data, Map map)
		{
			if (!ModLister.RoyaltyInstalled && Data.Count > 0 && Data.First<string>().StartsWith("psy."))
			{
				this.locked = true;
			}
			else
			{
				this.locked = false;
				if (!ModLister.RoyaltyInstalled || Data.Count == 0 || Data.First<string>().StartsWith("skl."))
				{
					this.rad = 0;
				}
				else
				{
					this.rad = 1;
				}
			}
			this.skills = new ASMultiBox<SkillDef>(24f, 4f, true, true);
			this.psycastbox = new ASMultiBox<AbilityDef>(24f, 4f, true, true);
			foreach (SkillDef skillDef in DefDatabase<SkillDef>.AllDefs)
			{
				SelectedItem<SkillDef> selectedItem = new SelectedItem<SkillDef>();
				selectedItem.Item = skillDef;
				foreach (string a in Data)
				{
					string b = "skl." + skillDef.defName;
					if (a == b)
					{
						selectedItem.Selected = true;
						break;
					}
				}
				this.skills.Add(selectedItem);
			}
			if (ModLister.RoyaltyInstalled)
			{
				foreach (AbilityDef abilityDef in from el in DefDatabase<AbilityDef>.AllDefs
				where typeof(Psycast).IsAssignableFrom(el.abilityClass)
				orderby el.displayOrder
				select el)
				{
					SelectedItem<AbilityDef> selectedItem2 = new SelectedItem<AbilityDef>();
					selectedItem2.Item = abilityDef;
					foreach (string a2 in Data)
					{
						string b2 = "psy." + abilityDef.defName;
						if (a2 == b2)
						{
							selectedItem2.Selected = true;
							break;
						}
					}
					selectedItem2.surpresslabelupdate = true;
					selectedItem2.Icon = abilityDef.uiIcon;
					selectedItem2.tooltip = abilityDef.description;
					this.psycastbox.Add(selectedItem2);
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x00018D2C File Offset: 0x00016F2C
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			if (this.rad == 0)
			{
				using (List<SkillDef>.Enumerator enumerator = this.skills.GetSelectedItems.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SkillDef skillDef = enumerator.Current;
						Data.Add("skl." + skillDef.defName);
					}
					goto IL_B2;
				}
			}
			if (this.rad == 1)
			{
				foreach (AbilityDef abilityDef in this.psycastbox.GetSelectedItems)
				{
					Data.Add("psy." + abilityDef.defName);
				}
			}
			IL_B2:
			this.skills.Clear();
			this.psycastbox.Clear();
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x00018E20 File Offset: 0x00017020
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems"
		};

		// Token: 0x0400015F RID: 351
		private bool locked;

		// Token: 0x04000160 RID: 352
		private int rad;

		// Token: 0x04000161 RID: 353
		private bool AllowRender;

		// Token: 0x04000162 RID: 354
		private ASMultiBox<SkillDef> skills;

		// Token: 0x04000163 RID: 355
		private ASMultiBox<AbilityDef> psycastbox;
	}
}
