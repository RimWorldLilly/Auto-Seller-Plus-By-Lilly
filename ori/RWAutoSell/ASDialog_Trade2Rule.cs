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

namespace RWAutoSell
{
	// Token: 0x0200001F RID: 31
	[NullableContext(1)]
	[Nullable(0)]
	internal class ASDialog_Trade2Rule : Window
	{
		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000A0FF File Offset: 0x000082FF
		public override Vector2 InitialSize
		{
			get
			{
				return this.InitialSize_;
			}
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000A107 File Offset: 0x00008307
		public override void PreOpen()
		{
			base.PreOpen();
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000A110 File Offset: 0x00008310
		public ASDialog_Trade2Rule(Tradeable Item) : base(null)
		{
			this.TradeItem = Item;
			Text.Font = GameFont.Medium;
			this.titlesize = Text.CalcSize("RWAutoSell.QuickCreate".Translate()).x / 2f;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseX = true;
			this.closeOnCancel = true;
			this.doCloseButton = true;
			if (TradeSession.Active)
			{
				Caravan caravan = TradeSession.playerNegotiator.GetCaravan();
				if (caravan == null)
				{
					this.Map = TradeSession.playerNegotiator.Map;
				}
				else
				{
					new List<Thing>();
					ASShoppingList asshoppingList = CaravanInventoryUtility.AllInventoryItems(caravan).FirstOrDefault((Thing x) => x is ASShoppingList) as ASShoppingList;
					if (asshoppingList != null)
					{
						Map origin = asshoppingList.Origin;
					}
				}
				this.Containers = new ASMultiBox<FilterContainer>(25f, 4f, true, true);
				foreach (IBaseFilter baseFilter in ASLibMod.GetSingleton.GetBaseFilters)
				{
					List<string> list = new List<string>();
					try
					{
						list = baseFilter.DataFromTradeable(this.TradeItem);
					}
					catch
					{
						Log.Message("Transferable to Data error in " + baseFilter.Label + " - this message is harmless");
					}
					if (list != null && list.Count<string>() != 0)
					{
						FilterContainer filterContainer = new FilterContainer(baseFilter.GetType())
						{
							FilterData = list
						};
						bool selected = false;
						if (baseFilter.GetType().FullName == "RWAutoSell.Filters.FilterCat" || baseFilter.GetType().FullName == "RWAutoSell.Filters.FilterPawnKind")
						{
							selected = true;
						}
						this.Containers.Add(new SelectedItem<FilterContainer>
						{
							Item = filterContainer,
							label = baseFilter.Label,
							Icon = filterContainer.Icon,
							Selected = selected,
							surpresslabelupdate = true
						});
					}
				}
			}
			if (this.Map != null)
			{
				this.Comp = ASMapComp.GetSingleton(this.Map);
				this.Matched = new ASListBox<ASRule>(25f, 4f, false, false);
				foreach (ASRule asrule in this.Comp.Rules)
				{
					bool flag;
					if (asrule.Nodes[0].RootNode.CheckGroup(this.TradeItem, out flag) && flag)
					{
						this.Matched.Add(asrule);
					}
				}
				this.Matched.OnClick += new ASListBox<ASRule>.ListItemHandler(this.RuleEdit);
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000A3FC File Offset: 0x000085FC
		public override void PreClose()
		{
			base.PreClose();
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000A404 File Offset: 0x00008604
		private void RuleEdit(object sender, GUIListEventArgs e)
		{
			if (this.Containers.GetSelectedItems.Count<FilterContainer>() > 0)
			{
				if (this.Matched[e.index].Item == null)
				{
					ASRule asrule = new ASRule();
					foreach (FilterContainer item in this.Containers.GetSelectedItems)
					{
						asrule.Nodes[0].RootNode.Filters.Add(item);
					}
					Find.UIRoot.windows.Add(new ASRuleNodesDialog(this.Map, this.Comp, asrule, true));
					return;
				}
				Find.UIRoot.windows.Add(new ASRuleNodesDialog(this.Map, this.Comp, this.Matched[e.index].Item, false));
				this.Close(true);
			}
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000A508 File Offset: 0x00008708
		public override void DoWindowContents(Rect inRect)
		{
			GUI.color = Color.white;
			Text.Font = GameFont.Medium;
			float num = inRect.width / 2f;
			Widgets.Label(new Rect(num - this.titlesize, 5f, this.titlesize * 3f, 40f), "RWAutoSell.QuickCreate".Translate());
			Rect rect = new Rect(5f, 30f, num - 10f, 30f);
			Rect outrect = new Rect(5f, 60f, num - 10f, inRect.height - 130f);
			Text.Font = GameFont.Small;
			Widgets.Label(rect, "RWAutoSell.QuickExisting".Translate());
			this.Matched.DrawListBox(outrect);
			Rect rect2 = new Rect(num + 5f, 30f, num - 10f, 30f);
			Rect outrect2 = new Rect(num + 5f, 60f, num - 10f, inRect.height - 130f);
			if (Widgets.ButtonText(rect2, "RWAutoSell.QuickNew".Translate(), true, true, true, null))
			{
				ASRule original = new ASRule(this.Containers.GetSelectedItems, this.TradeItem.AnyThing.def.LabelCap);
				Find.UIRoot.windows.Add(new ASRuleNodesDialog(this.Map, this.Comp, original, true));
				this.Close(true);
			}
			this.Containers.DrawListBox(outrect2);
		}

		// Token: 0x0400009B RID: 155
		private readonly Map Map;

		// Token: 0x0400009C RID: 156
		private ASMapComp Comp;

		// Token: 0x0400009D RID: 157
		private ASListBox<ASRule> Matched;

		// Token: 0x0400009E RID: 158
		private ASMultiBox<FilterContainer> Containers;

		// Token: 0x0400009F RID: 159
		private Vector2 scroll = Vector2.zero;

		// Token: 0x040000A0 RID: 160
		private readonly float titlesize;

		// Token: 0x040000A1 RID: 161
		private Tradeable TradeItem;

		// Token: 0x040000A2 RID: 162
		private Vector2 InitialSize_ = new Vector2(700f, 600f);
	}
}
