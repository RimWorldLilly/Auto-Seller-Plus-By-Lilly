using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200000B RID: 11
	public class ASMergerControls : Window
	{
		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600004E RID: 78 RVA: 0x0000440B File Offset: 0x0000260B
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(350f, 500f);
			}
		}

		// Token: 0x0600004F RID: 79 RVA: 0x0000441C File Offset: 0x0000261C
		public ASMergerControls() : base(null)
		{
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.doCloseX = true;
			this.doCloseButton = true;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00004458 File Offset: 0x00002658
		public override void PreOpen()
		{
			this.mergebox.Clear();
			foreach (IUnmerge unmerge in ASLibMod.GetSingleton.GetBaseUnMergers)
			{
				this.mergebox.Add(new SelectedItem<string>
				{
					surpresslabelupdate = true,
					Item = unmerge.GetType().FullName,
					label = unmerge.Label
				});
			}
			if (ASLibMod.GetSettings().EnabledUnmergers != null && ASLibMod.GetSettings().EnabledUnmergers.Count > 0)
			{
				using (IEnumerator<SelectedItem<string>> enumerator2 = this.mergebox.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SelectedItem<string> item = enumerator2.Current;
						if (ASLibMod.GetSettings().EnabledUnmergers.Any((string x) => x == item.Item))
						{
							item.Selected = true;
						}
					}
				}
			}
			base.PreOpen();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00004574 File Offset: 0x00002774
		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Medium;
			Rect rect = new Rect(inRect.x, inRect.y, inRect.width, 60f);
			Widgets.Label(rect, "RWAutoSell.MergeControl".Translate());
			Text.Font = GameFont.Small;
			Rect rect2 = new Rect(inRect.x, rect.yMax, inRect.width, 120f);
			Widgets.Label(rect2, "RWAutoSell.MergeControlDesc".Translate());
			Rect outrect = new Rect(inRect.x, rect2.yMax, inRect.width, inRect.height - (rect2.yMax + 40f));
			this.mergebox.DrawListBox(outrect);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000462C File Offset: 0x0000282C
		public override void PreClose()
		{
			base.PreClose();
			if (ASLibMod.GetSettings().EnabledUnmergers == null)
			{
				ASLibMod.GetSettings().EnabledUnmergers = new List<string>();
			}
			else
			{
				ASLibMod.GetSettings().EnabledUnmergers.Clear();
			}
			if (this.mergebox.GetSelectedItems.Count > 0)
			{
				ASLibMod.GetSettings().EnabledUnmergers.AddRange(this.mergebox.GetSelectedItems);
			}
			ASLibMod.GetSettings().Write();
		}

		// Token: 0x0400001E RID: 30
		[Nullable(1)]
		private ASMultiBox<string> mergebox = new ASMultiBox<string>(25f, 4f, true, false);
	}
}
