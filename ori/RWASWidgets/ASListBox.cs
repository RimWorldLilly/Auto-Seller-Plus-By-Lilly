using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000010 RID: 16
	[NullableContext(1)]
	[Nullable(0)]
	public class ASListBox<[Nullable(2)] T> : IGUIList<T>, IList<SelectedItem<T>>, ICollection<SelectedItem<T>>, IEnumerable<SelectedItem<T>>, IEnumerable
	{
		// Token: 0x14000004 RID: 4
		// (add) Token: 0x0600005F RID: 95 RVA: 0x0000454C File Offset: 0x0000274C
		// (remove) Token: 0x06000060 RID: 96 RVA: 0x00004584 File Offset: 0x00002784
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
		public event ASListBox<T>.ListItemHandler OnClick;

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000061 RID: 97 RVA: 0x000045B9 File Offset: 0x000027B9
		protected float Height { get; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000062 RID: 98 RVA: 0x000045C1 File Offset: 0x000027C1
		protected float Padding { get; }

		// Token: 0x06000063 RID: 99 RVA: 0x000045CC File Offset: 0x000027CC
		public ASListBox(float Height = 25f, float Padding = 4f, bool ShowCheckboxes = true, bool ShowIcons = true)
		{
			this.Height = Height;
			this.Padding = Padding;
			this.CheckBoxes = ShowCheckboxes;
			this.Icons = ShowIcons;
			this.OnClick += this.OnClickEvent;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00004639 File Offset: 0x00002839
		private void OnClickEvent(IGUIList<T> sender, GUIListEventArgs e)
		{
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000065 RID: 101 RVA: 0x0000463B File Offset: 0x0000283B
		public int Count
		{
			get
			{
				return this.GUIList.Count<SelectedItem<T>>();
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004648 File Offset: 0x00002848
		public bool CheckBoxes { get; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000067 RID: 103 RVA: 0x00004650 File Offset: 0x00002850
		public bool Icons { get; }

		// Token: 0x1700001A RID: 26
		public SelectedItem<T> this[int i]
		{
			get
			{
				return this.GUIList[i];
			}
			set
			{
				this.GUIList[i] = value;
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00004678 File Offset: 0x00002878
		public virtual T GetSelected
		{
			get
			{
				if (this.Selected != -1)
				{
					return this[this.Selected].Item;
				}
				return default(T);
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600006B RID: 107 RVA: 0x000046A9 File Offset: 0x000028A9
		public int GetSelectedIndex
		{
			get
			{
				return this.Selected;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000046B1 File Offset: 0x000028B1
		public virtual List<T> GetSelectedItems
		{
			get
			{
				if (this.Selected != -1)
				{
					return new T[]
					{
						this.GetSelected
					}.ToList<T>();
				}
				return new List<T>();
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x0600006D RID: 109 RVA: 0x000046DA File Offset: 0x000028DA
		public bool IsReadOnly
		{
			get
			{
				return ((ICollection<SelectedItem<T>>)this.GUIList).IsReadOnly;
			}
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000046E8 File Offset: 0x000028E8
		public virtual void ChangeSelected(int i)
		{
			this.Selected = i;
			TaggedString taggedString = string.Empty;
			if (this.Selected != -1)
			{
				taggedString = this[i].label;
			}
			this.OnClick(this, new GUIListEventArgs(i, taggedString));
		}

		// Token: 0x0600006F RID: 111 RVA: 0x0000473C File Offset: 0x0000293C
		public virtual void DrawListBox(Rect outrect)
		{
			GUI.color = Color.grey;
			Widgets.DrawHighlight(outrect);
			Widgets.DrawBox(outrect, 1, null);
			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			if (this.Count != 0)
			{
				Rect viewRect = new Rect(0f, 0f, outrect.width - 16f, (float)this.Count * (this.Height + this.Padding));
				float num = this.Icons ? (this.Height + this.Padding) : this.Padding;
				float width = viewRect.width - num - this.Padding * 2f;
				Widgets.BeginScrollView(outrect, ref this.scroll, viewRect, true);
				float num2 = 0f;
				for (int i = 0; i < this.Count; i++)
				{
					Rect rect = new Rect(this.Padding, num2, outrect.width - this.Padding * 2f, this.Height + this.Padding);
					SelectedItem<T> selectedItem = this[i];
					Rect rect2 = new Rect(num, num2, width, this.Height);
					Rect butRect = new Rect(this.Padding, num2, this.Height, this.Height);
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
					if (this.CheckBoxes)
					{
						bool flag = false;
						if (i == this.Selected)
						{
							flag = true;
						}
						if (this.Icons && ASWidgets.ButtonImageNoBad(butRect, selectedItem.Icon, selectedItem.color))
						{
							flag = true;
						}
						Widgets.CheckboxLabeled(rect2, this[i].label, ref flag, false, null, null, false, false);
						if (flag & this.Selected != i)
						{
							this.ChangeSelected(i);
						}
					}
					else
					{
						if (Widgets.ButtonInvisible(rect, true) && this.Selected != i)
						{
							this.ChangeSelected(i);
						}
						if (this.Icons)
						{
							ASWidgets.ButtonImageNoBad(butRect, selectedItem.Icon, selectedItem.color);
						}
						if (i == this.Selected)
						{
							GUI.color = Color.grey;
							Widgets.DrawHighlightSelected(rect);
							GUI.color = Color.white;
						}
						Widgets.Label(rect2, selectedItem.label);
					}
					Widgets.DrawHighlightIfMouseover(rect);
					num2 += this.Height + this.Padding;
				}
				Widgets.EndScrollView();
			}
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000049D2 File Offset: 0x00002BD2
		protected void RaiseClickEvent(int i, string arg)
		{
			this.OnClick(this, new GUIListEventArgs(i, arg));
		}

		// Token: 0x06000071 RID: 113 RVA: 0x000049E8 File Offset: 0x00002BE8
		public int GetItemIndex(T Item)
		{
			if (this.Count == 0)
			{
				return -1;
			}
			int i = 0;
			while (i < this.Count)
			{
				if (Item != null)
				{
					try
					{
						T item = this[i].Item;
						if (item.Equals(Item))
						{
							return i;
						}
						goto IL_60;
					}
					catch
					{
						goto IL_60;
					}
					goto IL_41;
				}
				goto IL_41;
				IL_60:
				i++;
				continue;
				IL_41:
				try
				{
					if (this[i].Item == null)
					{
						return i;
					}
				}
				catch
				{
					return i;
				}
				goto IL_60;
			}
			return -1;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004A84 File Offset: 0x00002C84
		public IEnumerator<SelectedItem<T>> GetEnumerator()
		{
			return this.GUIList.GetEnumerator();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004A96 File Offset: 0x00002C96
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004A9E File Offset: 0x00002C9E
		public void Clear()
		{
			this.Selected = -1;
			this.GUIList.Clear();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004AB2 File Offset: 0x00002CB2
		public void Add(T Item)
		{
			this.GUIList.Add(new SelectedItem<T>
			{
				Item = Item
			});
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004ACB File Offset: 0x00002CCB
		public void Add(SelectedItem<T> Item)
		{
			this.GUIList.Add(Item);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00004AD9 File Offset: 0x00002CD9
		public void AddRange(IEnumerable<SelectedItem<T>> Items)
		{
			this.GUIList.AddRange(Items);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00004AE8 File Offset: 0x00002CE8
		public void AddRange(IEnumerable<T> Items)
		{
			foreach (T item in Items)
			{
				this.Add(item);
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00004B30 File Offset: 0x00002D30
		public void AddRange(params T[] Items)
		{
			foreach (T item in Items)
			{
				this.Add(item);
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00004B5C File Offset: 0x00002D5C
		public int IndexOf(SelectedItem<T> item)
		{
			return this.GUIList.IndexOf(item);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00004B6A File Offset: 0x00002D6A
		public void Insert(int index, SelectedItem<T> item)
		{
			this.GUIList.Insert(index, item);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00004B79 File Offset: 0x00002D79
		public void Insert(int index, T item)
		{
			this.GUIList.Insert(index, new SelectedItem<T>
			{
				Item = item
			});
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00004B93 File Offset: 0x00002D93
		public void RemoveAt(int index)
		{
			this.GUIList.RemoveAt(index);
			if (this.Selected > index)
			{
				this.Selected--;
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00004BB8 File Offset: 0x00002DB8
		public bool Contains(SelectedItem<T> item)
		{
			return this.GUIList.Contains(item);
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00004BC8 File Offset: 0x00002DC8
		public bool Contains(T Item)
		{
			return this.GUIList.Any(delegate(SelectedItem<T> x)
			{
				T item = x.Item;
				return item.Equals(Item);
			});
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00004BF9 File Offset: 0x00002DF9
		public void CopyTo(SelectedItem<T>[] array, int arrayIndex)
		{
			((ICollection<SelectedItem<T>>)this.GUIList).CopyTo(array, arrayIndex);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00004C08 File Offset: 0x00002E08
		public bool Remove(SelectedItem<T> item)
		{
			return this.GUIList.Remove(item);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00004C18 File Offset: 0x00002E18
		public bool Remove(T item)
		{
			int itemIndex = this.GetItemIndex(item);
			return this.GUIList.Remove(this[itemIndex]);
		}

		// Token: 0x04000037 RID: 55
		protected int Selected = -1;

		// Token: 0x04000038 RID: 56
		protected List<SelectedItem<T>> GUIList = new List<SelectedItem<T>>();

		// Token: 0x04000039 RID: 57
		protected Vector2 scroll = Vector2.zero;

		// Token: 0x0400003C RID: 60
		protected int hovertime = -1;

		// Token: 0x0400003D RID: 61
		protected int hoverindex = -1;

		// Token: 0x0200001C RID: 28
		// (Invoke) Token: 0x060000FE RID: 254
		[NullableContext(0)]
		public delegate void ListItemHandler(IGUIList<T> sender, GUIListEventArgs e);
	}
}
