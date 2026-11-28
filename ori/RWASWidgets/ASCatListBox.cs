using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RWASWidgets
{
	// Token: 0x02000009 RID: 9
	[NullableContext(1)]
	[Nullable(0)]
	public class ASCatListBox
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000034 RID: 52 RVA: 0x00002DC4 File Offset: 0x00000FC4
		// (remove) Token: 0x06000035 RID: 53 RVA: 0x00002DFC File Offset: 0x00000FFC
		public event ASCatListBox.ListItemHandler OnClick;

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000036 RID: 54 RVA: 0x00002E31 File Offset: 0x00001031
		private float Height { get; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000037 RID: 55 RVA: 0x00002E39 File Offset: 0x00001039
		private float Padding { get; }

		// Token: 0x06000038 RID: 56 RVA: 0x00002E44 File Offset: 0x00001044
		public ASCatListBox(float Height, float Padding)
		{
			this.Height = Height;
			this.Padding = Padding;
			this.CheckBoxes = 0;
			this.Icons = 1;
			this.OnClick += this.OnClickEvent;
			this.CategoriesList.UpdateRevealStatus("");
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002EB6 File Offset: 0x000010B6
		private void OnClickEvent(object Sender, GUICatListEventArgs e)
		{
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600003A RID: 58 RVA: 0x00002EB8 File Offset: 0x000010B8
		public int Count
		{
			get
			{
				return this.CategoriesList.CountExpandedCats();
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600003B RID: 59 RVA: 0x00002EC5 File Offset: 0x000010C5
		public bool CheckBoxes { get; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600003C RID: 60 RVA: 0x00002ECD File Offset: 0x000010CD
		public bool Icons { get; }

		// Token: 0x0600003D RID: 61 RVA: 0x00002ED5 File Offset: 0x000010D5
		public void ChangeSelected(ThingCategoryDef CategoryDef)
		{
			if (!this.NarrowSelected(this.CategoriesList, CategoryDef))
			{
				this.Selected.Item = new CatTree(CategoryDef, 1, true);
				this.OnClick(this, new GUICatListEventArgs(this.Selected.Item));
			}
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002F18 File Offset: 0x00001118
		private bool NarrowSelected(CatTree cat, ThingCategoryDef CategoryDef)
		{
			foreach (CatTree catTree in cat.Children)
			{
				if (catTree.Category == CategoryDef)
				{
					this.ChangeSelected(catTree);
					return true;
				}
				if (catTree.Children.Count<CatTree>() != 0 && this.NarrowSelected(catTree, CategoryDef))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00002F98 File Offset: 0x00001198
		public void ChangeSelected(CatTree Category)
		{
			SoundDefOf.Click.PlayOneShotOnCamera(null);
			this.Selected.Selected = true;
			this.Selected.Item = Category;
			this.OnClick(this, new GUICatListEventArgs(Category));
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00002FD0 File Offset: 0x000011D0
		public void DrawListBox(Rect outrect)
		{
			GUI.color = Color.grey;
			Widgets.DrawHighlight(outrect);
			Widgets.DrawBox(outrect, 1, null);
			GUI.color = Color.white;
			int count = this.Count;
			if (count != 0)
			{
				Rect rect = new Rect(0f, 0f, outrect.width - 16f, (float)count * (this.Height + this.Padding));
				float num = this.Icons ? (this.Height + this.Padding) : this.Padding;
				float width = rect.width;
				float padding = this.Padding;
				Widgets.BeginScrollView(outrect, ref this.scroll, rect, true);
				int num2 = 0;
				if (this.CategoriesList.Children.Count != 0)
				{
					this.NarrowCats(this.CategoriesList, ref num2, rect);
				}
				Widgets.EndScrollView();
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000041 RID: 65 RVA: 0x0000309D File Offset: 0x0000129D
		public ThingCategoryDef GetSelected
		{
			get
			{
				if (this.Selected.Item != null)
				{
					return this.Selected.Item.Category;
				}
				return null;
			}
		}

		// Token: 0x06000042 RID: 66 RVA: 0x000030C0 File Offset: 0x000012C0
		private void NarrowCats(CatTree category, ref int y, Rect Treerect)
		{
			foreach (CatTree catTree in category.Children)
			{
				GUI.color = Color.grey;
				Rect rect = new Rect(Treerect.x + 10f + (float)category.Depth * (this.Padding * 3f), Treerect.y + (this.Height + this.Padding) * (float)y, 130f, this.Height);
				if (Mouse.IsOver(rect))
				{
					Widgets.DrawHighlight(rect);
				}
				if (catTree == this.Selected.Item)
				{
					Widgets.DrawHighlightSelected(rect);
				}
				GUI.color = Color.white;
				if (catTree.Category.iconPath != string.Empty && catTree.Category.iconPath != null)
				{
					Texture2D tex = ContentFinder<Texture2D>.Get(catTree.Category.iconPath, true);
					if (Widgets.ButtonImage(new Rect(Treerect.x + 10f + (float)category.Depth * (this.Padding * 3f), Treerect.y + (this.Height + this.Padding) * (float)y, this.Height, this.Height), tex, true, null))
					{
						this.ChangeSelected(catTree);
					}
				}
				Widgets.Label(new Rect(Treerect.x + this.Height + (float)category.Depth * (this.Padding * 4f), Treerect.y + (this.Height + this.Padding) * (float)y, Treerect.width + (float)category.Depth * (this.Padding * 4f), this.Height), catTree.Category.LabelCap);
				if (Widgets.ButtonInvisible(rect, true))
				{
					this.ChangeSelected(catTree);
				}
				GUI.color = Color.grey;
				if (catTree.Children.Count > 0)
				{
					Texture2D tex2 = (!catTree.expanded) ? ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Reveal", true) : ContentFinder<Texture2D>.Get("UI/Buttons/Dev/Collapse", true);
					if (Widgets.ButtonImage(new Rect(Treerect.x + (float)category.Depth * (this.Padding * 3f) - this.Padding, Treerect.y + (this.Height + this.Padding) * (float)y, this.Height, this.Height), tex2, true, null))
					{
						if (catTree.expanded)
						{
							catTree.expanded = false;
						}
						else
						{
							catTree.expanded = true;
						}
						this.CategoriesList.SaveRevealStatus();
					}
				}
				y++;
				if (catTree.Children.Count > 0 && catTree.expanded)
				{
					this.NarrowCats(catTree, ref y, Treerect);
				}
			}
		}

		// Token: 0x04000015 RID: 21
		private CatTree CategoriesList = new CatTree();

		// Token: 0x04000017 RID: 23
		private SelectedItem<CatTree> Selected = new SelectedItem<CatTree>();

		// Token: 0x04000018 RID: 24
		private Vector2 scroll = Vector2.zero;

		// Token: 0x0200001A RID: 26
		// (Invoke) Token: 0x060000F7 RID: 247
		[NullableContext(0)]
		public delegate void ListItemHandler(object Sender, GUICatListEventArgs e);
	}
}
