using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000026 RID: 38
	[NullableContext(1)]
	[Nullable(0)]
	[StaticConstructorOnStartup]
	public class RuleRowUI : ISectionUI
	{
		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000D219 File Offset: 0x0000B419
		// (set) Token: 0x06000126 RID: 294 RVA: 0x0000D230 File Offset: 0x0000B430
		public RuleRowUI DragRow
		{
			get
			{
				if (this.ParentUI != null)
				{
					return this.ParentUI.DragRow;
				}
				return null;
			}
			set
			{
				if (this.ParentUI != null)
				{
					this.ParentUI.DragRow = value;
				}
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000D246 File Offset: 0x0000B446
		public void AddAction(RowActions action)
		{
			if (this.ParentUI != null)
			{
				this.ParentUI.AddAction(action);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000128 RID: 296 RVA: 0x0000D25C File Offset: 0x0000B45C
		// (set) Token: 0x06000129 RID: 297 RVA: 0x0000D264 File Offset: 0x0000B464
		public ISectionUI ParentUI { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x0600012A RID: 298 RVA: 0x0000D26D File Offset: 0x0000B46D
		// (set) Token: 0x0600012B RID: 299 RVA: 0x0000D275 File Offset: 0x0000B475
		public bool IsDragging { get; set; }

		// Token: 0x0600012C RID: 300 RVA: 0x0000D280 File Offset: 0x0000B480
		public void RefreshFrom(RuleRowUI Row)
		{
			this.ChainOpen = Row.ChainOpen;
			this.DrawPos = Row.DrawPos;
			this.DrawSize = Row.DrawSize;
			this.Size = Row.Size;
			if (this.Rule.IsChain && this.ChainOpen && Row.Rule.IsChain && this.ChainedRows != null && this.ChainedRows.Count > 0 && this.ChainedRows.Count == Row.ChainedRows.Count<RuleRowUI>())
			{
				for (int i = 0; i < this.ChainedRows.Count<RuleRowUI>(); i++)
				{
					this.ChainedRows[i].RefreshFrom(Row.ChainedRows[i]);
				}
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x0600012D RID: 301 RVA: 0x0000D340 File Offset: 0x0000B540
		public IRuleComp RootComp
		{
			get
			{
				if (this.ParentUI != null)
				{
					return this.ParentUI.RootComp;
				}
				return null;
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600012E RID: 302 RVA: 0x0000D357 File Offset: 0x0000B557
		public List<RuleRowUI> Rows
		{
			get
			{
				return this.ChainedRows;
			}
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000D35F File Offset: 0x0000B55F
		public void AddOntoRowChain(RuleRowUI Row)
		{
			Row.comp = null;
			Row.ParentUI = this;
			Row.ChainedRule = this.Rule;
			this.Rule.ChainedAdd(Row.Rule);
			this.ChainedRows.Add(Row);
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000D398 File Offset: 0x0000B598
		public void InsertRow(RuleRowUI Row)
		{
			bool flag = this.Index < Row.Index;
			if (this.ParentUI == null)
			{
				return;
			}
			int num = this.ParentUI.Rows.IndexOf(this);
			if (!flag)
			{
				num++;
				if (num > this.ParentUI.Rows.Count<RuleRowUI>())
				{
					num--;
				}
			}
			if (this.comp != null)
			{
				this.comp.InsertRuleAt(num, Row.Rule);
			}
			else
			{
				if (this.ChainedRule == null)
				{
					return;
				}
				this.ChainedRule.ChainedInsert(num, Row.Rule);
			}
			Row.ParentUI = this.ParentUI;
			Row.ChainedRule = this.ChainedRule;
			Row.comp = this.comp;
			this.ParentUI.Rows.Insert(num, Row);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000D45C File Offset: 0x0000B65C
		public void Remove()
		{
			if (this.ParentUI != null)
			{
				this.ParentUI.Rows.Remove(this);
			}
			if (this.comp != null)
			{
				this.comp.Remove(this.Rule);
				return;
			}
			if (this.ChainedRule != null)
			{
				this.ChainedRule.ChainedRemove(this.Rule);
			}
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000D4B6 File Offset: 0x0000B6B6
		public void AppendToRoot()
		{
			this.RootComp.Add(this.Rule);
			this.comp = this.RootComp;
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000133 RID: 307 RVA: 0x0000D4D8 File Offset: 0x0000B6D8
		public float Height
		{
			get
			{
				float num = 30f;
				if (this.ChainOpen && this.ChainedRows != null && this.ChainedRows.Count<RuleRowUI>() > 0)
				{
					foreach (RuleRowUI ruleRowUI in this.ChainedRows)
					{
						num += ruleRowUI.Height;
					}
				}
				return num;
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000D554 File Offset: 0x0000B754
		public void EvaluateSize(Rect rect)
		{
			this.Size = new Vector2(rect.width, this.Height);
			if (this.UpdateRequired)
			{
				this.DrawSize = this.Size;
				this.UpdateRequired = false;
			}
			if (this.ChainOpen && this.ChainedRows != null && this.ChainedRows.Count<RuleRowUI>() > 0)
			{
				foreach (RuleRowUI ruleRowUI in this.ChainedRows)
				{
					ruleRowUI.EvaluateSize(new Rect(rect.x, rect.y, rect.width - 30f, rect.height));
				}
			}
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000D61C File Offset: 0x0000B81C
		public RuleRowUI(ISectionUI Parent, IRule Rule, IRule chainrule, Map map, float Y) : this(Parent, Rule, null, map, Y)
		{
			this.ChainedRule = chainrule;
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000D634 File Offset: 0x0000B834
		public RuleRowUI(ISectionUI Parent, IRule Rule, IRuleComp comp, Map map, float Y)
		{
			this.map = map;
			this.comp = comp;
			this.Rule = Rule;
			this.ParentUI = Parent;
			this.DrawPos = new Vector2(10f, Y);
			if ((comp == null || comp.ChainsAllowed) && Rule.IsChain)
			{
				int num = 0;
				Vector2 drawPos = this.DrawPos;
				drawPos.x += 30f;
				foreach (IRule rule in Rule.RuleChain())
				{
					RuleRowUI item = new RuleRowUI(this, rule, Rule, map, (float)num * 30f);
					this.ChainedRows.Add(item);
					num++;
				}
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000D724 File Offset: 0x0000B924
		public RuleRowUI(ISectionUI Parent, IRule Rule, IRuleComp comp, Map map, Vector2 Pos, float Y) : this(Parent, Rule, comp, map, Y)
		{
			this.DrawPos = Pos;
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000D73B File Offset: 0x0000B93B
		public RuleRowUI(ISectionUI Parent, IRule Rule, IRule chainrule, Map map, Vector2 Pos, float Y) : this(Parent, Rule, chainrule, map, Y)
		{
			this.DrawPos = Pos;
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000D754 File Offset: 0x0000B954
		private void renamechain(IRule current)
		{
			current.Label = "(" + "RWAutoSell.Cloned".Translate() + ")" + current.Label;
			if (current.IsChain)
			{
				foreach (IRule current2 in current.RuleChain())
				{
					this.renamechain(current2);
				}
			}
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000D7E0 File Offset: 0x0000B9E0
		public int DrawRow(Vector2 SnapPos, int index, bool instant = false)
		{
			this.Index = index;
			int num = index + 1;
			Rect rect = new Rect(SnapPos, this.Size);
			Rect rect2 = new Rect(SnapPos, this.Size)
			{
				height = 30f
			};
			Rect rect3 = new Rect(SnapPos, this.Size);
			if (this.DragRow != null && !this.IsDragging)
			{
				if (this.Rule.IsChain && Mouse.IsOver(rect2))
				{
					Widgets.DrawBox(rect2, 1, null);
				}
				else if (Event.current.mousePosition.y < SnapPos.y + 30f && this.DragRow.Index > this.Index)
				{
					rect3.y += this.DragRow.Height;
				}
				else if (Event.current.mousePosition.y > SnapPos.y && this.DragRow.Index < this.Index)
				{
					rect3.y -= this.DragRow.Height;
				}
			}
			if (this.DragRow == this)
			{
				Vector2 a = new Vector2(Event.current.mousePosition.x, this.DrawPos.y);
				this.DrawPos.y = Vector2.Lerp(a, Event.current.mousePosition - new Vector2(0f, 15f), Time.deltaTime * 10f).y;
			}
			else if (this.DrawPos != rect3.position)
			{
				if (instant)
				{
					this.DrawPos = SnapPos;
				}
				else
				{
					this.DrawPos = Vector2.Lerp(this.DrawPos, rect3.position, Time.deltaTime * 4f);
					if ((double)Math.Abs(this.DrawPos.y - rect3.y) < 0.1)
					{
						this.DrawPos = rect3.position;
					}
				}
			}
			this.DrawSize = Vector2.Lerp(this.DrawSize, this.Size, Time.deltaTime * 10f);
			Rect rect4 = new Rect(this.DrawPos, this.DrawSize);
			Rect rect5 = new Rect(this.DrawPos.x, this.DrawPos.y, this.DrawSize.x, 30f);
			Rect rect6 = new Rect(this.DrawPos.x, this.DrawPos.y, this.DrawSize.x, 30f);
			Color color = GUI.color;
			if (this.Rule.Active)
			{
				GUI.color = Color.white;
			}
			else
			{
				GUI.color = Color.grey;
			}
			if (this.RootComp.ChainsAllowed && this.Rule.IsChain)
			{
				rect5.x += 42f;
				rect5.width -= 6f;
			}
			Widgets.Label(rect5, this.Rule.ToString());
			GUI.color = color;
			Rect rect7 = new Rect(rect4.xMax - 24f, rect4.y + 3f, 24f, 24f);
			Rect rect8 = new Rect(rect4.xMax - 48f, rect7.y, 24f, 24f);
			Rect rect9 = new Rect(rect4.xMax - 72f, rect7.y, 24f, 24f);
			Rect rect10 = new Rect(rect4.x, rect7.y, 25f, 25f);
			Widgets.DrawTextureFitted(rect7, Widgets.CheckboxOffTex, 1f, 1f);
			Widgets.DrawTextureFitted(rect8, ASTextures.Suspend, 1f, 1f);
			Widgets.DrawTextureFitted(rect9, ASTextures.Clone, 1f, 1f);
			Rect rect11 = new Rect(rect10.x + 20f, rect10.y, rect10.width, rect10.height);
			if (this.RootComp.ChainsAllowed && this.Rule.IsChain)
			{
				Widgets.DrawTextureFitted(rect10, ASTextures.Chain, 1f, 1f);
				if (this.ChainOpen)
				{
					Widgets.DrawTextureFitted(rect11, RuleRowUI.Collapse, 1f, 1f);
				}
				else
				{
					Widgets.DrawTextureFitted(rect11, RuleRowUI.Reveal, 1f, 1f);
				}
			}
			if (this.DragRow != null)
			{
				this.MouseCheck = Vector2.zero;
				if (!instant)
				{
					if (this.IsDragging)
					{
						Widgets.DrawHighlight(rect4);
						if (Mouse.IsOver(rect) && Event.current.type == EventType.mouseUp && Event.current.button == 0)
						{
							RowActions action = new RowActions
							{
								RowShifted = this,
								OverRow = this,
								type = ActType.Clear
							};
							this.AddAction(action);
						}
					}
					else if (Mouse.IsOver(rect2) && Event.current.type == EventType.mouseUp && Event.current.button == 0)
					{
						if (this.Rule.IsChain)
						{
							RowActions action2 = new RowActions
							{
								RowShifted = this.DragRow,
								OverRow = this,
								type = ActType.Nest
							};
							this.AddAction(action2);
						}
						else
						{
							RowActions action3 = new RowActions
							{
								RowShifted = this.DragRow,
								OverRow = this,
								type = ActType.Move
							};
							this.AddAction(action3);
						}
						Event.current.Use();
					}
				}
			}
			else if (!this.IsDragging)
			{
				if (this.MouseCheck != Vector2.zero)
				{
					if (Math.Abs(this.MouseCheck.y - Event.current.mousePosition.y) > 5f)
					{
						this.DragRow = this;
					}
					else if (Mouse.IsOver(rect6) && Event.current.type == EventType.mouseUp && Event.current.button == 0)
					{
						this.MouseCheck = Vector2.zero;
						Event.current.Use();
						if (Mouse.IsOver(rect7))
						{
							RowActions action4 = new RowActions
							{
								RowShifted = this,
								OverRow = this,
								type = ActType.Delete
							};
							this.AddAction(action4);
						}
						else if (Mouse.IsOver(rect8))
						{
							this.Rule.Active = !this.Rule.Active;
						}
						else if (Mouse.IsOver(rect9))
						{
							IRule rule = this.Rule.DeepCopy();
							this.renamechain(rule);
							if (this.comp != null)
							{
								this.comp.Add(rule);
							}
							else
							{
								this.ChainedRule.ChainedAdd(rule);
							}
							ASLibMod.GuiRefresh = true;
						}
						else if (this.RootComp.ChainsAllowed && this.Rule.IsChain && (Mouse.IsOver(rect10) || Mouse.IsOver(rect11)))
						{
							if (!this.ChainOpen)
							{
								foreach (RuleRowUI ruleRowUI in this.ChainedRows)
								{
									ruleRowUI.DrawPos = this.DrawPos;
								}
							}
							this.ChainOpen = !this.ChainOpen;
							RowActions action5 = new RowActions
							{
								RowShifted = this,
								OverRow = this,
								type = ActType.Update
							};
							this.AddAction(action5);
						}
						else if (this.comp == null)
						{
							List<IRule> list = new List<IRule>();
							ISectionUI sectionUI = this;
							do
							{
								sectionUI = sectionUI.ParentUI;
								RuleRowUI ruleRowUI2 = sectionUI as RuleRowUI;
								if (ruleRowUI2 != null)
								{
									list.Add(ruleRowUI2.Rule);
								}
							}
							while (sectionUI.ParentUI != null);
							Find.WindowStack.Add(new ASRuleNodesDialog(this.map, this.RootComp, this.ChainedRule, list, this.Rule, false));
						}
						else
						{
							Find.WindowStack.Add(new ASRuleNodesDialog(this.map, this.comp, this.Rule, false));
						}
					}
				}
				else if (this.MouseCheck == Vector2.zero)
				{
					if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Mouse.IsOver(rect6))
					{
						this.MouseCheck = Event.current.mousePosition;
					}
					Widgets.DrawHighlightIfMouseover(rect6);
				}
			}
			if (this.ChainOpen && this.ChainedRows != null && this.ChainedRows.Count<RuleRowUI>() > 0)
			{
				Rect rect12 = new Rect(SnapPos.x + 30f, SnapPos.y + 30f, this.Size.x - 30f, this.Height);
				float num2 = 0f;
				if (this.IsDragging)
				{
					rect12.y = this.DrawPos.y;
					num2 += 30f;
				}
				for (int i = 0; i < this.ChainedRows.Count<RuleRowUI>(); i++)
				{
					bool instant2 = false;
					if (this.IsDragging || instant)
					{
						instant2 = true;
					}
					num = this.ChainedRows[i].DrawRow(rect12.position + new Vector2(0f, num2), num, instant2);
					num2 += this.ChainedRows[i].Height;
				}
			}
			return num;
		}

		// Token: 0x040000D2 RID: 210
		private static readonly Texture2D Reveal = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true);

		// Token: 0x040000D3 RID: 211
		private static readonly Texture2D Collapse = ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);

		// Token: 0x040000D6 RID: 214
		private Vector2 MouseCheck = Vector2.zero;

		// Token: 0x040000D7 RID: 215
		public IRule Rule;

		// Token: 0x040000D8 RID: 216
		private IRule ChainedRule;

		// Token: 0x040000D9 RID: 217
		private readonly Map map;

		// Token: 0x040000DA RID: 218
		private IRuleComp comp;

		// Token: 0x040000DB RID: 219
		public Vector2 DrawPos;

		// Token: 0x040000DC RID: 220
		public Vector2 DrawSize;

		// Token: 0x040000DD RID: 221
		public Vector2 Size;

		// Token: 0x040000DE RID: 222
		public int Index = -1;

		// Token: 0x040000DF RID: 223
		public bool UpdateRequired = true;

		// Token: 0x040000E0 RID: 224
		public bool ChainOpen;

		// Token: 0x040000E1 RID: 225
		public List<RuleRowUI> ChainedRows = new List<RuleRowUI>();
	}
}
