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
	// Token: 0x0200003F RID: 63
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterGene : IBaseFilter, IFilterReq
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x0600020A RID: 522 RVA: 0x0001366D File Offset: 0x0001186D
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems"
		};

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x0600020B RID: 523 RVA: 0x00013675 File Offset: 0x00011875
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FGene;
			}
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x0600020C RID: 524 RVA: 0x0001367C File Offset: 0x0001187C
		public string ModRequirement { get; } = "ludeon.rimworld.biotech";

		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600020D RID: 525 RVA: 0x00013684 File Offset: 0x00011884
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterGene".Translate();
			}
		}

		// Token: 0x0600020E RID: 526 RVA: 0x00013698 File Offset: 0x00011898
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (!ModLister.BiotechInstalled)
			{
				return false;
			}
			if (t.AnyThing is Pawn)
			{
				return false;
			}
			Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
			if (innerIfMinified != null)
			{
				GeneSetHolderBase geneSetHolderBase = innerIfMinified as GeneSetHolderBase;
				if (geneSetHolderBase != null)
				{
					foreach (GeneDef geneDef in geneSetHolderBase.GeneSet.GenesListForReading)
					{
						foreach (string b in Data)
						{
							if ("gen." + geneDef.defName == b)
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x00013778 File Offset: 0x00011978
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (!ModLister.BiotechInstalled)
			{
				return list;
			}
			if (t.AnyThing is Pawn)
			{
				return list;
			}
			Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
			if (innerIfMinified != null)
			{
				GeneSetHolderBase geneSetHolderBase = innerIfMinified as GeneSetHolderBase;
				if (geneSetHolderBase != null)
				{
					foreach (GeneDef geneDef in geneSetHolderBase.GeneSet.GenesListForReading)
					{
						list.Add("gen." + geneDef.defName);
					}
				}
			}
			return list;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0001381C File Offset: 0x00011A1C
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000211 RID: 529 RVA: 0x00013820 File Offset: 0x00011A20
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.dirty)
			{
				this.Clean();
			}
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			string a = Widgets.TextField(asrectMonitor.NextFromTop(25f).ContractedBy(2f), this.cachetext);
			this.tagbox.DrawListBox(asrectMonitor.NextFromTop(125f).ContractedBy(2f));
			this.Box.DrawListBox(asrectMonitor.Remaining().ContractedBy(2f));
			if (a != this.cachetext)
			{
				this.cachetext = a;
				this.Click(null, null);
			}
		}

		// Token: 0x06000212 RID: 530 RVA: 0x000138C0 File Offset: 0x00011AC0
		private void Click(IGUIList<string> list, GUIListEventArgs e)
		{
			this.nlist.Clear();
			this.Box.Clear();
			List<SelectedItem<GeneDef>> list2 = new List<SelectedItem<GeneDef>>();
			HashSet<string> hashSet = new HashSet<string>();
			if (this.cachetext != string.Empty)
			{
				string s = this.cachetext.ToLower();
				Func<AbilityDef, bool> <>9__3;
				Func<Aptitude, bool> <>9__4;
				foreach (SelectedItem<GeneDef> selectedItem in this.items)
				{
					int count = list2.Count;
					GeneDef item = selectedItem.Item;
					if (selectedItem.Selected)
					{
						list2.Add(selectedItem);
					}
					else if (item.label.ToLower().Contains(s))
					{
						list2.Add(selectedItem);
					}
					else
					{
						if (item.abilities != null && item.abilities.Count > 0)
						{
							IEnumerable<AbilityDef> abilities = item.abilities;
							Func<AbilityDef, bool> predicate;
							if ((predicate = <>9__3) == null)
							{
								predicate = (<>9__3 = ((AbilityDef el) => el.label.ToLower().Contains(s)));
							}
							if (abilities.Where(predicate).ToList<AbilityDef>().Count > 0)
							{
								list2.Add(selectedItem);
								goto IL_24B;
							}
						}
						if (item.aptitudes != null && item.aptitudes.Count > 0)
						{
							IEnumerable<Aptitude> aptitudes = item.aptitudes;
							Func<Aptitude, bool> predicate2;
							if ((predicate2 = <>9__4) == null)
							{
								predicate2 = (<>9__4 = ((Aptitude el) => el.skill != null && el.skill.label.ToLower().Contains(s)));
							}
							if (aptitudes.Where(predicate2).ToList<Aptitude>().Count > 0)
							{
								list2.Add(selectedItem);
								goto IL_24B;
							}
						}
						if (item.bodyType != null && Enum.GetName(typeof(GeneticBodyType), item.bodyType).Contains(s))
						{
							list2.Add(selectedItem);
						}
						else if (item.displayCategory.label.ToLower().Contains(s))
						{
							list2.Add(selectedItem);
						}
						else
						{
							foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefs)
							{
								if (xenotypeDef.label.ToLower().Contains(s) && xenotypeDef.genes != null && xenotypeDef.genes.Contains(selectedItem.Item))
								{
									list2.Add(selectedItem);
								}
							}
						}
					}
					IL_24B:
					if (count != list2.Count)
					{
						hashSet.Add(selectedItem.Item.displayCategory.defName);
					}
				}
				foreach (SelectedItem<string> selectedItem2 in this.taglist)
				{
					if (selectedItem2.Selected)
					{
						this.nlist.Add(selectedItem2);
					}
					else
					{
						using (HashSet<string>.Enumerator enumerator4 = hashSet.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								if (enumerator4.Current == selectedItem2.Item)
								{
									this.nlist.Add(selectedItem2);
								}
							}
						}
					}
				}
				if (this.tagbox.Count != this.nlist.Count)
				{
					this.dirty = true;
				}
				else
				{
					for (int i = 0; i < this.tagbox.Count; i++)
					{
						if (this.tagbox[i] != this.nlist[i])
						{
							this.dirty = true;
						}
					}
				}
			}
			else
			{
				list2 = this.items;
			}
			if (this.tagbox.GetSelectedItems.Count == 0)
			{
				this.Box.AddRange(list2.OrderBy(delegate(SelectedItem<GeneDef> el)
				{
					if (!el.Selected)
					{
						return 0;
					}
					return int.MinValue;
				}));
				return;
			}
			this.Box.AddRange((from el in list2
			where el.Selected || this.tagbox.Any((SelectedItem<string> x) => x.Selected && x.Item == el.Item.displayCategory.defName)
			select el).OrderBy(delegate(SelectedItem<GeneDef> el)
			{
				if (!el.Selected || this.tagbox.Any((SelectedItem<string> x) => x.Selected && x.Item == el.Item.displayCategory.defName))
				{
					return 0;
				}
				return int.MinValue;
			}));
		}

		// Token: 0x06000213 RID: 531 RVA: 0x00013D20 File Offset: 0x00011F20
		private void Clean()
		{
			this.tagbox.Clear();
			this.tagbox.AddRange(from n in this.nlist
			select n);
			this.dirty = false;
		}

		// Token: 0x06000214 RID: 532 RVA: 0x00013D74 File Offset: 0x00011F74
		public void RenderStarted(List<string> Data, Map map)
		{
			this.dirty = false;
			this.items = new List<SelectedItem<GeneDef>>();
			this.taglist = new List<SelectedItem<string>>();
			this.Box = new ASMultiBox<GeneDef>(25f, 4f, true, true);
			this.items.AddRange(from gene in DefDatabase<GeneDef>.AllDefs
			where gene.canGenerateInGeneSet
			orderby gene.displayOrderInCategory
			orderby gene.displayCategory.label
			select new SelectedItem<GeneDef>
			{
				color = gene.IconColor,
				Item = gene,
				Icon = gene.Icon,
				label = gene.label,
				Selected = Data.Contains("gen." + gene.defName),
				surpresslabelupdate = true,
				tooltip = gene.DescriptionFull
			});
			this.taglist.AddRange(from cat in DefDatabase<GeneCategoryDef>.AllDefs
			orderby cat.LabelCap.ToString()
			select new SelectedItem<string>
			{
				Item = cat.defName,
				label = cat.LabelCap,
				Selected = Data.Contains("tag." + cat.defName),
				surpresslabelupdate = true
			});
			this.cachetext = string.Empty;
			this.tagbox = new ASMultiBox<string>(25f, 4f, true, false);
			this.tagbox.AddRange(this.taglist.ToArray());
			this.Click(null, null);
			this.tagbox.OnClick += this.Click;
		}

		// Token: 0x06000215 RID: 533 RVA: 0x00013EEC File Offset: 0x000120EC
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			Data.AddRange(from el in this.Box
			where el.Selected
			select "gen." + el.Item.defName);
			Data.AddRange(from el in this.tagbox
			where el.Selected
			select "tag." + el.Item);
			if (this.tagbox != null)
			{
				this.tagbox.OnClick -= this.Click;
			}
			this.tagbox = null;
			this.Box = null;
			this.taglist = null;
		}

		// Token: 0x04000121 RID: 289
		private bool dirty;

		// Token: 0x04000122 RID: 290
		private List<SelectedItem<string>> nlist = new List<SelectedItem<string>>();

		// Token: 0x04000123 RID: 291
		private string cachetext = string.Empty;

		// Token: 0x04000124 RID: 292
		private ASMultiBox<GeneDef> Box;

		// Token: 0x04000125 RID: 293
		private ASMultiBox<string> tagbox;

		// Token: 0x04000126 RID: 294
		private List<SelectedItem<GeneDef>> items;

		// Token: 0x04000127 RID: 295
		private List<SelectedItem<string>> taglist;
	}
}
