using System;
using System.Runtime.CompilerServices;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000018 RID: 24
	public static class DynamicTradeInterfacePatcher
	{
		// Token: 0x060000B8 RID: 184 RVA: 0x00007EF4 File Offset: 0x000060F4
		[NullableContext(1)]
		public static void Draw(ref Rect rect, Tradeable row, Transactor transactor, ref bool refresh)
		{
			if (row == null || row.IsFavor || !row.HasAnyThing || row.ThingDef == null || row.ThingDef == ThingDefOf.Silver || !TradeSession.Active || (TradeSession.playerNegotiator != null && TradeSession.playerNegotiator.GetCaravan() != null))
			{
				return;
			}
			int num = (int)Mathf.Min(rect.width, rect.height);
			if (Widgets.ButtonImage(new Rect(rect.x, rect.y, (float)num, (float)num).ContractedBy(2f), ASTextures.ASyx, true, null))
			{
				Find.WindowStack.Add(new ASDialog_Trade2Rule(row));
			}
		}
	}
}
