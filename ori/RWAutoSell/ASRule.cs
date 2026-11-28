using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000011 RID: 17
	[NullableContext(1)]
	[Nullable(0)]
	public class ASRule : BaseRule, IExposable, IAdvanced
	{
		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000076 RID: 118 RVA: 0x000065FC File Offset: 0x000047FC
		// (set) Token: 0x06000077 RID: 119 RVA: 0x00006608 File Offset: 0x00004808
		public bool ShowAdvanced
		{
			get
			{
				return ASGameComp.GetSingleton.showadv;
			}
			set
			{
				ASGameComp.GetSingleton.showadv = value;
			}
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006615 File Offset: 0x00004815
		public int[] AdvancedNodes()
		{
			return new int[]
			{
				1
			};
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00006621 File Offset: 0x00004821
		public string[] AdvancedFields()
		{
			return null;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00006624 File Offset: 0x00004824
		public string[] AdvancedProperties()
		{
			return null;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006628 File Offset: 0x00004828
		public ASRule()
		{
			this.Nodes_ = new List<RuleRoot>
			{
				new RuleRoot("RWAutoSell.Trade".Translate()),
				new RuleRoot("RWAutoSell.Colony".Translate())
			};
		}

		// Token: 0x0600007C RID: 124 RVA: 0x0000668C File Offset: 0x0000488C
		public override IEnumerable<IRule> RuleChain()
		{
			ASRule.<RuleChain>d__15 <RuleChain>d__ = new ASRule.<RuleChain>d__15(-2);
			<RuleChain>d__.<>4__this = this;
			return <RuleChain>d__;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000669C File Offset: 0x0000489C
		public override void ChainedAdd(IRule Rule)
		{
			this.RuleChain_.Add((ASRule)Rule);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x000066B0 File Offset: 0x000048B0
		public override void ChainedReplace(IRule Origin, IRule Replacement)
		{
			ASRule asrule = Origin as ASRule;
			if (asrule != null)
			{
				ASRule asrule2 = Replacement as ASRule;
				if (asrule2 != null)
				{
					int num = this.RuleChain_.IndexOf(asrule);
					if (num == -1)
					{
						this.RuleChain_.Add(asrule2);
					}
					this.RuleChain_[num] = asrule2;
				}
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000066FC File Offset: 0x000048FC
		public override void ChainedInsert(int i, IRule Rule)
		{
			ASRule asrule = Rule as ASRule;
			if (asrule != null)
			{
				this.RuleChain_.Insert(i, asrule);
			}
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00006720 File Offset: 0x00004920
		public override void ChainedRemove(IRule Rule)
		{
			ASRule asrule = Rule as ASRule;
			if (asrule != null)
			{
				this.RuleChain_.Remove(asrule);
			}
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00006744 File Offset: 0x00004944
		public ASRule(List<FilterContainer> Containers, string startstring) : this()
		{
			this.RuleLabel = startstring + ": ";
			foreach (FilterContainer filterContainer in Containers)
			{
				this.RuleLabel = this.RuleLabel + filterContainer.GetFilter.Label + ", ";
				this.Nodes_[0].RootNode.Filters.Add(filterContainer);
			}
			if (this.RuleLabel.Count<char>() > startstring.Count<char>() + 2)
			{
				this.RuleLabel = this.RuleLabel.Substring(0, this.RuleLabel.Count<char>() - 2);
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000082 RID: 130 RVA: 0x00006814 File Offset: 0x00004A14
		public int SellCount
		{
			get
			{
				return this.sell;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000083 RID: 131 RVA: 0x0000681C File Offset: 0x00004A1C
		public int BuyPoint
		{
			get
			{
				return this.reorder;
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000084 RID: 132 RVA: 0x00006824 File Offset: 0x00004A24
		public int RestockLevel
		{
			get
			{
				return this.restock;
			}
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000682C File Offset: 0x00004A2C
		public override IRule DeepCopy()
		{
			ASRule asrule = new ASRule();
			asrule.RuleChain_ = (from rc in this.RuleChain_
			select rc.DeepCopy() as ASRule).ToList<ASRule>();
			asrule.TreatDefAsSeperate_ = this.TreatDefAsSeperate_;
			asrule.sell = this.sell;
			asrule.restock = this.restock;
			asrule.reorder = this.reorder;
			ASRule asrule2 = asrule;
			base.DeepCopyFromBase(asrule2);
			return asrule2;
		}

		// Token: 0x06000086 RID: 134 RVA: 0x000068AC File Offset: 0x00004AAC
		public ASRule MultipliedCopy(int multi)
		{
			ASRule asrule = (ASRule)this.DeepCopy();
			asrule.reorder *= multi;
			asrule.restock *= multi;
			asrule.sell *= multi;
			return asrule;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000068E3 File Offset: 0x00004AE3
		public void DrawPropertiesAdvanced(Rect inrect)
		{
			this.DrawProperties(inrect);
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000068EC File Offset: 0x00004AEC
		public override void DrawProperties(Rect inrect)
		{
			base.DrawProperties(inrect);
			ASTable astable = new ASTable(new Rect(inrect.x, inrect.y + 30f, inrect.width, 30f), 4, 1, 5f, 5f, true);
			int num = this.sell;
			int num2 = this.reorder;
			int num3 = this.restock;
			bool treatDefAsSeperate = this.TreatDefAsSeperate;
			ASWidgets.AlignedTextBox<int>(astable.GetRectangle(0, 0, -1, -1, false), "RWAutoSell.SellPoint".Translate(), ref num, ref this.sellcache, 0f, 2.1474836E+09f);
			ASWidgets.AlignedTextBox<int>(astable.GetRectangle(1, 0, -1, -1, false), "RWAutoSell.BuyPoint".Translate(), ref num2, ref this.reordercache, 0f, 2.1474836E+09f);
			ASWidgets.AlignedTextBox<int>(astable.GetRectangle(2, 0, -1, -1, false), "RWAutoSell.BuyLevel".Translate(), ref num3, ref this.restockcache, 0f, 2.1474836E+09f);
			Widgets.CheckboxLabeled(astable.GetRectangle(3, 0, -1, -1, false), "RWAutoSell.TreatDefSeperate".Translate(), ref treatDefAsSeperate, false, null, null, false, false);
			if (num != this.sell || num2 != this.reorder || num3 != this.restock || treatDefAsSeperate != this.TreatDefAsSeperate)
			{
				if (ASLibMod.nodeDialog != null)
				{
					ASLibMod.nodeDialog.RegisterChange();
				}
				this.sell = num;
				this.reorder = num2;
				this.restock = num3;
				this.TreatDefAsSeperate_ = treatDefAsSeperate;
			}
			TooltipHandler.TipRegion(astable.GetRectangle(0, 0, -1, -1, false), new TipSignal("RWAutoSell.DescSellPoint".Translate(), 1));
			TooltipHandler.TipRegion(astable.GetRectangle(1, 0, -1, -1, false), new TipSignal("RWAutoSell.DescBuyPoint".Translate(), 2));
			TooltipHandler.TipRegion(astable.GetRectangle(2, 0, -1, -1, false), new TipSignal("RWAutoSell.DescBuyLevel".Translate(), 3));
			TooltipHandler.TipRegion(astable.GetRectangle(3, 0, -1, -1, false), new TipSignal("RWAutoSell.DescTreatDefSeperate".Translate(), 4));
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00006AF0 File Offset: 0x00004CF0
		public bool TreatDefAsSeperate
		{
			get
			{
				return this.TreatDefAsSeperate_;
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00006AF8 File Offset: 0x00004CF8
		public void ToggleDefBehavoir()
		{
			this.TreatDefAsSeperate_ = !this.TreatDefAsSeperate_;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00006B0C File Offset: 0x00004D0C
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<bool>(ref this.TreatDefAsSeperate_, "TreatDefAsSeperate", true, false);
			Scribe_Values.Look<int>(ref this.sell, "RuleSell", 0, false);
			Scribe_Values.Look<int>(ref this.restock, "RuleRestock", 0, false);
			Scribe_Values.Look<int>(ref this.reorder, "RuleReorder", 0, false);
			Scribe_Collections.Look<ASRule>(ref this.RuleChain_, "Chain", LookMode.Undefined, Array.Empty<object>());
			if (this.Nodes_ == null || this.Nodes_.Count == 0)
			{
				this.Nodes_ = new List<RuleRoot>
				{
					new RuleRoot("RWAutoSell.Trade".Translate()),
					new RuleRoot("RWAutoSell.Colony".Translate())
				};
			}
			else if (this.Nodes_.Count == 1)
			{
				this.Nodes_.Add(new RuleRoot("RWAutoSell.Colony".Translate()));
			}
			if (this.RuleChain_ == null)
			{
				this.RuleChain_ = new List<ASRule>();
			}
		}

		// Token: 0x04000052 RID: 82
		private string sellcache;

		// Token: 0x04000053 RID: 83
		private string restockcache;

		// Token: 0x04000054 RID: 84
		private string reordercache;

		// Token: 0x04000055 RID: 85
		[AutoSellExportableValue(SaveName = "TreatDefAsSeperate")]
		private bool TreatDefAsSeperate_ = true;

		// Token: 0x04000056 RID: 86
		[AutoSellExportableValue(SaveName = "RuleSell")]
		private int sell;

		// Token: 0x04000057 RID: 87
		[AutoSellExportableValue(SaveName = "RuleRestock")]
		private int restock;

		// Token: 0x04000058 RID: 88
		[AutoSellExportableValue(SaveName = "RuleReorder")]
		private int reorder;

		// Token: 0x04000059 RID: 89
		[AutoSellExportableList(SaveName = "Chain")]
		protected List<ASRule> RuleChain_ = new List<ASRule>();
	}
}
