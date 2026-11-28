using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using RWASFilterLib;
using UnityEngine;
using Verse;

namespace RWAutoSell.Filters
{
	// Token: 0x0200003B RID: 59
	[NullableContext(1)]
	[Nullable(0)]
	public class FilterBonded : IBaseFilter, IWikiHelp
	{
		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x00011F2F File Offset: 0x0001012F
		public Texture2D MiniGraphic
		{
			get
			{
				return ASTextures.FBond;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x00011F36 File Offset: 0x00010136
		public string Label
		{
			get
			{
				return "RWAutoSell.FilterBonded".Translate();
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060001D5 RID: 469 RVA: 0x00011F47 File Offset: 0x00010147
		public string HelpPage
		{
			get
			{
				return "https://github.com/SupesSolutions/RWAutoSeller/wiki/Bonded-Filter";
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00011F50 File Offset: 0x00010150
		public bool CheckItem(Transferable t, List<string> Data)
		{
			if (Data.Count == 0)
			{
				return false;
			}
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.Animal)
			{
				bool flag = false;
				using (List<DirectPawnRelation>.Enumerator enumerator = pawn.relations.DirectRelations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.def == PawnRelationDefOf.Bond)
						{
							flag = true;
							break;
						}
					}
				}
				if ((flag && Data[0] == "bnd.true") || (!flag && Data[0] == "bnd.false"))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x0001200C File Offset: 0x0001020C
		public List<string> DataFromTradeable(Transferable t)
		{
			List<string> list = new List<string>();
			Pawn pawn = t.AnyThing as Pawn;
			if (pawn != null && pawn.RaceProps.Animal)
			{
				bool flag = false;
				using (List<DirectPawnRelation>.Enumerator enumerator = pawn.relations.DirectRelations.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.def == PawnRelationDefOf.Bond)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					list.Add("bnd.true");
				}
				else
				{
					list.Add("bnd.false");
				}
			}
			return list;
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x000120AC File Offset: 0x000102AC
		public List<string> DataOptions(List<string> Data, Map map)
		{
			return Data;
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x000120B0 File Offset: 0x000102B0
		public void DrawWidget(Rect inrect, List<string> Data)
		{
			if (this.AllowRender)
			{
				Widgets.CheckboxLabeled(new Rect(inrect.x + 4f, inrect.y + 2f, inrect.width - 6f, 24f), "RWAutoSell.FilterBondedIs".Translate(), ref this.Bonded, false, null, null, false, false);
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00012115 File Offset: 0x00010315
		public void RenderStarted(List<string> Data, Map map)
		{
			if (Data.Count == 0)
			{
				this.Bonded = true;
			}
			else if (Data[0] == "bnd.true")
			{
				this.Bonded = true;
			}
			else
			{
				this.Bonded = false;
			}
			this.AllowRender = true;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00012152 File Offset: 0x00010352
		public void RenderStopped(List<string> Data, Map map)
		{
			this.AllowRender = false;
			Data.Clear();
			if (this.Bonded)
			{
				Data.Add("bnd.true");
				return;
			}
			Data.Add("bnd.false");
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060001DC RID: 476 RVA: 0x00012180 File Offset: 0x00010380
		public string[] Tags { get; } = new string[]
		{
			"RWAutoSell.TagAnimals"
		};

		// Token: 0x0400010C RID: 268
		private bool AllowRender;

		// Token: 0x0400010D RID: 269
		private bool Bonded = true;
	}
}
