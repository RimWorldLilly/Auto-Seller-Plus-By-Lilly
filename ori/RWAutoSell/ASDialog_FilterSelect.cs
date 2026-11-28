using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200001C RID: 28
	[NullableContext(1)]
	[Nullable(0)]
	public class ASDialog_FilterSelect : Window
	{
		// Token: 0x060000CB RID: 203 RVA: 0x000090B3 File Offset: 0x000072B3
		public ASDialog_FilterSelect() : base(null)
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
		}

		// Token: 0x060000CC RID: 204 RVA: 0x000090D8 File Offset: 0x000072D8
		public override void PreOpen()
		{
			base.PreOpen();
			HashSet<string> hashSet = new HashSet<string>();
			foreach (IBaseFilter baseFilter in ASLibMod.GetSingleton.GetBaseFilters)
			{
				IFilterReq filterReq = baseFilter as IFilterReq;
				if (filterReq == null || !(filterReq.ModRequirement != string.Empty) || ModsConfig.IsActive(filterReq.ModRequirement))
				{
					for (int i = 0; i < baseFilter.Tags.Length; i++)
					{
						hashSet.Add(baseFilter.Tags[i].Translate());
					}
				}
			}
			this.Filtersbox = new ASListBox<string>(30f, 4f, false, true);
			this.Tagsbox = new ASMultiBox<string>(30f, 4f, true, false);
			this.Tagsbox.AddRange(hashSet);
			foreach (SelectedItem<string> selectedItem in this.Tagsbox)
			{
				selectedItem.label = selectedItem.Item;
				selectedItem.Selected = true;
			}
			this.Tagsbox.OnClick += new ASListBox<string>.ListItemHandler(this.TagSelected);
			this.TagSelected(null, null);
			this.Filtersbox.OnClick += new ASListBox<string>.ListItemHandler(this.FilterSelected);
		}

		// Token: 0x060000CD RID: 205 RVA: 0x0000924C File Offset: 0x0000744C
		public override void PostClose()
		{
			base.PostClose();
			this.Tagsbox.OnClick -= new ASListBox<string>.ListItemHandler(this.TagSelected);
			this.Filtersbox.OnClick -= new ASListBox<string>.ListItemHandler(this.FilterSelected);
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00009284 File Offset: 0x00007484
		private void TagSelected(object Sender, GUIListEventArgs e)
		{
			this.Filtersbox.Clear();
			foreach (IBaseFilter baseFilter in from el in ASLibMod.GetSingleton.GetBaseFilters
			orderby el.Label.ToLowerInvariant()
			orderby ASLibMod.GetSettings().HasFavFilter(el.GetType().FullName) descending
			select el into ex
			orderby ex.Label.ToLowerInvariant().StartsWith(this.cache.ToLowerInvariant()) descending
			select ex)
			{
				if (baseFilter.Tags.Any((string x) => this.Tagsbox.GetSelectedItems.Contains(x.Translate())) && (this.cache == string.Empty || baseFilter.Label.ToLowerInvariant().Contains(this.cache.ToLowerInvariant())))
				{
					IFilterReq filterReq = baseFilter as IFilterReq;
					if (filterReq == null || !(filterReq.ModRequirement != string.Empty) || ModsConfig.IsActive(filterReq.ModRequirement))
					{
						Color color = Color.white * 0.9f;
						if (ASLibMod.GetSettings().HasFavFilter(baseFilter.GetType().FullName))
						{
							color = new Color(1f, 1f, 0.9f);
						}
						this.Filtersbox.Add(new SelectedItem<string>
						{
							Item = baseFilter.GetType().FullName,
							label = baseFilter.Label.Colorize(color),
							Icon = baseFilter.MiniGraphic
						});
					}
				}
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00009438 File Offset: 0x00007638
		private void FilterSelected(object Sender, GUIListEventArgs e)
		{
			if (this.Filtersbox.GetSelectedItems.Count != 0)
			{
				IBaseFilter filter = ASLibMod.GetSingleton.GetFilter(this.Filtersbox.GetSelected);
				IRequiredMerger r = filter as IRequiredMerger;
				if (r != null)
				{
					if (ASLibMod.GetSettings().EnabledUnmergers == null || !ASLibMod.GetSettings().EnabledUnmergers.Any((string x) => x == r.RequiredEnabled()))
					{
						IUnmerge unmerge = ASLibMod.GetSingleton.GetBaseUnMergers.FirstOrDefault((IUnmerge x) => x.GetType().FullName == r.RequiredEnabled());
						string value = r.RequiredEnabled();
						if (unmerge != null)
						{
							value = unmerge.Label;
						}
						Find.WindowStack.Add(new ASDialog_EnableWarn<IRequiredMerger>(new Action<IRequiredMerger>(this.EnableAndSelect), r, "RWAutoSell.MergerRequired".Translate(), "RWAutoSell.MergerRequiredDesc".Translate(value, this.Filtersbox[this.Filtersbox.GetSelectedIndex].label)));
					}
					ASDialog_FilterSelect.FilterString = this.Filtersbox.GetSelected;
					this.Close(true);
					return;
				}
				ASDialog_FilterSelect.FilterString = this.Filtersbox.GetSelected;
				this.Close(true);
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000957F File Offset: 0x0000777F
		public void EnableAndSelect(IRequiredMerger r)
		{
			ASLibMod.GetSettings().EnabledUnmergers.Add(r.RequiredEnabled());
			ASLibMod.GetSettings().Write();
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x000095A0 File Offset: 0x000077A0
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(350f, 500f);
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000095B4 File Offset: 0x000077B4
		public override void DoWindowContents(Rect inRect)
		{
			GUI.color = Color.white;
			ASTable astable = new ASTable(inRect, 1, 3, 2f, 2f, true);
			this.Tagsbox.DrawListBox(astable.GetRectangle(0, 0, -1, -1, false));
			Rect rectangle = astable.GetRectangle(0, 1, -1, 2, false);
			Rect rect;
			Rect outrect;
			astable.SplitRectangle(rectangle, 30f, out rect, out outrect, false);
			string b = this.cache;
			this.cache = Widgets.TextField(rect, this.cache);
			if (this.cache != b)
			{
				this.TagSelected(null, null);
			}
			this.Filtersbox.DrawListBox(outrect);
		}

		// Token: 0x0400008F RID: 143
		public static string FilterString = string.Empty;

		// Token: 0x04000090 RID: 144
		private ASListBox<string> Filtersbox;

		// Token: 0x04000091 RID: 145
		private ASMultiBox<string> Tagsbox;

		// Token: 0x04000092 RID: 146
		private string cache = string.Empty;
	}
}
