using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000014 RID: 20
	[NullableContext(1)]
	[Nullable(0)]
	public class ASSplitBox<[Nullable(2)] T>
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060000AF RID: 175 RVA: 0x00005D84 File Offset: 0x00003F84
		// (remove) Token: 0x060000B0 RID: 176 RVA: 0x00005DBC File Offset: 0x00003FBC
		[Nullable(new byte[]
		{
			1,
			0
		})]
		[Nullable(new byte[]
		{
			1,
			0
		})]
		public event ASSplitBox<T>.ListItemHandler OnChange;

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00005DF1 File Offset: 0x00003FF1
		protected float Height { get; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00005DF9 File Offset: 0x00003FF9
		// (set) Token: 0x060000B3 RID: 179 RVA: 0x00005E01 File Offset: 0x00004001
		public bool HideChange { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000B4 RID: 180 RVA: 0x00005E0A File Offset: 0x0000400A
		protected float Padding { get; }

		// Token: 0x060000B5 RID: 181 RVA: 0x00005E14 File Offset: 0x00004014
		public ASSplitBox(float Height, float Padding, bool ShowCheckboxes = true, bool ShowIcons = true, string SelName = "", string DeSelName = "")
		{
			this.Leftbox = new ASMultiBox<T>(Height, Padding, ShowCheckboxes, ShowIcons);
			this.Rightbox = new ASMultiBox<T>(Height, Padding, ShowCheckboxes, ShowIcons);
			this.SelectedName = SelName;
			this.UnSelectedName = DeSelName;
			this.Height = Height;
			this.Padding = Padding;
			this.CheckBoxes = ShowCheckboxes;
			this.Icons = ShowIcons;
			this.OnChange += this.OnChangeEvent;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x00005EA7 File Offset: 0x000040A7
		private void OnChangeEvent(ASMultiBox<T> ToBox, bool ToSelected)
		{
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00005EAC File Offset: 0x000040AC
		public virtual void DrawListBox(Rect outrect)
		{
			GUI.color = Color.grey;
			Rect rect = outrect.LeftHalf();
			rect.width -= 10f;
			Rect rect2 = outrect.RightHalf();
			rect2.width = rect.width;
			rect2.x += 10f;
			if (this.SelectedName != string.Empty || this.UnSelectedName != string.Empty)
			{
				GUI.color = Color.white;
				Rect rect3 = new Rect(rect.x, rect.y, rect.width, 30f);
				Rect rect4 = new Rect(rect2.x, rect2.y, rect2.width, 30f);
				Widgets.Label(rect3, this.SelectedName);
				Widgets.Label(rect4, this.UnSelectedName);
				rect.height -= 30f;
				rect.y += 30f;
				rect2.height -= 30f;
				rect2.y += 30f;
				GUI.color = Color.grey;
			}
			Widgets.DrawHighlight(rect);
			Widgets.DrawHighlight(rect2);
			GUI.color = Color.white;
			if (this.Count != 0)
			{
				this.Leftbox.DrawListBox(rect);
				this.Rightbox.DrawListBox(rect2);
				if (!this.HideChange)
				{
					if (Widgets.ButtonText(new Rect(rect.xMax, rect.height / 2f - 10f, 20f, 20f), ">", true, true, true, null))
					{
						this.MoveToUnSelected();
					}
					if (Widgets.ButtonText(new Rect(rect.xMax, rect.height / 2f + 10f, 20f, 20f), "<", true, true, true, null))
					{
						this.MoveToSelected();
					}
				}
			}
			GUI.color = Color.grey;
			Widgets.DrawBox(rect, 1, null);
			Widgets.DrawBox(rect2, 1, null);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000060D0 File Offset: 0x000042D0
		public void MoveToSelected()
		{
			foreach (SelectedItem<T> selectedItem in this.Rightbox)
			{
				if (selectedItem.Selected)
				{
					this.Leftbox.Add(new SelectedItem<T>
					{
						Icon = selectedItem.Icon,
						surpresslabelupdate = selectedItem.surpresslabelupdate,
						color = selectedItem.color,
						Item = selectedItem.Item,
						label = selectedItem.label
					});
				}
			}
			this.RemoveOldSelected(this.Rightbox);
			this.OnChange(this.Leftbox, true);
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x00006188 File Offset: 0x00004388
		public void MoveToUnSelected()
		{
			foreach (SelectedItem<T> selectedItem in this.Leftbox)
			{
				if (selectedItem.Selected)
				{
					this.Rightbox.Add(new SelectedItem<T>
					{
						Icon = selectedItem.Icon,
						surpresslabelupdate = selectedItem.surpresslabelupdate,
						color = selectedItem.color,
						Item = selectedItem.Item,
						label = selectedItem.label
					});
				}
			}
			this.RemoveOldSelected(this.Leftbox);
			this.OnChange(this.Rightbox, false);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x00006240 File Offset: 0x00004440
		private void RemoveOldSelected(ASMultiBox<T> box)
		{
			for (int i = box.Count<SelectedItem<T>>() - 1; i > -1; i--)
			{
				if (box[i].Selected)
				{
					box.RemoveAt(i);
				}
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00006275 File Offset: 0x00004475
		public void AddSelected(T Item)
		{
			this.AddSelected(new SelectedItem<T>
			{
				Item = Item
			});
		}

		// Token: 0x060000BC RID: 188 RVA: 0x00006289 File Offset: 0x00004489
		public void AddSelected(SelectedItem<T> Item)
		{
			this.Leftbox.Add(Item);
		}

		// Token: 0x060000BD RID: 189 RVA: 0x00006297 File Offset: 0x00004497
		public void Add(T Item)
		{
			this.Add(new SelectedItem<T>
			{
				Item = Item
			});
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000062AB File Offset: 0x000044AB
		public void Add(SelectedItem<T> Item)
		{
			this.Rightbox.Add(Item);
		}

		// Token: 0x060000BF RID: 191 RVA: 0x000062B9 File Offset: 0x000044B9
		public void AddRange(IEnumerable<SelectedItem<T>> Items)
		{
			this.Rightbox.AddRange(Items);
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x000062C8 File Offset: 0x000044C8
		public void AddRange(IEnumerable<T> Items)
		{
			foreach (T item in Items)
			{
				this.Add(item);
			}
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x00006310 File Offset: 0x00004510
		public void AddRange(params T[] Items)
		{
			foreach (T item in Items)
			{
				this.Add(item);
			}
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000633C File Offset: 0x0000453C
		public void AddRangeSelected(IEnumerable<SelectedItem<T>> Items)
		{
			this.Leftbox.AddRange(Items);
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000634C File Offset: 0x0000454C
		public void AddRangeSelected(IEnumerable<T> Items)
		{
			foreach (T item in Items)
			{
				this.AddSelected(item);
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00006394 File Offset: 0x00004594
		public void AddRangeSelected(params T[] Items)
		{
			foreach (T item in Items)
			{
				this.AddSelected(item);
			}
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000063C0 File Offset: 0x000045C0
		public void Clear()
		{
			this.Rightbox.Clear();
			this.Leftbox.Clear();
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x000063D8 File Offset: 0x000045D8
		public bool Contains(SelectedItem<T> item)
		{
			return this.Leftbox.Contains(item) || this.Rightbox.Contains(item);
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x000063F8 File Offset: 0x000045F8
		public bool Contains(T Item)
		{
			return this.Leftbox.Any(delegate(SelectedItem<T> x)
			{
				T item = x.Item;
				return item.Equals(Item);
			}) || this.Rightbox.Any(delegate(SelectedItem<T> x)
			{
				T item = x.Item;
				return item.Equals(Item);
			});
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00006444 File Offset: 0x00004644
		public bool Remove(SelectedItem<T> item)
		{
			return this.Leftbox.Remove(item) || this.Rightbox.Remove(item);
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00006462 File Offset: 0x00004662
		public bool Remove(T item)
		{
			return this.Leftbox.Remove(item) || this.Rightbox.Remove(item);
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000CA RID: 202 RVA: 0x00006480 File Offset: 0x00004680
		public List<T> GetUnSelectedItems
		{
			get
			{
				if (this.Rightbox.Count<SelectedItem<T>>() != 0)
				{
					return (from el in this.Rightbox
					select el.Item).ToList<T>();
				}
				return new List<T>();
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000CB RID: 203 RVA: 0x000064D0 File Offset: 0x000046D0
		public List<T> GetSelectedItems
		{
			get
			{
				if (this.Leftbox.Count<SelectedItem<T>>() != 0)
				{
					return (from el in this.Leftbox
					select el.Item).ToList<T>();
				}
				return new List<T>();
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000CC RID: 204 RVA: 0x0000651F File Offset: 0x0000471F
		public bool CheckBoxes { get; }

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000CD RID: 205 RVA: 0x00006527 File Offset: 0x00004727
		public bool Icons { get; }

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000CE RID: 206 RVA: 0x0000652F File Offset: 0x0000472F
		public int Count
		{
			get
			{
				return this.Rightbox.Count<SelectedItem<T>>() + this.Leftbox.Count<SelectedItem<T>>();
			}
		}

		// Token: 0x04000050 RID: 80
		private ASMultiBox<T> Leftbox;

		// Token: 0x04000051 RID: 81
		private ASMultiBox<T> Rightbox;

		// Token: 0x04000052 RID: 82
		private readonly string SelectedName = string.Empty;

		// Token: 0x04000053 RID: 83
		private readonly string UnSelectedName = string.Empty;

		// Token: 0x04000058 RID: 88
		protected List<SelectedItem<T>> GUIList = new List<SelectedItem<T>>();

		// Token: 0x0200001F RID: 31
		// (Invoke) Token: 0x06000109 RID: 265
		[NullableContext(0)]
		public delegate void ListItemHandler(ASMultiBox<T> ToBox, bool ToSelected);
	}
}
