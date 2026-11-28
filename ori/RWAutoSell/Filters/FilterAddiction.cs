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
	// Token: 0x02000036 RID: 54
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterAddiction : IBaseFilter
	{
		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600019B RID: 411 RVA: 0x0000FDC5 File Offset: 0x0000DFC5
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FChem;
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600019C RID: 412 RVA: 0x0000FDCC File Offset: 0x0000DFCC
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagPeople"
		};

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600019D RID: 413 RVA: 0x0000FDD4 File Offset: 0x0000DFD4
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterAddiction".Translate();
			}
		}

		// Token: 0x0600019E RID: 414 RVA: 0x0000FDE8 File Offset: 0x0000DFE8
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (t.AnyThing != null)
			{
				Pawn pawn = t.AnyThing as Pawn;
				if (pawn != null)
				{
					using (List<Hediff>.Enumerator enumerator = pawn.health.hediffSet.hediffs.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Hediff hdif = enumerator.Current;
							if (Data.Any((string x) => x == "hdf." + hdif.def.defName))
							{
								return true;
							}
						}
					}
					return false;
				}
			}
			return false;
		}

		// Token: 0x0600019F RID: 415 RVA: 0x0000FE7C File Offset: 0x0000E07C
		public List<string> DataFromTradeable(Transferable t)
		{
			return new List<string>();
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x0000FE83 File Offset: 0x0000E083
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000FE88 File Offset: 0x0000E088
		public void RenderStarted(List<string> Data, Map map)
		{
			this.BoxList = new ASMultiBox<HediffDef>(30f, 4f, true, false);
			using (IEnumerator<HediffDef> enumerator = (from el in DefDatabase<ChemicalDef>.AllDefs
			where el.addictionHediff != null
			select el.addictionHediff).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					HediffDef ad = enumerator.Current;
					SelectedItem<HediffDef> item = new SelectedItem<HediffDef>
					{
						Item = ad,
						label = ad.LabelCap,
						Selected = Data.Any((string x) => x == "hdf." + ad.defName)
					};
					this.BoxList.Add(item);
				}
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000FF84 File Offset: 0x0000E184
		public void RenderStopped(List<string> Data, Map map)
		{
			Data.Clear();
			foreach (HediffDef hediffDef in this.BoxList.GetSelectedItems)
			{
				Data.Add("hdf." + hediffDef.defName);
			}
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000FFF4 File Offset: 0x0000E1F4
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			this.BoxList.DrawListBox(inrect.ContractedBy(4f));
		}

		// Token: 0x040000F2 RID: 242
		private ASMultiBox<HediffDef> BoxList;
	}
}
