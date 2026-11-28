using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWASWidgets
{
	// Token: 0x02000012 RID: 18
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public class ASMultiBox<[Nullable(2)] T> : ASListBox<T>
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000090 RID: 144 RVA: 0x000051E4 File Offset: 0x000033E4
		// (remove) Token: 0x06000091 RID: 145 RVA: 0x0000521C File Offset: 0x0000341C
		public event ASListBox<T>.ListItemHandler OnDblClick;

		// Token: 0x06000092 RID: 146 RVA: 0x00005251 File Offset: 0x00003451
		public ASMultiBox(float Height, float Padding, bool ShowCheckboxes = true, bool ShowIcons = true) : base(Height, Padding, ShowCheckboxes, ShowIcons)
		{
			this.OnDblClick += this.OnDblClickEvent;
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000527E File Offset: 0x0000347E
		private void OnDblClickEvent(IGUIList<T> sender, GUIListEventArgs e)
		{
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00005280 File Offset: 0x00003480
		public override T GetSelected
		{
			get
			{
				SelectedItem<T> selectedItem = this.GUIList.FirstOrDefault((SelectedItem<T> x) => x.Selected);
				if (selectedItem == null)
				{
					return default(T);
				}
				return selectedItem.Item;
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x000052CB File Offset: 0x000034CB
		public override void ChangeSelected(int i)
		{
			base[i].Selected = !base[i].Selected;
			base.ChangeSelected(i);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x000052F0 File Offset: 0x000034F0
		public override void DrawListBox(Rect outrect)
		{
			GUI.color = Color.grey;
			Widgets.DrawHighlight(outrect);
			Widgets.DrawBox(outrect, 1, null);
			GUI.color = Color.white;
			if (base.Count != 0)
			{
				Rect viewRect = new Rect(0f, 0f, outrect.width - 16f, (float)base.Count * (base.Height + base.Padding));
				float num = base.Icons ? (base.Height + base.Padding) : base.Padding;
				float width = viewRect.width - num - base.Padding;
				Widgets.BeginScrollView(outrect, ref this.scroll, viewRect, true);
				float num2 = 0f;
				bool flag = false;
				for (int i = 0; i < base.Count; i++)
				{
					Rect rect = new Rect(base.Padding, num2, outrect.width - base.Padding * 2f, base.Height + base.Padding);
					SelectedItem<T> selectedItem = base[i];
					Rect rect2 = new Rect(num, num2, width, base.Height);
					Rect butRect = new Rect(base.Padding, num2, base.Height, base.Height);
					if (Mouse.IsOver(rect) && selectedItem.tooltip != string.Empty)
					{
						if (this.hoverindex != i)
						{
							this.hoverindex = i;
							this.hovertime = 0;
						}
						if (this.hovertime > 120)
						{
							TooltipHandler.TipRegion(rect, selectedItem.tooltip);
						}
						else
						{
							this.hovertime++;
						}
					}
					if (Mouse.IsOver(rect))
					{
						flag = true;
						if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Event.current.clickCount == 2)
						{
							string item = string.Empty;
							if (this.Selected != -1)
							{
								item = base[i].label;
							}
							this.OnDblClick(this, new GUIListEventArgs(i, item));
						}
						if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
						{
							this.linedragid = i;
							this.isdragging = true;
							Event.current.Use();
						}
						if (Event.current.type == EventType.mouseUp && this.isdragging)
						{
							if (this.linedragid == i)
							{
								if (base.CheckBoxes || Event.current.control)
								{
									this.ChangeSelected(i);
									if (base[i].Selected && (this.lastclicked == -1 || (Event.current.control && !Event.current.shift)))
									{
										this.lastclicked = i;
									}
									else if (!base.CheckBoxes && Event.current.control && Event.current.shift && this.lastclicked != -1)
									{
										for (int j = 0; j < base.Count; j++)
										{
											if (j >= Math.Min(this.lastclicked, this.linedragid) && j <= Math.Max(this.lastclicked, this.linedragid))
											{
												base[j].Selected = true;
											}
										}
									}
								}
								else
								{
									for (int k = 0; k < base.Count; k++)
									{
										if (k == i && (!base[k].Selected || this.GetSelectedItems.Count > 1))
										{
											base[k].Selected = true;
										}
										else
										{
											base[k].Selected = false;
										}
									}
									if (base[i].Selected && (this.lastclicked == -1 || !Event.current.shift))
									{
										this.lastclicked = i;
									}
									else if (Event.current.shift && base[i].Selected && this.lastclicked != -1)
									{
										for (int l = 0; l < base.Count; l++)
										{
											if (l >= Math.Min(this.lastclicked, this.linedragid) && l <= Math.Max(this.lastclicked, this.linedragid))
											{
												base[l].Selected = true;
											}
											else
											{
												base[l].Selected = false;
											}
										}
									}
									else
									{
										this.lastclicked = -1;
									}
								}
							}
							else if (!base.CheckBoxes && !Event.current.control)
							{
								for (int m = 0; m < base.Count; m++)
								{
									if (m < Math.Min(i, this.linedragid) || m > Math.Max(i, this.linedragid))
									{
										base[m].Selected = false;
									}
									else
									{
										base[m].Selected = true;
									}
								}
							}
							else
							{
								for (int n = Math.Min(i, this.linedragid); n <= Math.Max(i, this.linedragid); n++)
								{
									base[n].Selected = true;
								}
							}
							this.linedragid = -1;
							this.isdragging = false;
							base.RaiseClickEvent(i, "Drag");
							Event.current.Use();
						}
					}
					if (this.linedragid != -1 && Mouse.IsOver(rect))
					{
						Widgets.DrawHighlight(new Rect(rect.x, (float)Mathf.Min(i, this.linedragid) * (base.Height + base.Padding), rect.width, (float)(Mathf.Max(i, this.linedragid) - Mathf.Min(i, this.linedragid)) * (base.Height + base.Padding) + (base.Height + base.Padding)));
					}
					if (base.CheckBoxes)
					{
						bool flag2 = selectedItem.Selected;
						if ((base.Icons && ASWidgets.ButtonImageNoBad(butRect, selectedItem.Icon, selectedItem.color)) || Widgets.ButtonInvisible(rect2, true))
						{
							flag2 = !flag2;
						}
						Widgets.Label(rect2, selectedItem.label);
						Widgets.CheckboxDraw(rect2.x + rect2.width - 24f, rect2.y + (rect2.height - 24f) / 2f, flag2, false, 24f, null, null);
						if (selectedItem.Selected != flag2)
						{
							if (flag2)
							{
								SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera(null);
							}
							else
							{
								SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera(null);
							}
							this.ChangeSelected(i);
						}
					}
					else
					{
						if (base.Icons)
						{
							ASWidgets.ButtonImageNoBad(butRect, selectedItem.Icon, selectedItem.color);
						}
						if (selectedItem.Selected)
						{
							GUI.color = Color.grey;
							Widgets.DrawHighlightSelected(rect);
							GUI.color = Color.white;
						}
						Widgets.Label(rect2, selectedItem.label);
					}
					Widgets.DrawHighlightIfMouseover(rect);
					num2 += base.Height + base.Padding;
				}
				if (!flag)
				{
					this.isdragging = false;
					this.linedragid = -1;
				}
				Widgets.EndScrollView();
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000097 RID: 151 RVA: 0x000059C4 File Offset: 0x00003BC4
		public override List<T> GetSelectedItems
		{
			get
			{
				if (base.Count != 0)
				{
					return (from el in this
					where el.Selected
					select el.Item).ToList<T>();
				}
				return new List<T>();
			}
		}

		// Token: 0x04000048 RID: 72
		private int linedragid = -1;

		// Token: 0x04000049 RID: 73
		private bool isdragging;

		// Token: 0x0400004A RID: 74
		private int lastclicked = -1;
	}
}
