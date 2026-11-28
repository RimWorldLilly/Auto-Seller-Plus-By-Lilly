using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(new byte[]
	{
		0,
		1
	})]
	internal class ASListBoxExt<[Nullable(2)] T> : ASListBox<T>
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000083 RID: 131 RVA: 0x00004C40 File Offset: 0x00002E40
		// (remove) Token: 0x06000084 RID: 132 RVA: 0x00004C78 File Offset: 0x00002E78
		public event ASListBox<T>.ListItemHandler OnRemove;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000085 RID: 133 RVA: 0x00004CB0 File Offset: 0x00002EB0
		// (remove) Token: 0x06000086 RID: 134 RVA: 0x00004CE8 File Offset: 0x00002EE8
		public event ASListBox<T>.ListItemHandler OnEdit;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000087 RID: 135 RVA: 0x00004D20 File Offset: 0x00002F20
		// (remove) Token: 0x06000088 RID: 136 RVA: 0x00004D58 File Offset: 0x00002F58
		public event ASListBox<T>.ListItemHandler OnAddNew;

		// Token: 0x06000089 RID: 137 RVA: 0x00004D90 File Offset: 0x00002F90
		public ASListBoxExt(float Height, float Padding, bool EditButton = false, bool RemoveButton = false, bool AddNewButton = false, string TranslationKey = "") : base(Height, Padding, false, false)
		{
			this.TranslationKey = TranslationKey;
			if (RemoveButton)
			{
				this.OnRemove += this.OnRemoveEvent;
				this.ShowRemove = RemoveButton;
			}
			if (EditButton)
			{
				this.OnEdit += this.OnEditEvent;
				this.ShowEdit = EditButton;
			}
			if (AddNewButton)
			{
				this.OnAddNew += this.OnAddNewEvent;
				this.ShowAddNew = AddNewButton;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00004E12 File Offset: 0x00003012
		private void OnRemoveEvent(IGUIList<T> sender, GUIListEventArgs e)
		{
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00004E14 File Offset: 0x00003014
		private void OnAddNewEvent(IGUIList<T> sender, GUIListEventArgs e)
		{
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00004E16 File Offset: 0x00003016
		private void OnEditEvent(IGUIList<T> sender, GUIListEventArgs e)
		{
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00004E18 File Offset: 0x00003018
		private string GetRowLabel(int index)
		{
			if (string.IsNullOrEmpty(this.TranslationKey))
			{
				return base[index].label;
			}
			return this.TranslationKey.Translate(base[index].label);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00004E55 File Offset: 0x00003055
		public override void ChangeSelected(int i)
		{
			if (this.Selected == i)
			{
				base.ChangeSelected(-1);
				return;
			}
			base.ChangeSelected(i);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00004E70 File Offset: 0x00003070
		public override void DrawListBox(Rect outrect)
		{
			GUI.color = Color.grey;
			Widgets.DrawHighlight(outrect);
			Widgets.DrawBox(outrect, 1, null);
			GUI.color = Color.white;
			Rect viewRect = new Rect(0f, 0f, outrect.width - 16f, (float)base.Count * (base.Height + base.Padding) + (this.ShowAddNew ? (base.Height + base.Padding) : 0f));
			float width = viewRect.width - (base.Padding * 2f + (this.ShowEdit ? (120f + base.Padding) : 0f) + (this.ShowRemove ? (120f + base.Padding) : 0f));
			Widgets.BeginScrollView(outrect, ref this.scroll, viewRect, true);
			float num = 0f;
			if (this.ShowAddNew)
			{
				if (Widgets.ButtonText(new Rect(base.Padding, num, outrect.width - base.Padding * 2f, base.Height + base.Padding), "Add New", true, true, true, null))
				{
					this.OnAddNew(this, new GUIListEventArgs(base.Count, string.Empty));
				}
				num += base.Height + base.Padding;
			}
			if (base.Count != 0)
			{
				for (int i = 0; i < base.Count; i++)
				{
					Rect rect = new Rect(base.Padding, num, outrect.width - base.Padding * 2f, base.Height + base.Padding);
					Rect rect2 = new Rect(base.Padding, num, width, base.Height);
					if (base[i].label != string.Empty)
					{
						if (Widgets.ButtonInvisible(rect2, true))
						{
							this.ChangeSelected(i);
						}
						Widgets.DrawHighlightIfMouseover(rect2);
						if (i == this.Selected)
						{
							GUI.color = Color.grey;
							Widgets.DrawHighlightSelected(rect);
							GUI.color = Color.white;
						}
						Widgets.Label(rect2, this.GetRowLabel(i));
						float num2 = 0f;
						if (this.ShowEdit)
						{
							if (Widgets.ButtonText(new Rect(rect2.xMax, rect2.y, 100f, rect2.height), "RWAutoSell.Edit".Translate(), true, true, true, null))
							{
								this.OnEdit(this, new GUIListEventArgs(i, base[i].label));
							}
							num2 += 100f + base.Padding;
						}
						if (this.ShowRemove)
						{
							if (Widgets.ButtonText(new Rect(rect2.xMax + num2, rect2.y, 100f, rect2.height), "RWAutoSell.Delete".Translate(), true, true, true, null))
							{
								string label = base[i].label;
								this.OnRemove(this, new GUIListEventArgs(i, label));
								base.RemoveAt(i);
								i--;
								if (this.Selected > i)
								{
									this.Selected--;
									this.ChangeSelected(this.Selected);
								}
							}
							num2 += 100f;
						}
					}
					num += base.Height + base.Padding;
				}
			}
			Widgets.EndScrollView();
		}

		// Token: 0x04000040 RID: 64
		private bool ShowEdit;

		// Token: 0x04000041 RID: 65
		private bool ShowRemove;

		// Token: 0x04000042 RID: 66
		private bool ShowAddNew;

		// Token: 0x04000043 RID: 67
		private string TranslationKey = string.Empty;
	}
}
