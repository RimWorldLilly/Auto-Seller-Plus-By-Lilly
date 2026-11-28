using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000020 RID: 32
	[NullableContext(1)]
	[Nullable(0)]
	public class ASRuleNodesDialog : Window, INodeDialog
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000EC RID: 236 RVA: 0x0000A68E File Offset: 0x0000888E
		// (set) Token: 0x060000ED RID: 237 RVA: 0x0000A696 File Offset: 0x00008896
		public FilterGroup Selected { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000EE RID: 238 RVA: 0x0000A69F File Offset: 0x0000889F
		// (set) Token: 0x060000EF RID: 239 RVA: 0x0000A6A8 File Offset: 0x000088A8
		private FilterContainer SelectedContainer
		{
			get
			{
				return this.SelectedContainer_;
			}
			set
			{
				if (this.SelectedContainer_ != value)
				{
					if (this.SelectedContainer_ != null && this.SelectedContainer_.HasFilter)
					{
						this.SelectedContainer_.RenderStop(this.map);
					}
					this.SelectedContainer_ = value;
					if (this.SelectedContainer_ != null && this.SelectedContainer_.HasFilter)
					{
						this.SelectedContainer_.RenderStart(this.map);
					}
				}
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000F0 RID: 240 RVA: 0x0000A711 File Offset: 0x00008911
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2((float)UI.screenWidth * 0.8f, (float)UI.screenHeight * 0.6f);
			}
		}

		// Token: 0x060000F1 RID: 241 RVA: 0x0000A730 File Offset: 0x00008930
		public ASRuleNodesDialog(Map map) : base(null)
		{
			this.closeOnAccept = false;
			this.closeOnCancel = false;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.map = map;
		}

		// Token: 0x060000F2 RID: 242 RVA: 0x0000A7E4 File Offset: 0x000089E4
		public ASRuleNodesDialog(Map map, IRuleComp Comp, bool AsChain = false) : this(map)
		{
			this.ruleComp = Comp;
			this.EditRule = (IRule)Activator.CreateInstance(Comp.GetCompType());
			if (Comp.ChainsAllowed)
			{
				this.EditRule.IsChain = AsChain;
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x0000A81E File Offset: 0x00008A1E
		public ASRuleNodesDialog(Map map, IRuleComp Comp, IRule FromChainRule, List<IRule> prechain, IRule Original, bool TreatAsNew = false) : this(map, Comp, Original, TreatAsNew)
		{
			this.ChainedTo = FromChainRule;
			this.PreChain = prechain;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x0000A83B File Offset: 0x00008A3B
		public ASRuleNodesDialog(Map map, IRuleComp Comp, IRule Original, bool TreatAsNew = false) : this(map)
		{
			this.ruleComp = Comp;
			if (!TreatAsNew)
			{
				this.OriginalRule = Original;
			}
			this.EditRule = Original.DeepCopy();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x0000A862 File Offset: 0x00008A62
		public void RegisterChange()
		{
			this.History.Push(this.EditRule.DeepCopy(), this.Selected, this.SelectedContainer, this.NodeTypesBar.CurrentIndex, this.NodePropertyBar.CurrentIndex);
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x0000A89C File Offset: 0x00008A9C
		public override void PreOpen()
		{
			base.PreOpen();
			ASLibMod.nodeDialog = this;
			this.ClickTracker = new ASDblTap();
			this.EditRule.SortAll();
			this.SelectedRoot = this.EditRule.Nodes[0].RootNode;
			this.Selected = this.EditRule.Nodes[0].RootNode;
			this.backcolor = ASMod.GetSettings().NodeWindowbackcolor;
			ASTab[] array = new ASTab[this.EditRule.Nodes.Count];
			for (int i = 0; i < this.EditRule.Nodes.Count; i++)
			{
				array[i] = new ASTab(this.EditRule.Nodes[i].Label, i, "");
			}
			if (this.EditRule != null)
			{
				IAdvanced advanced = this.EditRule as IAdvanced;
				if (advanced != null && !advanced.ShowAdvanced && advanced.AdvancedNodes() != null)
				{
					foreach (int num in advanced.AdvancedNodes())
					{
						array[num].Visible = false;
					}
				}
			}
			this.NodeTypesBar = new ASTabBar(array);
			List<ASTab> list = new List<ASTab>
			{
				new ASTab("RWAutoSell.PropGroup".Translate(), 0, "RWAutoSell.DescPropGroup".Translate()),
				new ASTab("RWAutoSell.PropTest".Translate(), 1, "RWAutoSell.DescPropTest".Translate()),
				new ASTab("RWAutoSell.PropFilter".Translate(), 2, "RWAutoSell.DescPropFilter".Translate())
			};
			if (ASMod.GetSettings().devTestAdditional)
			{
				list.Add(new ASTab("Raw", 3, ""));
			}
			this.NodePropertyBar = new ASTabBar(list.ToArray());
			this.NodeTypesBar.OnClick += this.RefreshPage;
			this.History = new UndoList();
			this.RegisterChange();
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x0000AAA8 File Offset: 0x00008CA8
		public override void PreClose()
		{
			if (this.SelectedContainer_ != null)
			{
				this.SelectedContainer_.RenderStop(this.map);
			}
			this.EditRule.VoidAllInput();
			this.NodeTypesBar.OnClick -= this.RefreshPage;
			base.PreClose();
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x0000AAF8 File Offset: 0x00008CF8
		public override void PostClose()
		{
			if (this.Accept)
			{
				if (this.ChainedTo == null)
				{
					if (this.OriginalRule == null)
					{
						this.ruleComp.Add(this.EditRule);
					}
					else
					{
						this.ruleComp.Replace(this.OriginalRule, this.EditRule);
					}
				}
				else if (this.OriginalRule == null)
				{
					this.ChainedTo.ChainedAdd(this.EditRule);
				}
				else
				{
					this.ChainedTo.ChainedReplace(this.OriginalRule, this.EditRule);
				}
				ASLibMod.GuiRefresh = true;
			}
			ASLibMod.nodeDialog = null;
			base.PostClose();
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x0000AB8D File Offset: 0x00008D8D
		public void CallbackClick(FilterGroup node)
		{
			this.Selected = node;
			this.SelectedContainer = null;
			this.NodePropertyBar.ForceSelectedByValue(0);
			this.SelectedRoot.SortPosition();
		}

		// Token: 0x060000FA RID: 250 RVA: 0x0000ABB4 File Offset: 0x00008DB4
		private void RefreshPage(ASTabBar sender, ASTab Tab)
		{
			if (this.SelectedRoot != null)
			{
				this.SelectedRoot.VoidInput();
			}
			this.Selected = null;
			this.SelectedRoot = this.EditRule.Nodes[Tab.Index].RootNode;
			this.SelectedRoot.SortPosition();
			this.RefreshSearch = true;
			this.scroll = Vector2.zero;
			this.propscroll = Vector2.zero;
			this.ScrollRaw = Vector2.zero;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x0000AC30 File Offset: 0x00008E30
		public override void DoWindowContents(Rect inRect)
		{
			this.ClickTracker.Ticker();
			float num = inRect.width - Mathf.Floor(inRect.width / 3f);
			this.PropertyWindow(new Rect(num + 4f, 0f, inRect.width - (num + 8f), inRect.height));
			float num2 = this.TopBar(new Rect(inRect.x, inRect.y, num, inRect.height - 60f));
			Widgets.DrawLineVertical(num, inRect.y, inRect.height);
			Rect inrect = new Rect(0f, num2, num, inRect.height - num2 - 60f);
			this.NodeWindow(inrect);
			Widgets.DrawLineVertical(num, inRect.y, num2);
			Rect rect = new Rect(num - 20f - this.ButtonSize.x * 2f, inRect.height - 55f, this.ButtonSize.x, this.ButtonSize.y);
			if (Widgets.ButtonText(rect, "AcceptButton".Translate(), true, true, this.EditRule.CheckEdit(), null))
			{
				this.Accept = true;
				this.Close(true);
				Event.current.Use();
			}
			if (Widgets.ButtonText(new Rect(num - 10f - this.ButtonSize.x, rect.y, this.ButtonSize.x, this.ButtonSize.y), "CancelButton".Translate(), true, true, true, null))
			{
				this.Close(true);
				Event.current.Use();
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x0000ADF0 File Offset: 0x00008FF0
		private void DoAlternate(int i, Rect inrect, float rowheight)
		{
			Rect rect = new Rect(0f, (float)i * rowheight, inrect.width, rowheight);
			if (i % 2 == 1)
			{
				Widgets.DrawLightHighlight(rect);
			}
			Widgets.DrawHighlightIfMouseover(rect);
		}

		// Token: 0x060000FD RID: 253 RVA: 0x0000AE28 File Offset: 0x00009028
		private bool PawnMod(Pawn p)
		{
			return (this.showcolonists && p.Faction == Faction.OfPlayer) || (this.showprisoners && p.IsPrisonerOfColony) || (this.showfriendlies && p.Faction != null && p.Faction != Faction.OfPlayer && !p.Faction.HostileTo(Faction.OfPlayer)) || (this.showenemies && p.Faction != null && p.Faction.HostileTo(Faction.OfPlayer)) || (this.showwild && p.Faction == null && p.AnimalOrWildMan());
		}

		// Token: 0x060000FE RID: 254 RVA: 0x0000AEC8 File Offset: 0x000090C8
		private void GroupDetails(Rect inrect)
		{
			float num = (float)(this.Selected.Groups.Count + this.Selected.Filters.Count + 2) * 22f;
			Rect rect = new Rect(0f, 0f, inrect.width - 5f, (num > inrect.height) ? num : inrect.height);
			int num2 = 0;
			Widgets.BeginScrollView(inrect, ref this.propscroll, rect, true);
			if (this.Selected.Parent != null)
			{
				Rect rect2 = new Rect(35f, (float)num2 * 22f, rect.width - 35f, 22f);
				this.DoAlternate(num2, rect, 22f);
				Widgets.DrawTextureFitted(new Rect(0f, (float)num2 * 22f, 35f, 22f), ASTextures.Group, 1f, 1f);
				Widgets.Label(rect2, "..");
				if (Mouse.IsOver(rect2))
				{
					TooltipHandler.TipRegion(rect2, new TipSignal("RWAutoSell.DescGroupNavigate".Translate()));
				}
				if (Widgets.ButtonInvisible(rect2, true) && this.ClickTracker.Click())
				{
					this.CallbackClick(this.Selected.Parent);
					this.indexsel = -1;
				}
				num2++;
			}
			int i = 0;
			while (i < this.Selected.Groups.Count)
			{
				Rect rect3 = new Rect(35f, (float)num2 * 22f, rect.width - 35f, 22f);
				this.DoAlternate(num2, rect, 22f);
				Widgets.DrawTextureFitted(new Rect(0f, (float)num2 * 22f, 35f, 22f), ASTextures.Group, 1f, 1f);
				Widgets.Label(rect3, this.Selected.Groups[i].NodeLabel);
				if (i == this.indexsel && this.groupsel)
				{
					Widgets.DrawHighlight(rect3);
				}
				if (Mouse.IsOver(rect3) && i == this.indexsel && this.groupsel)
				{
					TooltipHandler.TipRegion(rect3, new TipSignal("RWAutoSell.DescGroupSelect".Translate()));
				}
				if (Mouse.IsOver(rect3) && Event.current.isMouse && Event.current.type == EventType.mouseUp && Event.current.button == 1)
				{
					FilterGroup fg = this.Selected.Groups[i];
					Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>
					{
						new FloatMenuOption("RWAutoSell.DeleteGroup".Translate(), delegate()
						{
							this.RegisterChange();
							this.Selected.Groups.Remove(fg);
						}, MenuOptionPriority.Low, null, null, 0f, null, null, true, 0)
					}));
					Event.current.Use();
				}
				else if (Widgets.ButtonInvisible(rect3, true))
				{
					if (this.ClickTracker.Click())
					{
						this.CallbackClick(this.Selected.Groups[i]);
						this.indexsel = -1;
						break;
					}
					this.groupsel = true;
					this.indexsel = i;
				}
				i++;
				num2++;
			}
			Rect rect4 = new Rect(35f, (float)num2 * 22f, rect.width - 35f, 22f);
			this.DoAlternate(num2, rect, 22f);
			Widgets.Label(rect4, "RWAutoSell.AddGroup".Translate());
			if (Widgets.ButtonInvisible(rect4, true) && this.ClickTracker.Click())
			{
				this.RegisterChange();
				this.Selected.AddGroup();
			}
			num2++;
			int j = 0;
			while (j < this.Selected.Filters.Count)
			{
				Rect rect5 = new Rect(35f, (float)num2 * 22f, rect.width - 35f, 22f);
				this.DoAlternate(num2, rect, 22f);
				Color color = GUI.color;
				if (Mouse.IsOver(rect5) && Event.current.isMouse && Event.current.type == EventType.mouseUp && Event.current.button == 1)
				{
					FilterContainer fc = this.Selected.Filters[j];
					Find.WindowStack.Add(new FloatMenu(new List<FloatMenuOption>
					{
						new FloatMenuOption("RWAutoSell.DeleteFilter".Translate(), delegate()
						{
							this.RegisterChange();
							this.Selected.Filters.Remove(fc);
						}, MenuOptionPriority.Low, null, null, 0f, null, null, true, 0)
					}));
					Event.current.Use();
				}
				if (this.Selected.Filters[j].HasFilter)
				{
					if (this.Selected.Filters[j].IsActive)
					{
						GUI.color = Color.white;
					}
					else
					{
						GUI.color = Color.grey;
					}
					IBaseFilter getFilter = this.Selected.Filters[j].GetFilter;
					if (getFilter != null)
					{
						Widgets.DrawTextureFitted(new Rect(0f, (float)num2 * 22f, 35f, 22f), getFilter.MiniGraphic, 1f, 1f);
						if (j == this.indexsel && !this.groupsel)
						{
							Widgets.DrawHighlight(rect5);
						}
						if (Mouse.IsOver(rect5) && j == this.indexsel && !this.groupsel)
						{
							TooltipHandler.TipRegion(rect5, new TipSignal("RWAutoSell.DescFilterSelect".Translate()));
						}
						Widgets.Label(rect5, getFilter.Label);
						if (Widgets.ButtonInvisible(rect5, true))
						{
							if (this.ClickTracker.Click())
							{
								this.SelectedContainer = this.Selected.Filters[j];
								this.NodePropertyBar.ForceSelectedByValue(2);
							}
							else
							{
								this.groupsel = false;
								this.indexsel = j;
							}
						}
					}
					else
					{
						GUI.color = Color.red;
						Widgets.Label(rect5, "RWAutoSell.Unknown".Translate());
					}
				}
				else
				{
					GUI.color = Color.red;
					Widgets.Label(rect5, "RWAutoSell.Unknown".Translate());
				}
				GUI.color = color;
				j++;
				num2++;
			}
			Rect rect6 = new Rect(35f, (float)num2 * 22f, rect.width - 35f, 22f);
			this.DoAlternate(num2, rect, 22f);
			Widgets.Label(rect6, "RWAutoSell.AddFilter".Translate());
			if (this.waitingforselect)
			{
				if (ASDialog_FilterSelect.FilterString != string.Empty)
				{
					this.RegisterChange();
					this.waitingforselect = false;
					this.Selected.AddFilter(ASLibMod.GetSingleton.GetFilter(ASDialog_FilterSelect.FilterString).GetType());
					ASDialog_FilterSelect.FilterString = string.Empty;
				}
				else if (!Find.WindowStack.IsOpen(typeof(ASDialog_FilterSelect)))
				{
					this.waitingforselect = false;
				}
			}
			else if (Widgets.ButtonInvisible(rect6, true) && this.ClickTracker.Click())
			{
				ASDialog_FilterSelect.FilterString = string.Empty;
				this.waitingforselect = true;
				Find.WindowStack.Add(new ASDialog_FilterSelect());
			}
			Widgets.EndScrollView();
			this.RefreshSearch = true;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0000B5F4 File Offset: 0x000097F4
		private void TestSection(Rect inrect)
		{
			ASRectMonitor asrectMonitor = new ASRectMonitor(inrect);
			if (ASMod.GetSettings().devTestAdditional)
			{
				ASTable astable = new ASTable(asrectMonitor.NextFromTop(50f).position, new Vector2(70f, 25f), 6, 2, 4f, 0f, false);
				Rect rectangle = astable.GetRectangle(0, 0, -1, -1, false);
				Rect rectangle2 = astable.GetRectangle(1, 0, -1, -1, false);
				Rect rectangle3 = astable.GetRectangle(2, 0, -1, -1, false);
				Rect rectangle4 = astable.GetRectangle(3, 0, -1, -1, false);
				Rect rectangle5 = astable.GetRectangle(4, 0, -1, -1, false);
				Rect rectangle6 = astable.GetRectangle(5, 0, -1, -1, false);
				Rect rectangle7 = astable.GetRectangle(0, 1, -1, -1, false);
				Rect rectangle8 = astable.GetRectangle(1, 1, -1, -1, false);
				Rect rectangle9 = astable.GetRectangle(2, 1, -1, -1, false);
				if (Widgets.ButtonText(rectangle, "Refresh", true, true, true, null))
				{
					this.RefreshSearch = true;
				}
				Color color = GUI.color;
				GUI.color = (this.showcolonists ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle2, "Colonists", true, true, true, null))
				{
					this.showcolonists = !this.showcolonists;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showprisoners ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle3, "Prisoners", true, true, true, null))
				{
					this.showprisoners = !this.showprisoners;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showfriendlies ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle4, "Friendlies", true, true, true, null))
				{
					this.showfriendlies = !this.showfriendlies;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showenemies ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle5, "Enemies", true, true, true, null))
				{
					this.showenemies = !this.showenemies;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showwild ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle6, "Wild", true, true, true, null))
				{
					this.showwild = !this.showwild;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showgene ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle7, "GeneBanks", true, true, true, null))
				{
					this.showgene = !this.showgene;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showbook ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle8, "Bookcases", true, true, true, null))
				{
					this.showbook = !this.showbook;
					this.RefreshSearch = true;
				}
				GUI.color = (this.showoutfits ? Color.white : Color.grey);
				if (Widgets.ButtonText(rectangle9, "Outfit Stands", true, true, true, null))
				{
					this.showoutfits = !this.showoutfits;
					this.RefreshSearch = true;
				}
				GUI.color = color;
			}
			Rect outRect = asrectMonitor.Remaining();
			if (this.RefreshSearch)
			{
				if (this.SelectedContainer != null)
				{
					this.SelectedContainer.RenderStop(this.map);
					this.SelectedContainer.RenderStart(this.map);
				}
				this.searchlist.Clear();
				List<TransferableOneWay> list = ASLibTransferUtility.MapTradables(this.map, true, TransferAsOneMode.PodsOrCaravanPacking, new Func<Pawn, bool>(this.PawnMod), this.showgene, this.showbook, this.showoutfits);
				if (this.PreChain != null && this.PreChain.Count > 0)
				{
					foreach (IRule rule in this.PreChain)
					{
						list = ASLibTransferUtility.SortList<TransferableOneWay>(list, this.EditRule);
					}
				}
				list = ASLibTransferUtility.SortList<TransferableOneWay>(list, this.EditRule);
				for (int i = 0; i < list.Count; i++)
				{
					bool flag;
					if (list[i].HasAnyThing && (this.SelectedRoot.CheckGroup(list[i], out flag) && flag))
					{
						this.searchlist.Add(new ASRuleNodesDialog.TransfColPair(list[i], Color.white));
					}
				}
				if (this.PreChain != null && this.PreChain.Count > 0)
				{
					foreach (IRule rule2 in this.PreChain)
					{
						if (rule2.Nodes[this.NodeTypesBar.CurrentIndex].RootNode.Groups.Count<FilterGroup>() != 0 || rule2.Nodes[this.NodeTypesBar.CurrentIndex].RootNode.Filters.Count<FilterContainer>() != 0)
						{
							for (int j = 0; j < this.searchlist.Count; j++)
							{
								bool flag2;
								if (!rule2.Nodes[this.NodeTypesBar.CurrentIndex].RootNode.CheckGroup(this.searchlist[j].Trans, out flag2) || !flag2)
								{
									this.searchlist[j].color = Color.red;
								}
							}
						}
					}
				}
				this.RefreshSearch = false;
				return;
			}
			Rect viewRect = new Rect(0f, 0f, outRect.width - 10f, 30f * (float)this.searchlist.Count + 30f);
			Widgets.BeginScrollView(outRect, ref this.ScrollSearch, viewRect, true);
			Text.Anchor = TextAnchor.MiddleLeft;
			for (int k = 0; k < this.searchlist.Count; k++)
			{
				GUI.BeginGroup(new Rect(0f, (float)k * 30f, viewRect.width, 30f));
				TransferableUIUtility.DrawTransferableInfo(this.searchlist[k].Trans, new Rect(0f, 0f, viewRect.width, 30f), this.searchlist[k].color);
				GUI.EndGroup();
			}
			Widgets.EndScrollView();
			GenUI.ResetLabelAlign();
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0000BC84 File Offset: 0x00009E84
		private void PropertyWindow(Rect inrect)
		{
			if (this.Selected != null)
			{
				float num = 25f;
				Rect rect = new Rect(inrect.x + 5f, 0f, inrect.width - 10f, num);
				this.Selected.NodeLabel = Widgets.TextField(rect, this.Selected.NodeLabel);
				if (Mouse.IsOver(rect))
				{
					TooltipHandler.TipRegion(rect, new TipSignal("RWAutoSell.DescGroupName".Translate()));
				}
				Rect inRect = new Rect(inrect.x, inrect.y + num + 5f, inrect.width, 30f);
				float num2 = this.NodePropertyBar.DrawTabs(inRect);
				Rect inrect2 = new Rect(inrect.x, num2, inrect.width, inrect.height - num2);
				if (this.NodePropertyBar.CurrentIndex == 0)
				{
					this.GroupDetails(inrect2);
					return;
				}
				if (this.NodePropertyBar.CurrentIndex == 1)
				{
					this.TestSection(inrect2);
					return;
				}
				if (this.NodePropertyBar.CurrentIndex == 2)
				{
					if (this.SelectedContainer != null && this.SelectedContainer.HasFilter)
					{
						this.SelectedContainer.DrawWidget(inrect2);
					}
					this.RefreshSearch = true;
					return;
				}
				if (this.NodePropertyBar.CurrentIndex == 3 && this.SelectedContainer != null && this.SelectedContainer.HasFilter)
				{
					this.DevRawView(inrect2);
				}
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000BDE8 File Offset: 0x00009FE8
		private void DevRawView(Rect inrect)
		{
			if (this.RefreshSearch)
			{
				if (this.SelectedContainer != null)
				{
					this.SelectedContainer.RenderStop(this.map);
					this.SelectedContainer.RenderStart(this.map);
				}
				this.RefreshSearch = false;
			}
			Rect viewRect = new Rect(0f, 0f, inrect.width - 16f, (float)(this.SelectedContainer.FilterData.Count * 25));
			Widgets.BeginScrollView(inrect, ref this.ScrollRaw, viewRect, true);
			for (int i = 0; i < this.SelectedContainer.FilterData.Count<string>(); i++)
			{
				bool flag = i % 2 != 0;
				Rect rect = new Rect(4f, (float)i * 25f, viewRect.width - 8f, 20f);
				if (!flag)
				{
					Widgets.DrawTextHighlight(rect, 4f, null);
				}
				Widgets.Label(rect, this.SelectedContainer.FilterData[i]);
				TooltipHandler.TipRegion(rect, new TipSignal(this.SelectedContainer.FilterData[i]));
			}
			Widgets.EndScrollView();
		}

		// Token: 0x06000102 RID: 258 RVA: 0x0000BF04 File Offset: 0x0000A104
		private void UndoRedoAction(ASRuleNodesDialog.UndoRedo deli)
		{
			FilterGroup selected = this.Selected;
			FilterContainer selectedContainer = this.SelectedContainer;
			int currentIndex = this.NodeTypesBar.CurrentIndex;
			int currentIndex2 = this.NodePropertyBar.CurrentIndex;
			this.EditRule = deli(this.EditRule, ref selected, ref selectedContainer, ref currentIndex, ref currentIndex2);
			if (this.NodeTypesBar.CurrentIndex != currentIndex)
			{
				this.NodeTypesBar.ForceSelectedByValue(currentIndex);
			}
			this.SelectedRoot = this.EditRule.Nodes[currentIndex].RootNode;
			this.SelectedRoot.SortPosition();
			this.RefreshSearch = true;
			if (this.NodePropertyBar.CurrentIndex != currentIndex2)
			{
				this.NodePropertyBar.ForceSelectedByValue(currentIndex2);
			}
			this.Selected = selected;
			this.SelectedContainer_ = selectedContainer;
			if (selectedContainer != null)
			{
				this.SelectedContainer_.GetFilter.RenderStarted(this.SelectedContainer_.FilterData, this.map);
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000BFE8 File Offset: 0x0000A1E8
		private void NodeWindow(Rect inrect)
		{
			float num = ((float)this.SelectedRoot.GetWidth(1) + 1f) * 100f;
			float num2 = this.SelectedRoot.GroupHeight();
			Rect rect = new Rect(0f, 0f, (num > inrect.width) ? num : inrect.width, (num2 > inrect.height) ? num2 : inrect.height);
			Widgets.BeginScrollView(inrect, ref this.scroll, rect, true);
			Widgets.DrawBoxSolid(rect, this.backcolor);
			this.SelectedRoot.DrawNode(this);
			Widgets.EndScrollView();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x0000C080 File Offset: 0x0000A280
		private float TopBar(Rect inrect)
		{
			float num = this.NodeTypesBar.DrawTabs(inrect);
			if (this.EditRule != null)
			{
				if ((this.ruleComp.ChainsAllowed || this.ruleComp.SortingAllowed || this.EditRule is IAdvanced) && Widgets.ButtonImage(new Rect(inrect.xMax - 60f, inrect.y, 60f, 60f), ASTextures.AdvMenu, true, null))
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					if (this.ruleComp.ChainsAllowed)
					{
						list.Add(new FloatMenuOption("RWAutoSell.SetAsNormal".Translate() + (this.EditRule.IsChain ? "" : " [x]"), delegate()
						{
							this.RegisterChange();
							this.EditRule.IsChain = false;
						}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						list.Add(new FloatMenuOption("RWAutoSell.SetAsChain".Translate() + (this.EditRule.IsChain ? " [x]" : ""), delegate()
						{
							this.RegisterChange();
							this.EditRule.IsChain = true;
						}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
					}
					if (this.ruleComp.SortingAllowed)
					{
						list.Add(new FloatMenuOption("RWAutoSell.SelectSorter".Translate((this.EditRule.Sorter == null) ? "RWAutoSell.Unsorted".Translate() : (this.EditRule.Sorter.Label + "(" + (this.EditRule.SortDirection ? "RWAutoSell.Ascending" : "RWAutoSell.Descending").Translate() + ")")), delegate()
						{
							this.RegisterChange();
							Find.WindowStack.Add(new ASDialog_SorterSelect(this.EditRule));
						}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
					}
					IRule editRule = this.EditRule;
					IAdvanced adv = editRule as IAdvanced;
					if (adv != null)
					{
						if (adv.ShowAdvanced)
						{
							list.Add(new FloatMenuOption("RWAutoSell.HideAdvanced".Translate(), delegate()
							{
								adv.ShowAdvanced = false;
								if (adv.AdvancedNodes() != null)
								{
									foreach (int i2 in adv.AdvancedNodes())
									{
										this.NodeTypesBar[i2].Visible = false;
									}
								}
							}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						}
						else
						{
							list.Add(new FloatMenuOption("RWAutoSell.ShowAdvanced".Translate(), delegate()
							{
								adv.ShowAdvanced = true;
								if (adv.AdvancedNodes() != null)
								{
									foreach (int i2 in adv.AdvancedNodes())
									{
										this.NodeTypesBar[i2].Visible = true;
									}
								}
							}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
						}
					}
					FloatMenu window = new FloatMenu(list, "RWAutoSell.AdvancedMenu".Translate(), false);
					Find.WindowStack.Add(window);
				}
				if (this.History.CanUndo())
				{
					Rect rect = new Rect(inrect.xMax - 180f, inrect.y, 60f, 60f);
					if (Mouse.IsOver(rect))
					{
						TooltipHandler.TipRegion(rect, new TipSignal("RWAutoSell.Undo".Translate()));
					}
					if (Widgets.ButtonImage(rect, ASTextures.Undo, true, null))
					{
						this.UndoRedoAction(new ASRuleNodesDialog.UndoRedo(this.History.Undo));
					}
				}
				if (this.History.CanRedo())
				{
					Rect rect2 = new Rect(inrect.xMax - 120f, inrect.y, 60f, 60f);
					if (Mouse.IsOver(rect2))
					{
						TooltipHandler.TipRegion(rect2, new TipSignal("RWAutoSell.Redo".Translate()));
					}
					if (Widgets.ButtonImage(rect2, ASTextures.Redo, true, null))
					{
						this.UndoRedoAction(new ASRuleNodesDialog.UndoRedo(this.History.Redo));
					}
				}
				Rect inrect2 = new Rect(10f, num + 10f, inrect.width - 10f, 65f);
				Rect rect3 = new Rect(inrect2.x, inrect2.y, Mathf.Floor(inrect2.width / 2f), 30f);
				string text = this.EditRule.Label;
				text = ASWidgets.AlignedTextBox(rect3, "RWAutoSell.RuleDesc".Translate(), text);
				if (text != this.EditRule.Label)
				{
					this.RegisterChange();
					this.EditRule.Label = text;
				}
				TooltipHandler.TipRegion(rect3, new TipSignal("RWAutoSell.DescRuleDesc".Translate()));
				if (this.EditRule.IsChain)
				{
					Widgets.Label(new Rect(inrect2.x, rect3.yMax, Mathf.Floor(inrect.width * 0.7f), 60f), "RWAutoSell.ChainDesc".Translate());
				}
				else
				{
					IAdvanced advanced = this.EditRule as IAdvanced;
					if (advanced != null)
					{
						advanced.DrawPropertiesAdvanced(inrect2);
					}
					else
					{
						this.EditRule.DrawProperties(inrect2);
					}
				}
			}
			return num + 100f;
		}

		// Token: 0x040000A3 RID: 163
		private Vector2 ScrollSearch = Vector2.zero;

		// Token: 0x040000A4 RID: 164
		private Vector2 ScrollRaw = Vector2.zero;

		// Token: 0x040000A5 RID: 165
		private List<ASRuleNodesDialog.TransfColPair> searchlist = new List<ASRuleNodesDialog.TransfColPair>();

		// Token: 0x040000A6 RID: 166
		private bool RefreshSearch = true;

		// Token: 0x040000A7 RID: 167
		private const float rowheight = 22f;

		// Token: 0x040000A8 RID: 168
		private bool waitingforselect;

		// Token: 0x040000A9 RID: 169
		private ASTabBar NodeTypesBar;

		// Token: 0x040000AA RID: 170
		private ASTabBar NodePropertyBar;

		// Token: 0x040000AB RID: 171
		private ASDblTap ClickTracker;

		// Token: 0x040000AC RID: 172
		protected readonly Vector2 ButtonSize = new Vector2(160f, 40f);

		// Token: 0x040000AD RID: 173
		private readonly Map map;

		// Token: 0x040000AE RID: 174
		private Color backcolor;

		// Token: 0x040000AF RID: 175
		private Vector2 scroll = Vector2.zero;

		// Token: 0x040000B0 RID: 176
		private Vector2 propscroll = Vector2.zero;

		// Token: 0x040000B1 RID: 177
		private IRule EditRule;

		// Token: 0x040000B2 RID: 178
		private UndoList History;

		// Token: 0x040000B3 RID: 179
		private IRule ChainedTo;

		// Token: 0x040000B4 RID: 180
		private List<IRule> PreChain;

		// Token: 0x040000B5 RID: 181
		private readonly IRule OriginalRule;

		// Token: 0x040000B6 RID: 182
		private FilterGroup SelectedRoot;

		// Token: 0x040000B7 RID: 183
		private FilterContainer SelectedContainer_;

		// Token: 0x040000B8 RID: 184
		private bool Accept;

		// Token: 0x040000B9 RID: 185
		private IRuleComp ruleComp;

		// Token: 0x040000BB RID: 187
		private int indexsel = -1;

		// Token: 0x040000BC RID: 188
		private bool groupsel;

		// Token: 0x040000BD RID: 189
		private bool showcolonists = true;

		// Token: 0x040000BE RID: 190
		private bool showprisoners = true;

		// Token: 0x040000BF RID: 191
		private bool showfriendlies;

		// Token: 0x040000C0 RID: 192
		private bool showenemies;

		// Token: 0x040000C1 RID: 193
		private bool showwild;

		// Token: 0x040000C2 RID: 194
		private bool showgene = true;

		// Token: 0x040000C3 RID: 195
		private bool showbook = true;

		// Token: 0x040000C4 RID: 196
		private bool showoutfits = true;

		// Token: 0x0200006E RID: 110
		[Nullable(0)]
		private class TransfColPair
		{
			// Token: 0x06000385 RID: 901 RVA: 0x0001EAB7 File Offset: 0x0001CCB7
			public TransfColPair(TransferableOneWay t, Color c)
			{
				this.Trans = t;
				this.color = c;
			}

			// Token: 0x040001D1 RID: 465
			public TransferableOneWay Trans;

			// Token: 0x040001D2 RID: 466
			public Color color;
		}

		// Token: 0x0200006F RID: 111
		// (Invoke) Token: 0x06000387 RID: 903
		[NullableContext(0)]
		private delegate IRule UndoRedo(IRule Current, ref FilterGroup SelectedGroup, ref FilterContainer Filter, ref int node, ref int ptab);
	}
}
