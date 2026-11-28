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
	// Token: 0x0200004F RID: 79
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterStatistics : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000AC RID: 172
		// (get) Token: 0x060002CD RID: 717 RVA: 0x0001A026 File Offset: 0x00018226
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FStat;
			}
		}

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x060002CE RID: 718 RVA: 0x0001A02D File Offset: 0x0001822D
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Statistics-Filter";
			}
		}

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x060002CF RID: 719 RVA: 0x0001A034 File Offset: 0x00018234
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterStatistics".Translate();
			}
		}

		// Token: 0x060002D0 RID: 720 RVA: 0x0001A048 File Offset: 0x00018248
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.AnyThing != null)
			{
				Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
				string text = Data.FirstOrFallback((string x) => x.Substring(0, 4) == "std.", string.Empty);
				if (text == string.Empty)
				{
					return false;
				}
				StatDef namedSilentFail = DefDatabase<StatDef>.GetNamedSilentFail(text.Substring(4));
				float b = float.Parse(Data.FirstOrFallback((string x) => x.Substring(0, 4) == "max.", "max." + namedSilentFail.maxValue.ToString()).Substring(4));
				float a = float.Parse(Data.FirstOrFallback((string x) => x.Substring(0, 4) == "min.", "min." + namedSilentFail.minValue.ToString()).Substring(4));
				if (namedSilentFail != null)
				{
					if (!namedSilentFail.Worker.ShouldShowFor(StatRequest.For(innerIfMinified)))
					{
						return false;
					}
					float statValue = innerIfMinified.GetStatValue(namedSilentFail, true, -1);
					if (statValue >= Mathf.Min(a, b) && statValue <= Mathf.Max(a, b))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060002D1 RID: 721 RVA: 0x0001A18B File Offset: 0x0001838B
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x060002D2 RID: 722 RVA: 0x0001A192 File Offset: 0x00018392
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0001A198 File Offset: 0x00018398
		public void RenderStarted(List<string> Data, Map map)
		{
			string empty = string.Empty;
			this.StatsList = new ASListBox<StatDef>(25f, 4f, false, false);
			this.CatsList = new ASListBox<StatCategoryDef>(25f, 4f, false, false);
			string text = string.Empty;
			float f = float.NaN;
			float f2 = float.NaN;
			this.currentmax = 0f;
			this.currentmin = 0f;
			this.cachemax = string.Empty;
			this.cachemin = string.Empty;
			foreach (string text2 in Data)
			{
				try
				{
					string a = text2.Substring(0, 4);
					if (!(a == "std."))
					{
						if (!(a == "min."))
						{
							if (a == "max.")
							{
								f2 = float.Parse(text2.Substring(4));
							}
						}
						else
						{
							f = float.Parse(text2.Substring(4));
						}
					}
					else
					{
						text = text2.Substring(4);
					}
				}
				catch
				{
				}
			}
			foreach (StatCategoryDef statCategoryDef in from el in DefDatabase<StatCategoryDef>.AllDefs
			orderby el.displayOrder
			select el)
			{
				this.CatsList.Add(new SelectedItem<StatCategoryDef>
				{
					Item = statCategoryDef,
					label = statCategoryDef.LabelCap + " (" + statCategoryDef.defName + ")",
					surpresslabelupdate = true
				});
			}
			StatDef namedSilentFail = DefDatabase<StatDef>.GetNamedSilentFail(text);
			StatCategoryDef catdef = null;
			if (namedSilentFail != null)
			{
				catdef = namedSilentFail.category;
				int itemIndex = this.CatsList.GetItemIndex(catdef);
				if (itemIndex != -1)
				{
					this.CatsList.ChangeSelected(itemIndex);
				}
			}
			IEnumerable<StatDef> source = from el in DefDatabase<StatDef>.AllDefs
			orderby el.displayPriorityInCategory
			select el;
			Func<StatDef, bool> <>9__2;
			Func<StatDef, bool> predicate;
			if ((predicate = <>9__2) == null)
			{
				predicate = (<>9__2 = ((StatDef el) => catdef == null || el.category == catdef));
			}
			foreach (StatDef item in source.Where(predicate))
			{
				this.StatsList.Add(item);
			}
			this.StatsList.OnClick += this.OnClick;
			this.CatsList.OnClick += this.OnCatClick;
			if (text != string.Empty)
			{
				int itemIndex2 = this.StatsList.GetItemIndex(namedSilentFail);
				if (itemIndex2 != -1)
				{
					this.StatsList.ChangeSelected(itemIndex2);
				}
			}
			if (!float.IsNaN(f))
			{
				this.currentmin = f;
			}
			else if (this.StatsList.GetSelectedIndex != -1)
			{
				this.currentmin = this.minval;
			}
			if (!float.IsNaN(f2))
			{
				this.currentmax = f2;
			}
			else if (this.StatsList.GetSelectedIndex != -1)
			{
				this.currentmax = this.maxval;
			}
			this.UpdateAllCacheCurrent();
			this.AllowRender = true;
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0001A510 File Offset: 0x00018710
		public void RenderStopped(List<string> Data, Map map)
		{
			this.StatsList.OnClick -= this.OnClick;
			this.CatsList.OnClick -= this.OnCatClick;
			Data.Clear();
			if (this.StatsList.GetSelectedIndex != -1)
			{
				Data.Add("std." + this.StatsList.GetSelected.defName);
				Data.Add("min." + this.currentmin.ToString());
				Data.Add("max." + this.currentmax.ToString());
			}
			this.AllowRender = false;
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0001A5BC File Offset: 0x000187BC
		private void OnClick(IGUIList<StatDef> sender, GUIListEventArgs e)
		{
			if (e.index != -1)
			{
				this.minval = (float)Math.Round((double)sender[e.index].Item.minValue, 2);
				this.maxval = (float)Math.Round((double)sender[e.index].Item.maxValue, 2);
				this.cachemin = string.Empty;
				this.cachemax = string.Empty;
			}
		}

		// Token: 0x060002D6 RID: 726 RVA: 0x0001A630 File Offset: 0x00018830
		private void UpateCache(ref string Cache, float val)
		{
			if (this.StatsList.GetSelectedIndex != -1)
			{
				if (this.StatsList.GetSelected.toStringStyle == ToStringStyle.PercentOne || this.StatsList.GetSelected.toStringStyle == ToStringStyle.PercentTwo || this.StatsList.GetSelected.toStringStyle == ToStringStyle.PercentZero)
				{
					Cache = Math.Round((double)(val * 100f)).ToString();
					return;
				}
				Cache = Math.Round((double)val, 2).ToString();
			}
		}

		// Token: 0x060002D7 RID: 727 RVA: 0x0001A6B0 File Offset: 0x000188B0
		private void UpdateAllCacheCurrent()
		{
			if (this.StatsList.GetSelectedIndex != -1)
			{
				this.UpateCache(ref this.cachemax, this.currentmax);
				this.UpateCache(ref this.cachemin, this.currentmin);
			}
		}

		// Token: 0x060002D8 RID: 728 RVA: 0x0001A6E4 File Offset: 0x000188E4
		private void OnCatClick(IGUIList<StatCategoryDef> sender, GUIListEventArgs e)
		{
			if (e.index != -1)
			{
				StatCategoryDef catdef = sender[e.index].Item;
				this.StatsList.Clear();
				IEnumerable<StatDef> source = from el in DefDatabase<StatDef>.AllDefs
				orderby el.displayPriorityInCategory
				select el;
				Func<StatDef, bool> predicate;
				Func<StatDef, bool> <>9__1;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((StatDef el) => catdef == null || el.category == catdef));
				}
				foreach (StatDef item in source.Where(predicate))
				{
					this.StatsList.Add(item);
				}
				if (this.StatsList.Count > 0)
				{
					this.StatsList.ChangeSelected(0);
				}
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0001A7CC File Offset: 0x000189CC
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Rect inRect = new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 8f, 31f);
				ASTable astable = new ASTable(new Rect(inRect.x, inRect.yMax + 4f, inRect.width, inrect.height - 60f), 1, 3, 0f, 0f, false);
				this.CatsList.DrawListBox(astable.GetRectangle(0, 0, -1, -1, false));
				Rect inrect2;
				Rect outrect;
				astable.SplitRectangle(astable.GetRectangle(0, 1, -1, 2, false), 25f, out inrect2, out outrect, false);
				string a = this.searchstring;
				this.searchstring = ASWidgets.AlignedTextBox(inrect2, "RWAutoSell.FilterSearch".Translate(), this.searchstring);
				if (a != this.searchstring)
				{
					StatDef statDef = null;
					float num = 0f;
					float num2 = 0f;
					if (this.StatsList.GetSelectedIndex != -1)
					{
						statDef = this.StatsList[this.StatsList.GetSelectedIndex].Item;
						num = this.currentmin;
						num2 = this.currentmax;
					}
					this.StatsList.Clear();
					if (this.CatsList.GetSelectedIndex != -1)
					{
						foreach (StatDef statDef2 in from el in DefDatabase<StatDef>.AllDefs
						where el.category == this.CatsList.GetSelected
						orderby el.label
						select el)
						{
							if (this.searchstring.NullOrEmpty() || statDef2.defName.ToLowerInvariant().Contains(this.searchstring.ToLowerInvariant()) || statDef2.label.ToLowerInvariant().Contains(this.searchstring.ToLowerInvariant()))
							{
								this.StatsList.Add(statDef2);
							}
						}
						if (statDef != null)
						{
							int itemIndex = this.StatsList.GetItemIndex(statDef);
							if (itemIndex != -1)
							{
								this.StatsList.ChangeSelected(itemIndex);
								this.currentmin = num;
								this.currentmax = num2;
								this.UpdateAllCacheCurrent();
							}
						}
					}
				}
				this.StatsList.DrawListBox(outrect);
				ASTable astable2 = new ASTable(inRect, 6, 1, 0f, 0f, false);
				if (this.StatsList.GetSelectedIndex != -1)
				{
					if (Widgets.ButtonText(astable2.GetRectangle(0, 0, -1, -1, false), "RWAutoSell.Minimum".Translate(), true, true, true, null))
					{
						this.currentmin = this.minval;
						this.UpateCache(ref this.cachemin, this.currentmin);
					}
					if (Widgets.ButtonText(astable2.GetRectangle(5, 0, -1, -1, false), "RWAutoSell.Maximum".Translate(), true, true, true, null))
					{
						this.currentmax = this.maxval;
						this.UpateCache(ref this.cachemax, this.currentmax);
					}
					if (this.StatsList.GetSelected.toStringStyle == ToStringStyle.PercentOne || this.StatsList.GetSelected.toStringStyle == ToStringStyle.PercentTwo || this.StatsList.GetSelected.toStringStyle == ToStringStyle.PercentZero)
					{
						ASWidgets.TextFieldPercent(astable2.GetRectangle(1, 0, -1, -1, false), ref this.currentmin, ref this.cachemin, this.minval, this.maxval);
						ASWidgets.TextFieldPercent(astable2.GetRectangle(4, 0, -1, -1, false), ref this.currentmax, ref this.cachemax, this.minval, this.maxval);
					}
					else
					{
						ASWidgets.TextFieldStruct<float>(astable2.GetRectangle(1, 0, -1, -1, false), ref this.currentmin, ref this.cachemin, this.minval, this.maxval);
						ASWidgets.TextFieldStruct<float>(astable2.GetRectangle(4, 0, -1, -1, false), ref this.currentmax, ref this.cachemax, this.minval, this.maxval);
					}
					TextAnchor anchor = Text.Anchor;
					Text.Anchor = TextAnchor.MiddleCenter;
					string label = this.StatsList.GetSelected.Worker.GetStatDrawEntryLabel(this.StatsList.GetSelected, this.currentmin, this.StatsList.GetSelected.toStringNumberSense, StatRequest.ForEmpty(), true) + " - " + this.StatsList.GetSelected.Worker.GetStatDrawEntryLabel(this.StatsList.GetSelected, this.currentmax, this.StatsList.GetSelected.toStringNumberSense, StatRequest.ForEmpty(), true);
					Widgets.Label(astable2.GetRectangle(2, 0, 3, -1, false), label);
					Text.Anchor = anchor;
				}
			}
		}

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0001AC80 File Offset: 0x00018E80
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals",
			"RWAutoSell.TagPeople",
			"RWAutoSell.TagItems"
		};

		// Token: 0x04000176 RID: 374
		private string searchstring = string.Empty;

		// Token: 0x04000177 RID: 375
		private float minval;

		// Token: 0x04000178 RID: 376
		private float maxval = 1f;

		// Token: 0x04000179 RID: 377
		private float currentmin;

		// Token: 0x0400017A RID: 378
		private float currentmax;

		// Token: 0x0400017B RID: 379
		private string cachemin = string.Empty;

		// Token: 0x0400017C RID: 380
		private string cachemax = string.Empty;

		// Token: 0x0400017D RID: 381
		private ASListBox<StatDef> StatsList;

		// Token: 0x0400017E RID: 382
		private ASListBox<StatCategoryDef> CatsList;

		// Token: 0x0400017F RID: 383
		private bool AllowRender;
	}
}
