using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000005 RID: 5
	[NullableContext(1)]
	[Nullable(0)]
	public class CatTree
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000027DE File Offset: 0x000009DE
		// (set) Token: 0x06000016 RID: 22 RVA: 0x000027E6 File Offset: 0x000009E6
		public ThingCategoryDef Category
		{
			get
			{
				return this.Category_;
			}
			set
			{
				if (value != null)
				{
					this.label = value.LabelCap;
					this.Category_ = value;
				}
			}
		}

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000017 RID: 23 RVA: 0x00002803 File Offset: 0x00000A03
		// (set) Token: 0x06000018 RID: 24 RVA: 0x0000280B File Offset: 0x00000A0B
		public int DefCount { get; private set; }

		// Token: 0x06000019 RID: 25 RVA: 0x00002814 File Offset: 0x00000A14
		private bool AddBranch(ThingCategoryDef Child)
		{
			bool result;
			try
			{
				CatTree item = new CatTree(Child, this.Depth + 1, false);
				this.Children.Add(item);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002858 File Offset: 0x00000A58
		public CatTree()
		{
			this.Depth = 1;
			this.Category = ThingCategoryDef.Named("Root");
			this.CreateList();
		}

		// Token: 0x0600001B RID: 27 RVA: 0x000028AC File Offset: 0x00000AAC
		public CatTree(ThingCategoryDef cat, int Depth = 1, bool pseudotree = false)
		{
			this.Depth = Depth;
			this.Category = cat;
			if (!pseudotree)
			{
				this.CreateList();
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000028FC File Offset: 0x00000AFC
		private void CreateList()
		{
			foreach (ThingCategoryDef thingCategoryDef in this.Category.childCategories)
			{
				this.DefCount_ = thingCategoryDef.DescendantThingDefs.Count<ThingDef>();
				if (this.DefCount_ != 0)
				{
					this.AddBranch(thingCategoryDef);
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002970 File Offset: 0x00000B70
		public int CountExpandedCats()
		{
			int num = this.Children.Count;
			if (num != 0 && this.expanded)
			{
				foreach (CatTree catTree in this.Children)
				{
					num += catTree.CountExpandedCats();
				}
			}
			return num;
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000029E0 File Offset: 0x00000BE0
		public void UpdateRevealStatus(string namebuilder = "")
		{
			string text;
			if (namebuilder != "")
			{
				text = namebuilder + "/" + this.Category_.defName;
			}
			else
			{
				text = this.Category_.defName;
			}
			if (ASWidgetsMod.GetSettings().Collapsedlist.Contains(text))
			{
				this.expanded = false;
			}
			if (this.Children.Count > 0)
			{
				foreach (CatTree catTree in this.Children)
				{
					catTree.UpdateRevealStatus(text);
				}
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00002A8C File Offset: 0x00000C8C
		public void SaveRevealStatus()
		{
			ASWidgetsMod.GetSettings().Collapsedlist.Clear();
			string defName = this.Category_.defName;
			if (this.Children.Count > 0)
			{
				foreach (CatTree catTree in this.Children)
				{
					catTree.SaveRevealStatus(defName);
				}
			}
			ASWidgetsMod.GetSettings().Write();
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002B10 File Offset: 0x00000D10
		public void SaveRevealStatus(string namebuilder)
		{
			string text = namebuilder + "/" + this.Category_.defName;
			if (!this.expanded)
			{
				ASWidgetsMod.GetSettings().Collapsedlist.Add(text);
			}
			if (this.Children.Count > 0)
			{
				foreach (CatTree catTree in this.Children)
				{
					catTree.SaveRevealStatus(text);
				}
			}
		}

		// Token: 0x04000005 RID: 5
		public ThingCategoryDef Category_;

		// Token: 0x04000006 RID: 6
		public List<CatTree> Children = new List<CatTree>();

		// Token: 0x04000007 RID: 7
		public int Depth = 1;

		// Token: 0x04000008 RID: 8
		public string label = string.Empty;

		// Token: 0x04000009 RID: 9
		public bool expanded = true;

		// Token: 0x0400000A RID: 10
		private int DefCount_;
	}
}
