using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x0200000A RID: 10
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	public class ASChangeBox<[Nullable(2)] T> : ASListBox<T>
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000043 RID: 67 RVA: 0x00003394 File Offset: 0x00001594
		// (remove) Token: 0x06000044 RID: 68 RVA: 0x000033CC File Offset: 0x000015CC
		public event ASListBox<T>.ListItemHandler OnRightClick;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000045 RID: 69 RVA: 0x00003404 File Offset: 0x00001604
		// (remove) Token: 0x06000046 RID: 70 RVA: 0x0000343C File Offset: 0x0000163C
		public event ASListBox<T>.ListItemHandler OnPostRightClick;

		// Token: 0x06000047 RID: 71 RVA: 0x00003474 File Offset: 0x00001674
		public ASChangeBox(float Height = 25f, float Padding = 4f, bool ShowCheckboxes = true, bool ShowIcons = true, bool AllowDelete = true, bool AllowAdd = true) : base(Height, Padding, ShowCheckboxes, ShowIcons)
		{
			this.CanDelete = AllowDelete;
			this.CanAdd = AllowAdd;
			this.OnRightClick += this.OnRtClickEvent;
			this.OnPostRightClick += this.OnRtClickEventPost;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000034C0 File Offset: 0x000016C0
		private void OnRtClickEvent(IGUIList<T> sender, GUIListEventArgs e)
		{
			if (this.fm != null && this.fm.IsOpen)
			{
				return;
			}
			List<FloatMenuOption> list = new List<FloatMenuOption>();
			if (e.index != -1 && this.CanDelete)
			{
				list.Add(new FloatMenuOption("Delete".Translate(), delegate()
				{
					this.RemoveAt(e.index);
					this.OnPostRightClick(this, new GUIListEventArgs(-1, string.Empty));
				}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
			}
			if (this.CanAdd)
			{
				list.Add(new FloatMenuOption("RWAutoSell.Add".Translate(), delegate()
				{
					int index = this.Count<SelectedItem<T>>();
					this.Add(new SelectedItem<T>());
					this.OnPostRightClick(this, new GUIListEventArgs(index, string.Empty));
				}, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0));
			}
			this.fm = new FloatMenu(list);
			Find.WindowStack.Add(this.fm);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x0000359B File Offset: 0x0000179B
		private void OnRtClickEventPost(IGUIList<T> sender, GUIListEventArgs e)
		{
			this.fm = null;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x000035A4 File Offset: 0x000017A4
		public override void DrawListBox(Rect outrect)
		{
			GUI.color = Color.grey;
			Widgets.DrawHighlight(outrect);
			Widgets.DrawBox(outrect, 1, null);
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			if (base.Count != 0)
			{
				Rect viewRect = new Rect(0f, 0f, outrect.width - 16f, (float)base.Count * (base.Height + base.Padding));
				float num = base.Icons ? (base.Height + base.Padding) : base.Padding;
				float width = viewRect.width - num - base.Padding * 2f;
				Widgets.BeginScrollView(outrect, ref this.scroll, viewRect, true);
				float num2 = 0f;
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
					if (base.CheckBoxes)
					{
						bool flag = false;
						if (i == this.Selected)
						{
							flag = true;
						}
						if (base.Icons && ASWidgets.ButtonImageNoBad(butRect, selectedItem.Icon, selectedItem.color))
						{
							flag = true;
						}
						Widgets.CheckboxLabeled(rect2, base[i].label, ref flag, false, null, null, false, false);
						if (flag & this.Selected != i)
						{
							this.ChangeSelected(i);
						}
					}
					else
					{
						if (base.Icons)
						{
							ASWidgets.ButtonImageNoBad(butRect, selectedItem.Icon, selectedItem.color);
						}
						if (Widgets.ButtonInvisible(rect, true))
						{
							this.ChangeSelected(i);
						}
						if (i == this.Selected)
						{
							GUI.color = Color.grey;
							Widgets.DrawHighlightSelected(rect);
							GUI.color = Color.white;
						}
						Widgets.Label(rect2, selectedItem.label);
					}
					if (Mouse.IsOver(rect) && Event.current.button != 0 && Event.current.button == 1)
					{
						this.OnRightClick(this, new GUIListEventArgs(i, base[i].label));
					}
					Widgets.DrawHighlightIfMouseover(rect);
					num2 += base.Height + base.Padding;
				}
				Widgets.EndScrollView();
			}
		}

		// Token: 0x0400001D RID: 29
		private FloatMenu fm;

		// Token: 0x04000020 RID: 32
		private readonly bool CanDelete;

		// Token: 0x04000021 RID: 33
		private readonly bool CanAdd;
	}
}
