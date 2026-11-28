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
	// Token: 0x02000021 RID: 33
	[NullableContext(1)]
	[Nullable(0)]
	public class MainTabWindow_AutoSell : MainTabWindow
	{
		// Token: 0x06000108 RID: 264 RVA: 0x0000C588 File Offset: 0x0000A788
		public override void PreOpen()
		{
			base.PreOpen();
			if (ASLibMod.Initialised)
			{
				this.InitDia();
				return;
			}
			this.waitinit = true;
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000C5A8 File Offset: 0x0000A7A8
		private void InitDia()
		{
			List<ASTab> list = new List<ASTab>();
			PlanetTile planetTile = -1;
			foreach (MapParent mapParent in Find.WorldObjects.MapParents)
			{
				if (mapParent.HasMap && ((mapParent.Faction != null && mapParent.Faction.IsPlayer) || mapParent.Faction == null))
				{
					if (planetTile == -1)
					{
						planetTile = mapParent.Map.Tile;
					}
					if (Find.CurrentMap == mapParent.Map)
					{
						planetTile = mapParent.Map.Tile;
					}
					list.Add(new ASTab(mapParent.LabelCap, mapParent.Map.Tile, mapParent.Map.Tile.tileId, ""));
				}
			}
			this.Colony = new ASTabBar(list.ToArray());
			List<Type> getBaseRules = ASLibMod.GetSingleton.GetBaseRules;
			new List<ASTab>();
			SortedList<int, ASTab> sortedList = new SortedList<int, ASTab>();
			for (int i = 0; i < getBaseRules.Count; i++)
			{
				IRuleComp ruleComp = Find.CurrentMap.GetComponent(getBaseRules[i]) as IRuleComp;
				sortedList.Add(ruleComp.Priority, new ASTab(ruleComp.TabName, i, ""));
			}
			this.RuleTypeBar = new ASTabBar(sortedList.Values.ToArray<ASTab>());
			this.RuleTypeBar.OnClick += this.ChangeList;
			this.Colony.OnClick += this.RefreshCurrent;
			if (planetTile != -1)
			{
				this.Colony.ForceSelectedByTile(planetTile);
			}
			this.RefreshCurrent(null, null);
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000C780 File Offset: 0x0000A980
		public override void PostClose()
		{
			this.RuleTypeBar.OnClick -= this.ChangeList;
			this.Colony.OnClick -= this.RefreshCurrent;
			base.PostClose();
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000C7B8 File Offset: 0x0000A9B8
		internal void ChangeList(ASTabBar sender, ASTab Tab)
		{
			if (this.Colony.CurrentIndex != -1)
			{
				Map map = this.MapFromCurrentTile();
				if (map != null)
				{
					IRuleComp comp = map.GetComponent(ASLibMod.GetSingleton.GetBaseRules[this.RuleTypeBar.CurrentIndex]) as IRuleComp;
					this.MainList = new ASMainTabList(comp, map);
				}
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000C810 File Offset: 0x0000AA10
		private void RefreshCurrent(ASTabBar sender, ASTab Tab)
		{
			if (this.Colony.CurrentIndex != -1)
			{
				this.ChangeList(null, null);
			}
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000C828 File Offset: 0x0000AA28
		public float TopButtonBar(Rect inRect)
		{
			Rect rect = new Rect(inRect.x + 10f, inRect.y, MainTabWindow_AutoSell.ButtonSize.x, MainTabWindow_AutoSell.ButtonSize.y);
			Rect butRect = new Rect(inRect.x + 10f, rect.yMax + 10f, 30f, 30f);
			Rect rect2 = new Rect(rect.xMax + 10f, rect.y, MainTabWindow_AutoSell.ButtonSize.x, MainTabWindow_AutoSell.ButtonSize.y);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Map map = this.MapFromCurrentTile();
			if (map == null)
			{
				return rect.yMax;
			}
			IRuleComp ruleComp = map.GetComponent(ASLibMod.GetSingleton.GetBaseRules[this.RuleTypeBar.CurrentIndex]) as IRuleComp;
			if (ruleComp != null)
			{
				if (Widgets.ButtonText(rect, "RWAutoSell.CreateRuleShort".Translate(), true, true, true, null))
				{
					ASLibMod.SetActive<IRuleComp>(ruleComp);
					Find.WindowStack.Add(new ASRuleNodesDialog(map, ruleComp, false));
				}
				if (ruleComp.AdditonalName != string.Empty && Widgets.ButtonText(rect2, ruleComp.AdditonalName, true, true, true, null))
				{
					Find.WindowStack.Add(ruleComp.AdditionalWindow());
				}
				Rect rect3 = new Rect(inRect.xMax - MainTabWindow_AutoSell.ButtonSize.x, inRect.y, MainTabWindow_AutoSell.ButtonSize.x, MainTabWindow_AutoSell.ButtonSize.y);
				Rect rect4 = new Rect(rect3.x - MainTabWindow_AutoSell.ButtonSize.x - 10f, inRect.y, MainTabWindow_AutoSell.ButtonSize.x, MainTabWindow_AutoSell.ButtonSize.y);
				if (ruleComp.OptionName != string.Empty && Widgets.ButtonText(rect3, ruleComp.OptionName, true, true, true, null))
				{
					Window window = ruleComp.OptionsWindow();
					if (window != null)
					{
						Find.WindowStack.Add(window);
					}
				}
				if (Widgets.ButtonText(rect4, "RWAutoSell.Import".Translate(), true, true, true, null))
				{
					Find.WindowStack.Add(new ASDialog_Import(ruleComp));
				}
				if (ruleComp.ChainsAllowed && Widgets.ButtonImage(butRect, ASTextures.Chain, true, null))
				{
					Find.WindowStack.Add(new ASRuleNodesDialog(map, ruleComp, true));
				}
			}
			return butRect.yMax;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000CAA7 File Offset: 0x0000ACA7
		private Map MapFromCurrentTile()
		{
			return Find.Maps.FirstOrDefault((Map x) => x.Tile == this.Colony.CurrentTile);
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000CAC0 File Offset: 0x0000ACC0
		public override void DoWindowContents(Rect inRect)
		{
			if (this.waitinit)
			{
				Widgets.Label(inRect, "Initialising");
				if (ASLibMod.Initialised)
				{
					this.InitDia();
					this.waitinit = false;
				}
				return;
			}
			Rect inRect2 = new Rect(inRect.x, inRect.y, inRect.width, MainTabWindow_AutoSell.ButtonSize.y);
			if (this.Colony.CurrentIndex == -1)
			{
				return;
			}
			float num = this.Colony.DrawTabs(inRect2);
			Rect inRect3 = new Rect(inRect.x, num + 5f, inRect.width, MainTabWindow_AutoSell.ButtonSize.y);
			float num2 = this.RuleTypeBar.DrawTabs(inRect3);
			Rect inRect4 = new Rect(inRect.x, num2 + 5f, inRect.width - 10f, MainTabWindow_AutoSell.ButtonSize.y);
			float num3 = this.TopButtonBar(inRect4);
			Rect inrect = new Rect(0f, num3 + 10f, inRect.width, inRect.height - num3 - 10f);
			Map map = this.MapFromCurrentTile();
			if (map != null)
			{
				IRuleComp comp = map.GetComponent(ASLibMod.GetSingleton.GetBaseRules[this.RuleTypeBar.CurrentIndex]) as IRuleComp;
				this.DoTab(inrect, map, comp);
			}
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000CC0C File Offset: 0x0000AE0C
		private void DoTab(Rect inrect, Map map, IRuleComp comp)
		{
			Widgets.DrawLineHorizontal(15f, inrect.y, inrect.width - 30f);
			Rect inrect2 = new Rect(15f, inrect.y + 5f, inrect.width - 30f, inrect.height - 5f);
			this.MainList.Draw(inrect2);
		}

		// Token: 0x040000C5 RID: 197
		private ASMainTabList MainList;

		// Token: 0x040000C6 RID: 198
		private ASTabBar Colony = new ASTabBar(Array.Empty<ASTab>());

		// Token: 0x040000C7 RID: 199
		private ASTabBar RuleTypeBar = new ASTabBar(Array.Empty<ASTab>());

		// Token: 0x040000C8 RID: 200
		private Vector2 RuleListScrollPosition = Vector2.zero;

		// Token: 0x040000C9 RID: 201
		private const float DelButW = 60f;

		// Token: 0x040000CA RID: 202
		private const float MoveButW = 20f;

		// Token: 0x040000CB RID: 203
		private bool waitinit;

		// Token: 0x040000CC RID: 204
		public static readonly Vector2 ButtonSize = new Vector2(160f, 30f);
	}
}
