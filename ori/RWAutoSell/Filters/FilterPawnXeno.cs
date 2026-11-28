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
	// Token: 0x02000048 RID: 72
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterPawnXeno : IBaseFilter, IFilterReq
	{
		// Token: 0x17000091 RID: 145
		// (get) Token: 0x06000279 RID: 633 RVA: 0x000171A2 File Offset: 0x000153A2
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x0600027A RID: 634 RVA: 0x000171AA File Offset: 0x000153AA
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FPawnXeno;
			}
		}

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x0600027B RID: 635 RVA: 0x000171B1 File Offset: 0x000153B1
		public string ModRequirement { get; } = "ludeon.rimworld.biotech";

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x0600027C RID: 636 RVA: 0x000171B9 File Offset: 0x000153B9
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterPawnXeno".Translate();
			}
		}

		// Token: 0x0600027D RID: 637 RVA: 0x000171CC File Offset: 0x000153CC
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (!ModLister.BiotechInstalled)
			{
				return false;
			}
			Thing anyThing = t.AnyThing;
			Pawn p = anyThing as Pawn;
			if (p != null && p.genes != null)
			{
				Pawn_GeneTracker genes = p.genes;
				if (p.genes.UniqueXenotype)
				{
					if (genes.CustomXenotype != null && genes.CustomXenotype.name == genes.xenotypeName)
					{
						if (Data.Any((string x) => x == "cus." + genes.xenotypeName))
						{
							return true;
						}
					}
					else if (Data.Any((string x) => x == "unq." + p.genes.xenotypeName))
					{
						return true;
					}
				}
				else if (p.genes.Xenotype != null && Data.Any((string x) => x == "xen." + p.genes.Xenotype.defName))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600027E RID: 638 RVA: 0x000172C4 File Offset: 0x000154C4
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (!ModLister.BiotechInstalled)
			{
				return list;
			}
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.genes != null)
			{
				if (pawn.genes.UniqueXenotype)
				{
					foreach (CustomXenotype customXenotype in Current.Game.customXenotypeDatabase.customXenotypes)
					{
						if (customXenotype.name == pawn.genes.XenotypeLabel)
						{
							list.Add("cus." + customXenotype.name);
							return list;
						}
					}
					list.Add("unq." + pawn.genes.XenotypeLabel);
					return list;
				}
				if (pawn.genes.Xenotype != null)
				{
					list.Add("xen." + pawn.genes.Xenotype.defName);
				}
			}
			return list;
		}

		// Token: 0x0600027F RID: 639 RVA: 0x000173D8 File Offset: 0x000155D8
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000280 RID: 640 RVA: 0x000173DC File Offset: 0x000155DC
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			string a = Widgets.TextField(asrectMonitor.NextFromTop(25f).ContractedBy(2f), this.cachetext);
			this.Box.DrawListBox(asrectMonitor.Remaining().ContractedBy(2f));
			if (a != this.cachetext)
			{
				this.cachetext = a;
				this.Click(null, null);
			}
		}

		// Token: 0x06000281 RID: 641 RVA: 0x0001744C File Offset: 0x0001564C
		private void Click(IGUIList<string> list, GUIListEventArgs e)
		{
			this.nlist.Clear();
			this.Box.Clear();
			List<SelectedItem<string>> list2 = new List<SelectedItem<string>>();
			if (this.cachetext != string.Empty)
			{
				string s = this.cachetext.ToLower().Trim();
				using (IEnumerator<SelectedItem<string>> enumerator = (from el in this.items
				orderby el.label
				select el).GetEnumerator())
				{
					Predicate<GeneDef> <>9__1;
					Predicate<GeneDef> <>9__2;
					Predicate<GeneDef> <>9__4;
					while (enumerator.MoveNext())
					{
						SelectedItem<string> item = enumerator.Current;
						int count = list2.Count;
						if (item.Selected)
						{
							list2.Add(item);
						}
						else if (item.label.ToLower().Contains(s))
						{
							list2.Add(item);
						}
						else
						{
							if (item.Item.StartsWith("cus."))
							{
								using (List<CustomXenotype>.Enumerator enumerator2 = Current.Game.customXenotypeDatabase.customXenotypes.GetEnumerator())
								{
									while (enumerator2.MoveNext())
									{
										CustomXenotype customXenotype = enumerator2.Current;
										List<GeneDef> genes = customXenotype.genes;
										Predicate<GeneDef> predicate;
										if ((predicate = <>9__1) == null)
										{
											predicate = (<>9__1 = ((GeneDef x) => x.label.ToLower().Contains(s)));
										}
										if (genes.Any(predicate))
										{
											list2.Add(item);
											break;
										}
									}
									continue;
								}
							}
							if (item.Item.StartsWith("xen."))
							{
								using (IEnumerator<XenotypeDef> enumerator3 = DefDatabase<XenotypeDef>.AllDefs.GetEnumerator())
								{
									while (enumerator3.MoveNext())
									{
										XenotypeDef xenotypeDef = enumerator3.Current;
										List<GeneDef> genes2 = xenotypeDef.genes;
										Predicate<GeneDef> predicate2;
										if ((predicate2 = <>9__2) == null)
										{
											predicate2 = (<>9__2 = ((GeneDef x) => x.label.ToLower().Contains(s)));
										}
										if (genes2.Any(predicate2))
										{
											list2.Add(item);
											break;
										}
									}
									continue;
								}
							}
							if (item.Item.StartsWith("unq.") && !(item.Item != "unq." + "Unique".Translate()))
							{
								UniqueXeno uniqueXeno = this.UniqueXenos.FirstOrDefault((UniqueXeno x) => item.Item == "unq." + x.name);
								if (uniqueXeno != null)
								{
									List<GeneDef> genes3 = uniqueXeno.genes;
									Predicate<GeneDef> predicate3;
									if ((predicate3 = <>9__4) == null)
									{
										predicate3 = (<>9__4 = ((GeneDef x) => x.label.ToLower().Contains(s)));
									}
									if (genes3.Any(predicate3))
									{
										list2.Add(item);
									}
								}
							}
						}
					}
					goto IL_2B6;
				}
			}
			list2 = this.items;
			IL_2B6:
			this.Box.AddRange(list2.ToArray());
		}

		// Token: 0x06000282 RID: 642 RVA: 0x0001776C File Offset: 0x0001596C
		public void RenderStarted(List<string> Data, Map map)
		{
			this.UniqueXenos = new List<UniqueXeno>();
			this.dirty = false;
			this.items = new List<SelectedItem<string>>();
			this.Box = new ASMultiBox<string>(25f, 4f, true, true);
			List<CustomXenotype> customXenotypes = Current.Game.customXenotypeDatabase.customXenotypes;
			List<SelectedItem<string>> list = new List<SelectedItem<string>>();
			if (customXenotypes != null && customXenotypes.Count > 0)
			{
				foreach (CustomXenotype customXenotype in customXenotypes)
				{
					list.Add(new SelectedItem<string>
					{
						Item = "cus." + customXenotype.name,
						Icon = customXenotype.iconDef.Icon,
						label = customXenotype.name + " (" + "Custom".Translate() + ")",
						Selected = Data.Contains("cus." + customXenotype.name),
						surpresslabelupdate = true,
						tooltip = ""
					});
				}
			}
			if (map != null)
			{
				using (List<Pawn>.Enumerator enumerator2 = map.mapPawns.AllPawns.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						Pawn p = enumerator2.Current;
						if (p.genes != null && p.genes.UniqueXenotype && (this.nlist.Count <= 0 || (!this.nlist.Any((SelectedItem<string> x) => x.Item == "cus." + p.genes.xenotypeName) && !this.nlist.Any((SelectedItem<string> x) => x.Item == "unq." + p.genes.xenotypeName))))
						{
							if (p.genes.xenotypeName == "Unique".Translate())
							{
								list.Add(new SelectedItem<string>
								{
									Item = "unq." + p.genes.XenotypeLabel,
									Icon = p.genes.XenotypeIcon,
									label = p.genes.XenotypeLabelCap,
									Selected = Data.Contains("unq." + p.genes.XenotypeLabel),
									surpresslabelupdate = true,
									tooltip = ""
								});
							}
							else
							{
								this.UniqueXenos.Add(new UniqueXeno(p.genes.xenotypeName, (from el in p.genes.GenesListForReading
								select el.def).ToList<GeneDef>()));
								list.Add(new SelectedItem<string>
								{
									Item = "unq." + p.genes.XenotypeLabel,
									Icon = p.genes.XenotypeIcon,
									label = p.genes.XenotypeLabelCap + " (" + "Unique".Translate() + ")",
									Selected = Data.Contains("unq." + p.genes.XenotypeLabel),
									surpresslabelupdate = true,
									tooltip = ""
								});
							}
						}
					}
				}
			}
			foreach (XenotypeDef xenotypeDef in DefDatabase<XenotypeDef>.AllDefs)
			{
				list.Add(new SelectedItem<string>
				{
					Item = "xen." + xenotypeDef.defName,
					Icon = xenotypeDef.Icon,
					label = xenotypeDef.LabelCap,
					Selected = Data.Contains("xen." + xenotypeDef.defName),
					surpresslabelupdate = true,
					tooltip = xenotypeDef.description
				});
			}
			using (List<string>.Enumerator enumerator4 = Data.GetEnumerator())
			{
				while (enumerator4.MoveNext())
				{
					string s = enumerator4.Current;
					if (!list.Any((SelectedItem<string> x) => x.Item == s))
					{
						list.Add(new SelectedItem<string>
						{
							Item = s,
							label = s + " (" + "Unknown".Translate() + ")",
							Selected = true,
							surpresslabelupdate = true,
							tooltip = "A currently unknown Xenotype,\neither left over from a mod no longer installed or active,\nor from a pawn despawned or not on the current map\n\nThere is no harm leaving this selected"
						});
					}
				}
			}
			this.items.AddRange(from el in list
			orderby el.label.ToLower()
			select el);
			this.cachetext = string.Empty;
			this.Click(null, null);
		}

		// Token: 0x06000283 RID: 643 RVA: 0x00017D34 File Offset: 0x00015F34
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			Data.AddRange(from el in this.Box
			where el.Selected
			select el.Item);
			this.UniqueXenos = null;
			this.Box = null;
		}

		// Token: 0x0400014F RID: 335
		private List<UniqueXeno> UniqueXenos = new List<UniqueXeno>();

		// Token: 0x04000150 RID: 336
		private bool dirty;

		// Token: 0x04000151 RID: 337
		private List<SelectedItem<string>> nlist = new List<SelectedItem<string>>();

		// Token: 0x04000152 RID: 338
		private string cachetext = string.Empty;

		// Token: 0x04000153 RID: 339
		private ASMultiBox<string> Box;

		// Token: 0x04000154 RID: 340
		private List<SelectedItem<string>> items;
	}
}
