using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.AI;

namespace RWAutoSell.AI
{
	// Token: 0x02000058 RID: 88
	[NullableContext(1)]
	[Nullable(0)]
	public class ASWorkGiver_Bargain : WorkGiver_Scanner
	{
		// Token: 0x170000CE RID: 206
		// (get) Token: 0x0600033A RID: 826 RVA: 0x0001DFFF File Offset: 0x0001C1FF
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001E004 File Offset: 0x0001C204
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn Pawn)
		{
			return from el in Pawn.Map.listerBuildings.AllBuildingsColonistOfClass<Building_CommsConsole>()
			where el.def.defName == "SmallCommsConsole_TWB" || el.def.defName == ThingDefOf.CommsConsole.defName
			select el;
		}

		// Token: 0x0600033D RID: 829 RVA: 0x0001E071 File Offset: 0x0001C271
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !ASMapComp.GetSingleton(pawn.Map).MSettings.ASAITog || base.ShouldSkip(pawn, forced);
		}

		// Token: 0x0600033E RID: 830 RVA: 0x0001E094 File Offset: 0x0001C294
		public override Job JobOnThing(Pawn pawn, Thing t, bool Forced = false)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("JobDriver_AutoSell", true), t);
		}

		// Token: 0x0600033F RID: 831 RVA: 0x0001E0AC File Offset: 0x0001C2AC
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool Forced)
		{
			if (t.Faction == null)
			{
				return false;
			}
			if (t.Faction != pawn.Faction)
			{
				return false;
			}
			if (t.IsBurning() || t.IsForbidden(pawn))
			{
				return false;
			}
			if (!pawn.CanReserveAndReach(t, PathEndMode.InteractionCell, Danger.Some, 1, -1, null, false))
			{
				return false;
			}
			Building_CommsConsole building_CommsConsole = t as Building_CommsConsole;
			if (building_CommsConsole == null || !building_CommsConsole.CanUseCommsNow)
			{
				return false;
			}
			if (t.Map.passingShipManager.passingShips == null || t.Map.passingShipManager.passingShips.Count<PassingShip>() == 0)
			{
				return false;
			}
			bool flag = false;
			foreach (PassingShip passingShip in t.Map.passingShipManager.passingShips)
			{
				ITrader trader = passingShip as ITrader;
				if (trader != null && pawn.CanTradeWith(passingShip.Faction, trader.TraderKind))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			ASMapComp singleton = ASMapComp.GetSingleton(pawn.Map);
			return singleton.Rules.Count != 0 && (Forced || ((singleton.MSettings.SelectedNegogiators.Count<string>() != 0 || pawn == singleton.Negogiator) && (singleton.MSettings.SelectedNegogiators.Count<string>() <= 0 || singleton.MSettings.SelectedNegogiators.Contains(pawn.ThingID)) && singleton.HasNewShip));
		}
	}
}
