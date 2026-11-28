using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200001E RID: 30
	[NullableContext(1)]
	[Nullable(0)]
	public class ASDialog_SorterSelect : Window
	{
		// Token: 0x060000E0 RID: 224 RVA: 0x00009E80 File Offset: 0x00008080
		public ASDialog_SorterSelect(IRule rule) : base(null)
		{
			this.Rule = rule;
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseButton = true;
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x00009EA5 File Offset: 0x000080A5
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(350f, 500f);
			}
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00009EB8 File Offset: 0x000080B8
		public override void PreOpen()
		{
			base.PreOpen();
			this.Sortersbox = new ASListBox<IRuleSorter>(30f, 4f, false, false);
			this.Sortersbox.Add(new SelectedItem<IRuleSorter>
			{
				Item = null,
				label = "RWAutoSell.Unsorted".Translate()
			});
			foreach (IRuleSorter ruleSorter in from el in ASLibMod.GetSingleton.GetBaseSorters
			orderby el.Label.ToLowerInvariant()
			select el)
			{
				this.Sortersbox.Add(new SelectedItem<IRuleSorter>
				{
					Item = ruleSorter,
					label = ruleSorter.Label
				});
			}
			IRuleSorter sorter = this.Rule.Sorter;
			for (int i = 0; i < this.Sortersbox.Count<SelectedItem<IRuleSorter>>(); i++)
			{
				if (this.Sortersbox[i].Item == sorter)
				{
					this.Sortersbox.ChangeSelected(i);
					break;
				}
			}
			this.Sortersbox.OnClick += new ASListBox<IRuleSorter>.ListItemHandler(this.sorterclick);
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x00009FF0 File Offset: 0x000081F0
		public override void PostClose()
		{
			base.PostClose();
			if (this.Sortersbox != null)
			{
				this.Sortersbox.OnClick -= new ASListBox<IRuleSorter>.ListItemHandler(this.sorterclick);
			}
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000A017 File Offset: 0x00008217
		private void sorterclick(object Sender, GUIListEventArgs e)
		{
			if (this.Sortersbox.GetSelectedItems.Count != 0)
			{
				this.Rule.Sorter = this.Sortersbox.GetSelected;
			}
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000A044 File Offset: 0x00008244
		public override void DoWindowContents(Rect inRect)
		{
			GUI.color = Color.white;
			ASRectMonitor d = new ASRectMonitor(inRect);
			Rect rect = d.NextFromTop(24f);
			Rect rect2 = d.NextFromTop(24f);
			if (Widgets.RadioButtonLabeled(rect, "RWAutoSell.Ascending".Translate(), this.Rule.SortDirection, false))
			{
				this.Rule.SortDirection = true;
			}
			if (Widgets.RadioButtonLabeled(rect2, "RWAutoSell.Descending".Translate(), !this.Rule.SortDirection, false))
			{
				this.Rule.SortDirection = false;
			}
			d.NextFromBottom(Window.CloseButSize.y);
			this.Sortersbox.DrawListBox(d);
		}

		// Token: 0x04000099 RID: 153
		private ASListBox<IRuleSorter> Sortersbox;

		// Token: 0x0400009A RID: 154
		private IRule Rule;
	}
}
