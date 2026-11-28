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
	// Token: 0x0200004A RID: 74
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterQuality : IBaseFilter, IWikiHelp, IRuleSorter
	{
		// Token: 0x17000098 RID: 152
		// (get) Token: 0x0600028F RID: 655 RVA: 0x0001836C File Offset: 0x0001656C
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FQuality;
			}
		}

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x06000290 RID: 656 RVA: 0x00018373 File Offset: 0x00016573
		public string Label
		{
			get
			{
				return "Quality".Translate();
			}
		}

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x06000291 RID: 657 RVA: 0x00018384 File Offset: 0x00016584
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Quality-Filter";
			}
		}

		// Token: 0x06000292 RID: 658 RVA: 0x0001838C File Offset: 0x0001658C
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
				QualityCategory qualityCategory;
				if (First.HasAnyThing && First.AnyThing.TryGetQuality(out qualityCategory))
				{
					QualityCategory qualityCategory2;
					if (!Second.HasAnyThing || !Second.AnyThing.TryGetQuality(out qualityCategory2))
					{
						return -1;
					}
					if (qualityCategory == qualityCategory2)
					{
						return 0;
					}
					if (qualityCategory > qualityCategory2)
					{
						return 1;
					}
					return -1;
				}
				else
				{
					QualityCategory qualityCategory3;
					if (Second.HasAnyThing && Second.AnyThing.TryGetQuality(out qualityCategory3))
					{
						return 1;
					}
					return 0;
				}
			}
		}

		// Token: 0x06000293 RID: 659 RVA: 0x00018430 File Offset: 0x00016630
		public bool CheckItem(Transferable t, List<string> Data)
		{
			QualityCategory qualityCategory;
			if (t.AnyThing.TryGetQuality(out qualityCategory))
			{
				byte b = (byte)qualityCategory;
				using (List<string>.Enumerator enumerator = Data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						byte b2;
						if (byte.TryParse(enumerator.Current, out b2) && b == b2)
						{
							return true;
						}
					}
					return false;
				}
			}
			if (Data.Count == 0)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000294 RID: 660 RVA: 0x000184A8 File Offset: 0x000166A8
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			QualityCategory qualityCategory;
			if (t.AnyThing.TryGetQuality(out qualityCategory))
			{
				List<string> list2 = list;
				byte b = (byte)qualityCategory;
				list2.Add(b.ToString());
			}
			return list;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x000184DA File Offset: 0x000166DA
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x000184DD File Offset: 0x000166DD
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.allowrender)
			{
				this.QBox.DrawListBox(inrect);
			}
		}

		// Token: 0x06000297 RID: 663 RVA: 0x000184F4 File Offset: 0x000166F4
		public void RenderStarted(List<string> Data, Map map)
		{
			if (this.QBox != null)
			{
				this.QBox.Clear();
			}
			else
			{
				this.QBox = new ASMultiBox<QualityCategory>(26f, 4f, true, false);
			}
			foreach (QualityCategory qualityCategory in QualityUtility.AllQualityCategories)
			{
				ASListBox<QualityCategory> qbox = this.QBox;
				SelectedItem<QualityCategory> selectedItem = new SelectedItem<QualityCategory>();
				selectedItem.Item = qualityCategory;
				selectedItem.label = qualityCategory.GetLabel();
				byte b = (byte)qualityCategory;
				selectedItem.Selected = Data.Contains(b.ToString());
				qbox.Add(selectedItem);
			}
			this.allowrender = true;
		}

		// Token: 0x06000298 RID: 664 RVA: 0x000185AC File Offset: 0x000167AC
		public void RenderStopped(List<string> Data, Map map)
		{
			this.allowrender = false;
			Data.Clear();
			foreach (SelectedItem<QualityCategory> selectedItem in this.QBox)
			{
				if (selectedItem.Selected)
				{
					Data.Add(((byte)selectedItem.Item).ToString());
				}
			}
			this.QBox.Clear();
			this.QBox = null;
		}

		// Token: 0x1700009B RID: 155
		// (get) Token: 0x06000299 RID: 665 RVA: 0x00018630 File Offset: 0x00016830
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems"
		};

		// Token: 0x0400015C RID: 348
		private bool allowrender;

		// Token: 0x0400015D RID: 349
		private ASMultiBox<QualityCategory> QBox;
	}
}
