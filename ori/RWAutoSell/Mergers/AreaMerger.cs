using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RWASFilterLib;
using Verse;

namespace RWAutoSell.Mergers
{
	// Token: 0x0200002A RID: 42
	[NullableContext(1)]
	[Nullable(0)]
	public class AreaMerger : IUnmerge
	{
		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600014C RID: 332 RVA: 0x0000E3F8 File Offset: 0x0000C5F8
		public string Label
		{
			get
			{
				return "RWAutoSell.MergerArea".Translate();
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x0000E40C File Offset: 0x0000C60C
		private bool TreatAsItem(Thing thing, Thing thing2, Map map)
		{
			IntVec3 positionHeld = thing.PositionHeld;
			IntVec3 positionHeld2 = thing2.PositionHeld;
			List<Area> list = new List<Area>();
			List<Area> list2 = new List<Area>();
			foreach (Area area in map.areaManager.AllAreas)
			{
				if (area[positionHeld])
				{
					list.Add(area);
				}
				if (area[positionHeld2])
				{
					list2.Add(area);
				}
			}
			if (list.Count != list2.Count)
			{
				return false;
			}
			if (list.Count == 0)
			{
				return true;
			}
			int num = 0;
			using (List<Area>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Area a = enumerator.Current;
					if (list2.Any((Area x) => x == a))
					{
						num++;
					}
				}
			}
			return num == list.Count;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x0000E528 File Offset: 0x0000C728
		public bool AllowMerge(Thing thing, Thing thing2)
		{
			Map mapHeld = thing.MapHeld;
			if (mapHeld == null || thing.MapHeld != thing2.MapHeld || !thing.PositionHeld.InBounds(mapHeld) || !thing2.PositionHeld.InBounds(mapHeld))
			{
				return true;
			}
			Pawn pawn = thing as Pawn;
			if (pawn == null)
			{
				return !(thing2 is Pawn) && this.TreatAsItem(thing, thing2, mapHeld);
			}
			Pawn pawn2 = thing2 as Pawn;
			if (pawn2 == null)
			{
				return false;
			}
			if ((pawn.Faction == null || !pawn.Faction.IsPlayer) && (pawn2.Faction == null || !pawn2.Faction.IsPlayer))
			{
				return this.TreatAsItem(thing, thing2, mapHeld);
			}
			if (pawn.Faction != null && pawn.Faction.IsPlayer && pawn2.Faction != null && pawn2.Faction.IsPlayer)
			{
				Area areaRestrictionInPawnCurrentMap = pawn.playerSettings.AreaRestrictionInPawnCurrentMap;
				Area areaRestrictionInPawnCurrentMap2 = pawn2.playerSettings.AreaRestrictionInPawnCurrentMap;
				return areaRestrictionInPawnCurrentMap == areaRestrictionInPawnCurrentMap2 && this.TreatAsItem(thing, thing2, mapHeld);
			}
			return false;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x0000E62C File Offset: 0x0000C82C
		public bool AppliesTo(Thing thing)
		{
			return thing.MapHeld != null && thing.PositionHeld.InBounds(thing.MapHeld);
		}
	}
}
