using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Verse;

namespace RWAutoSell
{
	// Token: 0x02000024 RID: 36
	public class PlaceWorker_OnePerMap : PlaceWorker
	{
		// Token: 0x06000121 RID: 289 RVA: 0x0000D16C File Offset: 0x0000B36C
		[NullableContext(1)]
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			if (map.listerBuildings.AllBuildingsColonistOfClass<ASReportsPrinter>().Count<ASReportsPrinter>() != 0 || map.listerThings.ThingsOfDef(checkingDef.blueprintDef).Count<Thing>() != 0)
			{
				return new AcceptanceReport("RWAutoSell.PWOnlyOne".Translate());
			}
			return base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore, null);
		}
	}
}
