using System;
using System.Runtime.CompilerServices;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RWASWidgets
{
	// Token: 0x02000015 RID: 21
	[NullableContext(1)]
	[Nullable(0)]
	public class ASTab
	{
		// Token: 0x060000CF RID: 207 RVA: 0x00006548 File Offset: 0x00004748
		public ASTab(string Label, PlanetTile tile, int Index, string tp = "") : this(Label, Index, tp)
		{
			this.Tile = tile;
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x0000655C File Offset: 0x0000475C
		public ASTab(string Label, int Index, string tp = "")
		{
			this.Label = Label;
			this.Index = Index;
			this.Visible = true;
			this.ToolTip = new TipSignal(tp, Rand.Int);
			GameFont font = Text.Font;
			Text.Font = GameFont.Small;
			Vector2 vector = Text.CalcSize(Label);
			Text.Font = font;
			vector.y = ((vector.y + 5f < 30f) ? 30f : (vector.y + 5f));
			vector.x = ((vector.x + 5f < 100f) ? 100f : (vector.x + 5f));
			this.Size = vector;
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x0000660D File Offset: 0x0000480D
		public string Label { get; }

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000D2 RID: 210 RVA: 0x00006615 File Offset: 0x00004815
		// (set) Token: 0x060000D3 RID: 211 RVA: 0x0000661D File Offset: 0x0000481D
		public bool Visible { get; set; }

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000D4 RID: 212 RVA: 0x00006626 File Offset: 0x00004826
		public int Index { get; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000D5 RID: 213 RVA: 0x0000662E File Offset: 0x0000482E
		public PlanetTile Tile { get; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000D6 RID: 214 RVA: 0x00006636 File Offset: 0x00004836
		public Vector2 Size { get; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000D7 RID: 215 RVA: 0x0000663E File Offset: 0x0000483E
		public TipSignal ToolTip { get; }
	}
}
