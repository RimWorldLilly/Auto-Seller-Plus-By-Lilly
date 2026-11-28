using System;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using Verse;

namespace RWAutoSell.Mergers
{
	// Token: 0x0200002D RID: 45
	[NullableContext(1)]
	[Nullable(0)]
	public class ZoneMerger : IUnmerge
	{
		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000159 RID: 345 RVA: 0x0000E7E1 File Offset: 0x0000C9E1
		public string Label
		{
			get
			{
				return "RWAutoSell.MergerZone".Translate();
			}
		}

		// Token: 0x0600015A RID: 346 RVA: 0x0000E7F4 File Offset: 0x0000C9F4
		public bool AllowMerge(Thing thing, Thing thing2)
		{
			Map mapHeld = thing.MapHeld;
			if (mapHeld != null && thing.MapHeld == thing2.MapHeld && thing.PositionHeld.InBounds(mapHeld) && thing2.PositionHeld.InBounds(mapHeld))
			{
				Zone zone = mapHeld.zoneManager.ZoneAt(thing.PositionHeld);
				Zone zone2 = mapHeld.zoneManager.ZoneAt(thing2.PositionHeld);
				if (zone != zone2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600015B RID: 347 RVA: 0x0000E85E File Offset: 0x0000CA5E
		public bool AppliesTo(Thing thing)
		{
			return !(thing is Pawn) && (thing.MapHeld != null && thing.PositionHeld.InBounds(thing.MapHeld));
		}
	}
}
