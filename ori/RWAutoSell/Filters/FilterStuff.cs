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
	// Token: 0x02000050 RID: 80
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterStuff : IBaseFilter, IWikiHelp
	{
		// Token: 0x060002DD RID: 733 RVA: 0x0001AD00 File Offset: 0x00018F00
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
			if (innerIfMinified != null)
			{
				using (List<string>.Enumerator enumerator = Data.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string check = enumerator.Current;
						string a = check.Substring(0, 4);
						if (a == "nul." && innerIfMinified.Stuff == null)
						{
							return true;
						}
						if (a == "scd." && innerIfMinified.Stuff != null && innerIfMinified.Stuff.stuffProps.categories.Any((StuffCategoryDef x) => x.defName == check.Substring(4)))
						{
							return true;
						}
						if (a == "mat." && innerIfMinified.Stuff != null && innerIfMinified.Stuff.defName == check.Substring(4))
						{
							return true;
						}
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x060002DE RID: 734 RVA: 0x0001AE10 File Offset: 0x00019010
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x060002DF RID: 735 RVA: 0x0001AE13 File Offset: 0x00019013
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Stuff-Filter";
			}
		}

		// Token: 0x060002E0 RID: 736 RVA: 0x0001AE1C File Offset: 0x0001901C
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				float num = 180f;
				Rect viewRect = new Rect(inrect.x + 4f, inrect.y, inrect.width - 6f, inrect.height);
				if (this.StuffOps.Length > 5)
				{
					Rect outRect = new Rect(inrect.x + 4f, inrect.y, inrect.width - 4f, num);
					viewRect = new Rect(0f, 0f, outRect.width - 17f, 30f * (float)this.StuffOps.Length);
					Widgets.BeginScrollView(outRect, ref this.scrl, viewRect, true);
				}
				for (int i = 0; i < this.StuffOps.Length; i++)
				{
					if (Widgets.RadioButtonLabeled(new Rect(viewRect.x, viewRect.y + 30f * (float)i, viewRect.width, 30f), this.StuffOps[i].LabelCap, this.BoolOps[i], false))
					{
						this.BoolOps[i] = !this.BoolOps[i];
						this.OnUpdate();
					}
				}
				if (this.StuffOps.Length > 5)
				{
					Widgets.EndScrollView();
				}
				else
				{
					num = 30f * (float)this.StuffOps.Length;
				}
				Rect rect = new Rect(inrect.x + 4f, inrect.y + num, inrect.width - 6f, 30f);
				string text = this.searchdeflabel;
				string a = Widgets.TextField(rect, text);
				if (a != text)
				{
					this.searchdeflabel = a;
					this.OnUpdate();
				}
				this.MatBox.DrawListBox(new Rect(inrect.x + 4f, inrect.y + num + 30f, inrect.width - 6f, inrect.height - num - 30f));
			}
		}

		// Token: 0x060002E1 RID: 737 RVA: 0x0001B010 File Offset: 0x00019210
		private void OnUpdate()
		{
			List<string> list = (from el in this.MatBox
			where el.Selected
			select el.Item).ToList<string>();
			ASMultiBox<string> asmultiBox = new ASMultiBox<string>(26f, 5f, false, true);
			SelectedItem<string> selectedItem = new SelectedItem<string>();
			selectedItem.Item = "nul.";
			selectedItem.label = "RWAutoSell.FilterStuffNone".Translate();
			selectedItem.Selected = list.Any((string x) => x == "nul.");
			asmultiBox.Add(selectedItem);
			this.MatBox = asmultiBox;
			int k;
			int j;
			for (k = 0; k < this.StuffOps.Length; k = j + 1)
			{
				if (this.BoolOps[k])
				{
					this.MatBox.Add(new SelectedItem<string>
					{
						Item = "scd." + this.StuffOps[k].defName,
						label = "RWAutoSell.FilterStuffAny".Translate(this.StuffOps[k].LabelCap),
						Selected = list.Any((string x) => x == "scd." + this.StuffOps[k].defName)
					});
				}
				j = k;
			}
			int i;
			Func<ThingDef, bool> <>9__4;
			for (i = 0; i < this.StuffOps.Length; i = j + 1)
			{
				if (this.BoolOps[i])
				{
					IEnumerable<ThingDef> allDefs = DefDatabase<ThingDef>.AllDefs;
					Func<ThingDef, bool> predicate;
					if ((predicate = <>9__4) == null)
					{
						predicate = (<>9__4 = ((ThingDef x) => x.IsStuff && x.stuffProps.categories.Contains(this.StuffOps[i]) && (this.searchdeflabel == "" || x.label.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()) || x.defName.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()))));
					}
					using (IEnumerator<ThingDef> enumerator = allDefs.Where(predicate).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							ThingDef Mat = enumerator.Current;
							this.MatBox.Add(new SelectedItem<string>
							{
								Item = "mat." + Mat.defName,
								label = Mat.LabelCap,
								Selected = list.Any((string x) => x == "mat." + Mat.defName),
								Icon = Mat.uiIcon,
								color = Mat.uiIconColor
							});
						}
					}
				}
				j = i;
			}
		}

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x060002E2 RID: 738 RVA: 0x0001B2E4 File Offset: 0x000194E4
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterStuff".Translate();
			}
		}

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x060002E3 RID: 739 RVA: 0x0001B2F5 File Offset: 0x000194F5
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FStuff;
			}
		}

		// Token: 0x060002E4 RID: 740 RVA: 0x0001B2FC File Offset: 0x000194FC
		public void RenderStarted(List<string> Data, Map map)
		{
			HashSet<StuffCategoryDef> hashSet = new HashSet<StuffCategoryDef>();
			this.searchdeflabel = "";
			if (Data != null && Data.Count > 0)
			{
				foreach (string text in Data)
				{
					string a = text.Substring(0, 4);
					if (a == "scd.")
					{
						StuffCategoryDef namedSilentFail = DefDatabase<StuffCategoryDef>.GetNamedSilentFail(text.Substring(4));
						if (namedSilentFail != null)
						{
							hashSet.Add(namedSilentFail);
							continue;
						}
					}
					if (a == "mat.")
					{
						ThingDef namedSilentFail2 = DefDatabase<ThingDef>.GetNamedSilentFail(text.Substring(4));
						if (namedSilentFail2 != null && namedSilentFail2.IsStuff)
						{
							foreach (StuffCategoryDef item in namedSilentFail2.stuffProps.categories)
							{
								hashSet.Add(item);
							}
						}
					}
				}
			}
			this.StuffOps = DefDatabase<StuffCategoryDef>.AllDefs.ToArray<StuffCategoryDef>();
			this.BoolOps = new bool[this.StuffOps.Length];
			int i;
			Func<StuffCategoryDef, bool> <>9__0;
			int j;
			for (i = 0; i < this.StuffOps.Length; i = j + 1)
			{
				IEnumerable<StuffCategoryDef> source = hashSet;
				Func<StuffCategoryDef, bool> predicate;
				if ((predicate = <>9__0) == null)
				{
					predicate = (<>9__0 = ((StuffCategoryDef x) => x.defName == this.StuffOps[i].defName));
				}
				if (source.Any(predicate))
				{
					this.BoolOps[i] = true;
				}
				else
				{
					this.BoolOps[i] = false;
				}
				j = i;
			}
			ASMultiBox<string> asmultiBox = new ASMultiBox<string>(26f, 5f, false, true);
			SelectedItem<string> selectedItem = new SelectedItem<string>();
			selectedItem.Item = "nul.";
			selectedItem.label = "RWAutoSell.FilterStuffNone".Translate();
			selectedItem.Selected = Data.Any((string x) => x == "nul.");
			asmultiBox.Add(selectedItem);
			this.MatBox = asmultiBox;
			using (HashSet<StuffCategoryDef>.Enumerator enumerator3 = hashSet.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					StuffCategoryDef stuff = enumerator3.Current;
					this.MatBox.Add(new SelectedItem<string>
					{
						Item = "scd." + stuff.defName,
						label = "RWAutoSell.FilterStuffAny".Translate(stuff.LabelCap),
						Selected = Data.Any((string x) => x == "scd." + stuff.defName)
					});
				}
			}
			using (HashSet<StuffCategoryDef>.Enumerator enumerator3 = hashSet.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					StuffCategoryDef stuff = enumerator3.Current;
					IEnumerable<ThingDef> allDefs = DefDatabase<ThingDef>.AllDefs;
					Func<ThingDef, bool> predicate2;
					Func<ThingDef, bool> <>9__3;
					if ((predicate2 = <>9__3) == null)
					{
						predicate2 = (<>9__3 = ((ThingDef x) => x.IsStuff && x.stuffProps.categories.Contains(stuff)));
					}
					using (IEnumerator<ThingDef> enumerator4 = allDefs.Where(predicate2).GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							ThingDef Mat = enumerator4.Current;
							this.MatBox.Add(new SelectedItem<string>
							{
								Item = "mat." + Mat.defName,
								label = Mat.LabelCap,
								Selected = Data.Any((string x) => x == "mat." + Mat.defName),
								Icon = Mat.uiIcon,
								color = Mat.uiIconColor
							});
						}
					}
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0001B75C File Offset: 0x0001995C
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			for (int i = 0; i < this.MatBox.Count; i++)
			{
				if (this.MatBox[i].Selected)
				{
					Data.Add(this.MatBox[i].Item);
				}
			}
			this.MatBox = null;
			this.StuffOps = null;
			this.AllowRender = false;
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0001B7C4 File Offset: 0x000199C4
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Thing innerIfMinified = t.AnyThing.GetInnerIfMinified();
			if (innerIfMinified != null && !(innerIfMinified is Pawn))
			{
				if (innerIfMinified.Stuff != null)
				{
					list.Add("mat." + innerIfMinified.Stuff.defName);
				}
				else
				{
					list.Add("nul.");
				}
			}
			return list;
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060002E7 RID: 743 RVA: 0x0001B81F File Offset: 0x00019A1F
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagItems"
		};

		// Token: 0x04000181 RID: 385
		private string searchdeflabel;

		// Token: 0x04000182 RID: 386
		private StuffCategoryDef[] StuffOps;

		// Token: 0x04000183 RID: 387
		private bool[] BoolOps;

		// Token: 0x04000184 RID: 388
		private ASMultiBox<string> MatBox;

		// Token: 0x04000185 RID: 389
		private bool AllowRender;

		// Token: 0x04000186 RID: 390
		private Vector2 scrl = Vector2.zero;
	}
}
