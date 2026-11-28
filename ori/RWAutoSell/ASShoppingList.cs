using System;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell
{
	// Token: 0x0200000D RID: 13
	[NullableContext(1)]
	[Nullable(0)]
	public class ASShoppingList : ThingWithComps
	{
		// Token: 0x06000054 RID: 84 RVA: 0x000046AC File Offset: 0x000028AC
		public override void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.Maptile, "OriginTile", -1, false);
			base.ExposeData();
			LoadSaveMode mode = Scribe.mode;
			Scribe_Values.Look<int>(ref this.Maptile, "OriginLayer", 0, false);
			base.ExposeData();
			LoadSaveMode mode2 = Scribe.mode;
		}

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000055 RID: 85 RVA: 0x000046F9 File Offset: 0x000028F9
		public Map Origin
		{
			get
			{
				return Find.Maps.FirstOrDefault((Map x) => x.Tile.tileId == this.Maptile && x.Tile.Layer.LayerID == this.MapLayer);
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004714 File Offset: 0x00002914
		public override void SpawnSetup(Map map, bool respawningAfterLoaded)
		{
			base.SpawnSetup(map, respawningAfterLoaded);
			if (!respawningAfterLoaded && this.Maptile == -1)
			{
				this.Maptile = map.Tile.tileId;
			}
			if (!respawningAfterLoaded && this.MapLayer == -1)
			{
				this.MapLayer = map.Tile.Layer.LayerID;
			}
		}

		// Token: 0x0400001F RID: 31
		private int Maptile = -1;

		// Token: 0x04000020 RID: 32
		private int MapLayer = -1;
	}
}
