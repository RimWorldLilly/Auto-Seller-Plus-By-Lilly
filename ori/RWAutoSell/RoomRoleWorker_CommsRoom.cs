using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000025 RID: 37
	public class RoomRoleWorker_CommsRoom : RoomRoleWorker
	{
		// Token: 0x06000123 RID: 291 RVA: 0x0000D1D0 File Offset: 0x0000B3D0
		[NullableContext(1)]
		public override float GetScore(Room room)
		{
			int num = 0;
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (int i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				if (containedAndAdjacentThings[i] is Building_CommsConsole)
				{
					num++;
				}
			}
			return (float)num * 5f;
		}
	}
}
