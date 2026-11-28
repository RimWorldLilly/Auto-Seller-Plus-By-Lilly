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
	// Token: 0x0200003D RID: 61
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterCat : IBaseFilter, IWikiHelp, IRuleSorter
	{
		// Token: 0x060001EA RID: 490 RVA: 0x000126A4 File Offset: 0x000108A4
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
			if (innerIfMinified != null)
			{
				foreach (string b in Data)
				{
					if ("thg." + innerIfMinified.def.defName == b)
					{
						return true;
					}
					if (innerIfMinified.def.thingCategories != null && innerIfMinified.def.thingCategories.Count > 0)
					{
						foreach (ThingCategoryDef thingCategoryDef in innerIfMinified.def.thingCategories)
						{
							if ("cat." + thingCategoryDef.defName == b)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x000127AC File Offset: 0x000109AC
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
				int num = TransferableComparer_Category.Compare(First.ThingDef, Second.ThingDef);
				if (num == 0)
				{
					return First.AnyThing.def.label.CompareTo(Second.AnyThing.def.label);
				}
				return num;
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00012827 File Offset: 0x00010A27
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0001282C File Offset: 0x00010A2C
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				ASTable astable = new ASTable(inrect, 2, 2, 10f, 4f, true);
				Rect rect;
				Rect rect2;
				astable.SplitRectangle(0, 0, 20f, out rect, out rect2, false);
				this.CatBox.DrawListBox(astable.GetRectangle(0, 0, -1, -1, false));
				Rect rect3;
				Rect outrect;
				astable.SplitRectangle(1, 0, 30f, out rect3, out outrect, false);
				string text = this.searchdeflabel;
				Color color = GUI.color;
				GUI.color = Color.white;
				string a = Widgets.TextField(rect3, text);
				GUI.color = color;
				if (a != text)
				{
					this.searchdeflabel = a;
					if (this.CatBox != null && this.CatBox.GetSelected != null)
					{
						this.ChangeDefBox(this.CatBox.GetSelected);
					}
				}
				this.MidBox.DrawListBox(outrect);
				Rect rectangle = astable.GetRectangle(0, 1, 1, -1, false);
				Rect rect4;
				Rect outrect2;
				astable.SplitRectangle(rectangle, 30f, out rect4, out outrect2, false);
				Widgets.CheckboxLabeled(rect4, "RWAutoSell.SubCat".Translate(), ref this.IncSub, false, null, null, false, false);
				this.SelectedBox.DrawListBox(outrect2);
			}
		}

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x060001EE RID: 494 RVA: 0x0001294B File Offset: 0x00010B4B
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterCat".Translate();
			}
		}

		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0001295C File Offset: 0x00010B5C
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FCat;
			}
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060001F0 RID: 496 RVA: 0x00012963 File Offset: 0x00010B63
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Category-Filter";
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0001296C File Offset: 0x00010B6C
		public void RenderStarted(List<string> Data, Map map)
		{
			this.DefList = new List<ThingDef>();
			this.searchdeflabel = "";
			this.SelectedBox = new ASChangeBox<string>(20f, 5f, false, true, true, false);
			this.IncSub = ASMod.GetSettings().IncCat;
			this.OldInc = this.IncSub;
			foreach (string text in Data)
			{
				string a = text.Substring(0, 4);
				string text2 = text.Substring(4);
				if (a == "thg.")
				{
					ThingDef namedSilentFail = DefDatabase<ThingDef>.GetNamedSilentFail(text2);
					if (namedSilentFail != null)
					{
						this.SelectedBox.Add(new SelectedItem<string>
						{
							Item = text,
							label = namedSilentFail.LabelCap,
							Icon = namedSilentFail.uiIcon,
							color = namedSilentFail.uiIconColor
						});
					}
					else
					{
						this.SelectedBox.Add(new SelectedItem<string>
						{
							Item = text,
							label = "RWAutoSell.Unknown".Translate() + " " + text2
						});
					}
				}
				else if (a == "cat.")
				{
					ThingCategoryDef namedSilentFail2 = DefDatabase<ThingCategoryDef>.GetNamedSilentFail(text2);
					if (namedSilentFail2 != null)
					{
						this.SelectedBox.Add(new SelectedItem<string>
						{
							Item = text,
							label = "RWAutoSell.FilterStuffAny".Translate(namedSilentFail2.LabelCap),
							Icon = namedSilentFail2.icon
						});
					}
					else
					{
						this.SelectedBox.Add(new SelectedItem<string>
						{
							Item = text,
							label = "RWAutoSell.Unknown".Translate() + " " + text2
						});
					}
				}
			}
			this.CatBox = new ASCatListBox(20f, 5f);
			this.MidBox = new ASMultiBox<ThingDef>(20f, 5f, false, true);
			this.CatBox.OnClick += this.CatClick;
			this.MidBox.OnClick += new ASListBox<ThingDef>.ListItemHandler(this.DefClick);
			this.SelectedBox.OnPostRightClick += new ASListBox<string>.ListItemHandler(this.SelectedRClick);
			this.SelectedBox.OnRightClick += new ASListBox<string>.ListItemHandler(this.SelectedPreRClick);
			this.AllowRender = true;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00012BF4 File Offset: 0x00010DF4
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			for (int i = 0; i < this.SelectedBox.Count; i++)
			{
				Data.Add(this.SelectedBox[i].Item);
			}
			this.CatBox.OnClick -= this.CatClick;
			this.CatBox = null;
			this.MidBox.OnClick -= new ASListBox<ThingDef>.ListItemHandler(this.DefClick);
			this.MidBox = null;
			this.SelectedBox.OnRightClick -= new ASListBox<string>.ListItemHandler(this.SelectedPreRClick);
			this.SelectedBox.OnPostRightClick -= new ASListBox<string>.ListItemHandler(this.SelectedRClick);
			this.SelectedBox = null;
			this.AllowRender = false;
			if (this.IncSub != this.OldInc)
			{
				ASMod.GetSettings().IncCat = this.IncSub;
				ASMod.GetSettings().Write();
			}
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00012CD4 File Offset: 0x00010ED4
		private void SelectedPreRClick(object Sender, GUIListEventArgs e)
		{
			this.remitem = this.SelectedBox[e.index];
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00012CF4 File Offset: 0x00010EF4
		private void SelectedRClick(object Sender, GUIListEventArgs e)
		{
			foreach (SelectedItem<ThingDef> selectedItem in this.MidBox)
			{
				if (selectedItem.Item != null && "thg." + selectedItem.Item.defName == this.remitem)
				{
					selectedItem.Selected = false;
				}
				else if (selectedItem.Item == null && "cat." + this.CatBox.GetSelected.defName == this.remitem)
				{
					selectedItem.Selected = false;
				}
			}
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00012DA4 File Offset: 0x00010FA4
		private void DefClick(object Sender, GUIListEventArgs e)
		{
			for (int i = 0; i < this.MidBox.Count; i++)
			{
				if (this.MidBox[i].Selected)
				{
					if (this.MidBox[i].Item == null)
					{
						using (IEnumerator<ThingCategoryDef> enumerator = this.GetCategories().GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ThingCategoryDef thingCategoryDef = enumerator.Current;
								if (!this.SelectedBox.Contains("cat." + thingCategoryDef.defName))
								{
									this.SelectedBox.Add(new SelectedItem<string>
									{
										Item = "cat." + thingCategoryDef.defName,
										label = "RWAutoSell.FilterStuffAny".Translate(thingCategoryDef.LabelCap),
										Icon = thingCategoryDef.icon
									});
								}
							}
							goto IL_274;
						}
					}
					if (!this.SelectedBox.Contains("thg." + this.MidBox[i].Item.defName))
					{
						this.SelectedBox.Add(new SelectedItem<string>
						{
							Item = "thg." + this.MidBox[i].Item.defName,
							label = this.MidBox[i].Item.LabelCap,
							Icon = this.MidBox[i].Item.uiIcon,
							color = this.MidBox[i].Item.uiIconColor
						});
					}
				}
				else
				{
					if (this.MidBox[i].Item == null)
					{
						using (IEnumerator<ThingCategoryDef> enumerator = this.GetCategories().GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								ThingCategoryDef thingCategoryDef2 = enumerator.Current;
								if (this.SelectedBox.Contains("cat." + thingCategoryDef2.defName))
								{
									this.SelectedBox.RemoveAt(this.SelectedBox.GetItemIndex("cat." + thingCategoryDef2.defName));
								}
							}
							goto IL_274;
						}
					}
					if (this.SelectedBox.Contains("thg." + this.MidBox[i].Item.defName))
					{
						this.SelectedBox.RemoveAt(this.SelectedBox.GetItemIndex("thg." + this.MidBox[i].Item.defName));
					}
				}
				IL_274:;
			}
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00013058 File Offset: 0x00011258
		private IEnumerable<ThingCategoryDef> GetCategories()
		{
			List<ThingCategoryDef> list = new List<ThingCategoryDef>();
			if (this.IncSub)
			{
				list.AddRange(this.CatBox.GetSelected.ThisAndChildCategoryDefs);
			}
			else
			{
				list.Add(this.CatBox.GetSelected);
			}
			return list;
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x000130A0 File Offset: 0x000112A0
		private void CatClick(object Sender, GUICatListEventArgs e)
		{
			if (e.Node != null)
			{
				this.DefList = (from el in e.Node.Category.childThingDefs
				where el != null && (el.EverHaulable || el.Minifiable)
				select el).ToList<ThingDef>();
				this.DefList.Sort((ThingDef a, ThingDef b) => a.LabelCap.ToString().CompareTo(b.LabelCap));
				this.ChangeDefBox(e.Node.Category);
			}
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00013130 File Offset: 0x00011330
		private void ChangeDefBox(ThingCategoryDef cat)
		{
			SelectedItem<ThingDef> item = new SelectedItem<ThingDef>
			{
				Item = null,
				label = "RWAutoSell.FilterStuffAny".Translate(cat.label),
				Icon = cat.icon,
				Selected = false
			};
			List<ThingDef> items = (this.searchdeflabel == string.Empty) ? this.DefList : (from el in this.DefList
			where el.label.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()) || el.defName.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant())
			orderby el.label.ToLowerInvariant().StartsWith(this.searchdeflabel.ToLowerInvariant()) descending
			select el).ToList<ThingDef>();
			this.MidBox.Clear();
			this.MidBox.Add(item);
			this.MidBox.AddRange(items);
			foreach (SelectedItem<ThingDef> selectedItem in this.MidBox)
			{
				if (selectedItem.Item == null)
				{
					if (this.SelectedBox.Contains("cat." + this.CatBox.GetSelected.defName))
					{
						selectedItem.Selected = true;
					}
				}
				else if (this.SelectedBox.Contains("thg." + selectedItem.Item.defName))
				{
					selectedItem.Selected = true;
				}
			}
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x00013284 File Offset: 0x00011484
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> result = new List<string>();
			Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
			if (innerIfMinified != null && innerIfMinified.def.thingCategories != null && innerIfMinified.def.thingCategories.Count > 0)
			{
				result = new List<string>
				{
					"thg." + innerIfMinified.def.defName
				};
			}
			return result;
		}

		// Token: 0x1700006D RID: 109
		// (get) Token: 0x060001FA RID: 506 RVA: 0x000132E8 File Offset: 0x000114E8
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems",
			"RWAutoSell.TagFood"
		};

		// Token: 0x04000114 RID: 276
		private List<ThingDef> DefList = new List<ThingDef>();

		// Token: 0x04000115 RID: 277
		private string searchdeflabel;

		// Token: 0x04000116 RID: 278
		private ASCatListBox CatBox;

		// Token: 0x04000117 RID: 279
		private ASMultiBox<ThingDef> MidBox;

		// Token: 0x04000118 RID: 280
		private ASChangeBox<string> SelectedBox;

		// Token: 0x04000119 RID: 281
		private bool AllowRender;

		// Token: 0x0400011A RID: 282
		private bool IncSub;

		// Token: 0x0400011B RID: 283
		private bool OldInc;

		// Token: 0x0400011C RID: 284
		private string remitem = string.Empty;
	}
}
