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
	// Token: 0x02000043 RID: 67
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterLearning : IBaseFilter, IFilterReq
	{
		// Token: 0x17000081 RID: 129
		// (get) Token: 0x0600023B RID: 571 RVA: 0x00014B64 File Offset: 0x00012D64
		public string ModRequirement { get; } = "ludeon.rimworld.anomaly";

		// Token: 0x17000082 RID: 130
		// (get) Token: 0x0600023C RID: 572 RVA: 0x00014B6C File Offset: 0x00012D6C
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems"
		};

		// Token: 0x17000083 RID: 131
		// (get) Token: 0x0600023D RID: 573 RVA: 0x00014B74 File Offset: 0x00012D74
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FBook;
			}
		}

		// Token: 0x17000084 RID: 132
		// (get) Token: 0x0600023E RID: 574 RVA: 0x00014B7B File Offset: 0x00012D7B
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterLearning".Translate();
			}
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00014B8C File Offset: 0x00012D8C
		public bool CheckItem(Transferable t, List<string> Data)
		{
			string text;
			Data.FirstSubString("bkt.", out text, string.Empty);
			Book book = t.AnyThing as Book;
			return book != null && (text == string.Empty || book.def.defName == text) && DoerBoxWorker.CheckWorkers(book, Data);
		}

		// Token: 0x06000240 RID: 576 RVA: 0x00014BE4 File Offset: 0x00012DE4
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x06000241 RID: 577 RVA: 0x00014BEB File Offset: 0x00012DEB
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00014BF0 File Offset: 0x00012DF0
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			int num = (int)inrect.height - 150;
			int num2 = 150;
			if (this.CurDoerWorkers != null && this.CurDoerWorkers.Count<SelectedItem<DoerBoxWorker>>() != 0)
			{
				int num3 = this.CurDoerWorkers.Sum(delegate(SelectedItem<DoerBoxWorker> x)
				{
					if (x.Item.Checked)
					{
						return x.Item.Expandedsize;
					}
					return 0;
				});
				int num4 = this.CurDoerWorkers.Count<SelectedItem<DoerBoxWorker>>() * 30 + 30;
				num4 += num3;
				int num5 = this.CurDoerWorkers.Count((SelectedItem<DoerBoxWorker> x) => x.Item.HasBox());
				if (num5 > 0)
				{
					if (num - num4 > 150 * num5)
					{
						num2 = (num - num4) / num5;
					}
					else
					{
						num = num4 + num5 * 150;
					}
				}
			}
			this.Boxtype.DrawListBox(asrectMonitor.NextFromTop(150f));
			if (this.CurDoerWorkers != null && this.CurDoerWorkers.Count<SelectedItem<DoerBoxWorker>>() != 0)
			{
				Rect rect = new Rect(0f, 0f, inrect.width - 20f, (float)num);
				Widgets.BeginScrollView(asrectMonitor.Remaining(), ref this.scroll, rect, true);
				ASRectMonitor asrectMonitor2 = new ASRectMonitor(rect);
				foreach (SelectedItem<DoerBoxWorker> selectedItem in this.CurDoerWorkers)
				{
					int maxheight = (int)asrectMonitor2.Remaining().height;
					if (selectedItem.Item.HasBox())
					{
						maxheight = selectedItem.Item.Expandedsize + num2 + 30;
					}
					int num6 = selectedItem.Item.Draw(asrectMonitor2.Remaining(), maxheight);
					asrectMonitor2.NextFromTop((float)num6);
				}
				Widgets.EndScrollView();
			}
		}

		// Token: 0x06000243 RID: 579 RVA: 0x00014DE0 File Offset: 0x00012FE0
		private void Click(IGUIList<FilterLearning.Bookkind> sender, GUIListEventArgs e)
		{
			if (e.index != -1)
			{
				FilterLearning.Bookkind item = sender[e.index].Item;
				if (item.ThingDef == null)
				{
					this.CurDoerWorkers = this.Doers;
					return;
				}
				this.CurDoerWorkers = item.Items;
			}
		}

		// Token: 0x06000244 RID: 580 RVA: 0x00014E2C File Offset: 0x0001302C
		private List<SelectedItem<DoerBoxWorker>> CurrentDoers(ThingDef def, List<string> data)
		{
			List<SelectedItem<DoerBoxWorker>> list = new List<SelectedItem<DoerBoxWorker>>();
			if (def != null && def.comps != null)
			{
				CompProperties_Book compProperties = def.GetCompProperties<CompProperties_Book>();
				if (compProperties != null)
				{
					List<ReadingOutcomeProperties> doers = compProperties.doers;
					int? num = (doers != null) ? new int?(doers.Count<ReadingOutcomeProperties>()) : null;
					int num2 = 0;
					if (num.GetValueOrDefault() > num2 & num != null)
					{
						List<SelectedItem<DoerBoxWorker>> list2 = new List<SelectedItem<DoerBoxWorker>>();
						foreach (ReadingOutcomeProperties readingOutcomeProperties in compProperties.doers)
						{
							SelectedItem<DoerBoxWorker> selectedItem = DoerBoxWorker.Create(readingOutcomeProperties.DoerClass, data, this.Doers);
							if (selectedItem != null && selectedItem.Item != null)
							{
								if (selectedItem.Item.GetType() == typeof(DoerBoxWorker_Unknown))
								{
									list2.Add(selectedItem);
								}
								else
								{
									list.Add(selectedItem);
								}
							}
						}
						foreach (SelectedItem<DoerBoxWorker> item in list2)
						{
							list.Add(item);
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00014F70 File Offset: 0x00013170
		public void RenderStarted(List<string> Data, Map map)
		{
			this.Doers = new List<SelectedItem<DoerBoxWorker>>();
			this.CurDoerWorkers = new List<SelectedItem<DoerBoxWorker>>();
			List<FilterLearning.Bookkind> list = new List<FilterLearning.Bookkind>();
			FilterLearning.Bookkind item = new FilterLearning.Bookkind
			{
				ThingDef = null,
				Label = "RWAutoSell.FilterLearningAnyBook".Translate()
			};
			list.Add(item);
			foreach (ThingDef thingDef in from el in DefDatabase<ThingDef>.AllDefsListForReading
			where el.HasComp<CompBook>()
			select el)
			{
				FilterLearning.Bookkind bookkind = new FilterLearning.Bookkind
				{
					ThingDef = thingDef
				};
				new List<SelectedItem<DoerBoxWorker>>();
				using (List<SelectedItem<DoerBoxWorker>>.Enumerator enumerator2 = this.CurrentDoers(thingDef, Data).GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SelectedItem<DoerBoxWorker> t = enumerator2.Current;
						SelectedItem<DoerBoxWorker> selectedItem = this.Doers.FirstOrDefault((SelectedItem<DoerBoxWorker> x) => x.Item == t.Item);
						if (selectedItem == null)
						{
							this.Doers.Add(t);
							selectedItem = t;
							selectedItem.Item.InitSettings(Data);
						}
						bookkind.Items.Add(selectedItem);
					}
				}
				list.Add(bookkind);
			}
			List<int> list2 = new List<int>();
			List<SelectedItem<DoerBoxWorker>> list3 = new List<SelectedItem<DoerBoxWorker>>();
			for (int i = 0; i < this.Doers.Count; i++)
			{
				if (this.Doers[i].Item is DoerBoxWorker_Unknown)
				{
					list3.Add(this.Doers[i]);
					list2.Add(i);
				}
			}
			if (list2.Count > 0)
			{
				for (int j = list2.Count - 1; j >= 0; j--)
				{
					this.Doers.RemoveAt(list2[j]);
				}
				foreach (SelectedItem<DoerBoxWorker> item2 in list3)
				{
					this.Doers.Add(item2);
				}
			}
			this.Boxtype = new ASListBox<FilterLearning.Bookkind>(25f, 4f, false, true);
			string a;
			Data.FirstSubString("bkt.", out a, string.Empty);
			int num = 0;
			int num2 = 0;
			foreach (FilterLearning.Bookkind bookkind2 in list)
			{
				SelectedItem<FilterLearning.Bookkind> selectedItem2 = new SelectedItem<FilterLearning.Bookkind>
				{
					Item = bookkind2
				};
				if (bookkind2.ThingDef != null)
				{
					selectedItem2.Icon = bookkind2.ThingDef.uiIcon;
					selectedItem2.tooltip = bookkind2.ThingDef.description;
					if (a != string.Empty && a == bookkind2.ThingDef.defName)
					{
						num = num2;
					}
				}
				else if (a == string.Empty)
				{
					num = num2;
				}
				this.Boxtype.Add(selectedItem2);
				num2++;
			}
			this.Boxtype.ChangeSelected(num);
			this.Click(this.Boxtype, new GUIListEventArgs(num, this.Boxtype[num].label));
			this.Boxtype.OnClick += this.Click;
		}

		// Token: 0x06000246 RID: 582 RVA: 0x000152FC File Offset: 0x000134FC
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			FilterLearning.Bookkind getSelected = this.Boxtype.GetSelected;
			if (getSelected != null && getSelected.ThingDef != null)
			{
				Data.Add("bkt." + getSelected.ThingDef.defName);
			}
			if (this.Boxtype != null)
			{
				this.Boxtype.OnClick -= this.Click;
			}
			foreach (SelectedItem<DoerBoxWorker> selectedItem in this.CurDoerWorkers)
			{
				if (selectedItem.Item.Checked)
				{
					foreach (string item in selectedItem.Item.RenderStop())
					{
						Data.Add(item);
					}
				}
			}
		}

		// Token: 0x04000132 RID: 306
		private List<SelectedItem<DoerBoxWorker>> Doers;

		// Token: 0x04000133 RID: 307
		private List<SelectedItem<DoerBoxWorker>> CurDoerWorkers;

		// Token: 0x04000136 RID: 310
		private ASListBox<FilterLearning.Bookkind> Boxtype;

		// Token: 0x04000137 RID: 311
		private Vector2 scroll = Vector2.zero;

		// Token: 0x02000098 RID: 152
		[Nullable(0)]
		private class Bookkind
		{
			// Token: 0x170000D5 RID: 213
			// (get) Token: 0x0600041A RID: 1050 RVA: 0x0001F6A5 File Offset: 0x0001D8A5
			// (set) Token: 0x0600041B RID: 1051 RVA: 0x0001F6AD File Offset: 0x0001D8AD
			public ThingDef ThingDef { get; set; }

			// Token: 0x170000D6 RID: 214
			// (get) Token: 0x0600041C RID: 1052 RVA: 0x0001F6B6 File Offset: 0x0001D8B6
			// (set) Token: 0x0600041D RID: 1053 RVA: 0x0001F6D7 File Offset: 0x0001D8D7
			public string Label
			{
				get
				{
					if (this.ThingDef != null)
					{
						return this.ThingDef.LabelCap;
					}
					return this._label;
				}
				set
				{
					this._label = value;
				}
			}

			// Token: 0x170000D7 RID: 215
			// (get) Token: 0x0600041E RID: 1054 RVA: 0x0001F6E0 File Offset: 0x0001D8E0
			public string Description
			{
				get
				{
					if (this.ThingDef != null)
					{
						return this.ThingDef.description;
					}
					return "";
				}
			}

			// Token: 0x0600041F RID: 1055 RVA: 0x0001F6FB File Offset: 0x0001D8FB
			public override string ToString()
			{
				return this.Label;
			}

			// Token: 0x04000243 RID: 579
			private string _label;

			// Token: 0x04000244 RID: 580
			public List<SelectedItem<DoerBoxWorker>> Items = new List<SelectedItem<DoerBoxWorker>>();
		}
	}
}
