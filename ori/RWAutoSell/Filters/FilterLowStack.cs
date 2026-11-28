using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RimWorld.Planet;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000044 RID: 68
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterLowStack : IBaseFilter, IWikiHelp
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x06000248 RID: 584 RVA: 0x0001542A File Offset: 0x0001362A
		public string[] Tags
		{
			get
			{
				return new string[]
				{
					"RWAutoSell.TagItems",
					"RWAutoSell.TagFood"
				};
			}
		}

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x06000249 RID: 585 RVA: 0x00015442 File Offset: 0x00013642
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FLStack;
			}
		}

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x0600024A RID: 586 RVA: 0x00015449 File Offset: 0x00013649
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Low-Stockpile-Filter";
			}
		}

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x0600024B RID: 587 RVA: 0x00015450 File Offset: 0x00013650
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterLow".Translate();
			}
		}

		// Token: 0x0600024C RID: 588 RVA: 0x00015464 File Offset: 0x00013664
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Map map = null;
			Thing thing = null;
			Tradeable tradeable = t as Tradeable;
			if (tradeable != null)
			{
				thing = tradeable.FirstThingColony.GetInnerIfMinified();
			}
			if (thing == null)
			{
				thing = t.AnyThing.GetInnerIfMinified();
			}
			if (ASLibMod.ActiveComp != null)
			{
				map = ASLibMod.ActiveComp.map;
			}
			else if (thing != null)
			{
				map = thing.MapHeld;
			}
			if (Data.Count > 0)
			{
				int num = 0;
				bool doEquped = Data[1] == "op.te" || Data[1] == "op.se";
				if (Data.Count == 1 || Data[1] == "op.t" || Data[1] == "op.te")
				{
					num = this.GetCount(thing, map, doEquped);
				}
				else if (Data.Count >= 3 && (Data[1] == "op.s" || Data[1] == "op.se") && Data[2] != "null")
				{
					ThingDef namedSilentFail = DefDatabase<ThingDef>.GetNamedSilentFail(Data[2]);
					if (namedSilentFail != null)
					{
						num = this.GetCount(namedSilentFail, map, doEquped);
					}
				}
				int num2;
				if (int.TryParse(Data[0], out num2))
				{
					return num < num2;
				}
			}
			return false;
		}

		// Token: 0x0600024D RID: 589 RVA: 0x000155A2 File Offset: 0x000137A2
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x0600024E RID: 590 RVA: 0x000155AC File Offset: 0x000137AC
		private int GetCount(Thing t, Map map, bool DoEquped = false)
		{
			int result = 0;
			if (t != null)
			{
				ThingDef def = t.def;
				result = this.GetCount(def, map, DoEquped);
			}
			return result;
		}

		// Token: 0x0600024F RID: 591 RVA: 0x000155D0 File Offset: 0x000137D0
		private int GetCount(ThingDef def, Map map, bool DoEquped = false)
		{
			int num = 0;
			if (map != null && map.IsPlayerHome)
			{
				foreach (Thing outerThing in CaravanFormingUtility.AllReachableColonyItems(map, true, true, false))
				{
					Thing innerIfMinified = outerThing.GetInnerIfMinified();
					if (innerIfMinified.def == def)
					{
						num += innerIfMinified.stackCount;
					}
				}
				if (DoEquped)
				{
					if (def.IsApparel)
					{
						Predicate<Apparel> <>9__0;
						foreach (Pawn pawn in map.mapPawns.FreeColonists)
						{
							if (pawn.apparel != null && pawn.apparel.WornApparelCount > 0)
							{
								List<Apparel> wornApparel = pawn.apparel.WornApparel;
								Predicate<Apparel> predicate;
								if ((predicate = <>9__0) == null)
								{
									predicate = (<>9__0 = ((Apparel x) => x.def == def));
								}
								if (wornApparel.Any(predicate))
								{
									num++;
									continue;
								}
							}
							if (def.IsWeapon)
							{
								foreach (Pawn pawn2 in map.mapPawns.FreeColonists)
								{
									if (pawn2.equipment != null && pawn2.equipment.HasAnything() && pawn2.equipment.Primary != null && pawn2.equipment.Primary.def == def)
									{
										num++;
									}
								}
							}
						}
					}
					foreach (Pawn pawn3 in map.mapPawns.FreeColonists)
					{
						if (pawn3.inventory != null)
						{
							foreach (Thing outerThing2 in pawn3.inventory.innerContainer)
							{
								Thing innerIfMinified2 = outerThing2.GetInnerIfMinified();
								if (innerIfMinified2.def == def)
								{
									num += innerIfMinified2.stackCount;
								}
							}
						}
					}
				}
			}
			return num;
		}

		// Token: 0x06000250 RID: 592 RVA: 0x0001584C File Offset: 0x00013A4C
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000251 RID: 593 RVA: 0x00015850 File Offset: 0x00013A50
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Rect inrect2 = inrect.TopPartPixels(30f);
				int num;
				int.TryParse(Data[0], out num);
				ASWidgets.AlignedTextBox<int>(inrect2, "RWAutoSell.FilterLowFlagIf".Translate(), ref num, ref this.Cache, 0f, 2.1474836E+09f);
				Data[0] = num.ToString();
				Rect rect = new Rect(inrect2.x, inrect2.yMax + 40f, inrect.width, 30f);
				Rect rect2 = new Rect(inrect2.x, rect.yMax, inrect.width, 30f);
				Rect rect3 = new Rect(inrect2.x, rect2.yMax, inrect.width, 30f);
				Rect rect4 = new Rect(inrect2.x, rect3.yMax, inrect.width, 30f);
				bool flag = true;
				bool flag2 = Data[1] == "op.s" || Data[1] == "op.se";
				flag = (Data[1] == "op.se" || Data[1] == "op.te");
				if (Widgets.RadioButtonLabeled(rect, "RWAutoSell.FilterLowRowDef".Translate(), !flag2, false))
				{
					if (this.deflist != null)
					{
						this.deflist.OnClick -= this.Deflist_OnClick;
					}
					Data[1] = (flag ? "op.te" : "op.t");
					this.deflist = null;
					this.selecteddef = null;
					flag2 = false;
				}
				if (Widgets.RadioButtonLabeled(rect2, "RWAutoSell.FilterLowSpecDef".Translate(), flag2, false))
				{
					Data[1] = (flag ? "op.se" : "op.s");
					this.selecteddef = null;
					this.searchdeflabel = "";
					this.deflist = new ASListBox<ThingDef>(25f, 4f, false, true);
					this.deflist.OnClick += this.Deflist_OnClick;
					flag2 = true;
				}
				bool flag3 = flag;
				Widgets.CheckboxLabeled(rect3, "RWAutoSell.FilterLowIncEqip".Translate(), ref flag, false, null, null, false, false);
				if (flag3 != flag)
				{
					Data[1] = (flag2 ? "op.s" : "op.t") + (flag ? "e" : string.Empty);
				}
				if (flag2)
				{
					string text = this.searchdeflabel;
					string a = Widgets.TextField(rect4, text);
					if (a != text)
					{
						if (this.selecteddef != null)
						{
							this.selecteddef = null;
						}
						this.searchdeflabel = a;
						this.deflist.Clear();
						this.deflist.AddRange((from el in DefDatabase<ThingDef>.AllDefs
						where el.EverHaulable && (el.label.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()) || el.defName.ToLowerInvariant().Contains(this.searchdeflabel.ToLowerInvariant()))
						orderby el.label.ToLowerInvariant().StartsWith(this.searchdeflabel.ToLowerInvariant()) descending
						select el).ToList<ThingDef>());
					}
					if (this.selecteddef != null)
					{
						if (Data.Count < 3)
						{
							Data.Add(this.selecteddef.defName);
						}
						if (Data.Count >= 3)
						{
							Data[2] = this.selecteddef.defName;
						}
						Rect rect5 = new Rect(rect4.x + 40f, rect4.yMax + 10f, inrect.width - 40f, 30f);
						Rect outerRect = new Rect(rect4.x, rect4.yMax + 10f, 30f, 30f);
						Widgets.Label(rect5, this.selecteddef.LabelCap);
						if (this.icon != null)
						{
							Color color = GUI.color;
							GUI.color = this.col;
							Widgets.DrawTextureFitted(outerRect, this.icon, 1f, 1f);
							GUI.color = color;
							return;
						}
					}
					else
					{
						Rect outrect = new Rect(rect4.x, rect4.yMax + 10f, inrect.width, inrect.height - (rect4.yMax + 10f - inrect.y) - 10f);
						this.deflist.DrawListBox(outrect);
					}
				}
			}
		}

		// Token: 0x06000252 RID: 594 RVA: 0x00015C7C File Offset: 0x00013E7C
		private void Deflist_OnClick(IGUIList<ThingDef> sender, GUIListEventArgs e)
		{
			ThingDef getSelected = sender.GetSelected;
			if (getSelected != null)
			{
				this.searchdeflabel = getSelected.LabelCap;
				this.selecteddef = getSelected;
				this.icon = getSelected.uiIcon;
				this.col = getSelected.uiIconColor;
			}
		}

		// Token: 0x06000253 RID: 595 RVA: 0x00015CC4 File Offset: 0x00013EC4
		public void RenderStarted(List<string> Data, Map map)
		{
			if (Data.Count == 0)
			{
				Data.Add("0");
			}
			if (Data.Count == 1)
			{
				Data.Add("op.t");
			}
			if (Data.Count == 2)
			{
				Data.Add("null");
			}
			if (Data[1] == "op.s" || Data[1] == "op.se")
			{
				ThingDef thingDef = null;
				if (Data[2] != "null")
				{
					thingDef = DefDatabase<ThingDef>.GetNamedSilentFail(Data[2]);
				}
				this.deflist = new ASListBox<ThingDef>(25f, 4f, false, true);
				this.deflist.OnClick += this.Deflist_OnClick;
				if (thingDef != null)
				{
					this.selecteddef = thingDef;
					this.searchdeflabel = thingDef.LabelCap;
					this.icon = thingDef.uiIcon;
					this.col = thingDef.uiIconColor;
				}
				else
				{
					this.selecteddef = null;
					this.searchdeflabel = Data[2];
					this.deflist.AddRange(from el in DefDatabase<ThingDef>.AllDefs
					where el.label.ToLowerInvariant().Contains(Data[2].ToLowerInvariant()) || el.defName.ToLowerInvariant().Contains(Data[2].ToLowerInvariant())
					select el);
				}
			}
			this.AllowRender = true;
		}

		// Token: 0x06000254 RID: 596 RVA: 0x00015E3C File Offset: 0x0001403C
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			if (this.deflist != null)
			{
				if (Data.Count >= 2 && (Data[1] == "op.s" || Data[1] == "op.se") && (Data[2] == "null" || Data[2] == string.Empty) && this.deflist.Count == 1)
				{
					Data[2] = this.deflist[0].Item.defName;
				}
				this.deflist.OnClick -= this.Deflist_OnClick;
				this.deflist = null;
			}
			this.Cache = null;
			this.icon = null;
			this.col = Color.white;
			this.selecteddef = null;
			this.searchdeflabel = string.Empty;
		}

		// Token: 0x04000138 RID: 312
		private ASListBox<ThingDef> deflist;

		// Token: 0x04000139 RID: 313
		private ThingDef selecteddef;

		// Token: 0x0400013A RID: 314
		private Texture2D icon;

		// Token: 0x0400013B RID: 315
		private Color col = Color.white;

		// Token: 0x0400013C RID: 316
		private string searchdeflabel;

		// Token: 0x0400013D RID: 317
		private string Cache;

		// Token: 0x0400013E RID: 318
		private bool AllowRender;
	}
}
