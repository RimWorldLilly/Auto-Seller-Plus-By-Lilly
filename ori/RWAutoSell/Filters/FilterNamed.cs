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
	// Token: 0x02000045 RID: 69
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterNamed : IBaseFilter, IWikiHelp
	{
		// Token: 0x17000089 RID: 137
		// (get) Token: 0x06000258 RID: 600 RVA: 0x00015FA6 File Offset: 0x000141A6
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FNamed;
			}
		}

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x06000259 RID: 601 RVA: 0x00015FAD File Offset: 0x000141AD
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterNamed".Translate();
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x0600025A RID: 602 RVA: 0x00015FBE File Offset: 0x000141BE
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Named-Filter";
			}
		}

		// Token: 0x0600025B RID: 603 RVA: 0x00015FC8 File Offset: 0x000141C8
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				using (List<string>.Enumerator enumerator = Data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.StartsWith("nnm." + pawn.ThingID))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x0600025C RID: 604 RVA: 0x0001603C File Offset: 0x0001423C
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null)
			{
				list.Add("nnm." + pawn.ThingID + "\t" + pawn.Name.ToStringShort);
			}
			return list;
		}

		// Token: 0x0600025D RID: 605 RVA: 0x00016088 File Offset: 0x00014288
		public List<string> DataOptions(List<string> Data, Map map)
		{
			List<string> list = new List<string>();
			foreach (Pawn pawn in map.mapPawns.AllPawns)
			{
				if ((pawn.IsColonist && this.colonists) || (pawn.IsPrisonerOfColony && this.prisoners) || (pawn.AnimalOrWildMan() && pawn.Faction == Faction.OfPlayerSilentFail && this.animals))
				{
					list.Add("nnm." + pawn.ThingID + "\t" + pawn.Name.ToStringShort);
				}
			}
			list = list.Union(Data).ToList<string>();
			return list;
		}

		// Token: 0x0600025E RID: 606 RVA: 0x00016150 File Offset: 0x00014350
		private int GetChangeInt()
		{
			return ((this.colonists > false) ? 1 : 0) + (this.prisoners ? 2 : 0) + (this.animals ? 4 : 0);
		}

		// Token: 0x0600025F RID: 607 RVA: 0x00016178 File Offset: 0x00014378
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				int changeInt = this.GetChangeInt();
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 4f, 24f), "RWAutoSell.FilterNamedCol".Translate(), ref this.colonists, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 4f + 24f, inrect.width - 4f, 24f), "RWAutoSell.FilterNamedPrs".Translate(), ref this.prisoners, false, null, null, false, false);
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 4f + 24f + 24f, inrect.width - 4f, 24f), "RWAutoSell.FilterNamedAnm".Translate(), ref this.animals, false, null, null, false, false);
				if (changeInt != this.GetChangeInt())
				{
					this.UpdateBox();
				}
				this.SelectedBox.DrawListBox(new Rect(inrect.x + 4f, inrect.y + 4f + 72f, inrect.width - 4f, inrect.height - 4f - 72f));
			}
		}

		// Token: 0x06000260 RID: 608 RVA: 0x000162F8 File Offset: 0x000144F8
		private void UpdateBox()
		{
			if (this.TempMap == null)
			{
				return;
			}
			List<string> getSelectedItems = this.SelectedBox.GetSelectedItems;
			this.SelectedBox.Clear();
			foreach (string text in this.DataOptions(getSelectedItems, this.TempMap))
			{
				if (text.Contains('\t'))
				{
					ASListBox<string> selectedBox = this.SelectedBox;
					SelectedItem<string> selectedItem = new SelectedItem<string>();
					selectedItem.Item = text;
					selectedItem.label = text.Substring(text.FirstIndexOf((char x) => x == '\t') + 1);
					selectedItem.Selected = getSelectedItems.Contains(text);
					selectedBox.Add(selectedItem);
				}
			}
		}

		// Token: 0x06000261 RID: 609 RVA: 0x000163CC File Offset: 0x000145CC
		private void UpdateSetting(ref bool setting, string value)
		{
			if (value.Substring(4).ToLowerInvariant() == "true")
			{
				setting = true;
				return;
			}
			setting = false;
		}

		// Token: 0x06000262 RID: 610 RVA: 0x000163F0 File Offset: 0x000145F0
		public void RenderStarted(List<string> Data, Map map)
		{
			this.TempMap = map;
			this.SelectedBox = new ASMultiBox<string>(20f, 5f, true, false);
			foreach (string text in from el in Data
			where !el.Contains('\t')
			select el)
			{
				if (text.Substring(0, 4) == "col.")
				{
					this.UpdateSetting(ref this.colonists, text);
				}
				else if (text.Substring(0, 4) == "prs.")
				{
					this.UpdateSetting(ref this.prisoners, text);
				}
				else if (text.Substring(0, 4) == "anm.")
				{
					this.UpdateSetting(ref this.animals, text);
				}
			}
			foreach (string text2 in this.DataOptions(Data, map))
			{
				if (text2.Contains('\t') && text2.Substring(0, 4) == "nnm.")
				{
					ASListBox<string> selectedBox = this.SelectedBox;
					SelectedItem<string> selectedItem = new SelectedItem<string>();
					selectedItem.Item = text2;
					selectedItem.label = text2.Substring(text2.FirstIndexOf((char x) => x == '\t') + 1);
					selectedItem.Selected = Data.Contains(text2);
					selectedBox.Add(selectedItem);
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x06000263 RID: 611 RVA: 0x00016598 File Offset: 0x00014798
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			Data.Add("col." + this.colonists.ToString());
			Data.Add("prs." + this.prisoners.ToString());
			Data.Add("anm." + this.animals.ToString());
			for (int i = 0; i < this.SelectedBox.Count; i++)
			{
				if (this.SelectedBox[i].Selected)
				{
					Data.Add(this.SelectedBox[i].Item);
				}
			}
			this.TempMap = null;
			this.SelectedBox = null;
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x06000264 RID: 612 RVA: 0x00016651 File Offset: 0x00014851
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople"
		};

		// Token: 0x0400013F RID: 319
		private ASMultiBox<string> SelectedBox;

		// Token: 0x04000140 RID: 320
		private bool AllowRender;

		// Token: 0x04000141 RID: 321
		private bool colonists = true;

		// Token: 0x04000142 RID: 322
		private bool prisoners = true;

		// Token: 0x04000143 RID: 323
		private bool animals = true;

		// Token: 0x04000144 RID: 324
		private Map TempMap;
	}
}
