using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200000A RID: 10
	[NullableContext(1)]
	[Nullable(0)]
	public class ASMainTabList : ISectionUI
	{
		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600003D RID: 61 RVA: 0x00003E00 File Offset: 0x00002000
		// (set) Token: 0x0600003E RID: 62 RVA: 0x00003E08 File Offset: 0x00002008
		public RuleRowUI DragRow
		{
			get
			{
				return this.dragrow_;
			}
			set
			{
				if (this.dragrow_ != value && this.dragrow_ != null)
				{
					this.dragrow_.IsDragging = false;
				}
				this.dragrow_ = value;
				if (this.dragrow_ != null)
				{
					this.dragrow_.IsDragging = true;
				}
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00003E42 File Offset: 0x00002042
		// (set) Token: 0x06000040 RID: 64 RVA: 0x00003E4A File Offset: 0x0000204A
		public ISectionUI ParentUI { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000041 RID: 65 RVA: 0x00003E53 File Offset: 0x00002053
		// (set) Token: 0x06000042 RID: 66 RVA: 0x00003E5B File Offset: 0x0000205B
		public IRuleComp RootComp { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000043 RID: 67 RVA: 0x00003E64 File Offset: 0x00002064
		private Map Map { get; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00003E6C File Offset: 0x0000206C
		public List<RuleRowUI> Rows { get; } = new List<RuleRowUI>();

		// Token: 0x06000045 RID: 69 RVA: 0x00003E74 File Offset: 0x00002074
		public void RemoveRow(RuleRowUI Row)
		{
			this.Rows.Remove(Row);
			this.evalutatedSize = false;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00003E8A File Offset: 0x0000208A
		public void AddAction(RowActions action)
		{
			this.RowActs.Push(action);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00003E98 File Offset: 0x00002098
		private void DoActionsStack()
		{
			if (this.RowActs.Count<RowActions>() > 0)
			{
				do
				{
					RowActions rowActions = this.RowActs.Pop();
					switch (rowActions.type)
					{
					case ActType.Move:
					{
						RuleRowUI rowShifted = rowActions.RowShifted;
						rowShifted.Remove();
						rowActions.OverRow.InsertRow(rowShifted);
						this.DragRow = null;
						break;
					}
					case ActType.MoveEnd:
					{
						RuleRowUI rowShifted2 = rowActions.RowShifted;
						rowShifted2.Remove();
						rowShifted2.ParentUI = this;
						rowShifted2.AppendToRoot();
						this.Rows.Add(rowShifted2);
						this.DragRow = null;
						break;
					}
					case ActType.Nest:
					{
						RuleRowUI rowShifted3 = rowActions.RowShifted;
						rowShifted3.Remove();
						rowActions.OverRow.AddOntoRowChain(rowShifted3);
						this.DragRow = null;
						break;
					}
					case ActType.Delete:
						rowActions.RowShifted.Remove();
						break;
					case ActType.Clear:
						this.DragRow = null;
						break;
					case ActType.Update:
					{
						this.height = this.UpdateHeight();
						float y;
						if (rowActions.OverRow.ChainOpen)
						{
							y = 30f;
						}
						else
						{
							y = 0f;
						}
						foreach (RuleRowUI ruleRowUI in rowActions.OverRow.ChainedRows)
						{
							ruleRowUI.Size = new Vector2(rowActions.OverRow.Size.x - 30f, y);
						}
						break;
					}
					}
				}
				while (this.RowActs.Count<RowActions>() > 0);
				this.evalutatedSize = false;
				this.DragRow = null;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004034 File Offset: 0x00002234
		public ASMainTabList(IRuleComp Comp, Map map)
		{
			this.Map = map;
			this.RootComp = Comp;
			this.Rows = new List<RuleRowUI>();
			this.InitRowsList();
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004088 File Offset: 0x00002288
		private void InitRowsList()
		{
			this.Rows.Clear();
			float num = 0f;
			int num2 = 0;
			foreach (IRule rule in this.RootComp.EnumerateRules())
			{
				Vector2 pos = new Vector2(10f, num);
				RuleRowUI ruleRowUI = new RuleRowUI(this, rule, this.RootComp, this.Map, pos, (float)num2 * 30f);
				num += ruleRowUI.Height;
				this.Rows.Add(ruleRowUI);
				num2++;
			}
			this.height = num;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004134 File Offset: 0x00002334
		public float UpdateHeight()
		{
			float num = 0f;
			int num2 = this.Rows.Count<RuleRowUI>();
			for (int i = 0; i < num2; i++)
			{
				num += this.Rows[i].Height;
			}
			return num;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004174 File Offset: 0x00002374
		public void CheckRefresh()
		{
			if (ASLibMod.GuiRefresh)
			{
				List<RuleRowUI> list = new List<RuleRowUI>();
				list.AddRange(this.Rows.AsEnumerable<RuleRowUI>());
				this.InitRowsList();
				int num = 0;
				while (num < this.RootComp.Count() && num < list.Count<RuleRowUI>())
				{
					this.Rows[num].RefreshFrom(list[num]);
					num++;
				}
				this.UpdateHeight();
				ASLibMod.GuiRefresh = false;
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000041EC File Offset: 0x000023EC
		public void EvaluateSize(Rect rect)
		{
			foreach (RuleRowUI ruleRowUI in this.Rows)
			{
				((ISectionUI)ruleRowUI).EvaluateSize(new Rect(rect.x, rect.y, rect.width - 20f, rect.height));
			}
			this.height = this.UpdateHeight();
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004270 File Offset: 0x00002470
		public void Draw(Rect inrect)
		{
			this.CheckRefresh();
			this.DoActionsStack();
			Rect rect = new Rect(0f, 0f, inrect.width - 10f, this.height);
			if (!this.evalutatedSize)
			{
				this.EvaluateSize(rect);
			}
			Widgets.BeginScrollView(inrect, ref this.RuleListScrollPosition, rect, true);
			GUI.color = Color.white;
			float num = 0f;
			int index = 0;
			for (int i = 0; i < this.Rows.Count; i++)
			{
				Rect rect2 = new Rect(rect.x + 10f, num, rect.width - 20f, this.Rows[i].Height);
				num += this.Rows[i].Height;
				index = this.Rows[i].DrawRow(rect2.position, index, false);
				if (i % 2 == 1)
				{
					Widgets.DrawLightHighlight(rect2);
				}
			}
			if (this.height < inrect.height && Mouse.IsOver(new Rect(rect.x, num, rect.width - 20f, inrect.height - this.height)) && this.DragRow != null && Event.current.type == EventType.mouseUp && Event.current.button == 0)
			{
				RowActions action = new RowActions
				{
					RowShifted = this.DragRow,
					OverRow = this.DragRow,
					type = ActType.MoveEnd
				};
				this.AddAction(action);
				Event.current.Use();
			}
			Widgets.EndScrollView();
		}

		// Token: 0x04000015 RID: 21
		private RuleRowUI dragrow_;

		// Token: 0x04000017 RID: 23
		private Vector2 RuleListScrollPosition = Vector2.zero;

		// Token: 0x04000018 RID: 24
		private float height;

		// Token: 0x0400001B RID: 27
		private bool evalutatedSize;

		// Token: 0x0400001D RID: 29
		private Stack<RowActions> RowActs = new Stack<RowActions>();
	}
}
