using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Verse;
using Verse.AI;

namespace RWAutoSell.AI
{
	// Token: 0x02000059 RID: 89
	[NullableContext(1)]
	[Nullable(0)]
	internal class ASWorkGiver_PawnBargain : WorkGiver_Scanner
	{
		// Token: 0x170000CF RID: 207
		// (get) Token: 0x06000340 RID: 832 RVA: 0x0001E230 File Offset: 0x0001C430
		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		// Token: 0x06000341 RID: 833 RVA: 0x0001E234 File Offset: 0x0001C434
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			IEnumerable<Thing> result;
			try
			{
				result = (from pn in pawn.Map.mapPawns.AllPawns
				let pt = pn
				where pn.Faction != Faction.OfPlayer && pt != null && pt.CanTradeNow && pt.TradeCurrency == TradeCurrency.Silver
				select pn).Union(from tn in pawn.Map.listerBuildings.allBuildingsColonist
				let tt = tn as ITrader
				where tn.Faction != Faction.OfPlayer && tt != null && tt.CanTradeNow && tt.TradeCurrency == TradeCurrency.Silver
				select tn);
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000343 RID: 835 RVA: 0x0001E36C File Offset: 0x0001C56C
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			return !ASMapComp.GetSingleton(pawn.Map).MSettings.ASAITog || base.ShouldSkip(pawn, forced);
		}

		// Token: 0x06000344 RID: 836 RVA: 0x0001E38F File Offset: 0x0001C58F
		public override Job JobOnThing(Pawn pawn, Thing t, bool Forced)
		{
			return new Job(DefDatabase<JobDef>.GetNamed("JobDriver_PawnAutoSell", true), t);
		}

		// Token: 0x06000345 RID: 837 RVA: 0x0001E3A8 File Offset: 0x0001C5A8
		public override bool HasJobOnThing(Pawn pawn, Thing t, bool Forced)
		{
			if (t == null)
			{
				return false;
			}
			ITrader trader = t as ITrader;
			if (trader == null)
			{
				return false;
			}
			if (trader.TradeCurrency != TradeCurrency.Silver)
			{
				return false;
			}
			if (!trader.CanTradeNow)
			{
				return false;
			}
			if (!pawn.CanTradeWith(t.Faction, trader.TraderKind))
			{
				return false;
			}
			if (Forced)
			{
				return true;
			}
			ASMapComp singleton = ASMapComp.GetSingleton(pawn.Map);
			if (singleton.trader.Any((ASTraderCount x) => x.Equals(t)))
			{
				return false;
			}
			if (singleton.MSettings.SelectedNegogiators.Count<string>() == 0 && pawn != ASMapComp.GetSingleton(pawn.Map).Negogiator)
			{
				return false;
			}
			if (singleton.MSettings.SelectedNegogiators.Count<string>() > 0 && !singleton.MSettings.SelectedNegogiators.Contains(pawn.ThingID))
			{
				return false;
			}
			Pawn pawn2 = t as Pawn;
			if (pawn2 != null)
			{
				if (!pawn.CanReach(pawn2.pather.Destination, PathEndMode.Touch, Danger.Some, false, false, TraverseMode.ByPawn))
				{
					return false;
				}
				if (!pawn.Map.areaManager.Home.ActiveCells.Contains(t.Position))
				{
					if (pawn2.pather.Moving && !pawn2.pather.Destination.Cell.InHorDistOf(pawn2.Position, 12f))
					{
						return false;
					}
					pawn2.pather.Destination.Cell.InHorDistOf(pawn2.Position, 12f);
					return true;
				}
			}
			else if (!pawn.Map.areaManager.Home.ActiveCells.Contains(t.Position))
			{
				return false;
			}
			return true;
		}
	}
}
