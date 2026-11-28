using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000041 RID: 65
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterIdeo : IBaseFilter, IFilterReq
	{
		// Token: 0x1700007A RID: 122
		// (get) Token: 0x06000225 RID: 549 RVA: 0x00014518 File Offset: 0x00012718
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x06000226 RID: 550 RVA: 0x00014520 File Offset: 0x00012720
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FIdeo;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x06000227 RID: 551 RVA: 0x00014527 File Offset: 0x00012727
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterIdeo".Translate();
			}
		}

		// Token: 0x1700007D RID: 125
		// (get) Token: 0x06000228 RID: 552 RVA: 0x00014538 File Offset: 0x00012738
		public string ModRequirement { get; } = "ludeon.rimworld.ideology";

		// Token: 0x06000229 RID: 553 RVA: 0x00014540 File Offset: 0x00012740
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (ModsConfig.IdeologyActive)
			{
				Pawn pawn = t.AnyThing as Pawn;
				if (pawn != null && pawn.ideo != null && pawn.ideo.Ideo != null && Data.Contains("ide." + pawn.ideo.Ideo.name))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x000145A0 File Offset: 0x000127A0
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			if (ModsConfig.IdeologyActive)
			{
				Pawn pawn = t.AnyThing as Pawn;
				if (pawn != null && pawn.ideo != null && pawn.ideo.Ideo != null)
				{
					list.Add("ide." + pawn.ideo.Ideo.name);
				}
			}
			return list;
		}

		// Token: 0x0600022B RID: 555 RVA: 0x000145FF File Offset: 0x000127FF
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0600022C RID: 556 RVA: 0x00014602 File Offset: 0x00012802
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			this.Box.DrawListBox(inrect);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x00014610 File Offset: 0x00012810
		public void RenderStarted(List<string> Data, Map map)
		{
			this.Box = new ASMultiBox<string>(25f, 4f, true, true);
			this.Box.AddRange(from el in Current.Game.World.ideoManager.IdeosInViewOrder
			select new SelectedItem<string>
			{
				Item = "ide." + el.name,
				Icon = el.Icon,
				color = el.Color,
				label = el.name,
				surpresslabelupdate = true,
				tooltip = el.description,
				Selected = Data.Contains("ide." + el.name)
			});
		}

		// Token: 0x0600022E RID: 558 RVA: 0x00014671 File Offset: 0x00012871
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			Data.AddRange(this.Box.GetSelectedItems);
		}

		// Token: 0x0400012D RID: 301
		private ASMultiBox<string> Box;
	}
}
