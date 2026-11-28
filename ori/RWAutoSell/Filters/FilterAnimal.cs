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
	// Token: 0x02000038 RID: 56
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterAnimal : IBaseFilter, IWikiHelp
	{
		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001B1 RID: 433 RVA: 0x000107B2 File Offset: 0x0000E9B2
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FAnimal;
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001B2 RID: 434 RVA: 0x000107B9 File Offset: 0x0000E9B9
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterAnimal".Translate();
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x000107CC File Offset: 0x0000E9CC
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn == null || pawn.RaceProps == null || !pawn.RaceProps.Animal)
			{
				return false;
			}
			int intelligenceOrder = pawn.RaceProps.trainability.intelligenceOrder;
			if (Data.Count<string>() == 0)
			{
				return false;
			}
			bool result = true;
			foreach (string text in Data)
			{
				if (text.Substring(0, 4) == "prg.")
				{
					if (pawn.health.hediffSet.HasHediff(HediffDefOf.Pregnant, true) == (text.Substring(4) == "false"))
					{
						result = false;
						break;
					}
				}
				else if (text.Substring(0, 4) == "htr.")
				{
					TrainableDef namedSilentFail = DefDatabase<TrainableDef>.GetNamedSilentFail(text.Substring(4));
					if (namedSilentFail != null)
					{
						if (namedSilentFail.requiredTrainability.intelligenceOrder > intelligenceOrder)
						{
							return false;
						}
						if (pawn.training == null || !pawn.training.HasLearned(namedSilentFail))
						{
							return false;
						}
					}
				}
				else if (text.Substring(0, 4) == "ctr.")
				{
					string text2 = text.Substring(4);
					if (!(text2 == "None"))
					{
						TrainabilityDef namedSilentFail2 = DefDatabase<TrainabilityDef>.GetNamedSilentFail(text2);
						if (namedSilentFail2 != null && namedSilentFail2.intelligenceOrder > intelligenceOrder)
						{
							return false;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00010964 File Offset: 0x0000EB64
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Rect inRect = new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 8f, 50f);
				ASTable astable = new ASTable(inRect, 2, 2, 2f, 0f, true);
				Widgets.CheckboxLabeled(astable.GetRectangle(0, 0, 1, -1, false), "RWAutoSell.FilterPregnant".Translate(), ref this.CheckPreg, false, null, null, false, false);
				if (this.CheckPreg)
				{
					if (Widgets.RadioButtonLabeled(astable.GetRectangle(0, 1, -1, -1, false), "Yes".Translate(), this.Preg, false))
					{
						this.Preg = true;
					}
					if (Widgets.RadioButtonLabeled(astable.GetRectangle(1, 1, -1, -1, false), "No".Translate(), !this.Preg, false))
					{
						this.Preg = false;
					}
				}
				Rect rect = new Rect(inrect.x + 4f, inRect.yMax + 4f, inRect.width, 25f);
				Widgets.Label(rect, "RWAutoSell.FilterIntelligence".Translate());
				Rect outrect = new Rect(inRect.x, rect.yMax, rect.width, (inrect.height - (rect.yMax - inrect.y)) / 2f - 25f);
				this.IntelligenceList.DrawListBox(outrect);
				Rect rect2 = new Rect(inrect.x + 4f, outrect.yMax + 4f, inRect.width, 25f);
				Widgets.Label(rect2, "RWAutoSell.FilterTraining".Translate());
				Rect outrect2 = new Rect(inRect.x, rect2.yMax, rect2.width, outrect.height);
				this.TrainedList.DrawListBox(outrect2);
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00010B4C File Offset: 0x0000ED4C
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.Animal)
			{
				list.Add("ctr." + pawn.RaceProps.trainability.defName);
				int intelligenceOrder = pawn.RaceProps.trainability.intelligenceOrder;
				foreach (TrainableDef trainableDef in DefDatabase<TrainableDef>.AllDefs)
				{
					if (trainableDef.requiredTrainability.intelligenceOrder <= intelligenceOrder && pawn.training.HasLearned(trainableDef))
					{
						list.Add("htr." + trainableDef.defName);
					}
				}
				if (pawn.health.hediffSet.HasHediff(HediffDefOf.Pregnant, true))
				{
					list.Add("prg.true");
				}
				else
				{
					list.Add("prg.false");
				}
			}
			return list;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x00010C54 File Offset: 0x0000EE54
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001B7 RID: 439 RVA: 0x00010C57 File Offset: 0x0000EE57
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Animal-Filter";
			}
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00010C60 File Offset: 0x0000EE60
		public void RenderStarted(List<string> Data, Map map)
		{
			this.IntelligenceList = new ASMultiBox<TrainabilityDef>(25f, 4f, false, false);
			this.TrainedList = new ASMultiBox<TrainableDef>(25f, 4f, true, true);
			HashSet<TrainableDef> hashSet = new HashSet<TrainableDef>();
			TrainabilityDef trainabilityDef = TrainabilityDefOf.None;
			foreach (string text in Data)
			{
				if (text.Substring(0, 4) == "prg.")
				{
					this.CheckPreg = true;
					if (text.Substring(4) == "false")
					{
						this.Preg = false;
					}
					else
					{
						this.Preg = true;
					}
				}
				else if (text.Substring(0, 4) == "htr.")
				{
					TrainableDef namedSilentFail = DefDatabase<TrainableDef>.GetNamedSilentFail(text.Substring(4));
					if (namedSilentFail != null)
					{
						hashSet.Add(namedSilentFail);
					}
				}
				else if (text.Substring(0, 4) == "ctr.")
				{
					TrainabilityDef namedSilentFail2 = DefDatabase<TrainabilityDef>.GetNamedSilentFail(text.Substring(4));
					if (namedSilentFail2 != null)
					{
						trainabilityDef = namedSilentFail2;
					}
				}
			}
			if (hashSet.Count > 0)
			{
				hashSet.Max((TrainableDef x) => x.requiredTrainability.intelligenceOrder);
			}
			foreach (TrainabilityDef trainabilityDef2 in from el in DefDatabase<TrainabilityDef>.AllDefs
			orderby el.intelligenceOrder
			select el)
			{
				if (trainabilityDef == null || trainabilityDef2 == trainabilityDef)
				{
					this.IntelligenceList.Add(new SelectedItem<TrainabilityDef>
					{
						Item = trainabilityDef2,
						Selected = true
					});
				}
				else
				{
					this.IntelligenceList.Add(trainabilityDef2);
				}
			}
			foreach (TrainableDef trainableDef in from el in DefDatabase<TrainableDef>.AllDefs
			where !el.specialTrainable
			orderby el.requiredTrainability.intelligenceOrder
			select el)
			{
				SelectedItem<TrainableDef> selectedItem = new SelectedItem<TrainableDef>
				{
					Item = trainableDef,
					Selected = false
				};
				Log.Message(trainableDef.icon);
				try
				{
					if (trainableDef.icon != string.Empty)
					{
						selectedItem.Icon = trainableDef.Icon;
					}
				}
				catch
				{
				}
				if (hashSet.Contains(trainableDef))
				{
					selectedItem.Selected = true;
				}
				else
				{
					selectedItem.Selected = false;
				}
				this.TrainedList.Add(selectedItem);
			}
			this.AllowRender = true;
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00010F50 File Offset: 0x0000F150
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			if (this.CheckPreg)
			{
				if (this.Preg)
				{
					Data.Add("prg.true");
				}
				else
				{
					Data.Add("prg.false");
				}
			}
			foreach (TrainableDef trainableDef in this.TrainedList.GetSelectedItems)
			{
				Data.Add("htr." + trainableDef.defName);
			}
			if (this.IntelligenceList.GetSelected != null)
			{
				Data.Add("ctr." + this.IntelligenceList.GetSelected.defName);
			}
			this.AllowRender = false;
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001BA RID: 442 RVA: 0x0001101C File Offset: 0x0000F21C
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals"
		};

		// Token: 0x040000FA RID: 250
		private bool CheckPreg;

		// Token: 0x040000FB RID: 251
		private bool Preg;

		// Token: 0x040000FC RID: 252
		private bool AllowRender;

		// Token: 0x040000FD RID: 253
		private ASListBox<TrainabilityDef> IntelligenceList;

		// Token: 0x040000FE RID: 254
		private ASMultiBox<TrainableDef> TrainedList;
	}
}
