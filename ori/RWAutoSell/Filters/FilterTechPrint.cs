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
	// Token: 0x02000051 RID: 81
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterTechPrint : IBaseFilter, IFilterReq
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060002E9 RID: 745 RVA: 0x0001B84E File Offset: 0x00019A4E
		public string ModRequirement { get; } = "ludeon.rimworld.royalty";

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060002EA RID: 746 RVA: 0x0001B856 File Offset: 0x00019A56
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems"
		};

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x060002EB RID: 747 RVA: 0x0001B85E File Offset: 0x00019A5E
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FTech;
			}
		}

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x060002EC RID: 748 RVA: 0x0001B865 File Offset: 0x00019A65
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterTechPrint".Translate();
			}
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0001B878 File Offset: 0x00019A78
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.AnyThing != null)
			{
				ThingDef thingDef = t.ThingDef;
				bool flag;
				if (thingDef == null)
				{
					flag = false;
				}
				else
				{
					List<string> tradeTags = thingDef.tradeTags;
					flag = ((tradeTags != null) ? new bool?(tradeTags.Contains("Techprint")) : null).GetValueOrDefault();
				}
				CompTechprint compTechprint;
				if (flag && t.AnyThing.TryGetComp(out compTechprint))
				{
					ResearchProjectDef def = (compTechprint != null) ? compTechprint.Props.project : null;
					if (def != null && Data.Any((string x) => x == "rs." + def.defName || x == "rs.any"))
					{
						if (!Data.Any((string x) => x.StartsWith("ck.triglock.")))
						{
							return !def.TechprintRequirementMet;
						}
						char c = Data.First((string x) => x.StartsWith("ck.triglock.")).Last<char>();
						if (c == '0')
						{
							return true;
						}
						if (c == '1')
						{
							return def.TechprintRequirementMet;
						}
						return c == '2' && !def.TechprintRequirementMet;
					}
				}
			}
			return false;
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0001B9B7 File Offset: 0x00019BB7
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0001B9BE File Offset: 0x00019BBE
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0001B9C4 File Offset: 0x00019BC4
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			bool flag = this.showcom;
			int num = this.triglock;
			ASWidgets.ASCheckbox(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterTechShowUnlk".Translate(), ref this.showcom);
			if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterTechTrgLkUnlk".Translate(), this.triglock == 0, false))
			{
				this.triglock = 0;
			}
			if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterTechTrgUnlk".Translate(), this.triglock == 1, false))
			{
				this.triglock = 1;
			}
			if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterTechTrgLk".Translate(), this.triglock == 2, false))
			{
				this.triglock = 2;
			}
			if (flag != this.showcom || num != this.triglock)
			{
				this.refresh();
			}
			this.box.DrawListBox(asrectMonitor.Remaining().ContractedBy(2f));
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0001BAD4 File Offset: 0x00019CD4
		private Color Getcolor(ResearchProjectDef def)
		{
			Color result = Color.white;
			if (def == null)
			{
				return result;
			}
			switch (this.triglock)
			{
			case 0:
				result = Color.white;
				break;
			case 1:
				if (def.TechprintRequirementMet)
				{
					result = Color.white;
				}
				else
				{
					result = Color.grey;
				}
				break;
			case 2:
				if (def.TechprintRequirementMet)
				{
					result = Color.grey;
				}
				else
				{
					result = Color.white;
				}
				break;
			}
			return result;
		}

		// Token: 0x060002F2 RID: 754 RVA: 0x0001BB40 File Offset: 0x00019D40
		private void refresh()
		{
			using (IEnumerator<SelectedItem<ResearchProjectDef>> enumerator = this.box.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SelectedItem<ResearchProjectDef> d = enumerator.Current;
					this.resitems.First((SelectedItem<ResearchProjectDef> x) => x.Item == d.Item).Selected = d.Selected;
				}
			}
			this.box.Clear();
			this.shownitems.Clear();
			foreach (SelectedItem<ResearchProjectDef> selectedItem in this.resitems)
			{
				if (selectedItem.Item == null || this.showcom || !selectedItem.Item.TechprintRequirementMet)
				{
					this.shownitems.Add(new SelectedItem<ResearchProjectDef>
					{
						Item = selectedItem.Item,
						color = selectedItem.color,
						label = selectedItem.label.Colorize(this.Getcolor(selectedItem.Item)),
						Selected = selectedItem.Selected,
						Icon = selectedItem.Icon,
						surpresslabelupdate = selectedItem.surpresslabelupdate,
						tooltip = selectedItem.tooltip
					});
				}
			}
			this.box.AddRange(this.shownitems.ToArray());
		}

		// Token: 0x060002F3 RID: 755 RVA: 0x0001BCB8 File Offset: 0x00019EB8
		public void RenderStarted(List<string> Data, Map map)
		{
			this.showcom = Data.Contains("ck.showcom");
			if (Data.Any((string x) => x.StartsWith("ck.triglock.")))
			{
				if (Data.Contains("ck.triglock.0"))
				{
					this.triglock = 0;
				}
				else if (Data.Contains("ck.triglock.1"))
				{
					this.triglock = 1;
				}
				else if (Data.Contains("ck.triglock.2"))
				{
					this.triglock = 2;
				}
			}
			else
			{
				this.triglock = 2;
			}
			this.box = new ASMultiBox<ResearchProjectDef>(25f, 4f, true, true);
			this.resitems = new List<SelectedItem<ResearchProjectDef>>();
			this.shownitems = new List<SelectedItem<ResearchProjectDef>>();
			SelectedItem<ResearchProjectDef> item = new SelectedItem<ResearchProjectDef>
			{
				Item = null,
				label = "RWAutoSell.FilterTechAny".Translate(),
				Selected = Data.Contains("rs.any"),
				surpresslabelupdate = true,
				tooltip = "RWAutoSell.FilterTechAnyTP".Translate()
			};
			this.resitems.Add(item);
			foreach (ResearchProjectDef researchProjectDef in from ResearchProjectDef d in DefDatabase<ResearchProjectDef>.AllDefs
			orderby d.tab != ResearchTabDefOf.Main, d.tab.defName != "Anomaly", d.tab.defName, d.defName
			where !d.TechprintRequirementMet || d.techprintCount > 0
			select d)
			{
				Texture2D icon;
				if (researchProjectDef.modContentPack.IsCoreMod || researchProjectDef.modContentPack.IsOfficialMod)
				{
					icon = researchProjectDef.modContentPack.ModMetaData.Expansion.Icon;
				}
				else
				{
					icon = researchProjectDef.modContentPack.ModMetaData.Icon;
				}
				SelectedItem<ResearchProjectDef> item2 = new SelectedItem<ResearchProjectDef>
				{
					Item = researchProjectDef,
					Icon = icon,
					label = researchProjectDef.LabelCap,
					Selected = Data.Contains("rs." + researchProjectDef.defName),
					color = (researchProjectDef.TechprintRequirementMet ? Color.green : Color.white),
					surpresslabelupdate = true,
					tooltip = string.Concat(new string[]
					{
						researchProjectDef.modContentPack.ModMetaData.Name.Colorize(Color.green),
						"\n\n",
						researchProjectDef.TechprintsApplied.ToString(),
						"/",
						researchProjectDef.techprintCount.ToString(),
						"\n\n",
						researchProjectDef.description
					})
				};
				this.resitems.Add(item2);
			}
			this.refresh();
		}

		// Token: 0x060002F4 RID: 756 RVA: 0x0001C008 File Offset: 0x0001A208
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			foreach (string item in (from v in this.box
			where v.Selected
			select v).Select(delegate(SelectedItem<ResearchProjectDef> v)
			{
				string str = "rs.";
				ResearchProjectDef item2 = v.Item;
				return str + (((item2 != null) ? item2.defName : null) ?? "any");
			}))
			{
				Data.Add(item);
			}
			if (this.showcom)
			{
				Data.Add("ck.showcom");
			}
			Data.Add("ck.triglock." + this.triglock.ToString());
			this.shownitems.Clear();
			this.resitems.Clear();
			this.box = null;
		}

		// Token: 0x04000188 RID: 392
		private bool showcom;

		// Token: 0x04000189 RID: 393
		private int triglock = 2;

		// Token: 0x0400018A RID: 394
		private ASMultiBox<ResearchProjectDef> box;

		// Token: 0x0400018B RID: 395
		private List<SelectedItem<ResearchProjectDef>> resitems;

		// Token: 0x0400018C RID: 396
		private List<SelectedItem<ResearchProjectDef>> shownitems;
	}
}
