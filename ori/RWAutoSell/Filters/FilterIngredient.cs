using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using RWAutoSell.Mergers;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000042 RID: 66
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterIngredient : IBaseFilter, IRequiredMerger
	{
		// Token: 0x1700007E RID: 126
		// (get) Token: 0x06000230 RID: 560 RVA: 0x000146B1 File Offset: 0x000128B1
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterIngredient".Translate();
			}
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x06000231 RID: 561 RVA: 0x000146C2 File Offset: 0x000128C2
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagFood"
		};

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x06000232 RID: 562 RVA: 0x000146CA File Offset: 0x000128CA
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FIng;
			}
		}

		// Token: 0x06000233 RID: 563 RVA: 0x000146D4 File Offset: 0x000128D4
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.AnyThing.def.HasComp(typeof(CompIngredients)))
			{
				CompIngredients compIngredients = t.AnyThing.TryGetComp<CompIngredients>();
				if (compIngredients == null)
				{
					return false;
				}
				if (compIngredients.ingredients.Count == 0 && (Data.Count == 0 || Data.Contains("spc.nul")))
				{
					return true;
				}
				foreach (ThingDef thingDef in compIngredients.ingredients)
				{
					string item = "ing." + thingDef.defName;
					if (Data.Contains(item))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06000234 RID: 564 RVA: 0x00014798 File Offset: 0x00012998
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			this.box.DrawListBox(inrect);
		}

		// Token: 0x06000235 RID: 565 RVA: 0x000147A8 File Offset: 0x000129A8
		public void RenderStarted(List<string> Data, Map map)
		{
			this.box = new ASMultiBox<ThingDef>(30f, 4f, true, true);
			List<ThingDef> list = (from ThingDef el in DefDatabase<ThingDef>.AllDefs
			where el.HasComp(typeof(CompIngredients))
			select el).ToList<ThingDef>();
			HashSet<ThingDef> hashSet = new HashSet<ThingDef>();
			foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
			{
				ThingDef producedThingDef = recipeDef.ProducedThingDef;
				if (list.Contains(producedThingDef))
				{
					foreach (IngredientCount ingredientCount in recipeDef.ingredients)
					{
						if (ingredientCount.IsFixedIngredient)
						{
							hashSet.Add(ingredientCount.FixedIngredient);
						}
						else
						{
							foreach (ThingDef item in ingredientCount.filter.AllowedThingDefs)
							{
								hashSet.Add(item);
							}
						}
					}
				}
			}
			SelectedItem<ThingDef> selectedItem = new SelectedItem<ThingDef>();
			selectedItem.Item = null;
			selectedItem.label = "None".Translate();
			selectedItem.surpresslabelupdate = true;
			selectedItem.Selected = Data.Any((string x) => x == "spc.nul");
			SelectedItem<ThingDef> item2 = selectedItem;
			this.box.Add(item2);
			this.box.AddRange(from el in hashSet
			orderby el.defName != "Meat_Human" && el.defName != "Meat_Megaspider"
			select el);
			foreach (SelectedItem<ThingDef> selectedItem2 in this.box)
			{
				if (selectedItem2.Item != null && Data.Contains("ing." + selectedItem2.Item.defName))
				{
					selectedItem2.Selected = true;
				}
			}
		}

		// Token: 0x06000236 RID: 566 RVA: 0x000149FC File Offset: 0x00012BFC
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			if (this.box != null)
			{
				foreach (ThingDef thingDef in this.box.GetSelectedItems)
				{
					if (thingDef == null)
					{
						Data.Add("spc.nul");
					}
					else
					{
						Data.Add("ing." + thingDef.defName);
					}
				}
			}
			this.box = null;
		}

		// Token: 0x06000237 RID: 567 RVA: 0x00014A88 File Offset: 0x00012C88
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (t.AnyThing.def.HasComp(typeof(CompIngredients)))
			{
				CompIngredients compIngredients = t.AnyThing.TryGetComp<CompIngredients>();
				if (compIngredients != null && compIngredients.ingredients != null && compIngredients.ingredients.Count > 0)
				{
					foreach (ThingDef thingDef in compIngredients.ingredients)
					{
						list.Add("ing." + thingDef.defName);
					}
				}
			}
			return list;
		}

		// Token: 0x06000238 RID: 568 RVA: 0x00014B34 File Offset: 0x00012D34
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x06000239 RID: 569 RVA: 0x00014B37 File Offset: 0x00012D37
		public string RequiredEnabled()
		{
			return typeof(IngredientMerger).FullName;
		}

		// Token: 0x04000131 RID: 305
		private ASMultiBox<ThingDef> box;
	}
}
