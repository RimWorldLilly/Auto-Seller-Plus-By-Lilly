using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using RWASWidgets;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x02000056 RID: 86
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterWorkTag : IBaseFilter, IWikiHelp
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x06000322 RID: 802 RVA: 0x0001D24A File Offset: 0x0001B44A
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FWorkTag;
			}
		}

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x06000323 RID: 803 RVA: 0x0001D251 File Offset: 0x0001B451
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x06000324 RID: 804 RVA: 0x0001D259 File Offset: 0x0001B459
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterWorkTag".Translate();
			}
		}

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x06000325 RID: 805 RVA: 0x0001D26A File Offset: 0x0001B46A
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/WorkTag-Filter";
			}
		}

		// Token: 0x06000326 RID: 806 RVA: 0x0001D274 File Offset: 0x0001B474
		public bool CheckItem(Transferable t, List<string> Data)
		{
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.Humanlike)
			{
				bool result = true;
				foreach (string value in Data)
				{
					WorkTags w = WorkTags.None;
					try
					{
						w = (WorkTags)Enum.Parse(typeof(WorkTags), value, true);
					}
					catch
					{
						continue;
					}
					if (pawn.WorkTagIsDisabled(w))
					{
						result = false;
					}
				}
				return result;
			}
			return false;
		}

		// Token: 0x06000327 RID: 807 RVA: 0x0001D314 File Offset: 0x0001B514
		public void RenderStarted(List<string> Data, Map map)
		{
			this.SelectedBox = new ASMultiBox<string>(20f, 5f, true, false);
			foreach (string text in Enum.GetNames(typeof(WorkTags)))
			{
				this.SelectedBox.Add(new SelectedItem<string>
				{
					Item = text,
					label = ("WorkTag" + text).Translate(),
					Selected = Data.Contains(text),
					surpresslabelupdate = true
				});
			}
			this.AllowRender = true;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x0001D3A8 File Offset: 0x0001B5A8
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			foreach (string item in this.SelectedBox.GetSelectedItems)
			{
				Data.Add(item);
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x0001D410 File Offset: 0x0001B610
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				this.SelectedBox.DrawListBox(new Rect(inrect.x + 4f, inrect.y + 4f, inrect.width - 4f, inrect.height - 4f));
			}
		}

		// Token: 0x0600032A RID: 810 RVA: 0x0001D46C File Offset: 0x0001B66C
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.Humanlike)
			{
				foreach (string text in Enum.GetNames(typeof(WorkTags)))
				{
					WorkTags w = (WorkTags)Enum.Parse(typeof(WorkTags), text);
					if (!pawn.WorkTagIsDisabled(w))
					{
						list.Add(text);
					}
				}
			}
			return list;
		}

		// Token: 0x0600032B RID: 811 RVA: 0x0001D4E9 File Offset: 0x0001B6E9
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x0400019C RID: 412
		private ASMultiBox<string> SelectedBox;

		// Token: 0x0400019D RID: 413
		private bool AllowRender;
	}
}
