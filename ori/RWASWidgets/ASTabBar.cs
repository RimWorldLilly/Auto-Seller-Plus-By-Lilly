using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000016 RID: 22
	[NullableContext(1)]
	[Nullable(0)]
	public class ASTabBar
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x060000D8 RID: 216 RVA: 0x00006648 File Offset: 0x00004848
		// (remove) Token: 0x060000D9 RID: 217 RVA: 0x00006680 File Offset: 0x00004880
		public event ASTabBar.TabItemHandler OnClick;

		// Token: 0x17000033 RID: 51
		public ASTab this[int i]
		{
			get
			{
				return this.Tabs[i];
			}
			set
			{
				this.Tabs[i] = value;
			}
		}

		// Token: 0x060000DC RID: 220 RVA: 0x000066D2 File Offset: 0x000048D2
		private void OnClickEvent(ASTabBar sender, ASTab e)
		{
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000DD RID: 221 RVA: 0x000066D4 File Offset: 0x000048D4
		public string CurrentLabel
		{
			get
			{
				return this.Tabs[this.selected].Label;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000066EC File Offset: 0x000048EC
		public int CurrentIndex
		{
			get
			{
				if (this.Tabs.Count == 0)
				{
					return -1;
				}
				return this.Tabs[this.selected].Index;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000DF RID: 223 RVA: 0x00006713 File Offset: 0x00004913
		public PlanetTile CurrentTile
		{
			get
			{
				if (this.Tabs.Count == 0)
				{
					return -1;
				}
				return this.Tabs[this.selected].Tile;
			}
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006740 File Offset: 0x00004940
		public void ForceSelectedByValue(int Index)
		{
			if (this.Tabs.Count<ASTab>() != 0)
			{
				for (int i = 0; i < this.Tabs.Count<ASTab>(); i++)
				{
					if (this[i].Index == Index)
					{
						this.selected = i;
						this.OnClick(this, this[i]);
						return;
					}
				}
			}
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000679C File Offset: 0x0000499C
		public void ForceSelectedByTile(PlanetTile tile)
		{
			if (this.Tabs.Count<ASTab>() != 0)
			{
				for (int i = 0; i < this.Tabs.Count<ASTab>(); i++)
				{
					if (this[i].Tile == tile)
					{
						this.selected = i;
						this.OnClick(this, this[i]);
						return;
					}
				}
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x000067FC File Offset: 0x000049FC
		public ASTabBar(params ASTab[] ASTabs)
		{
			if (ASTabs.Count<ASTab>() > 0)
			{
				this.Tabs = ASTabs.ToList<ASTab>();
				this.tallest = this.Tabs.Max((ASTab x) => x.Size.y);
				this.selected = 0;
			}
			else
			{
				this.selected = -1;
			}
			this.Selectedbackcolour = ASWidgetsMod.GetSettings().SelectedTabBackColor;
			this.backcolour = ASWidgetsMod.GetSettings().TabBackColor;
			this.Selectedbordercolour = ASWidgetsMod.GetSettings().TabBorderColor;
			this.OnClick += this.OnClickEvent;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x000068B4 File Offset: 0x00004AB4
		public float DrawTabs(Rect inRect)
		{
			float result = 0f;
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			Rect rect = new Rect(inRect.x + 10f, inRect.y, 0f, 0f);
			if (this.selected != -1 && this.Tabs != null && this.Tabs.Count<ASTab>() != 0)
			{
				for (int i = 0; i < this.Tabs.Count<ASTab>(); i++)
				{
					ASTab astab = this.Tabs[i];
					if (astab.Visible)
					{
						Rect rect2 = new Rect(rect.xMax, rect.y, astab.Size.x, astab.Size.y);
						result = rect2.yMax;
						Rect rect3 = rect2.ContractedBy(2f);
						if (this.selected == i)
						{
							Widgets.DrawBoxSolid(rect3, this.Selectedbackcolour);
							GUI.color = this.Selectedbordercolour;
							Widgets.DrawBox(rect2, 1, null);
							GUI.color = Color.white;
						}
						else if (Mouse.IsOver(rect2))
						{
							Color color = new Color((this.backcolour.r + 1f) / 2f, (this.backcolour.g + 1f) / 2f, (this.backcolour.b + 1f) / 2f);
							Widgets.DrawBoxSolid(rect3, color);
						}
						else
						{
							Widgets.DrawBoxSolid(rect3, this.backcolour);
						}
						Widgets.Label(rect3, astab.Label);
						if (astab.ToolTip.text != "" && astab.ToolTip.text != string.Empty)
						{
							TooltipHandler.TipRegion(rect2, astab.ToolTip);
						}
						rect = rect2;
						if (Widgets.ButtonInvisible(rect2, true))
						{
							this.selected = i;
							if (i != -1)
							{
								this.OnClick(this, this[i]);
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x04000062 RID: 98
		private Color backcolour;

		// Token: 0x04000063 RID: 99
		private Color Selectedbackcolour;

		// Token: 0x04000064 RID: 100
		private Color Selectedbordercolour;

		// Token: 0x04000065 RID: 101
		private readonly float tallest;

		// Token: 0x04000066 RID: 102
		private List<ASTab> Tabs = new List<ASTab>();

		// Token: 0x04000067 RID: 103
		private int selected;

		// Token: 0x02000022 RID: 34
		// (Invoke) Token: 0x06000114 RID: 276
		[NullableContext(0)]
		public delegate void TabItemHandler(ASTabBar sender, ASTab e);
	}
}
