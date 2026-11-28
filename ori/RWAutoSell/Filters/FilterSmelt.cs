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
	// Token: 0x0200004E RID: 78
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterSmelt : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x060002BC RID: 700 RVA: 0x00019951 File Offset: 0x00017B51
		public string[] Tags
		{
			get
			{
				return new string[]
				{
					"RWAutoSell.TagItems"
				};
			}
		}

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x060002BD RID: 701 RVA: 0x00019961 File Offset: 0x00017B61
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FSmelt;
			}
		}

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x060002BE RID: 702 RVA: 0x00019968 File Offset: 0x00017B68
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterSmelt".Translate();
			}
		}

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x060002BF RID: 703 RVA: 0x00019979 File Offset: 0x00017B79
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Smelt-Filter";
			}
		}

		// Token: 0x060002C0 RID: 704 RVA: 0x00019980 File Offset: 0x00017B80
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Thing anyThing = t.AnyThing;
			if (anyThing.Smeltable && Data != null && this.SmeltProducts(anyThing).Count<ThingDef>() != 0)
			{
				if (Data.Contains("all.smeltable"))
				{
					return true;
				}
				if (Data.Contains("spc.smeltable"))
				{
					foreach (ThingDef thingDef in this.SmeltProducts(anyThing))
					{
						string b = "thg." + thingDef.defName;
						using (List<string>.Enumerator enumerator2 = Data.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current == b)
								{
									return true;
								}
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x060002C1 RID: 705 RVA: 0x00019A64 File Offset: 0x00017C64
		private IEnumerable<ThingDef> SmeltProducts(Thing t)
		{
			FilterSmelt.<SmeltProducts>d__14 <SmeltProducts>d__ = new FilterSmelt.<SmeltProducts>d__14(-2);
			<SmeltProducts>d__.<>3__t = t;
			return <SmeltProducts>d__;
		}

		// Token: 0x060002C2 RID: 706 RVA: 0x00019A74 File Offset: 0x00017C74
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (t.AnyThing != null && t.AnyThing.Smeltable)
			{
				foreach (ThingDef thingDef in this.SmeltProducts(t.AnyThing))
				{
					list.Add("thg." + thingDef.defName);
				}
			}
			return list;
		}

		// Token: 0x060002C3 RID: 707 RVA: 0x00019AF4 File Offset: 0x00017CF4
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060002C4 RID: 708 RVA: 0x00019AF8 File Offset: 0x00017CF8
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				ASRectMonitor asrectMonitor = new ASRectMonitor(inrect.ContractedBy(2f));
				if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterSmeltAny".Translate(), this.smeltall, false))
				{
					this.smeltall = true;
				}
				if (Widgets.RadioButtonLabeled(asrectMonitor.NextFromTop(30f), "RWAutoSell.FilterSmeltProduce".Translate(), !this.smeltall, false))
				{
					this.smeltall = false;
				}
				if (!this.smeltall)
				{
					this.selecteddefs.DrawListBox(asrectMonitor.NextFromBottom(200f));
					string text = this.searchdeflabel;
					string a = Widgets.TextField(asrectMonitor.NextFromTop(35f), text);
					if (a != text)
					{
						this.searchdeflabel = a;
						this.MatBox.Clear();
						StuffCategoryDef sd = DefDatabase<StuffCategoryDef>.AllDefs.FirstOrDefault((StuffCategoryDef x) => x.defName.ToLowerInvariant() == this.searchdeflabel.ToLowerInvariant() || x.label.ToLowerInvariant() == this.searchdeflabel.ToLowerInvariant());
						if (sd != null)
						{
							this.MatBox.AddRange((from el in DefDatabase<ThingDef>.AllDefs
							where el.EverHaulable && ((el.stuffCategories != null && el.stuffCategories.Contains(sd)) || el.label.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()) || el.defName.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()))
							orderby el.label.ToLowerInvariant().StartsWith(this.searchdeflabel.ToLowerInvariant()) descending
							select el).ToList<ThingDef>());
						}
						else
						{
							this.MatBox.AddRange((from el in DefDatabase<ThingDef>.AllDefs
							where el.EverHaulable && (el.label.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()) || el.defName.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()))
							orderby el.label.ToLowerInvariant().StartsWith(this.searchdeflabel.ToLowerInvariant()) descending
							select el).ToList<ThingDef>());
						}
						foreach (SelectedItem<ThingDef> selectedItem in this.MatBox)
						{
							if (this.selecteddefs.Contains(selectedItem.Item))
							{
								selectedItem.Selected = true;
							}
						}
					}
					this.MatBox.DrawListBox(asrectMonitor.Remaining());
				}
			}
		}

		// Token: 0x060002C5 RID: 709 RVA: 0x00019CF0 File Offset: 0x00017EF0
		private void MatBox_OnClick(IGUIList<ThingDef> sender, GUIListEventArgs e)
		{
			foreach (SelectedItem<ThingDef> selectedItem in this.MatBox)
			{
				if (this.selecteddefs.Contains(selectedItem.Item) && !selectedItem.Selected)
				{
					this.selecteddefs.Remove(selectedItem.Item);
				}
				else if (!this.selecteddefs.Contains(selectedItem.Item) && selectedItem.Selected)
				{
					this.selecteddefs.Add(selectedItem.Item);
				}
			}
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x00019D94 File Offset: 0x00017F94
		public void RenderStarted(List<string> Data, Map map)
		{
			this.searchdeflabel = "";
			this.MatBox = new ASMultiBox<ThingDef>(26f, 5f, true, true);
			this.selecteddefs = new ASChangeBox<ThingDef>(26f, 5f, false, true, true, false);
			this.MatBox.OnClick += this.MatBox_OnClick;
			if (Data == null || Data.Count == 0)
			{
				Data.Add("all.smeltable");
			}
			if (Data.Contains("all.smeltable"))
			{
				this.smeltall = true;
			}
			else
			{
				this.smeltall = false;
				foreach (string text in Data)
				{
					if (text.StartsWith("thg."))
					{
						ThingDef namedSilentFail = DefDatabase<ThingDef>.GetNamedSilentFail(text.Substring(4));
						if (namedSilentFail != null && !this.selecteddefs.Contains(namedSilentFail))
						{
							this.selecteddefs.Add(namedSilentFail);
						}
					}
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x00019EA0 File Offset: 0x000180A0
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			this.MatBox.OnClick -= this.MatBox_OnClick;
			Data.Clear();
			if (this.smeltall)
			{
				Data.Add("all.smeltable");
				return;
			}
			Data.Add("spc.smeltable");
			foreach (SelectedItem<ThingDef> x in this.selecteddefs)
			{
				ThingDef thingDef = x;
				Data.Add("thg." + thingDef.defName);
			}
		}

		// Token: 0x04000171 RID: 369
		private bool smeltall = true;

		// Token: 0x04000172 RID: 370
		private bool AllowRender;

		// Token: 0x04000173 RID: 371
		private ASMultiBox<ThingDef> MatBox;

		// Token: 0x04000174 RID: 372
		private ASChangeBox<ThingDef> selecteddefs;

		// Token: 0x04000175 RID: 373
		private string searchdeflabel = "";
	}
}
